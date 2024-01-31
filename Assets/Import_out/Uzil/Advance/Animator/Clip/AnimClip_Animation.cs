using System.Collections.Generic;

using Uzil;
using Uzil.Misc;

namespace Uzil.Anim {

public class AnimClip_Animation : AnimClip {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/
	
	public string animName;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** [IMemoable] 紀錄為Json格式 */
	public override object ToMemo () {
		DictSO data = (DictSO) base.ToMemo();


		return data;
	}

	/** [IMemoable] 讀取Json格式 */
	public override void LoadMemo (object memoJson) {

		DictSO data = DictSO.Json(memoJson);
		if (data == null) return;

		if (data.ContainsKey("animName")) {
			this.animName = data.GetString("animName");
		}


	}

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/


}

}