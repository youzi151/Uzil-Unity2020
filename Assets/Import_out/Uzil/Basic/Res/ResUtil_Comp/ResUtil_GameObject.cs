using System.Collections.Generic;

using UnityEngine;

namespace Uzil.Res {

public class ResUtil_GameObject {
	public bool isDebug = false;

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/* 每一個Prefab的pool */
	private Dictionary<GameObject, Queue<GameObject>> prefabPoolDict = new Dictionary<GameObject, Queue<GameObject>>();
	/* GObj對應的Prefab記錄 */
	private Dictionary<GameObject, GameObject> gObjPrefabDict = new Dictionary<GameObject, GameObject>();

	/* Path對應的Prefab記錄 */
	private Dictionary<string, GameObject> path2PrefabDict = new Dictionary<string, GameObject>();

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/* 預載物件 */
	public void Preload (string path, int count = 1) {
		this.Preload(this.getPrefabFromPath(path), count);
	}
	public void Preload (GameObject prefab, int count = 1) {
		GameObject instance;
		for (int i = 0; i < count; i++) {
			instance = GameObject.Instantiate(prefab);
			this.getOrCreatePrefabPool(prefab).Enqueue(instance);
			instance.SetActive(false);
		}
	}

	/* 取出物件 */
	public GameObject ReUse (string path, Event recoveryEvent = null) {	
		return this.ReUse(this.getPrefabFromPath(path), recoveryEvent);
	}
	public GameObject ReUse (GameObject prefab, Event recoveryEvent = null) {
		if (prefab == null) return null;
	
		// 取得 該物件的pool
		Queue<GameObject> pool = this.getOrCreatePrefabPool(prefab);

		GameObject gObj;

		// 若 沒有該物件的pool
		if (pool.Count <= 0) {
			gObj = GameObject.Instantiate(prefab);
			if (gObj == null) return null;
		} else {
			gObj = pool.Dequeue();
			gObj.SetActive(true);
		}
		
		// 回收事件
		if (recoveryEvent != null) {
			EventListener recoveryCB = new EventListener(()=>{
				this.Recovery(gObj);
			}).Once().ID("ResourceManager.recoveryCB");
			recoveryEvent += recoveryCB;
		}


		// 加入追蹤記錄
		this.gObjPrefabDict.Add(gObj, prefab);

		return gObj;
	}

	/* 放入物件 */
	public void Recovery (GameObject gObj) {
		// 若 id不在記錄中
		if (! this.gObjPrefabDict.ContainsKey(gObj)) return;

		GameObject prefab = this.gObjPrefabDict[gObj];
		// 從記錄中移除		
		this.gObjPrefabDict.Remove(gObj);
		// 放入該物件的pool
		this.prefabPoolDict[prefab].Enqueue(gObj);

		gObj.SetActive(false);

	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

	/* 取得或建立物件池 */
	private Queue<GameObject> getOrCreatePathPool (string path) {
		return this.getOrCreatePrefabPool(this.getPrefabFromPath(path));
	}
	private Queue<GameObject> getOrCreatePrefabPool (GameObject prefab) {
		Queue<GameObject> queue;

		// 若 該物件的pool不存在，則建立
		if (this.prefabPoolDict.ContainsKey(prefab) == false){
			queue = new Queue<GameObject>();
			this.prefabPoolDict.Add(prefab, queue);
		}else{
			queue = this.prefabPoolDict[prefab];
		}

		return queue;
	}
	
	/*從路徑取得物件Prefab*/
	private GameObject getPrefabFromPath(string path){
		//若有則返回
		if (this.path2PrefabDict.ContainsKey(path)){
			return this.path2PrefabDict[path];
		}
		//若還沒記錄到Path:Prefab
		GameObject prefab = (GameObject)Resources.Load(path);
		// print(path);
		this.path2PrefabDict.Add(path, prefab);
	
		return prefab;
	}

}



}