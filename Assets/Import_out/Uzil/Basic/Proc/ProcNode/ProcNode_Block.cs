using System;
using System.Collections.Generic;

using UnityEngine;

namespace Uzil.Proc {

/*
 * 用於在ProcessEvent執行完(在otherArgs存上一個"isDone":true的變數)後才
 * 自動進行下一節點，而不是檢查ProcessCondition
 */

public class ProcNode_Block : ProcNode {

	/*======================================Constructor==========================================*/

	/*=========================================Members===========================================*/

	/* 是否在條件滿足時，自動關閉此節點 */
	public bool isCloseOnComplete = false;

	/*========================================Components=========================================*/

	/* 此節點會執行的事件 */
	public List<string> eventList = new List<string>();

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Update is called once per frame
	public void Update () {
		this.CheckIfComplete();
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

		// event
		// this.onProcessBegin.Call();

		if (this.eventList.Count == 0) this.CheckIfComplete();
	}

	/* 結束此節點進程 */
	public override void End () {
		ProcMgr pMgr = this.getMgr();

		// 改變狀態
		this.state = ProcNodeState.Inactive;

		// 結束每一個事件
		foreach (string eachEvent in this.eventList) {
			ProcEvent pEvent = pMgr.GetEvent(eachEvent);
			pEvent.End();
		}

		// event
		// this.onProcessEnd.Call();
	}

	/* 紀錄為Json格式 */
	protected override DictSO WriteToMemo () {

		/* 父類別 */
		DictSO data = base.WriteToMemo();

		/* 事件 */
		data.Set("eventList", this.eventList);

		/* 是否達成條件後關閉 */
		data.Set("isCloseOnComplete", this.isCloseOnComplete ? 1 : 0);

		return data;
	}

	/* 讀取Json格式 */
	protected override void FromMemo (DictSO data) {

		/* 父類別 */
		base.FromMemo(data);

		/* 初始化事件 */
		if (data.ContainsKey("eventList")) {
			this.tryInitEventList(data);
		}

		/* 初始化是否達成條件後關閉 */
		if (data.ContainsKey("isCloseOnComplete")) {
			this.isCloseOnComplete = data.GetInt("isCloseOnComplete") > 0 ? true : false;
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

		// 從列表中移除============
		if (this.nextNodeList.Contains(nodeID) == false) return;
		this.nextNodeList.Remove(nodeID);

		// 移除參考================
		// 嘗試 以ID來取得 節點
		ProcNode processNode = pMgr.GetNode(nodeID);
		if (processNode == null) return;

		// 移除參考
		pMgr.RemoveRef(processNode, this);
	}

	/* 加入事件 */
	public void AddEvent (string eventIDorStr) {
		ProcMgr pMgr = this.getMgr();

		// 試著取得 事件物件==================

		// 嘗試 當作ID來取得
		ProcEvent processEvent = pMgr.GetEvent(eventIDorStr);

		// 若 不存在 則 建立事件物件
		if (processEvent == null) {
			processEvent = pMgr.CreateEvent(eventIDorStr);
		}
		else { }


		//取得ID ===========================
		string eventID;

		//若 該事件物件 存在 則 取得ID
		if (processEvent != null) {
			eventID = processEvent.id;
		}

		//若該事件物件不存在 (取得失敗&&建立失敗)
		else {
			// 若是 prefab格式 則 忽略此事件物件 (代表 該prefab發生錯誤、建立失敗)
			DictSO prefabData = DictSO.Json(eventIDorStr, /*isLogError*/false);
			if (prefabData != null) return;

			// 不是prefab 則 直接設為ID
			else {
				eventID = eventIDorStr;
			}
		}

		// 若 已有記錄在List中 則 跳過
		if (this.eventList.Contains(eventID)) return;

		// 設置關係、註冊相關事件==============

		// 若 建立後 已經存在 則 設置
		if (processEvent != null) {

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
			pMgr.AddRef(/*target*/processEvent, /*user*/this, /*onChanged*/cb);

			// 放到子物件
			processEvent.gameObject.transform.SetParent(this.gameObject.transform);
		}

		//加入ID到List======================
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
		ProcEvent procEvent = pMgr.GetEvent(eventID);
		if (procEvent == null) return;

		// 移除參考
		pMgr.RemoveRef(procEvent, this);
	}

	/* 檢查是否完成事件 */
	public void CheckIfComplete () {

		// 不在啟用狀態下則不作用
		if (this.state != ProcNodeState.Active) return;

		// 先預設 條件狀態 已完成
		bool isComplete = true;

		// 若有條件尚未完成，則 條件狀態 改為 未完成，並跳出
		ProcMgr pMgr = this.getMgr();
		foreach (string eachEvent in this.eventList) {
			ProcEvent pEvent = pMgr.GetEvent(eachEvent);

			// 若 有完成 則 檢查下一個
			if (pEvent.isDone) {
				continue;
			}

			// 視為未完成事件
			isComplete = false;
			break;
		}

		// 若 已完成 則 呼叫
		if (isComplete) {
			this.onEventComplete();
		}
	}


	/*====================================Private Function=======================================*/

	/* 當完成條件時呼叫 */
	private void onEventComplete () {
		this.state = ProcNodeState.Complete;

		// 若 設定為自動結束節點 則 呼叫結束
		if (isCloseOnComplete) {
			this.End();
		}

		// 啟用每個下一節點
		ProcMgr procMgr = this.getMgr();
		foreach (string eachID in this.nextNodeList) {
			ProcNode node = procMgr.GetNode(eachID);

			if (node == null) continue;

			// 防無限循環
			if (node == this) {
				procMgr.BeginNode(node);
			}
			else {
				node.Begin();
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

	/*初始化事件*/
	private void tryInitEventList (DictSO data) {
		// try{
		// 輪詢所有事件
		List<string> eventList = data.GetList<string>("eventList");
		foreach (string eachEvent in eventList) {
			this.AddEvent(eachEvent);
		}
		// }catch(Exception e){
		// 	Debug.Log(e);
		// 	Debug.Log(data["eventList"].ToString());
		// 	Debug.Log("[ProcNode_General] : InitByJson event error");
		// }

	}


	/*初始化參數*/
	private void tryInitIsCloseOnComplete (DictSO data) {
		try {
			string isCloseOnComplete_str = data["isCloseOnComplete"].ToString();

			if (isCloseOnComplete_str == "true") {
				this.isCloseOnComplete = true;
			}
			else if (isCloseOnComplete_str == "false") {
				this.isCloseOnComplete = false;
			}

		}
		catch (Exception e) {
			Debug.Log(e);
			Debug.Log("[ProcNode_General] : InitByJson isCloseOnComplete error");
		}
	}
}


}