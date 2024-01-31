using System;
using System.Collections.Generic;

using UnityEngine;

namespace Uzil {

/* 
 * 呼叫器
 * 1. 設置
 * 2. 可取消
 * 3. 指定實體與對應場景
 */

public class Callbacks : MonoBehaviour {
	public bool isDebug = false;

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/
	
	/* Key:實體 表 */
	public static Dictionary<string, Callbacks> key2Instance = new Dictionary<string, Callbacks>();

	/* 取得實體(快捷) */
	public static Callbacks main {
		get {
			return Callbacks.Inst();
		}
	}

	/*=====================================Static Funciton=======================================*/
	
	/* 取得實體 */
	public static Callbacks Inst (string key = null) {
		if (key == null) key = "_default";

		Callbacks instance = null;

		// 若 已存在 則 取用
		if (Callbacks.key2Instance.ContainsKey(key)) {
			
			instance = Callbacks.key2Instance[key];

		}
		// 否則 建立
		else {
			
			// 取得根物件
			GameObject invokerRoot = RootUtil.GetMember("Invoke.Callbacks");
			
			// 建立
			GameObject instanceGObj = RootUtil.GetChild(key, invokerRoot);
			instance = instanceGObj.AddComponent<Callbacks>();
			instance.instanceKey = key;

			// 設置 當 場景卸載 檢查所屬場景
			RootUtil.onSceneUnload.Add((data)=>{
				string sceneName = data.GetString("sceneName");

				// 若 無所屬場景 則 返回
				if (instance.belongSceneName == null) return;

				// 若 卸載場景 為 此實體所屬場景 則 移除此實體
				if (sceneName == instance.belongSceneName) {
					Callbacks.Del(instance.instanceKey);
				}
			});
			
			// 加入註冊
			Callbacks.key2Instance.Add(key, instance);

		}

		return instance;
	}

	/* 移除實體 */
	public static void Del (string key) {
		if (Callbacks.key2Instance.ContainsKey(key) == false) return;

		// 取得實體
		Callbacks instance = Callbacks.key2Instance[key];

		// 銷毀物件
		GameObject.Destroy(instance.gameObject);

		// 移除註冊
		Callbacks.key2Instance.Remove(key);
	}

	/*=========================================Members===========================================*/

	/* 任務 */
	public List<InvokeTask_Call> taskList = new List<InvokeTask_Call>();

	/* 
	 * 所屬場景
	 * 若該場景銷毀 則 連帶銷毀此Callbacks
	 */
	public string belongSceneName = null;


	/* 實體鍵值 */
	private string instanceKey = null;

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	void Update () {

	}

	void OnDestroy() {
		Callbacks.Del(this.instanceKey);	
	}

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/* 單次呼叫 */
	public InvokeTask_Call Add (Action act) {
		
		InvokeTask_Call task = new InvokeTask_Call();
		task.SetAct(act);
		
		if (this.isDebug){
			string tags_str = "";
			foreach (string eachTag in task.tags) {
				tags_str = tags_str + eachTag;
			}
			Debug.Log("[Callbacks] : Add InvokeTask_Call (tags:"+tags_str+")");
		}
		
		this.taskList.Add(task);

		return task;
	}

	public bool Call (string tag, DictSO data = null) {
		bool isExist = false;

		List<InvokeTask_Call> toCall = new List<InvokeTask_Call>();

		foreach (InvokeTask_Call eachInvokeTask_Call in this.taskList) {
			if (eachInvokeTask_Call.IsTag(tag)) {
				toCall.Add(eachInvokeTask_Call); 
			}
		}

		foreach (InvokeTask_Call eachToCall in toCall) {
			eachToCall.Do(data);
			isExist = true;

			if (eachToCall.callTimes > 0) {
				if (eachToCall.callTimes-- <= 0) {
					this.taskList.Remove(eachToCall);
				}
			}
		}

		return isExist;
	}

	/* 取消 */
	public void Cancel (string tag) {
		List<InvokeTask_Call> toRm = new List<InvokeTask_Call>();
		foreach (InvokeTask_Call eachInvokeTask_Call in this.taskList) {
			if (eachInvokeTask_Call.IsTag(tag)) {
				toRm.Add(eachInvokeTask_Call); 
			}
		}

		foreach (InvokeTask_Call eachToRm in toRm) {
			this.taskList.Remove(eachToRm);
		}
	}

	/* 是否存在 */
	public bool IsExist (string tag) {
		foreach (InvokeTask_Call eachInvokeTask_Call in this.taskList) {
			if (eachInvokeTask_Call.IsTag(tag)) {
				return true;
			}
		}
		return false;
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/


}


}