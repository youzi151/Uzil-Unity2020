using System;
using System.Collections.Generic;

using UnityEngine;

using Uzil.Util;

namespace Uzil.Proc {

public abstract class ProcNode : MonoBehaviour, IMemoable {

	/*======================================Constructor==========================================*/

	/*=========================================Members===========================================*/

	/* 節點ID */
	[SerializeField]
	public string id {get; private set;}

	/* 節點狀態 */
	public ProcNodeState state = ProcNodeState.Inactive;

	/* 節點類型 */
	public string typeName {
		get {
			string prefix = "ProcNode_";
			string name = this.GetType().Name;
			if (name.StartsWith(prefix)) {
				name = name.Substring(prefix.Length, name.Length - prefix.Length);
			}
			return name;
		}
	}

	/* 所屬ProcMgr */
	public string mgrID = null;

	/*========================================Components=========================================*/

	/* 後續的節點ID */
	public List<string> nextNodeList = new List<string>();

	/*==========================================Event============================================*/
	
	// 為了能夠Memo暫時不使用
	// public Callback onProcessBegin = new Callback();
	// public Callback onProcessEnd = new Callback();

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/* [IMemoable] 紀錄為Json格式 */
	public object ToMemo () {
		return this.WriteToMemo();
	}
	
    /* [IMemoable] 讀取Json格式 */
	public void LoadMemo (object memoJson) {
		DictSO data = DictSO.Json(memoJson);
		this.FromMemo(data);
	}

	/*=====================================Public Function=======================================*/

	/*== 設定 =============*/	

	/* 設定ID */
	public string SetID (string id) {
		string newID;
		// 若 已存在ID 且 有所屬Mgr 則 請求更名
		if (this.id != null && this.mgrID != null) {
			newID = this.getMgr().ChangeNodeID(this.id, id);
			// this.setID(newID); // 讓Mgr來實際執行
		} 
		// 否則直接設置
		else {
			newID = id;
			this.setID(newID);
		}
		return newID;
	}

	/* 設定ID (受Mgr設置，需要對應)*/
	public void SetIDByMgr (string mgrID, string id) {
		if (this.mgrID != null && this.mgrID != mgrID) return;
		this.setID(id);
	}

	/*== 使用 =============*/	

	/* 開始此節點進程，通常受上一節點呼叫 */
	public virtual void Begin () {
		//內容繼承後自定義
	}
	
	/* 結束此節點進程 */
	public virtual void End () {
		//內容繼承後自定義
	}

	/* 加入下個節點 */
	public virtual void AddNext (string id) {
		if (this.nextNodeList.Contains(id)) return;
		this.nextNodeList.Add(id);
	}

	/* 移除下個節點 */
	public virtual void RemoveNext (string id) {
		this.nextNodeList.Remove(id);
	}

	/* 被移除時呼叫 */
	public virtual void OnRemove () {
		
	}

	/* 呼叫其他事件 */
	public virtual void Call (string msg, DictSO data = null) {
		// 繼承後自定義
	}

	protected virtual DictSO WriteToMemo () {
		DictSO data = new DictSO();

		/* 節點類型 */
		data.Set("node", this.typeName);

		/* 狀態 */
		data.Set("state", EnumUtil.ToString(this.state));


		/* 下個節點列表 */
		data.Set("nextNodeList", this.nextNodeList);
		
		return data;
	}

	protected virtual void FromMemo (DictSO data) {

		/* 狀態 */
		if (data.ContainsKey("state")) {
			this.state = EnumUtil.Parse<ProcNodeState>(data.GetString("state"));
		}
		

		/* 下個節點列表 */
		if (data.ContainsKey("nextNodeList")) {
			this.nextNodeList.Clear();
			List<object> nextNodeIDList_obj = data.GetList("nextNodeList");
			foreach (object each in nextNodeIDList_obj) {
				if (this.nextNodeList.Contains(each.ToString()) == false){
					this.AddNext(each.ToString());
				}
			}
		}
		
		return;
	}



	/*===================================Protected Function======================================*/
	
	/* 取得所屬ProcMgr */
	protected ProcMgr getMgr () {
		return ProcMgr.Inst(this.mgrID);
	}

	/* (選擇性使用) 新增節點 並 寫入參考 */
	protected void addNextNodeWithRef (string nodeIDorStr) {
		ProcMgr procMgr = this.getMgr();

		// 試著取得 節點物件==================

		// 嘗試 當作ID來取得
		ProcNode procNode = procMgr.GetNode(nodeIDorStr);

		// 若不存在 則 建立節點物件
		if (procNode == null) {
			procNode = procMgr.CreateNode(nodeIDorStr);
		} else {}

		// 取得ID ===========================
		string nodeID;

		// 若 該節點物件 存在 則 取得ID
		if (procNode != null) {
			nodeID = procNode.id;
		}

		// 若該節點物件不存在 (取得失敗&&建立失敗)
		else {
			// 若是prefab格式，則 忽略此節點物件 (代表 該prefab發生錯誤、建立失敗)
			DictSO prefabData = DictSO.Json(nodeIDorStr, /*isLogError*/false);
			if (prefabData != null) {
				Debug.Log("[ProcNode.addNextNodeWithRef] : Json PrefabFail. str:"+nodeIDorStr);
				return;
			}

			// 不是prefab則直接設為ID
			else {
				nodeID = nodeIDorStr;	
			}
		}

		// 若 已有記錄在List中 則 跳過
		if (this.nextNodeList.Contains(nodeID)) return;

		// 設置關係、註冊相關節點==============

		// 若 建立後 已經存在 則 設置
		if (procNode != null){
			// 建立參考關係
			EventListener cb = null;
			cb = EventListener.New((data)=>{

				if (data.GetString("msg") != "onIDChanged") {
					return;
				}

				// 當ID變更，自動修正this.nextNodeList
				string oldID = data.GetString("oldID");
				string newID = data.GetString("newID");

				int idx = this.nextNodeList.IndexOf(oldID);
				
				// 如果新ID不存在 則 加入，已存在 則 忽略
				if (this.nextNodeList.Contains(newID) == false){
					this.nextNodeList.Insert(idx, newID);
					this.nextNodeList.Remove(oldID);
				}
			});
			procMgr.AddRef(/*target*/procNode, /*user*/this, /*onChanged*/cb);
		}

		// 加入ID到List======================
		this.nextNodeList.Add(nodeID);
	}

	/* 實際設置ID */
	protected void setID (string newID) {
		this.gameObject.name = newID;
		this.id = newID;
	}

	/*====================================Private Function=======================================*/

}


}