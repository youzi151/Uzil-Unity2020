using UnityEngine;

namespace Uzil.Proc {

public class ProcEvent : MonoBehaviour,  IMemoable {

	/*======================================Constructor==========================================*/

	/*=========================================Members===========================================*/

	/* 辨識名稱 */
	[SerializeField]
	public string id {get; private set;}

	/* 變數 */
	public DictSO otherArgs = new DictSO();

	/* 事件類型 */
	public string typeName {
		get {
			string prefix = "ProcEvent_";
			string name = this.GetType().Name;
			if (name.StartsWith(prefix)) {
				name = name.Substring(prefix.Length, name.Length - prefix.Length);
			}
			return name;
		}
	}

	/* 是否執行完成(用於一些特殊需要判斷是否執行完畢的狀況) */
	public bool isDone = false;

	/* 所屬的 ProcMgr */
	public string mgrID = null;

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

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
			newID = this.getMgr().ChangeEventID(this.id, id);
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

	/* 進入此事件的所屬節點時呼叫 */
	public virtual void Begin(){
		//內容繼承後自定義
	}
	
	/* 結束此事件的所屬節點時呼叫 */
	public virtual void End(){
		//內容繼承後自定義
	}


	public void OnDone(){
		this.isDone = true;
	}

	/*===================================Protected Function======================================*/
	
	/* [IMemoable] 紀錄為Json格式 */
	protected virtual DictSO WriteToMemo(){
		this.otherArgs.Set("event", this.typeName);
		this.otherArgs.Set("isEventEnd", this.isDone);
		return this.otherArgs;
	}

	/* [IMemoable] 讀取Json格式 */
	protected virtual void FromMemo(DictSO data){
		this.otherArgs = data;

		if (data.ContainsKey("isEventEnd")){
			this.isDone = data.GetBool("isEventEnd");
		}
	}

	/* 取得所屬ProcMgr */
	protected ProcMgr getMgr () {
		return ProcMgr.Inst(this.mgrID);
	}

	
	/* 實際設置ID */
	protected void setID (string newID) {
		this.gameObject.name = newID;
		this.id = newID;
	}


	/*====================================Private Function=======================================*/


}

}