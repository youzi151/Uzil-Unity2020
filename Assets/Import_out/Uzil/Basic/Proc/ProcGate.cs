using UnityEngine;

namespace Uzil.Proc {

public class ProcGate : MonoBehaviour, IMemoable {

	/*======================================Constructor==========================================*/

	/*=========================================Members===========================================*/

	/* 辨識名稱 */
	[SerializeField]
	public string id {get; private set;}

	/* 是否正在偵聽 */
	protected bool _isListening = false;
	public bool isListening {
		get {
			return this._isListening;
		}
	}
	
	/* 是否已滿足條件 */
	protected bool _isComplete = false;
	public bool isComplete {
		get {
			//最後一次檢查
			// this.CheckIfComplete();//不使用，避免與ProcessNode造成循環呼叫
			return this._isComplete;
		}
	}

	/* 所屬的 ProcMgr */
	public string mgrID = null;

	// [HideInInspector] 
	/* 所屬的 node ID */
	//用於通知 Check條件
	public string nodeID;


	/*變數*/
	public DictSO otherArgs;

	/* 條件類型 */
	public string typeName {
		get {
			string prefix = "ProcGate_";
			string name = this.GetType().Name;
			if (name.StartsWith(prefix)) {
				name = name.Substring(prefix.Length, name.Length - prefix.Length);
			}
			return name;
		}
	}

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/
	
	// public Event onComplete = new Event();
	// public Event onBegin = new Event();
	// public Event onPause = new Event();
	// public Event onResume = new Event();
	// public Event onEnd = new Event();

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
			newID = this.getMgr().ChangeGateID(this.id, id);
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
	
	//NOTE : 每一個函式呼叫內容順序不儘一樣，因為要符合使用者期待。
	
	/*開始偵聽*/
	public void Begin () {
		this._isListening = true;
		this._begin();
		// this.onBegin.Call();//event
	}

	/*暫停偵聽*/
	public void Pause () {
		this._isListening = false;
		this._pause();
		// this.onPause.Call();//event
	}

	/*繼續偵聽*/
	public void Resume () {
		this._resume();
		// this.onResume.Call();//event
		this._isListening = true;
	}
	
	/*停止偵聽*/
	public void End () {
		this._isListening = false;
		this._end();
		// this.onEnd.Call();//event
	}

	/*重置此條件*/
	public void Reset () {
		this._isComplete = false;
		this._reset();
	}

	/*檢查是否已完成*/
	public virtual void CheckIfComplete () {
		//內容繼承後自定義
		//例如:
		if (this.isListening){
			//作某種檢查
			
			//若沒通過
			// return;

			//若通過
			this.complete();
		}
	}

	public void Complete () {
		this.complete();
	}

	/*===================================Protected Function======================================*/

	/* 設置為完成 */
	protected void complete () {
		// Debug.Log("[ProcCondition]: ["+this.id+"] : isComplete");
		this._isComplete = true;
		// this.onComplete.Call(); //event
		if (this.nodeID != null){
			ProcNode node = this.getMgr().GetNode(this.nodeID);
			
			if (node != null){
				node.Call("CallCheck");
			}
		}

	}
	/* 設置為未完成 */
	protected void uncomplete () {
		this._isComplete = false;
	}


	/* 開始偵聽 */
	protected virtual void _begin () {
		//內容繼承後自定義
	}

	/* 暫停偵聽 */
	protected virtual void _pause () {
		//內容繼承後自定義
	}

	/* 繼續偵聽 */
	protected virtual void _resume () {
		//內容繼承後自定義
	}
	
	/* 暫停偵聽 */
	protected virtual void _end () {
		//內容繼承後自定義
	}

	/* 重置此條件 */
	protected virtual void _reset () {
		//內容繼承後自定義
	}

	/* 總是檢查 */
	//若符合條件，則設為完成。
	//直到下一幀會自動復原為未完成，並再檢查一次
	protected void check_always () {
		if (this._isComplete) this.Reset();

		this.CheckIfComplete();
	}


	/* 紀錄為Json格式 */
	protected virtual DictSO WriteToMemo () {
		DictSO data = new DictSO();
		data.Set("isListening", this._isListening);
		data.Set("isComplete", this._isComplete);
		data.Set("otherArgs", this.otherArgs);
		data.Set("node", this.nodeID);
		data.Set("gate", this.typeName);
		return data;
	}

	/* 讀取Json格式 */
	protected virtual void FromMemo (DictSO data) {
		this.nodeID = data.GetString("node");
		this.otherArgs = data.GetDictSO("otherArgs");

		bool isListening = data.GetBool("isListening");
		bool isComplete = data.GetBool("isComplete");
		if (isComplete) {
			
		} else {
			if (isListening) {
				this.Resume();
			}else{
				this.Pause();
			}
		}

		this._isListening = isListening;
		this._isComplete = isComplete;
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