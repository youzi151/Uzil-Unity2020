using System.Collections.Generic;

using UnityEngine;

namespace Uzil {

/* 
 * 提供Uzil功能中，需要掛載到遊戲物件如Unity的GameObject, Cocos的Node...等的功能，
 * 可以快速藉由單一呼叫，來建立並取得遊戲物件實體。並集中管理。
 */

public class RootUtil : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/
	
	private static bool isDestroyed = false;

	/* 根物件 */
	public static GameObject rootGObj;

	/* 成員 */
	public static Dictionary<string, GameObject> key2Members = new Dictionary<string, GameObject>();

	/* 當場景卸載 */
	public static Event onSceneUnload = new Event();

	/*=====================================Static Funciton=======================================*/

	/* 取得根物件 */
	public static GameObject GetRoot () {
		if (RootUtil.isDestroyed) return null;
		
		GameObject rootGObj = RootUtil.rootGObj;
		
		// 1.直接回傳
		if (rootGObj != null) {
			return rootGObj;
		}

		// 2.場上尋找
		rootGObj = GameObject.Find("_Uzil");
		if (rootGObj != null) {
			RootUtil.rootGObj = rootGObj;
			return rootGObj;
		}

		// 3.建立
		rootGObj = new GameObject("_Uzil");
		GameObject.DontDestroyOnLoad(rootGObj);
		
		// 當 有場景卸載時 
		UnityEngine.SceneManagement.SceneManager.sceneUnloaded += (scene)=>{
			RootUtil.onSceneUnload.Call(
				DictSO.New().Ad("sceneName", scene.name)
			);
		};


		RootUtil.rootGObj = rootGObj;

		return rootGObj;
	}

	/* 取得成員 */
	public static GameObject GetMember (string key) {
		if (RootUtil.isDestroyed) return null;

		GameObject member = null;

		// 若 註冊表中已有 則 取用
		if (RootUtil.key2Members.ContainsKey(key)) {
			
			member = RootUtil.key2Members[key];

		}
		// 否則 依照 Key 為名稱 建立 物件 並 註冊
		else {
		
			member = RootUtil.GetChild(key, RootUtil.GetRoot());
			RootUtil.key2Members.Add(key, member);
		}

		return member;
	}

	/* 取得子物件 */
	public static GameObject GetChild (string path, GameObject parent) {
		if (RootUtil.isDestroyed) return null;

		// 以"."來分隔 並判斷為 父子物件階層表示
		string[] names = path.Split('.');

		if (parent == null) return null;

		// 當前末端
		Transform last = parent.transform;

		// 每一個物件階層
		for (int idx = 0; idx < names.Length; idx++) {
			
			// 物件名稱
			string name = names[idx];
			if (name == "" || name == null) continue;

			// 物件
			GameObject gObj;

			// 尋找已存在物件
			Transform exist = last.Find(name);
			// 若 已存在物件 則取用
			if (exist != null) {
				gObj = exist.gameObject;
			}
			// 否則 建立並設置
			else {
				gObj = new GameObject(names[idx]);	
				gObj.transform.SetParent(last);
			}
			
			// 設 當前末端 為 此物件
			last = gObj.transform;
		}

		// 返回 最末端物件
		return last.gameObject;
	}

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDestroy() {
		RootUtil.isDestroyed = true;
	}
	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}
