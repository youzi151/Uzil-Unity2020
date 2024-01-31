using System.Collections.Generic;

using UnityEngine;

using Uzil.Util;

namespace Uzil.Anim {

public class PropHandler_Float : PropHandler {

	/*======================================Constructor==========================================*/

	public PropHandler_Float (string propName) {
		this.propName = propName;
	}

	/*=====================================Static Members========================================*/
	protected const float STEP = 0.00048828125f;
	protected const int STEP_POWCOUNT = 10;

	/*=====================================Static Funciton=======================================*/

	public static float Calculate (Vector2 a, Vector2 b, Vector2 easeOut, Vector2 easeIn, float percent) {
		if (easeOut == Vector2.zero && easeIn == Vector2.zero) {
			return Mathf.Lerp(a.y, b.y, percent);
		}

		float frameLength = b.x - a.x;

		float p2x = /* 0f +  */(easeOut.x / frameLength);
		float p3x = 1f + (easeIn.x / frameLength);

		float p1y = a.y;
		float p2y = a.y + easeOut.y;
		float p3y = b.y + easeIn.y;
		float p4y = b.y;

		float t = BezierUtil.GetT(/* p2 */p2x, /* p3 */p3x, /* x */percent, PropHandler_Float.STEP_POWCOUNT, PropHandler_Float.STEP);
	
		float res = BezierUtil.GetPoint(p1y, p2y, p3y, p4y, t);
		return res;
	}

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 設置至目標 */
	public override void SetToTarget (PropTarget target, object value, bool isAddtive = false) {
		target.SetTo(this.propName, DictSO.Float(value), isAddtive);
	}

	/** 取得 插值 */
	public override object CalculateValue (PropKeyframe keyframe, PropKeyframe keyframe_next, float percent, int frameRate = 60) {
		if (keyframe_next == null || percent == 0) return keyframe.value;

		float val = keyframe.GetVal<float>();
		float val_next = keyframe_next.GetVal<float>();

		object _easeOut = this.EaseOutFrom(keyframe.easeOut);
		object _easeIn = this.EaseInFrom(keyframe.easeIn);

		float fixPercent = Mathf.Floor(percent * frameRate) / frameRate;

		Vector2 easeOut;
		Vector2 easeIn;
		if (_easeOut == null && _easeIn == null) {
			return keyframe.value;
		} else {
			easeOut = _easeOut != null? (Vector2) _easeOut : Vector2.zero;
			easeIn  = _easeIn  != null? (Vector2) _easeIn  : Vector2.zero;
		}

		float res = PropHandler_Float.Calculate(new Vector2(keyframe.frame, val), new Vector2(keyframe_next.frame, val_next), easeOut, easeIn, fixPercent);

		return res;
	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/


}

}