using System.Collections.Generic;

using UnityEngine;

using Uzil.Util;

namespace Uzil.Anim {

public class PropHandler_String : PropHandler {

	/*======================================Constructor==========================================*/

	public PropHandler_String (string propName) {
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
		string str = value.ToString();
		target.SetTo(this.propName, str, isAddtive);
	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/


}

}