using System.Collections.Generic;

using UnityEngine;

namespace Uzil.Anim {

public class PropHandler_Vector2List : PropHandler {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** 設置至目標 */
	public override void SetToTarget (PropTarget target, object value, bool isAddtive = false) {
		List<Vector2> points = (List<Vector2>) value;
		target.SetTo(this.propName, points.ToArray(), isAddtive);
	}

	/** 讀取從可記錄格式 */
	public override object ValueFrom (object value) {
		return DictSO.List<Vector2>(value);
	}

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/


}

}