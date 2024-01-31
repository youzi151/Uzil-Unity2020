using System.Collections.Generic;

using UnityEngine;

using Uzil.Util;

namespace Uzil.Anim {

public class PropHandler_Bool : PropHandler {

	/*======================================Constructor==========================================*/

	public PropHandler_Bool (string propName) {
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
		bool boo = DictSO.Bool(value);
		target.SetTo(this.propName, boo, isAddtive);
	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/


}

}