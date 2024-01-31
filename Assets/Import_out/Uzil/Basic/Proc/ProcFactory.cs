using System;
using System.Collections.Generic;

using UnityEngine;

using Uzil.Res;
using Uzil.Util;

namespace Uzil.Proc {

public class ProcFactory {
	protected static bool isDebug = false;
	protected static void log(object log){
		if (isDebug) Debug.Log(log);
	}

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*預製Process與json資料的對照庫*/
	public static Dictionary<string, string> name2DataDict;

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/
	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/* 建立節點 */
	public static T CreateNode<T> () where T : ProcNode {
		T pNode = new GameObject().AddComponent<T>();
		return pNode;
	}
	/* 建立節點 */
	public static ProcNode CreateNodeByName (string name, string mgrID) {
		string json = ProcFactory.LoadPrefabJson(name);
		return ProcFactory.CreateNodeByJson(json, mgrID);
	}
	/* 建立節點 */
	public static ProcNode CreateNodeByJson (string json, string mgrID) {
		log(json);
		DictSO data = DictSO.Json(json);

		if (data == null || data.Count <= 0) return null;

		// 若是預製物件
		if (data.ContainsKey("prefab")) {
			data = DictSO.Json(ProcFactory.LoadPrefabJson(data["prefab"].ToString()));
		}

		// 若讀取到的不是Node，回傳空
		if (data.ContainsKey("node") == false) return null;

		// 取得Node名稱
		string nodeName = data.GetString("node");

		// 取得類別
		Type nodeType;
		if (nodeName.StartsWith("ProcNode_")) {
			nodeType = TypeUtil.FindType(nodeName);
		} else {
			nodeType = TypeUtil.FindType("ProcNode_"+nodeName);
		}
		
		// 若 還是取得不到類別 則 回傳空
		if (nodeType == null) {
			Debug.Log("[ProcFactory]: NodeName["+nodeName+"] NodeType["+nodeType+"] not exist.");
			return null;
		}

		// 建立物件 並以 node名稱 加入Component
		GameObject nodeGObj = new GameObject(nodeName);
		ProcNode procNode = (ProcNode) nodeGObj.AddComponent(nodeType);

		// 設置Mgr
		procNode.mgrID = mgrID;

		// 設置ID
		if (data.ContainsKey("id")) {
			string id = data["id"].ToString();
			procNode.SetID(id);
		} else {
			// 預設ID
			procNode.SetID("_node_"+nodeName);
		}

		// 初始化
		if (data.ContainsKey("param")) {
			((IMemoable)procNode).LoadMemo(data["param"].ToString());
		}

		return procNode;
	}

	/* 建立事件 */
	public static T CreateEvent<T> () where T : ProcEvent {
		T pEvent = new GameObject().AddComponent<T>();
		return pEvent;
	}
	/* 建立事件 */
	public static ProcEvent CreateEventByName (string name, string mgrID) {
		string json = ProcFactory.LoadPrefabJson(name);
		return ProcFactory.CreateEventByJson(json, mgrID);
	}
	/* 建立事件 */
	public static ProcEvent CreateEventByJson (string json, string mgrID) {
		log(json);
		DictSO data = DictSO.Json(json);

		if (data == null) return null;

		// 若是預製物件
		if (data.ContainsKey("prefab")) {
			data = DictSO.Json(ProcFactory.LoadPrefabJson(data.GetString("prefab")));
		}

		// 若讀取到的不是event，回傳空
		if (data.ContainsKey("event") == false) return null;

		// 取得event名稱
		string eventName = data.GetString("event");

		// 取得類別
		Type eventType;
		if (eventName.StartsWith("ProcEvent_") ){
			eventType = TypeUtil.FindType(eventName);
		} else {
			eventType = TypeUtil.FindType("ProcEvent_"+eventName);
		}
		
		// 若 還是取得不到類別 則 回傳空
		if (eventType == null) {
			Debug.Log("[ProcFactory]: EventType["+eventType+"] not exist.");
			return null;
		}

		// 建立物件 並以 event名稱 加入Component
		GameObject eventGObj = new GameObject(eventName);
		ProcEvent procEvent = (ProcEvent) eventGObj.AddComponent(eventType);

		// 設置Mgr
		procEvent.mgrID = mgrID;

		// 設置ID
		if (data.ContainsKey("id")) {
			string id = data.GetString("id");
			procEvent.SetID(id);
		} else {
			// 預設ID
			procEvent.SetID("_event_"+eventName);
		}

		// 初始化
		if (data.ContainsKey("param")) {
			((IMemoable)procEvent).LoadMemo(data["param"].ToString());
		}

		return procEvent;
	}


	/* 建立條件 */
	public static T CreateGate<T> () where T : ProcGate {
		T pGate = new GameObject().AddComponent<T>();
		return pGate;
	}
	/* 建立條件 */
	public static ProcGate CreateGateByName (string name, string mgrID) {
		string json = ProcFactory.LoadPrefabJson(name);
		return ProcFactory.CreateGateByJson(json, mgrID);
	}
	/* 建立條件 */
	public static ProcGate CreateGateByJson (string json, string mgrID) {
		log(json);
		DictSO data = DictSO.Json(json);

		if (data == null) return null;

		// 若是預製物件
		if (data.ContainsKey("prefab")) {
			data = DictSO.Json(ProcFactory.LoadPrefabJson(data["prefab"].ToString()));
		}

		// 若讀取到的不是gate，回傳空
		if (data.ContainsKey("gate") == false) return null;

		// 取得gate名稱
		string gateName = data.GetString("gate");

		// 取得類別
		Type gateType;
		if (gateName.StartsWith("ProcGate_")) {
			gateType = TypeUtil.FindType(gateName);
		} else {
			gateType = TypeUtil.FindType("ProcGate_"+gateName);
		}
		
		//若 還是取得不到類別 則 回傳空
		if (gateType == null) {
			Debug.Log("[ProcFactory]: GateType["+gateType+"] not exist.");
			return null;
		}

		//建立物件 並以 gate名稱 加入Component
		GameObject gateGObj = new GameObject(gateName);
		ProcGate procGate = (ProcGate) gateGObj.AddComponent(gateType);

		// 設置Mgr
		procGate.mgrID = mgrID;

		// 設置ID
		if (data.ContainsKey("id")) {
			string id = data["id"].ToString();
			procGate.SetID(id);
		} else {
			// 預設ID
			procGate.SetID("_gate_"+gateName);
		}

		// 初始化
		if (data.ContainsKey("param")) {
			((IMemoable)procGate).LoadMemo(data["param"].ToString());
		}

		return procGate;
	}


	/* 讀取預製Process */
	public static string LoadPrefabJson (string prefabName) {
		// 初始化 內建的 "name:procJson的對照庫"
		if (ProcFactory.name2DataDict == null) {
			ProcFactory.initPrefabProcessDict();
		}

		string json;
		//若 對照庫中有 則 從中取得
		if (ProcFactory.name2DataDict.ContainsKey(prefabName)) {
			json = ProcFactory.name2DataDict[prefabName];
		}

		//否則 外部讀入
		else {
			
			json = ResMgr.Get<string>(new ResReq(PathUtil.Combine(Const_Proc.PROC_JSON_ROOT, prefabName)));
			
			if (json == "" || json == null) return null;

			//加入對照庫
			ProcFactory.name2DataDict.Add(prefabName, json);

		}
		return json;
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

	/* 初始化 內建的 "name:procJson的對照庫" */
	private static void initPrefabProcessDict () {
		ProcFactory.name2DataDict = new Dictionary<string, string>();
		
		TextAsset[] defaultProcessPrefabTxts = Resources.LoadAll<TextAsset>(Const_Proc.PROC_PREFAB_ROOT);
		foreach (TextAsset each in defaultProcessPrefabTxts){
			ProcFactory.name2DataDict.Add(each.name, each.text);
		}
	}



}

}