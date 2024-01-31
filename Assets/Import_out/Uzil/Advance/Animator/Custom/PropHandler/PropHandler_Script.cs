using System.Collections.Generic;

using UnityEngine;

using Uzil.Util;

namespace Uzil.Anim {

public class Prop_ScriptCMD {
	public float frame;
	public string script;
}

public class PropHandler_Script : PropHandler {

	/*======================================Constructor==========================================*/

	public PropHandler_Script (string propName) {
		this.propName = propName;
	}

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 取得 插值 */
	public override object CalculateValue (PropKeyframe keyframe, PropKeyframe keyframe_next, float percent, int frameRate = 60) {
		Prop_ScriptCMD cmd = new Prop_ScriptCMD();
		cmd.script = keyframe.value.ToString();
		cmd.frame = keyframe.frame;
		return cmd;
	}
	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/


}

}