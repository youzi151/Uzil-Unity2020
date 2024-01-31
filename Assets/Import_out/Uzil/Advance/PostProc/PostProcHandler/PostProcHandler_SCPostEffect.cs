#if SCPE

using System;
using System.Collections.Generic;

using Uzil.ObjInfo;
using Uzil.Values;

using SCPE;

using UnityEngine.Rendering.PostProcessing;

namespace Uzil.PostProc {
public class PostProcHandler_SCPostEffect : PostProcHandler {
	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/
	
	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	public PostProcessProfile profile = null;

	public Dictionary<string, Dictionary<string, object>> effect2Param2defaultVal = new Dictionary<string, Dictionary<string, object>>();

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/


	/** 處理 效果參數 */
	public override void HandleEffect (Dictionary<string, Dictionary<string, Vals>> effect2Params) {
		if (this.profile == null) return;

		if (effect2Params.ContainsKey("scanline")) {
			this.handleScanline(effect2Params["scanline"]);
		}

		if (effect2Params.ContainsKey("colorSplit")) {
			this.handleColorSplit(effect2Params["colorSplit"]);
		}

		if (effect2Params.ContainsKey("tubeDistortion")) {
			this.handleTubeDistortion(effect2Params["tubeDistortion"]);
		}

		if (effect2Params.ContainsKey("colorize")) {
			this.handleColorize(effect2Params["colorize"]);
		}

		if (effect2Params.ContainsKey("bloom")) {
			this.handleBloom(effect2Params["bloom"]);
		}

		
	}

	

	/*===================================Protected Function======================================*/

	protected void handleScanline (Dictionary<string, Vals> effectParams) {

		Scanlines settings = this.profile.GetSetting<Scanlines>();

		if (effectParams.ContainsKey("active")) {
			settings.active = (bool) effectParams["active"].GetCurrent();
		}

		if (effectParams.ContainsKey("intensity")) {
			settings.intensity.value = this.Float(effectParams["intensity"].GetCurrent());
		}

		if (effectParams.ContainsKey("lines")) {
			settings.amount.value = this.Int(effectParams["lines"].GetCurrent());
		}

		if (effectParams.ContainsKey("speed")) {
			settings.speed.value = this.Float(effectParams["speed"].GetCurrent());
		}
	}

	protected void handleColorSplit (Dictionary<string, Vals> effectParams) {

		ColorSplit settings = this.profile.GetSetting<ColorSplit>();

		if (effectParams.ContainsKey("active")) {
			settings.active = (bool) effectParams["active"].GetCurrent();
		}

		if (effectParams.ContainsKey("offset")) {
			settings.offset.value = this.Float(effectParams["offset"].GetCurrent());
		}
	}


	protected void handleTubeDistortion (Dictionary<string, Vals> effectParams) {

		TubeDistortion settings = this.profile.GetSetting<TubeDistortion>();

		if (effectParams.ContainsKey("active")) {
			settings.active = (bool) effectParams["active"].GetCurrent();
		}

		if (effectParams.ContainsKey("amount")) {
			settings.amount.value = this.Float(effectParams["amount"].GetCurrent());
		}
	}

	
	protected void handleColorize (Dictionary<string, Vals> effectParams) {
		Colorize settings = this.profile.GetSetting<Colorize>();

		if (effectParams.ContainsKey("active")) {
			settings.active = (bool) effectParams["active"].GetCurrent();
		}

		if (effectParams.ContainsKey("intensity")) {
			settings.intensity.value = this.Float(effectParams["intensity"].GetCurrent());
		}

		if (effectParams.ContainsKey("colorRamp")) {
			TextureInfo info = new TextureInfo(effectParams["colorRamp"].GetCurrent());
			settings.colorRamp.value = info.GetTexture();
		}
	}


	protected void handleBloom (Dictionary<string, Vals> effectParams) {

		Bloom settings = this.profile.GetSetting<Bloom>();

		if (effectParams.ContainsKey("active")) {
			settings.active = (bool) effectParams["active"].GetCurrent();
		}

		if (effectParams.ContainsKey("intensity")) {
			settings.intensity.value = this.Float(effectParams["intensity"].GetCurrent());
		}
		
		if (effectParams.ContainsKey("threshold")) {
			settings.threshold.value = this.Float(effectParams["threshold"].GetCurrent());
		}
		
		if (effectParams.ContainsKey("softKnee")) {
			settings.softKnee.value = this.Float(effectParams["softKnee"].GetCurrent());
		}
		
		if (effectParams.ContainsKey("diffusion")) {
			settings.diffusion.value = this.Float(effectParams["diffusion"].GetCurrent());
		}
		
		if (effectParams.ContainsKey("color")) {
			settings.color.value = DictSO.Color(effectParams["color"].GetCurrent());
		}
		
	}


	/*====================================Private Function=======================================*/

	protected float Float (object val) {
		return Convert.ToSingle(val);
	}

	protected int Int (object val) {
		return Convert.ToInt32(val);
	}
}


}

#endif