using System.Collections.Generic;

using UnityEngine;

using Uzil.Util;
using Uzil.ObjInfo;

namespace Uzil.Anim {

public class PropHandler_Texture : PropHandler {

	/*======================================Constructor==========================================*/

	public PropHandler_Texture (string propName) {
		this.propName = propName;
	}

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 設置至目標 */
	public override void SetToTarget (PropTarget target, object value, bool isAddtive = false) {
		TextureInfo texInfo = (TextureInfo) value;
		target.SetTo(this.propName, texInfo, isAddtive);
	}

	/** 讀取從可記錄格式 */
	public override object ValueFrom (object value) {
		TextureInfo texInfo;
		DictSO texInfoData = DictSO.Json(value);
		if (texInfoData == null) {
			texInfo = new TextureInfo(value.ToString());
		} else {
			texInfo = new TextureInfo(texInfoData);
		}
		return texInfo;
	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/


}

}