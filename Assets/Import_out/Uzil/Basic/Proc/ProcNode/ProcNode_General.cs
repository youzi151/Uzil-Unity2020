using System;
using System.Collections.Generic;

using UnityEngine;

namespace Uzil.Proc {

public class ProcNode_General : ProcNode {

	/*======================================Constructor==========================================*/

	/*=========================================Members===========================================*/

	/* 是否在條件滿足時，自動關閉此節點 */
	public bool isCloseOnComplete = true;

	/* 是否呼叫檢查 */
	// private bool isCallCheck = false;

	/*========================================Components=========================================*/

	/* 此節點會執行的事件 */
	public List<string> eventList = new List<string>();

	/* 要進行後續節點的條件 */
	public List<string> gateToNextList = new List<string>();

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Update is called once per frame
	public void Update () {
		// if (isCallCheck){
		// 	this.CheckIfComplete();
		// 	isCallCheck = false;
		// }
	}

	/*========================================Interface==========================================*/

	/* 開始此節點進程，通常受上一節點呼叫 */
	public override void Begin () {
		ProcMgr pMgr = this.getMgr();


		// 改變狀態
		this.state = ProcNodeState.Active;

		// 開始每一個事件
		foreach (string eachEvent in this.eventList) {
			ProcEvent pEvent = pMgr.GetEvent(eachEvent);
			if (pEvent == null) continue;
			pEvent.Begin();
		}

		// 開始每一個條件檢查
		foreach (string eachGate in this.gateToNextList) {
			ProcGate pGate = pMgr.GetGate(eachGate);
			if (pGate == null) {
				continue;
			}
			pGate.Reset();
			pGate.nodeID = this.id;
			pGate.Begin();
		}

		//event
		// this.onProcessBegin.Call();

		if (this.gateToNextList.Count == 0) this.CheckIfComplete();
	}

	/* 結束此節點進程 */
	public override void End () {
		ProcMgr pMgr = this.getMgr();

		if (pMgr == null) return;

		// 改變狀態
		this.state = ProcNodeState.Inactive;

		// 結束每一個事件
		foreach (string eachEvent in this.eventList) {
			ProcEvent pEvent = pMgr.GetEvent(eachEvent);
			if (pEvent != null) {
				pEvent.End();
			}
		}

		// 結束每一個條件檢查
		foreach (string eachGate in this.gateToNextList) {
			ProcGate pGate = pMgr.GetGate(eachGate);
			if (pGate != null) {
				pGate.End();
			}
		}

		// event
		// this.onProcessEnd.Call();
	}

	/* 呼叫其他事件 */
	public override void Call (string msg, DictSO data = null) {
		if (msg == "CallCheck") {
			this.CallCheck();
		}
	}

	/* 紀錄為Json格式 */
	protected override DictSO WriteToMemo () {

		/* 父類別 */
		DictSO data = base.WriteToMemo();

		/* 事件 */
		data.Set("eventList", this.eventList);

		/* 條件 */
		data.Set("gateList", this.gateToNextList);

		/* 是否達成條件後關閉 */
		data.Set("isCloseOnComplete", this.isCloseOnComplete);

		return data;
	}

	/* 讀取Json格式 */
	protected override void FromMemo (DictSO data) {

		/* 父類別 */
		base.FromMemo(data);

		/* 初始化事件 */
		if (data.ContainsKey("eventList"))
			this.tryInitEventList(data);

		/* 初始化條件 */
		if (data.ContainsKey("gateList"))
			this.tryInitGateList(data);

		/* 初始化是否達成條件後關閉 */
		if (data.ContainsKey("isCloseOnComplete")) {
			this.isCloseOnComplete = data.GetBool("isCloseOnComplete");
		}

	}

	/*=====================================Public Function=======================================*/

	/* 加入下個節點 */
	public override void AddNext (string nodeIDorStr) {
		this.addNextNodeWithRef(nodeIDorStr);
	}

	/* 移除下個節點 */
	public override void RemoveNext (string nodeID) {
		ProcMgr pMgr = this.getMgr();

		//從列表中移除============
		if (this.nextNodeList.Contains(nodeID) == false) return;
		this.nextNodeList.Remove(nodeID);

		// 移除參考================
		// 嘗試 以ID來取得 節點
		ProcNode pNode = pMgr.GetNode(nodeID);
		if (pNode == null) return;

		// 移除參考
		pMgr.RemoveRef(pNode, this);
	}

	/* 加入事件 */
	public void AddEvent (string eventIDorStr) {
		ProcMgr pMgr = this.getMgr();

		// 試著取得 事件物件==================

		// 嘗試 當作ID來取得
		ProcEvent pEvent = pMgr.GetEvent(eventIDorStr);

		// 若 不存在 則 建立事件物件
		if (pEvent == null) {
			pEvent = pMgr.CreateEvent(eventIDorStr);
		}


		// 取得ID ===========================
		string eventID;

		// 若 該事件物件 存在 則 取得ID
		if (pEvent != null) {
			eventID = pEvent.id;
		}

		// 若 該事件物件不存在 (取得失敗&&建立失敗)
		else {
			DictSO prefabData = DictSO.Json(eventIDorStr, /*isLogError*/false);
			// 若是 prefab格式 則 忽略此事件物件 (代表 該prefab發生錯誤、建立失敗)
			if (prefabData != null) {
				return;
			}
			// 不是prefab 則 直接設為ID
			else {
				eventID = eventIDorStr;
			}
		}

		// 若 已有記錄在List中 則 跳過
		if (this.eventList.Contains(eventID)) return;

		// 設置關係、註冊相關事件==============

		// 若 建立後 已經存在 則 設置
		if (pEvent != null) {

			// 建立參考關係
			EventListener cb = null;
			cb = EventListener.New((data) => {
				if (data.GetString("msg") != "onIDChanged") {
					return;
				}

				// 當ID變更，自動修正this.eventList
				string oldID = data.GetString("oldID");
				string newID = data.GetString("newID");

				int idx = this.eventList.IndexOf(oldID);
				if (idx == -1) {
					cb.RemoveSelf();
					return;
				}

				this.eventList.Insert(idx, newID);
				this.eventList.Remove(oldID);
			});

			// 新增參考
			pMgr.AddRef(/*target*/pEvent, /*user*/this, /*onChanged*/cb);

			// 放到子物件
			pEvent.gameObject.transform.SetParent(this.gameObject.transform);
		}

		// 加入ID到List======================
		this.eventList.Add(eventID);

	}

	/* 移除事件 */
	public void RemoveEvent (string eventID) {
		ProcMgr pMgr = this.getMgr();

		// 從列表中移除============
		if (this.eventList.Contains(eventID) == false) return;
		this.eventList.Remove(eventID);

		// 移除參考================
		// 嘗試 以ID來取得 事件
		ProcEvent pEvent = pMgr.GetEvent(eventID);
		if (pEvent == null) return;

		// 移除參考
		pMgr.RemoveRef(pEvent, this);
	}

	/* 加入條件 */
	public void AddGate (string gateIDorStr) {
		ProcMgr pMgr = this.getMgr();

		// 試著取得 條件物件==================

		// 嘗試 當作ID來取得
		ProcGate pGate = pMgr.GetGate(gateIDorStr);

		// 若 不存在 則 建立條件物件
		if (pGate == null) {
			pGate = pMgr.CreateGate(gateIDorStr);
			// if (pGate != null){
			// 	pGate.id = pMgr.ChangeGateID(pGate.id, this.id + pGate.id);
			// }
		}


		//取得ID ===========================
		string gateID;

		// 若 該條件物件 存在 則 取得ID
		if (pGate != null) {
			gateID = pGate.id;
		}

		// 若 該條件物件不存在 (取得失敗&&建立失敗)
		else {
			DictSO prefabData = DictSO.Json(gateIDorStr, /*isLogError*/false);
			// 若是prefab格式 則 忽略此條件物件 (代表 該prefab發生錯誤、建立失敗)
			if (prefabData != null) {
				return;
			}
			// 不是prefab則直接設為ID
			else {
				gateID = gateIDorStr;
			}
		}

		// 若 已有記錄在List中 則 跳過
		if (this.gateToNextList.Contains(gateID)) return;

		// 設置關係、註冊相關條件==============

		// 若 建立後 已經存在 則 設置
		if (pGate != null) {
			// 建立參考關係
			EventListener cb = null;
			cb = EventListener.New((data) => {
				if (data.GetString("msg") != "onIDChanged") {
					return;
				}

				// 當ID變更，自動修正this.gateToNextList
				string oldID = data.GetString("oldID");
				string newID = data.GetString("newID");

				int idx = this.gateToNextList.IndexOf(oldID);
				if (idx == -1) {
					cb.RemoveSelf();
					return;
				}

				this.gateToNextList.Insert(idx, newID);
				this.gateToNextList.Remove(oldID);
			});

			// 新增參考
			pMgr.AddRef(/*target*/pGate, /*user*/this, /*onChanged*/cb);

			// 放到子物件
			pGate.gameObject.transform.SetParent(this.gameObject.transform);
		}

		// 加入ID到List======================
		this.gateToNextList.Add(gateID);
	}

	/* 移除條件 */
	public void RemoveGate (string gateID) {
		ProcMgr pMgr = this.getMgr();

		//從列表中移除============
		if (this.gateToNextList.Contains(gateID) == false) return;
		this.gateToNextList.Remove(gateID);

		//移除參考================
		//嘗試 以ID來取得 條件
		ProcGate pGate = pMgr.GetGate(gateID);
		if (pGate == null) return;

		//移除參考
		pMgr.RemoveRef(pGate, this);
	}

	/* 檢查是否完成條件 */
	public void CheckIfComplete () {

		// 不在啟用狀態下 則 不作用
		if (this.state != ProcNodeState.Active) return;

		// 先預設 條件狀態 已完成
		bool isComplete = true;

		// 若 有條件尚未完成 則 條件狀態 改為 未完成 並跳出
		ProcMgr pMgr = this.getMgr();
		foreach (string eachGate in this.gateToNextList) {
			ProcGate pGate = pMgr.GetGate(eachGate);
			if (pGate.isComplete == false) {
				isComplete = false;
				break;
			}
		}

		// 若 已完成 則 呼叫
		if (isComplete) {
			this.onGateComplete();
		}
	}

	public void CallCheck () {
		// this.isCallCheck = true;
		this.CheckIfComplete();
	}

	/*====================================Private Function=======================================*/

	/* 當完成條件時呼叫 */
	private void onGateComplete () {
		this.state = ProcNodeState.Complete;

		// 若 設定為自動結束節點，則呼叫結束
		if (this.isCloseOnComplete) {
			this.End();
		}

		// 啟用每個下一節點
		ProcMgr pMgr = this.getMgr();
		List<string> list = new List<string>(this.nextNodeList);
		foreach (string eachID in list) {
			ProcNode pNode = pMgr.GetNode(eachID);

			if (pNode == null) continue;

			// 防無限循環
			if (pNode == this) {
				pMgr.BeginNode(pNode);
			}
			else {
				pNode.Begin();
			}
		}
	}


	/* 初始化下一節點 */
	private void tryInitNextNodeList (DictSO data) {
		try {
			List<string> nextNodeIDList = data.GetList<string>("nextNodeIDList");

			foreach (string eachID in nextNodeIDList) {
				// TODO:
				// 增加id變更追蹤
				this.AddNext(eachID);
			}

		}
		catch (Exception e) {
			Debug.Log(e);
		}
	}

	/* 初始化事件 */
	private void tryInitEventList (DictSO data) {
		
		List<string> eventList = data.GetList<string>("eventList");
		foreach (string eachEvent in eventList) {
			this.AddEvent(eachEvent);
		}

	}

	/* 初始化條件 */
	private void tryInitGateList (DictSO data) {
		
		List<string> gateList = data.GetList<string>("gateList");
		foreach (string eachGate in gateList) {
			this.AddGate(eachGate);
		}

	}

}

}