using System.Collections.Generic;

using Uzil;
using Uzil.i18n;
using Options = Uzil.Options.Option;

namespace UZAPI {

public class Option {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/
	
	/** 設置語言 */
	public static void SetLanguage (string langCode) {
		Options.SetLanguage(langCode);
	}

	/** 取得語言 */
	public static string GetLanguage () {
		return Options.GetLanguage();
	}

	/** 取得語言資訊列表 */
	public static string GetLanguageInfos () {
		List<LanguageInfo> _langInfos = Options.GetLanguageInfos();
		if (_langInfos == null) return null;
		
		List<DictSO> langInfos = new List<DictSO>();

		for (int idx = 0; idx < _langInfos.Count; idx++) {
			langInfos.Add(_langInfos[idx].ToMemo() as DictSO);
		}

		return DictSO.ToJson(langInfos);
	}

	/** 設置音量 */
	public static void SetVolumeLinear (string mixerName, float volume) {
		Options.SetVolumeLinear(mixerName, volume);
	}

	/** 取得音量 */
	public static float GetVolumeLinear (string mixerName) {
		return Options.GetVolumeLinear(mixerName);
	}

	/** 取得 綁定 */
	public static string GetKeyBindings () {
		return DictSO.ToJson(Options.GetKeyBindings());
	}

	/** 取得 綁定 */
	public static string GetKeyBinding (string id) {
		return DictSO.ToJson(Options.GetKeyBinding(id));
	}

	/** 設置 綁定 */
	public static void SetKeyBinding (string id, string keys_json) {
		List<int> srcKeys = DictSO.List<int>(keys_json);
		Options.SetKeyBinding(id, srcKeys);
	}

	/** 取得 解析度 */
	public static string GetResolution () {
		return Options.GetResolution().ToJson();
	}

	/** 設置 解析度 */
	public static void SetResolution (int width, int height) {
		Options.SetResolution(width, height);
	}

	/** 取得 可支援的解析度 */
	protected static string _temp_supportResolutions = null;
	public static string GetSupportResolutions () {
		if (Option._temp_supportResolutions != null) return Option._temp_supportResolutions;

		List<DictSO> resolutions = Options.GetSupportResolutions();
		
		Option._temp_supportResolutions = DictSO.ToJson(resolutions);

		return Option._temp_supportResolutions;
	}

	/** 取得 視窗模式 */
	public static int GetFullScreenMode () {
		return Options.GetFullScreenMode();
	}

	/** 設置 視窗模式 */
	public static void SetFullScreenMode (int fullScreenMode) {
		Options.SetFullScreenMode(fullScreenMode);
	}

	/** 是否 於背景執行 */
	public static bool IsRunInBackground () {
		return Options.IsRunInBackground();
	}

	/** 設置 是否於背景執行 */
	public static void SetRunInBackground (bool isRunInBackground) {
		Options.SetRunInBackground(isRunInBackground);
	}

	/** 是否 可以於背景執行 */
	public static bool IsRunInBackgroundAble () {
#if UNITY_STANDALONE
		return true;
#else
		return false;
#endif
	}

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}

}