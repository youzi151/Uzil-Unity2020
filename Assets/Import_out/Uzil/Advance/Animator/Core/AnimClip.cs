using System.Collections.Generic;

using Uzil;

namespace Uzil.Anim {

public class AnimClip {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 識別 */
	public string id = "";

	/** 影格速率 */
	public int frameRate = 24;

	/** 是否循環 */
	public bool isLoop = false;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** [IMemoable] 紀錄為Json格式 */
	public virtual object ToMemo () {
		DictSO data = new DictSO();

		/* 名稱 */
		data.Set("id", this.id);

		/* 影格速率 */
		data.Set("frameRate", this.frameRate);

		/* 是否循環 (有限支援) */
		data.Set("isLoop", this.isLoop);		

		return data;
	}

	/** [IMemoable] 讀取Json格式 */
	public virtual void LoadMemo (object memoJson) {

		DictSO data = DictSO.Json(memoJson);

		if (data.ContainsKey("id")) {
			this.id = data.GetString("id");
		}

		if (data.ContainsKey("frameRate")) {
			this.frameRate = data.GetInt("frameRate");
		}

		if (data.ContainsKey("isLoop")) {
			this.isLoop = data.GetBool("isLoop");
		}
		
	}

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/


}

}