using System;
using System.Collections.Generic;

using UnityEngine;

namespace Uzil {

/* 
 * 呼叫器
 * 1. 計時
 * 2. 可取消
 * 3. 指定實體與對應場景
 */

public class Invoker : MonoBehaviour {
	public bool isDebug = false;

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/
	
	/** Key:實體 表 */
	public static Dictionary<string, Invoker> key2Instance = new Dictionary<string, Invoker>();

	/** 取得實體(快捷) */
	public static Invoker main {
		get {
			return Invoker.Inst();
		}
	}

	/** 取得實體(快捷) */
	public static Invoker system {
		get {
			Invoker sys = Invoker.Inst("_system");
			sys.timeKey = "_system";
			return sys;
		}
	}

	/*=====================================Static Funciton=======================================*/
	
	/** 取得實體 */
	public static Invoker Inst (string key = null) {
		if (key == null) key = "_default";
		
		Invoker instance = null;

		// 若 已存在 則 取用
		if (Invoker.key2Instance.ContainsKey(key)) {
			
			instance = Invoker.key2Instance[key];

		}
		// 否則 建立
		else {
			
			// 取得根物件
			GameObject invokerRoot = RootUtil.GetMember("Invoke.Invoker");
			
			// 建立
			GameObject instanceGObj = RootUtil.GetChild(key, invokerRoot);
			instance = instanceGObj.AddComponent<Invoker>();
			instance.instanceKey = key;

			// 設置 當 場景卸載 檢查所屬場景
			RootUtil.onSceneUnload.Add((data)=>{
				string sceneName = data.GetString("sceneName");

				// 若 無所屬場景 則 返回
				if (instance.belongSceneName == null) return;

				// 若 卸載場景 為 此實體所屬場景 則 移除此實體
				if (sceneName == instance.belongSceneName) {
					Invoker.Del(instance.instanceKey);
				}
			});
			
			// 加入註冊
			Invoker.key2Instance.Add(key, instance);

		}

		return instance;
	}

	/** 移除實體 */
	public static void Del (string key) {
		if (Invoker.key2Instance.ContainsKey(key) == false) return;

		// 取得實體
		Invoker instance = Invoker.key2Instance[key];

		// 銷毀物件
		GameObject.Destroy(instance.gameObject);

		// 移除註冊
		Invoker.key2Instance.Remove(key);
	}

	/*=========================================Members===========================================*/

	/** 任務 */
	public List<InvokeTask_Delay> taskList = new List<InvokeTask_Delay>();
	public Stack<InvokeTask_Delay> toAddTaskStack = new Stack<InvokeTask_Delay>();

	/** 使用的時間 */
	public string timeKey = "_default";

	/** 
	 * 所屬場景
	 * 若該場景銷毀 則 連帶銷毀此Invoker
	 */
	private string belongSceneName = null;


	/** 實體鍵值 */
	private string instanceKey = null;

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	void Update () {

		// 當前時間
		float timeNow = this.getNow();
		
		// 要呼叫的任務
		List<InvokeTask_Delay> toCall = new List<InvokeTask_Delay>();

		// 因為task中也有可能呼叫Invoker，所以要轉為Array來處理避免執行時更動List
		InvokeTask_Delay[] taskArray = this.taskList.ToArray();

		// 是否有刪除任務
		bool isDel = false;

		// 每個任務
		for (int i = 0; i < taskArray.Length; i++) {
			InvokeTask_Delay eachTask = taskArray[i];
			// 若 當前時間 超過 任務的呼叫時間 則 加入呼叫列表
			if (timeNow > eachTask.callTime){
				toCall.Add(this.taskList[i]);
			}
		}

		// 每一個要呼叫的任務
		foreach (InvokeTask_Delay each in toCall) {
			
			try {
				// 若 還存在 (沒有被移除)
				if (this.taskList.Contains(each)){

					// 呼叫任務
					each.Do();

					// 移除任務
					this.taskList.Remove(each);
		
					// 標記有刪除任務
					isDel = true;
				}
			} catch (Exception){}

		}

		if (this.isDebug && isDel) {
			Debug.Log("[Invoker] : left "+this.taskList.Count+" tasks.");
		}

		while (this.toAddTaskStack.Count > 0) {
			this.taskList.Add(this.toAddTaskStack.Pop());
		}
		this.taskList.Sort((a, b)=>{
			return a.callTime.CompareTo(b.callTime);
		});
	}

	void OnDestroy() {
		Invoker.Del(this.instanceKey);	
	}

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 單次呼叫 */
	public InvokeTask_Delay Once (Action act, float delay = 0f) {
		InvokeTask_Delay task = new InvokeTask_Delay();
		task.SetAct(act);
		task.callTime = this.getNow() + delay;
		
		if (this.isDebug) {
			print("[Invoker] : Add Task, delay: "+delay);
		}
		
		this.toAddTaskStack.Push(task);

		return task;
	}

	/** 取消 */
	public List<InvokeTask_Delay> Cancel (string tag) {
		List<InvokeTask_Delay> toRm = new List<InvokeTask_Delay>();
		foreach (InvokeTask_Delay eachTask in this.taskList) {
			if (eachTask.IsTag(tag)) toRm.Add(eachTask); 
		}
		foreach (InvokeTask_Delay eachToRm in toRm) {
			this.taskList.Remove(eachToRm);
		}
		return toRm;
	}

	/** 取消 */
	public void Cancel (InvokeTask_Delay task) {
		if (this.taskList.Contains(task) == false) return;
		this.taskList.Remove(task);
	}

	/** 清空 */
	public void Clear () {
		this.taskList.Clear();
		this.toAddTaskStack.Clear();
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

	private float getNow () {

		if (this.timeKey == null) {
			return UnityEngine.Time.time;
		} else if (this.timeKey == "_realTime") {
			return UnityEngine.Time.realtimeSinceStartup;
		} else {
			// 試著取得 時間實體
			TimeInstance timeInstance = TimeUtil.Inst(this.timeKey);
			return timeInstance.time;
		}
	}

}


}