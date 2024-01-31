using System;
using System.Collections.Generic;

using UnityEngine;

namespace Uzil.Proc {

public class ProcMgr : MonoBehaviour, IMemoable {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	public const string defaultKey = "_default";
	
	/* Key:實體 表 */
	public static Dictionary<string, ProcMgr> key2Instance = new Dictionary<string, ProcMgr>();

	/*=====================================Static Funciton=======================================*/

	/* 取得實體 */
	public static ProcMgr Inst (string key = ProcMgr.defaultKey) {
		ProcMgr instance = null;
		
		if (key == null) key = ProcMgr.defaultKey;

		// 若 實體存在 則 取用
		if (ProcMgr.key2Instance.ContainsKey(key)) {
			
			instance = ProcMgr.key2Instance[key];

		}
		// 否則 建立
		else {
			
			// 取得 根物件
			GameObject timesGObj = RootUtil.GetMember("Proc");
			GameObject gObj = RootUtil.GetChild(/* name */key, /* parent */timesGObj);

			// 建立
			instance = gObj.AddComponent<ProcMgr>();
			instance.Init(key);
			ProcMgr.key2Instance.Add(key, instance);

		}

		return instance;
	}

	/* 銷毀 */
	public static void Del (string key = ProcMgr.defaultKey) {
		if (ProcMgr.key2Instance.ContainsKey(key) == false) return;
		ProcMgr instance = ProcMgr.key2Instance[key];
		ProcMgr.key2Instance.Remove(key);
		instance.Destroy();
	}

	/* 銷毀 */
	public static void Del (ProcMgr pMgr) {
		ProcMgr.Del(pMgr.id);
	}



	/*=========================================Members===========================================*/

	/* 識別 */
	private string _id = ProcMgr.defaultKey;
	public string id {
		get {
			return this._id;
		}
	}
	
	/* 一個循環能跑幾個節點 */
	public int nodeRunPerCircle = 100;
	public int leftNodeRunTime = 100;

	/* Node名單 */
	protected Dictionary<string, ProcNode> nodeDict = new Dictionary<string, ProcNode>();

	/* Event名單 */
	protected Dictionary<string, ProcEvent> eventDict = new Dictionary<string, ProcEvent>();

	/* Gate名單 */
	protected Dictionary<string, ProcGate> gateDict = new Dictionary<string, ProcGate>();

	/* 自動分配的ID序號 */
	// private int idFix = 0;

	/* 關聯列表 */
	private Dictionary<object, List<object>> referenceDict = new Dictionary<object, List<object>>();

	/* ID與事件列表 */
	private Dictionary<object, Event> obj2OnChangeEventDict = new Dictionary<object, Event>();

	/* 參考物件與事件列表 */
	private Dictionary<KeyValuePair<object, object>, EventListener> targetAndReferencer2Event = new Dictionary<KeyValuePair<object, object>, EventListener>();


	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/* 當 節點被建立 */
	public Event onNodeExist = new Event();

	/*======================================Unity Function=======================================*/
	public void Update () {
		// if (Input.GetKeyDown(KeyCode.T)) {
			// Debug.Log((this as IMemoable).ToMemo());


			// //輪詢每一個 參考目標
			// foreach (KeyValuePair<object, List<object>> eachPair in this.referenceDict){
			// 	foreach (object each in eachPair.Value){
			// 		Debug.Log(each.ToString());
			// 	}
			// }
		// }
	}
	/*========================================Interface==========================================*/

	/* [IMemoable] 匯出成Json */
	public object ToMemo () {
		DictSO data = new DictSO();

		/* ID */
		data.Set("id", this.id);
		
		/* Node */
		DictSO nodeData = new DictSO();
		foreach (KeyValuePair<string, ProcNode> eachNode in this.nodeDict){
			if (eachNode.Value == null) continue;
			// nodeData.Set(eachNode.Key, (eachNode.Value as IMemoable).ToMemo());
			nodeData.Set(eachNode.Value.id, (eachNode.Value as IMemoable).ToMemo());
		}
		data.Set("node", nodeData);

		/* Event */
		DictSO eventData = new DictSO();
		foreach (KeyValuePair<string, ProcEvent> eachEvent in this.eventDict){
			if (eachEvent.Value == null) continue;
			// eventData.Set(eachEvent.Key, (eachEvent.Value as IMemoable).ToMemo());
			eventData.Set(eachEvent.Value.id, (eachEvent.Value as IMemoable).ToMemo());	
		}
		data.Set("event", eventData);

		/* gate */
		DictSO gateData = new DictSO();
		foreach (KeyValuePair<string, ProcGate> eachGate in this.gateDict){
			if (eachGate.Value == null) continue;
			// gateData.Set(eachGate.Key, (eachGate.Value as IMemoable).ToMemo());
			gateData.Set(eachGate.Value.id, (eachGate.Value as IMemoable).ToMemo());
		}
		data.Set("gate", gateData);
		
		return data;
	}
	/* [IMemoable] 從Json匯入 */
	public void LoadMemo (object memoJson) {
		DictSO data = DictSO.Json(memoJson);

		List<string> readedIDs = new List<string>();

		//XXX:暫時解
		List<string> eventBeforeCreate = new List<string>(this.eventDict.Keys);
		List<string> gateBeforeCreate = new List<string>(this.gateDict.Keys);
		List<string> nodeBeforeCreate = new List<string>(this.nodeDict.Keys);
		foreach (string id in eventBeforeCreate) {
			this.RemoveEvent(id);
		}
		foreach (string id in gateBeforeCreate) {
			this.RemoveGate(id);
		}
		foreach (string id in nodeBeforeCreate) {
			this.RemoveNode(id);
		}

		/* Event */
		DictSO eventData = data.GetDictSO("event");
		if (eventData != null) {
			foreach (KeyValuePair<string, object> eachEvent in eventData) {
				ProcEvent pEvent;
				//若存在 則 取得
				if (this.eventDict.ContainsKey(eachEvent.Key)){
					pEvent = this.eventDict[eachEvent.Key];
				}
				//若不存在 則 建立
				else {
					pEvent = this.CreateEvent(eachEvent.Value.ToString());
					if (pEvent == null) continue;
					pEvent.SetID(eachEvent.Key);
				}
				//覆寫
				pEvent.mgrID = this.id;
				(pEvent as IMemoable).LoadMemo(eachEvent.Value.ToString());

				readedIDs.Add(eachEvent.Key);
			}
			//原本存在 但沒寫到的 則 移除
			// foreach (string id in this.subtract<string>(eventBeforeCreate, readedIDs)){
			// 	this.RemoveEvent(id);
			// }
		}

		//========================
		readedIDs.Clear();//重複使用
		//========================

		/* gate */
		DictSO gateData = data.GetDictSO("gate");
		if (gateData != null) {
			foreach (KeyValuePair<string, object> eachGate in gateData) {
				ProcGate pGate;
				//若存在 則 取得
				if (this.gateDict.ContainsKey(eachGate.Key)) {
					pGate = this.gateDict[eachGate.Key];
				}
				//若不存在 則 建立
				else {
					pGate = this.CreateGate(eachGate.Value.ToString());
					if (pGate == null) continue;
					pGate.SetID(eachGate.Key);
				}
				//覆寫
				pGate.mgrID = this.id;
				(pGate as IMemoable).LoadMemo(eachGate.Value.ToString());

				readedIDs.Add(eachGate.Key);
			}
			//原本存在 但沒寫到的 則 移除
			// foreach (string id in this.subtract<string>(gateBeforeCreate, readedIDs)){
			// 	this.RemoveGate(id);
			// }
		}

		//========================
		readedIDs.Clear();//重複使用
		//========================

		/* Node */
		DictSO nodeData = data.GetDictSO("node");
		if (nodeData != null) {
			foreach (KeyValuePair<string, object> eachNode in nodeData) {
				ProcNode pNode;
				//若存在 則 取得
				if (this.nodeDict.ContainsKey(eachNode.Key)) {
					pNode = this.nodeDict[eachNode.Key];
				}
				//若不存在 則 建立
				else {
					pNode = this.CreateNode(eachNode.Value.ToString());
					if (pNode == null) continue;
					pNode.SetID(eachNode.Key);
				}
				//覆寫
				pNode.mgrID = this.id;
				(pNode as IMemoable).LoadMemo(eachNode.Value.ToString());

				readedIDs.Add(eachNode.Key);
			}
			//原本存在 但沒寫到的 則 移除
			// foreach (string id in this.subtract<string>(nodeBeforeCreate, readedIDs)){
			// 	this.RemoveNode(id);
			// }
		}

		Debug.Log("[ProcMgr]: LoadMemo Done");

	}


	/*=====================================Public Function=======================================*/

	/* 初始化 */
	public void Init (string key) {
		this._id = key;
	}

	/* 銷毀 */
	public void Destroy () {
		List<string> nodes = new List<string>(this.nodeDict.Keys);
		foreach (string each in nodes) {
			this.RemoveNode(each);
		}
		List<string> events = new List<string>(this.eventDict.Keys);
		foreach (string each in events) {
			this.RemoveEvent(each);
		}
		List<string> gates = new List<string>(this.gateDict.Keys);
		foreach (string each in gates) {
			this.RemoveGate(each);
		}
		
		GameObject.DestroyImmediate(this.gameObject);
	}
	
	/* 安全啟動(檢查循環次數) */
	public void BeginNode (ProcNode node) {
		if (this.leftNodeRunTime-- < 0) {
			
			Invoker.main.Once(()=>{
				this.BeginNode(node);
			}, 0);

			this.leftNodeRunTime = this.nodeRunPerCircle;

			return;
		}else{
			node.Begin();
		}
	}

	/* 啟動類型為AutoStart的Node節點 */
	public void StartFirstNode () {

		List<ProcNode> autoStartNodes = new List<ProcNode>();

		foreach (KeyValuePair<string, ProcNode> pair in this.nodeDict) {
			if (pair.Value is ProcNode_AutoStart){
				autoStartNodes.Add(pair.Value);
			}
		}

		foreach (ProcNode node in autoStartNodes) {
			node.Begin();
		}

	}

	/* 更改ID (傳入舊ID與想要的新ID，並回傳實際通過的ID) */
	public string ChangeNodeID(string nodeID, string wishID) {
		if (nodeID == wishID) return wishID;
		ProcNode pNode = this.GetNode(nodeID);
		if (pNode == null) return wishID;

		string reqID = this.requestID(wishID, (tryID)=>{
			return this.nodeDict.ContainsKey(tryID) == false;
		});
		pNode.SetIDByMgr(this.id, reqID);

		this.nodeDict.Remove(nodeID);
		this.nodeDict.Add(pNode.id, pNode);

		//event
		this.sendChangeIDEvent(/*target*/pNode, /*oldID*/nodeID, /*newID*/pNode.id);

		DictSO data = new DictSO();
		data.Set("id", pNode.id);
		data.Set("node", pNode);
		this.onNodeExist.Call(data);

		return pNode.id;
	}
	public string ChangeEventID(string eventID, string wishID){
		if (eventID == wishID) return wishID;
		ProcEvent pEvent = this.GetEvent(eventID);
		if (pEvent == null) return wishID;

		string fixID = this.requestID(wishID, (tryID)=>{
			return this.eventDict.ContainsKey(tryID) == false;
		});
		pEvent.SetIDByMgr(this.id, fixID);

		this.eventDict.Remove(eventID);
		this.eventDict.Add(pEvent.id, pEvent);

		//event
		this.sendChangeIDEvent(/*target*/pEvent, /*oldID*/eventID, /*newID*/pEvent.id);

		return pEvent.id;
	}
	public string ChangeGateID(string gateID, string wishID){
		if (gateID == wishID) return wishID;
		ProcGate pGate = this.GetGate(gateID);
		if (pGate == null) return wishID;

		string fixID = this.requestID(wishID, (tryID)=>{
			return this.gateDict.ContainsKey(tryID) == false;
		});
		pGate.SetIDByMgr(this.id, fixID);

		this.gateDict.Remove(gateID);
		this.gateDict.Add(pGate.id, pGate);

		//event
		this.sendChangeIDEvent(/*target*/pGate, /*oldID*/gateID, /*newID*/pGate.id);

		return pGate.id;
	}

	/*取得資料=======================*/

	/* 以ID從追蹤Dict取得Node */
	public ProcNode GetNode(string id){
		if (!nodeDict.ContainsKey(id)) return null;
		else return nodeDict[id];
	}
	/* 以ID從追蹤Dict取得Event */
	public ProcEvent GetEvent(string id){
		if (!eventDict.ContainsKey(id)) return null;
		else return eventDict[id];
	}
	/* 以ID從追蹤Dict取得Node */
	public ProcGate GetGate(string id){
		if (!gateDict.ContainsKey(id)) return null;
		else return gateDict[id];
	}


	/*讀取/建立=======================*/

	/* 讀取Json並建立 */
	public void ReadJson (string jsonArray) {
		List<object> nodeList = DictSO.List(jsonArray);

		if (nodeList == null) return;

		foreach (object eachNode in nodeList) {
			this.CreateNode(eachNode.ToString());
		}

	}

	/* 建立節點 */
	public ProcNode CreateNode (string json) {
		ProcNode pNode = ProcFactory.CreateNodeByJson(json, this.id);
		if (pNode == null && json.StartsWith("@")) {
			json = json.Substring(1, json.Length-1);
			pNode = ProcFactory.CreateNodeByName(json, this.id);
		}
		if (pNode == null) return null;
		//  pNode.gameObject.transform.SetParent(this.gameObject.transform);
		
		DictSO data = DictSO.Json(json);

		// 讀取/自動建立ID
		string nodeID = pNode.id;
		if (data != null) {
			if (data.ContainsKey("id")) {
				nodeID = data.GetString("id");
			} else if (data.ContainsKey("name")) {
				nodeID = data.GetString("name");
			} else {}
		}

		nodeID = this.requestID(nodeID, (tryID)=>{
			return this.nodeDict.ContainsKey(tryID) == false;
		});
		// 設置 (強制)
		pNode.SetIDByMgr(this.id, nodeID);

		this.AddNode(pNode);
		
		return pNode;
	}

	/* 建立事件 */
	public ProcEvent CreateEvent (string json) {
		ProcEvent pEvent = ProcFactory.CreateEventByJson(json, this.id);
		if (pEvent == null && json.StartsWith("@")) {
			json = json.Substring(1, json.Length-1);
			pEvent = ProcFactory.CreateEventByName(json, this.id);
		}
		if (pEvent == null) return null;

		DictSO data = DictSO.Json(json);

		// 讀取/自動建立ID
		string eventID = pEvent.id;
		if (data != null){
			if (data.ContainsKey("id")) {
				eventID = data.GetString("id");
			} else if (data.ContainsKey("name")) {
				eventID = data.GetString("name");
			} else {}
		}

		// 取得 
		eventID = this.requestID(eventID, (tryID)=>{
			return this.eventDict.ContainsKey(tryID) == false;
		});
		// 設置 (強制)
		pEvent.SetIDByMgr(this.id, eventID);

		this.AddEvent(pEvent);

		return pEvent;
	}

	/* 建立條件 */
	public ProcGate CreateGate (string json) {
		ProcGate pGate = ProcFactory.CreateGateByJson(json, this.id);
		if (pGate == null && json.StartsWith("@")) {
			json = json.Substring(1, json.Length-1);
			pGate = ProcFactory.CreateGateByName(json, this.id);
		}
		if (pGate == null) return null;

		DictSO data = DictSO.Json(json);

		// 讀取/自動建立ID
		string gateID = pGate.id;
		if (data != null){
			if (data.ContainsKey("id")) {
				gateID = data.GetString("id");
			} else if (data.ContainsKey("name")) {
				gateID = data.GetString("name");
			} else {}
		}

		gateID = this.requestID(gateID, (tryID)=>{
			return this.gateDict.ContainsKey(tryID) == false;
		});
		// 設置 (強制)
		pGate.SetIDByMgr(this.id, gateID);

		this.AddGate(pGate);

		return pGate;
	}

	/*新增=======================*/
	
	/* 新增節點 */
	public void AddNode (ProcNode pNode, bool isOverwrite = false) {

		// 若無ID 則 以 匿名 並 修正到不重複為止
		if (pNode.id == null) {
			string fix = this.requestID("_node_", (tryID)=>{
				return this.nodeDict.ContainsKey(tryID) == false;
			});
			pNode.SetIDByMgr(this.id, fix);
		}

		// 若 已存在於列表中 則 
		if (this.nodeDict.ContainsKey(pNode.id)) {
			// 若 非覆寫 則 返回
			if (isOverwrite == false) return;
			// 移除 舊有
			this.RemoveNode(pNode.id);
		}
		
		// 設置父物件(整理、瀏覽用)
		pNode.gameObject.transform.SetParent(this.gameObject.transform);
		// 加入名單
		this.nodeDict.Add(pNode.id, pNode);

		// 設置所屬Mgr
		pNode.mgrID = this.id;

		// event
		this.onNodeExist.Call(
			DictSO.New().Set("id", pNode.id)
						.Set("node", pNode)
		);
	}

	/* 新增事件 */
	public void AddEvent (ProcEvent pEvent, bool isOverwrite = false) {

		// 若無ID 則 以 匿名 並 修正到不重複為止
		if (pEvent.id == null) {
			string reqID = this.requestID("_event_", (tryID)=>{
				return this.eventDict.ContainsKey(tryID) == false;
			});
			pEvent.SetIDByMgr(this.id, reqID);
		}

		// 若 已存在於列表中 則 
		if (this.eventDict.ContainsKey(pEvent.id)) {
			// 若 非覆寫 則 返回
			if (isOverwrite == false) return;
			// 移除 舊有
			this.RemoveEvent(pEvent.id);
		}
		
		// 設置父物件(整理、瀏覽用)
		pEvent.gameObject.transform.SetParent(this.gameObject.transform);
		// 加入名單
		this.eventDict.Add(pEvent.id, pEvent);
		// 設置所屬Mgr
		pEvent.mgrID = this.id;
	}

	/* 新增條件 */
	public void AddGate (ProcGate pGate, bool isOverwrite = false) {

		// 若無ID 則 以 匿名 並 修正到不重複為止
		if (pGate.id == null) {
			string reqID = this.requestID("_gate_", (tryID)=>{
				return this.gateDict.ContainsKey(tryID) == false;
			});
			pGate.SetIDByMgr(this.id, reqID);
		}

		// 若 已存在於列表中 則 
		if (this.gateDict.ContainsKey(pGate.id)) {
			// 若 非覆寫 則 返回
			if (isOverwrite == false) return;
			// 移除 舊有
			this.RemoveGate(pGate.id);
		}
		
		// 設置父物件(整理、瀏覽用)
		pGate.gameObject.transform.SetParent(this.gameObject.transform);
		// 加入名單
		this.gateDict.Add(pGate.id, pGate);
		// 設置所屬Mgr
		pGate.mgrID = this.id;
	} 

	/*移除==========================*/

	/* 移除節點 */
	public void RemoveNode (string id) {
		if (this.nodeDict.ContainsKey(id) == false) return;
		ProcNode pNode = this.GetNode(id);
		
		this.RemoveRef(pNode);
		this.nodeDict.Remove(id);

		pNode.mgrID = null;
		pNode.OnRemove();
		
		if (pNode == null) return;
		if (pNode.gameObject != null) GameObject.Destroy(pNode.gameObject);
	}
	/* 移除事件 */
	public void RemoveEvent (string id) {
		if (this.eventDict.ContainsKey(id) == false) return;
		ProcEvent pEvent = this.GetEvent(id);
		
		this.RemoveRef(pEvent);
		this.eventDict.Remove(id);

		pEvent.mgrID = null;
		
		if (pEvent != null) GameObject.Destroy(pEvent.gameObject);
	}
	/* 移除條件 */
	public void RemoveGate (string id) {
		if (this.gateDict.ContainsKey(id) == false) return;
		ProcGate pGate = this.GetGate(id);
		
		this.RemoveRef(pGate);
		this.gateDict.Remove(id);

		pGate.mgrID = null;
		
		if (pGate != null) GameObject.Destroy(pGate.gameObject);
	}
	public void Remove (object obj) {
		string id;
		if (obj is ProcNode) {
			id = (obj as ProcNode).id;
		}
		else if (obj is ProcEvent) {
			id = (obj as ProcEvent).id;
		}
		else if (obj is ProcGate) {
			id = (obj as ProcGate).id;
		}else{
			id = "_null";
		}
		this.Remove(id);
	}
	public void Remove (string id) {
		if (this.nodeDict.ContainsKey(id)) {
			this.RemoveNode(id);
		}
		if (this.eventDict.ContainsKey(id)) {
			this.RemoveEvent(id);
		}
		if (this.gateDict.ContainsKey(id)) {
			this.RemoveGate(id);
		}
	}

	/*參考==========================*/

	/* 加入參考 */
	public void AddRef (object target, object referencer, EventListener onChanged = null) {
		if (target == null) return;
		// 若無目標紀錄則建立
		if (this.referenceDict.ContainsKey(target) == false) {
			this.referenceDict.Add(target, new List<object>());
		}

		// 加入參考者
		List<object> refList = this.referenceDict[target];
		if (refList.Contains(referencer) == false) {
			refList.Add(referencer);
		}

		if (onChanged != null) {
			
			// 加入event
			if (this.obj2OnChangeEventDict.ContainsKey(target) == false) {
				this.obj2OnChangeEventDict.Add(target, new Event());		
			}
			this.obj2OnChangeEventDict[target] += onChanged;

			// 加入參考物件與事件列表
			KeyValuePair<object, object> eventKey = new KeyValuePair<object, object>(target, referencer);
			if (this.targetAndReferencer2Event.ContainsKey(eventKey) == false) {
				this.targetAndReferencer2Event.Add(eventKey, onChanged);
			}
			
		}
	}

	/* 移除參考 */
	public void RemoveRef (object target, object referencer) {
		if (this.referenceDict.ContainsKey(target) == false) return;
		List<object> refList = this.referenceDict[target];
		if (refList.Contains(referencer) == false) return;

		//移除參考者
		refList.Remove(referencer);		
		//移除事件
		this.removeRefEvent(target, referencer);

		//若已經沒有參考者 則 移除目標
		if (refList.Count == 0) {
			//Node節點例外，其他需要被銷毀
			if (target is ProcNode) return;
			this.Remove(target);
		}	

	}
	public void RemoveRef (object referencer) {

		//要準備移除的目標
		List<object> toRemoveTarget = new List<object>();
		
		//輪詢每一個 參考目標
		foreach (KeyValuePair<object, List<object>> eachPair in this.referenceDict) {
			List<object> refList = eachPair.Value;
			object target = eachPair.Key;
			
			if (refList.Contains(referencer)) {
				//移除參考者
				refList.Remove(referencer);
				//移除事件
				this.removeRefEvent(target, referencer);
			}

			//若已經沒有參考者 則 將目標加入待移除列表
			if (refList.Count == 0) {
				toRemoveTarget.Add(eachPair.Key);
			}

		}
		
		//移除目標
		foreach (object each in toRemoveTarget) {
			this.referenceDict.Remove(each);

			//Node節點例外，其他需要被銷毀
			if (each is ProcNode) continue;

			this.Remove(each);
		}
	}

	/*事件==========================*/

	/* 註冊當節點建立 */
	public EventListener RegOnNodeExist(string id, Action<ProcNode> cb) {
		ProcNode pNode = this.GetNode(id);
		if (pNode != null) {
			cb(pNode);
			return null;
		}

		EventListener listener = null;
		listener = new EventListener((data)=>{
			if (data.GetString("id") == id) {
				cb( (ProcNode)data.Get("node"));
				this.onNodeExist -= listener;
			}
		});
		this.onNodeExist += listener;
		return listener;
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

	private string requestID(string id, Func<string, bool> passGate) {
		if (passGate(id)) {
			return id;
		}
		int maxTry = 10000;
		// 當未通過，遞增後綴數字
		int idFix = 0;
		while (!passGate(id+"_"+idFix) && maxTry-->0) {
			idFix++;
		}
		
		return id+"_"+idFix;
	}

	private ICollection<T> subtract<T> (ICollection<T> a, ICollection<T> b) {
		List<T> result = new List<T>(a);
		foreach (T each in b) {
			if (result.Contains(each)) result.Remove(each);
		}
		return result;
	}

	private void sendChangeIDEvent (object target, string oldID, string newID) {
		if (this.obj2OnChangeEventDict.ContainsKey(target) == false) return;

		DictSO data = new DictSO();

		string msg = "onIDChanged";

		data.Set("msg", msg);
		data.Set("oldID", oldID);
		data.Set("newID", newID);

		this.obj2OnChangeEventDict[target].Call(data);
	}

	private void removeRefEvent (object target, object referencer) {
		//以關聯來找尋事件
		KeyValuePair<object, object> eventKey = new KeyValuePair<object, object>(target, referencer);

		if (this.targetAndReferencer2Event.ContainsKey(eventKey) == false)
			return;

		//從目標的事件列表中 移除 該事件
		EventListener cb = this.targetAndReferencer2Event[eventKey];
		this.obj2OnChangeEventDict[target].RemoveListener(cb);
		
		//移除事件參考
		this.targetAndReferencer2Event.Remove(eventKey);
	}
}

}