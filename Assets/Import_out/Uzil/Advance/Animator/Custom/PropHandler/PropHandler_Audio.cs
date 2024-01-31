using System.Collections.Generic;

using UnityEngine;

using Uzil.Util;
using Uzil.ObjInfo;

namespace Uzil.Anim {

public class PropHandler_Audio : PropHandler {

	/*======================================Constructor==========================================*/

	public PropHandler_Audio (string propName) {
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
		AudioInfo info;
		DictSO infoData = DictSO.Json(value);
		if (infoData == null) {
			info = new AudioInfo(value.ToString());
		} else {
			info = new AudioInfo(infoData);
		}
		target.SetTo(this.propName, info, isAddtive);
	}

	/** 取得 插值 */
	public override object CalculateValue (PropKeyframe keyframe, PropKeyframe keyframe_next, float percent, int frameRate = 60) {
		if (keyframe_next == null || percent == 0) return keyframe.value;

		DictSO audioA = DictSO.Json(keyframe.value);
		DictSO audioB = DictSO.Json(keyframe_next.value);

		DictSO newInfo = audioA.GetCopy();

		object _easeOut = this.EaseOutFrom(keyframe.easeOut);
		object _easeIn = this.EaseInFrom(keyframe.easeIn);

		Vector2 easeOut;
		Vector2 easeIn;
		if (_easeOut == null && _easeIn == null) {
			return keyframe.value;
		} else {
			easeOut = _easeOut != null? (Vector2) _easeOut : Vector2.zero;
			easeIn  = _easeIn  != null? (Vector2) _easeIn  : Vector2.zero;
		}

		if (newInfo.ContainsKey("currentTime") && audioB.ContainsKey("currentTime")) {
			float val = PropHandler_Float.Calculate(
				new Vector2(keyframe.frame, newInfo.GetFloat("currentTime")), new Vector2(keyframe_next.frame, (float) audioB.GetFloat("currentTime")),
				easeOut, easeIn, percent
			);
			newInfo.Set("currentTime", val);
		}

		if (newInfo.ContainsKey("volume") && audioB.ContainsKey("volume")) {
			float val = PropHandler_Float.Calculate(
				new Vector2(keyframe.frame, newInfo.GetFloat("volume")), new Vector2(keyframe_next.frame, audioB.GetFloat("volume")),
				easeOut, easeIn, percent
			);
			// Debug.Log(keyframe.frame+"/"+newInfo.GetFloat("volume")+" to "+keyframe_next.frame+"/"+audioB.GetFloat("volume")+" = "+val);
			// Debug.Log("percent:"+percent);
			newInfo.Set("volume", val);
		}

		return newInfo;
	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/


}

}