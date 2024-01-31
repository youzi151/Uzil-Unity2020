using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Uzil {

/* 
 * 每幀更新呼叫器
 * 1. 註冊/註銷 非綁定物件的Update呼叫
 * 2. 
 * 3. 指定實體與對應場景
 */

public class InvokerUpdate : MonoBehaviour {
	public bool isDebug = false;
	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	
	/* Key:實體 表 */
	public static Dictionary<string, InvokerUpdate> key2Instance = new Dictionary<string, InvokerUpdate>();

	/* 取得實體(快捷) */
	public static InvokerUpdate main {
		get {
			return InvokerUpdate.Inst();
		}
	}

	/*=====================================Static Funciton=======================================*/

	/* 取得實體 */
	public static InvokerUpdate Inst (string key = null) {
		if (key == null) key = "_default";
		
		InvokerUpdate instance = null;

		// 若 已存在 則 取用
		if (InvokerUpdate.key2Instance.ContainsKey(key)) {
			
			instance = InvokerUpdate.key2Instance[key];

		}
		// 否則 建立
		else {
			
			// 取得根物件
			GameObject invokerRoot = RootUtil.GetMember("Invoke.InvokerUpdate");
			
			// 建立
			GameObject instanceGObj = RootUtil.GetChild(key, invokerRoot);
			instance = instanceGObj.AddComponent<InvokerUpdate>();
			instance.instanceKey = key;

			// 設置 當 場景卸載 檢查所屬場景
			RootUtil.onSceneUnload.Add((data)=>{
				string sceneName = data.GetString("sceneName");

				// 若 無所屬場景 則 返回
				if (instance.belongSceneName == null) return;

				// 若 卸載場景 為 此實體所屬場景 則 移除此實體
				if (sceneName == instance.belongSceneName) {
					InvokerUpdate.Del(instance.instanceKey);
				}
			});
			
			// 加入註冊
			InvokerUpdate.key2Instance.Add(key, instance);

		}

		return instance;
	}

	/* 移除實體 */
	public static void Del (string key) {
		if (InvokerUpdate.key2Instance.ContainsKey(key) == false) return;

		// 取得實體
		InvokerUpdate instance = InvokerUpdate.key2Instance[key];

		// 銷毀物件
		GameObject.Destroy(instance.gameObject);

		// 移除註冊
		InvokerUpdate.key2Instance.Remove(key);
	}

	/*=========================================Members===========================================*/

	/* 任務 */
	public List<InvokeTask_Priority> taskList = new List<InvokeTask_Priority>();

	/* 
	 * 所屬場景
	 * 若該場景銷毀 則 連帶銷毀此Invoker
	 */
	private string belongSceneName = null;


	/* 實體鍵值 */
	private string instanceKey = null;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		// 因為task中也有可能呼叫Invoker，所以要轉為Array來處理避免執行時更動List
		InvokeTask_Priority[] taskArray = this.taskList.ToArray();

		// 每一個要呼叫的任務
		foreach (InvokeTask_Priority each in taskArray) {
			
			// 呼叫任務
			try {
				// 若 還存在 (沒有被移除)
				if (this.taskList.Contains(each)) {
					each.Do();
				}
			} catch (Exception e) {
				if (this.isDebug) {
					Debug.LogError(e);
				}
			}

		}
	}

	void OnDestroy() {
		InvokerUpdate.Del(this.instanceKey);	
	}

	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 新增 */
	public InvokeTask_Priority Add (Action act, float priority = 0f) {
		InvokeTask_Priority task = new InvokeTask_Priority();
		task.SetAct(act);
		task.priority = priority;
		
		if (this.isDebug){
			print("[InvokerUpdate] : Add Task");
		}
		
		this.taskList.Add(task);
		this.taskList.Sort((a, b)=>{
			return a.priority.CompareTo(b.priority);
		});

		return task;
	}

	/** 取消 */
	public List<InvokeTask_Priority> Cancel (string tag) {

		List<InvokeTask_Priority> toRm = new List<InvokeTask_Priority>();
		foreach (InvokeTask_Priority eachTask in this.taskList) {
			if (eachTask.IsTag(tag)) toRm.Add(eachTask); 
		}
		
		foreach (InvokeTask_Priority eachToRm in toRm) {
			this.taskList.Remove(eachToRm);
		}

		return toRm;
	}

	/** 取消 */
	public void Cancel (InvokeTask_Priority task) {
		if (this.taskList.Contains(task) == false) return;
		this.taskList.Remove(task);
	}

	/** 清空 */
	public void Clear () {
		this.taskList.Clear();
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}
