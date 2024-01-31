using Uzil;
using Uzil.Proc;

namespace UZAPI {

public class Proc {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*==節點===================*/

	/** 節點開始 */
	public static void NodeBegin (string pMgrKey, string nodeID) {
		ProcNode node = ProcMgr.Inst(pMgrKey).GetNode(nodeID);
		if (node == null) return;
		node.Begin();
	}

	/** 節點開始 */
	public static void NodeEnd (string pMgrKey, string nodeID) {
		ProcNode node = ProcMgr.Inst(pMgrKey).GetNode(nodeID);
		if (node == null) return;
		node.End();
	}

	/** 創建節點 */
	public static string NodeSet (string pMgrKey, string nodeID, string data_json) {
		ProcMgr pMgr = ProcMgr.Inst(pMgrKey);
		
		// 取得現有
		ProcNode pNode = pMgr.GetNode(nodeID);

		// 若不存在 則 建立
		if (pNode == null) {
			pNode = pMgr.CreateNode(data_json);
		}
		// 存在 則 讀入資料
		else {
			DictSO data = DictSO.Json(data_json);
			if (data != null){
				if (data.ContainsKey("param")) {
					data = data.GetDictSO("param");
				}
				pNode.LoadMemo(data);
			}
		}

		// 若依然不存在 則 返回 空
		if (pNode == null) return null;

		// 檢查並設置ID
		string id = pNode.id;
		if (id != nodeID && nodeID != null) {
			id = pMgr.ChangeNodeID(pNode.id, nodeID);
		}

		// 返回
		return id;
	}

	/** 取得節點資料 */
	public static string NodeGet (string pMgrKey, string nodeID) {
		ProcMgr pMgr = ProcMgr.Inst(pMgrKey);
		ProcNode pNode = pMgr.GetNode(nodeID);
		if (pNode == null) return null;

		return ((DictSO) pNode.ToMemo()).ToJson();
	}

	/** 移除節點 */
	public static void NodeDel (string pMgrKey, string nodeID) {
		ProcMgr pMgr = ProcMgr.Inst(pMgrKey);
		pMgr.RemoveNode(nodeID);
	}
	
	/*== 事件 ===============*/

	/** 創建事件 */
	public static string EventSet (string pMgrKey, string eventID, string data_json) {
		ProcMgr pMgr = ProcMgr.Inst(pMgrKey);
		
		// 取得現有
		ProcEvent pEvent = pMgr.GetEvent(eventID);

		// 若不存在 則 建立
		if (pEvent == null) {
			pEvent = pMgr.CreateEvent(data_json);
		}
		// 存在 則 讀入資料
		else {
			DictSO data = DictSO.Json(data_json);
			if (data != null){
				if (data.ContainsKey("param")) {
					data = data.GetDictSO("param");
				}
				pEvent.LoadMemo(data);
			}
		}

		// 若依然不存在 則 返回 空
		if (pEvent == null) return null;

		// 檢查並設置ID
		string id = pEvent.id;
		if (id != eventID && eventID != null) {
			id = pMgr.ChangeEventID(pEvent.id, eventID);
		}

		// 返回
		return id;
	}

	/** 取得事件資料 */
	public static string EventGet (string pMgrKey, string eventID) {
		ProcMgr pMgr = ProcMgr.Inst(pMgrKey);
		ProcEvent pEvent = pMgr.GetEvent(eventID);
		if (pEvent == null) return null;

		return ((DictSO) pEvent.ToMemo()).ToJson();
	}

	/** 移除事件 */
	public static void EventDel (string pMgrKey, string eventID) {
		ProcMgr pMgr = ProcMgr.Inst(pMgrKey);
		pMgr.RemoveEvent(eventID);
	}

	/*==條件===================*/

	/** 條件開始 */
	public static void GateComplete (string pMgrKey, string gateID) {
		ProcGate gate = ProcMgr.Inst(pMgrKey).GetGate(gateID);
		if (gate == null) return;
		gate.Complete();
	}

	/** 條件檢查 */
	public static void GateCheck (string pMgrKey, string gateID) {
		ProcGate gate = ProcMgr.Inst(pMgrKey).GetGate(gateID);
		if (gate == null) return;
		gate.CheckIfComplete();
	}

	/** 創建條件 */
	public static string GateSet (string pMgrKey, string gateID, string data_json) {
		ProcMgr pMgr = ProcMgr.Inst(pMgrKey);
		
		// 取得現有
		ProcGate pGate = pMgr.GetGate(gateID);

		// 若不存在 則 建立
		if (pGate == null) {
			pGate = pMgr.CreateGate(data_json);
		}
		// 存在 則 讀入資料
		else {
			DictSO data = DictSO.Json(data_json);
			if (data != null){
				if (data.ContainsKey("param")) {
					data = data.GetDictSO("param");
				}
				pGate.LoadMemo(data);
			}
		}

		// 若依然不存在 則 返回 空
		if (pGate == null) return null;

		// 檢查並設置ID
		string id = pGate.id;
		if (id != gateID && gateID != null) {
			id = pMgr.ChangeGateID(pGate.id, gateID);
		}

		// 返回
		return id;
	}

	/** 取得事件資料 */
	public static string GateGet (string pMgrKey, string gateID) {
		ProcMgr pMgr = ProcMgr.Inst(pMgrKey);
		ProcGate pGate = pMgr.GetGate(gateID);
		if (pGate == null) return null;

		return ((DictSO) pGate.ToMemo()).ToJson();
	}

	/** 移除條件 */
	public static void GateDel (string pMgrKey, string gateID) {
		ProcMgr pMgr = ProcMgr.Inst(pMgrKey);
		pMgr.RemoveGate(gateID);
	}


	/*== 條件 時機 ===========*/

	/** 條件開始 */
	public static void GateBegin (string pMgrKey, string gateID) {
		ProcGate gate = ProcMgr.Inst(pMgrKey).GetGate(gateID);
		if (gate == null) return;
		gate.Begin();
	}
	/** 條件暫停 */
	public static void GatePause (string pMgrKey, string gateID) {
		ProcGate gate = ProcMgr.Inst(pMgrKey).GetGate(gateID);
		if (gate == null) return;
		gate.Pause();
	}
	/** 條件繼續 */
	public static void GateResume (string pMgrKey, string gateID) {
		ProcGate gate = ProcMgr.Inst(pMgrKey).GetGate(gateID);
		if (gate == null) return;
		gate.Resume();
	}
	/** 條件結束 */
	public static void GateEnd (string pMgrKey, string gateID) {
		ProcGate gate = ProcMgr.Inst(pMgrKey).GetGate(gateID);
		if (gate == null) return;
		gate.End();
	}
	/** 條件重置 */
	public static void GateReset (string pMgrKey, string gateID) {
		ProcGate gate = ProcMgr.Inst(pMgrKey).GetGate(gateID);
		if (gate == null) return;
		gate.Reset();
	}

	/*條件 參數===============*/
	/** 條件取得參數 */
	public static string GateGetArgs (string pMgrKey, string gateID) {
		ProcGate gate = ProcMgr.Inst(pMgrKey).GetGate(gateID);
		if (gate == null) return "{}";
		return gate.otherArgs.ToJson();
	}
	/** 條件設置參數 */
	public static void GateSetArgs (string pMgrKey, string gateID, string argsJson) {
		ProcGate gate = ProcMgr.Inst(pMgrKey).GetGate(gateID);
		if (gate == null) return;
		gate.otherArgs = DictSO.Json(argsJson);
	}




	public static string Debug_Log (string pMgrKey) {
		return ProcMgr.Inst(pMgrKey).ToMemo().ToString();
	}

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}

}