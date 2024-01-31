using UnityEngine;

namespace Uzil.Proc {

public class ProcEvent_Log : ProcEvent {

	/*======================================Constructor==========================================*/

	/*=========================================Members===========================================*/

	public string type = "_default";

	public string msg;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/* 進入此事件的所屬節點時呼叫 */
	public override void Begin () {
		switch (this.type) {
			case "Error":
				Debug.LogError(this.msg);
				break;
			case "Message":
				Debug.Log(this.msg);
				break;

			default:
				Debug.Log(this.msg);
				break;
		}
	}

	/* 結束此事件的所屬節點時呼叫 */
	public override void End () {
		// 內容繼承後自定義
	}

	/* 紀錄為Json格式 */
	protected override DictSO WriteToMemo () {
		DictSO data = base.WriteToMemo();

		/* type */
		if (this.type != "_default") {
			data.Set("type", this.type);
		}

		/* msg */
		data.Set("msg", this.msg);

		return data;
	}

	/* 讀取Json格式 */
	protected override void FromMemo (DictSO data) {

		this.msg = data.GetString("msg");

		if (data.ContainsKey("type")) {
			this.type = data.GetString("type");
		}

	}

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/


}

}