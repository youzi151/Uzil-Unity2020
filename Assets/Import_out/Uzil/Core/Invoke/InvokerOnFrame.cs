using UnityEngine;
using System;
using System.Collections.Generic;

namespace Uzil {

/* 
 * 幀呼叫器
 * 避免重複呼叫，確保只在每幀Update中呼叫1次，即使已經被多次要求呼叫
 */

public class InvokerOnFrame : MonoBehaviour {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	private static InvokerOnFrame _instance = null;

	/* 取得實體 */
	private static InvokerOnFrame inst () {
		InvokerOnFrame instance = InvokerOnFrame._instance;

		if (instance == null) {
			
			// 取得根物件
			GameObject instanceGObj = RootUtil.GetMember("Async.InvokerOnFrame");
			
			// 建立
			instance = instanceGObj.AddComponent<InvokerOnFrame>();

			InvokerOnFrame._instance = instance;

		}

		return instance;
	}

	/* 呼叫 */
	public static void Once (string key, Action act) {
		InvokerOnFrame instance = InvokerOnFrame.inst();
		if (instance.key2Action.ContainsKey(key)) {
			instance.key2Action[key] = act;
		} else {
			instance.key2Action.Add(key, act);
		}
	}

	
	/* 取消 */
	public static void Cancel (string key) {
		InvokerOnFrame instance = InvokerOnFrame.inst();
		if (instance.key2Action.ContainsKey(key) == false) return;
			
		instance.key2Action.Remove(key);
	}

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/* 鍵值 對應 執行內容 */
	public Dictionary<string, Action> key2Action = new Dictionary<string, Action>();

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	void LateUpdate () {

		// 準備 與 實際呼叫 分開
		// 避免 可能在 某一呼叫中 把 另一呼叫 取消 的狀況發生

		// 準備要呼叫的
		List<string> toCall = new List<string>();
		// 每一筆 加入要呼叫的
		foreach (KeyValuePair<string, Action> pair in this.key2Action) {
			string key = pair.Key;
			toCall.Add(key);
		}

		// 每個要呼叫的
		foreach (string eachKey in toCall) {
			if (this.key2Action.ContainsKey(eachKey) == false) continue;
			
			Action act = this.key2Action[eachKey];
			
			// 執行
			act();

			this.key2Action.Remove(eachKey);
		}
	}

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

}


}