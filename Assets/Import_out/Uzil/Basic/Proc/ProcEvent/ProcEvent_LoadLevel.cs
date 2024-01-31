using Uzil.SceneLevel;

namespace Uzil.Proc {

public class ProcEvent_LoadLevel : ProcEvent {

	/*======================================Constructor==========================================*/

	/*=========================================Members===========================================*/

	/* 要讀取的場景 */
	public string levelPrepareToLoad = "_null";

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/* 進入此事件的所屬節點時呼叫 */
	public override void Begin () {
		if (this.levelPrepareToLoad == "_null") return;
		if (this.levelPrepareToLoad == null || this.levelPrepareToLoad == "") return;

		SceneLevelMgr.Load(this.levelPrepareToLoad);
	}

	/* 結束此事件的所屬節點時呼叫 */
	public override void End () {
		// 內容繼承後自定義
	}

	/* 紀錄為Json格式 */
	protected override DictSO WriteToMemo () {
		DictSO data = base.WriteToMemo();

		/*msg*/
		data.Set("levelName", this.levelPrepareToLoad);

		return data;
	}

	/* 讀取Json格式 */
	protected override void FromMemo (DictSO data) {

		if (data.ContainsKey("levelName")) {
			this.levelPrepareToLoad = data.GetString("levelName");
		}

	}

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/


}

}