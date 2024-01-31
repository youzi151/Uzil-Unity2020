using System;
using System.Collections.Generic;

using UnityEngine;

namespace Uzil.Proc {

public enum GroupRunType {
	Waterfall,
	Parallel
}

public class ProcNode_Group : ProcNode {

	/*======================================Constructor==========================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/* 執行方式  (waterfall:依序執行, parallel:平行執行) */
	private GroupRunType runType = GroupRunType.Waterfall;/*waterfall or parallel*/
	/* 此節點會執行的事件 */
	public List<string> nodeList = new List<string>();

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

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
		ProcNode pNode = pMgr.GetNode(nodeID);
		if (pNode == null) return;

		// 移除參考
		pMgr.RemoveRef(pNode, this);
	}

	/* 開始此節點進程，通常受上一節點呼叫 */
	public override void Begin () {
		ProcMgr pMgr = this.getMgr();

		this.SetRunType(this.runType);

		if (this.nodeList.Count == 0) return;

		if (this.runType == GroupRunType.Waterfall) {

			ProcNode node = pMgr.GetNode(this.nodeList[0]);
			node.Begin();

		}
		else if (this.runType == GroupRunType.Parallel) {

			foreach (string nodeID in this.nodeList) {
				ProcNode node = pMgr.GetNode(nodeID);
				node.Begin();
			}

		}

	}

	/* 結束此節點進程 */
	public override void End () {
		ProcMgr pMgr = this.getMgr();

		if (this.nodeList.Count == 0) return;

		if (this.runType == GroupRunType.Waterfall) {

			foreach (string nodeID in this.nodeList) {

				ProcNode node = pMgr.GetNode(nodeID);

				if (node.state == ProcNodeState.Active) {
					node.End();
				}
			}

		}
		else if (this.runType == GroupRunType.Parallel) {

			foreach (string nodeID in this.nodeList) {
				ProcNode node = pMgr.GetNode(nodeID);
				node.End();
			}

		}

	}

	public override void OnRemove () {
		ProcMgr pMgr = this.getMgr();
		foreach (string eachID in this.nodeList) {
			pMgr.RemoveNode(eachID);
		}
	}

	/* 紀錄為Json格式 */
	protected override DictSO WriteToMemo () {

		/* 父類別 */
		DictSO data = base.WriteToMemo();

		/* 執行類型 */
		data.Set("runType", DictSO.EnumTo(this.runType));

		/* 事件 */
		data.Set("nodeList", this.nodeList);

		return data;
	}

	/* 讀取Json格式 */
	protected override void FromMemo (DictSO data) {

		/* 父類別 */
		base.FromMemo(data);

		/* 初始化事件 */
		if (data.ContainsKey("nodeList")) {
			this.tryInitNodeList(data);
		}

		/* 執行類型 */
		if (data.ContainsKey("runType")) {
			this.SetRunType(data.GetEnum<GroupRunType>("runType"));
		}

	}

	/*=====================================Public Function=======================================*/

	/* 設置執行類型 */
	public void SetRunType (GroupRunType runType) {

		this.runType = runType;

		ProcMgr pMgr = this.getMgr();

		// 每一個節點
		for (int i = 0; i < (this.nodeList.Count - 1/* 不計最後一個 */); i++) {

			// TODO : 子Node尚未建立卻被呼叫
			string nodeID = this.nodeList[i];
			string nextNodeID = this.nodeList[i + 1];

			// 向ProcessManager註冊 當該ID的節點建立 則 設定關聯
			pMgr.RegOnNodeExist(nodeID, (node) => {
				if (runType == GroupRunType.Waterfall) {
					node.AddNext(nextNodeID);
				}
				else if (runType == GroupRunType.Parallel) {
					node.RemoveNext(nextNodeID);
				}
				else { }
			});

		}

		// 向ProcessManager註冊，當該ID的節點(最後一個)建立
		if (this.nodeList.Count > 0) {
			pMgr.RegOnNodeExist(this.nodeList[this.nodeList.Count - 1], (node) => {
				// 若是 流水式 則 把此Group節點的後續節點，設至 Group中的最後一個節點 的後續節點
				if (runType == GroupRunType.Waterfall) {
					foreach (string nodeID in this.nextNodeList) {
						node.AddNext(nodeID);
					}
				}
				else if (runType == GroupRunType.Parallel) {
					foreach (string nodeID in this.nextNodeList) {
						node.RemoveNext(nodeID);
					}
					// TODO : 向此Group節點中的所有節點，註冊一個檢查器，若所有的節點都完成時 才 執行此Group的後續節點
				}
				else { }
			});
		}
	}

	/*====================================Private Function=======================================*/

	/* 初始化事件 */
	private void tryInitNodeList (DictSO data) {
		try {

			ProcMgr pMgr = this.getMgr();

			// 有順序性，所以要先清空
			this.nodeList.Clear();

			// 輪巡所有節點
			List<string> nodeList_list = data.GetList<string>("nodeList");
			foreach (string eachNode in nodeList_list) {

				// 試著取得 節點物件==================

				// 嘗試 當作ID來取得
				ProcNode pNode = pMgr.GetNode(eachNode);

				// 若 不存在 則 當作Json{}來建立節點物件
				if (pNode == null) {
					pNode = pMgr.CreateNode(eachNode);
					// if (pNode != null){
					// 	pNode.id = pMgr.ChangeNodeID(pNode.id, this.id + pNode.id);
					// 	Debug.Log(this.id + pNode.id);
					// }
				}
				else { }


				// 取得ID ===========================
				string nodeID;

				// 若 該節點物件 存在 則 取得ID
				if (pNode != null) {
					nodeID = pNode.id;
				}

				// 若該節點物件不存在 (取得失敗&&建立失敗)
				else {
					// 若是 prefab格式，則 忽略此節點物件 (代表 該prefab發生錯誤、建立失敗)
					DictSO prefabData = DictSO.Json(eachNode, /*isLogError*/false);
					if (prefabData != null) {
						Debug.Log("[ProcNode_Group] : Json PrefabFail. str:" + eachNode);
						continue;
					}

					// 不是 prefab 則 直接設為ID
					else {
						nodeID = eachNode;
					}
				}

				// 若 尚未記錄在List中 則 加入ID到List
				if (this.nodeList.Contains(nodeID) == false) {
					this.nodeList.Add(nodeID);
				}

				if (pNode != null) {
					pNode.gameObject.transform.SetParent(this.gameObject.transform);
				}
				else {
					pMgr.RegOnNodeExist(nodeID, (node) => {
						node.gameObject.transform.SetParent(this.gameObject.transform);
					});
				}

			}

		}
		catch (Exception e) {
			Debug.Log(e);
			Debug.Log(data["nodeList"].ToString());
			Debug.Log("[ProcNode_Group] : InitByJson Node error");
		}

	}

}

}