using System;
using System.Collections.Generic;

using UnityEngine;

using Uzil.i18n;
using Uzil.Audio;
using Uzil.UserData;
using Uzil.InputPipe;
using WindowStyle = Uzil.Misc.WindowStyle;
using DeleteUnityGarbage = Uzil.Misc.DeleteUnityGarbage;

namespace Uzil.Options {


/**
 * 設定相關
 * 每個Set行為都包含2項行為
 *   1. 對 運行中遊戲 改變設置
 *   2. (可選) 是否將改變的設置 寫入到 config 中
 */

public class Option {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	// 個人設定
	public static readonly string FileName_Game = "game.cfg";
	public static readonly string FileName_Keybinding = "keybinding.cfg";
	
	// 裝置設定
	public static readonly string FileName_Display = "display.cfg";
	public static readonly string FileName_Audio = "audio.cfg";

	/*=====================================Static Funciton=======================================*/

	/** 從設定檔讀取 */
	public static void LoadConfig (Action onLoaded = null) {
		bool isSaveConfig_no = false;

		Async.Waterfall(
			new List<System.Action<System.Action<bool>>>{

				// 顯示 =============
				(next)=>{

					#if !UNITY_EDITOR
						WindowStyle.Inst().onSetDone.Add(()=>{
							next(true);
						}).Once();
					#endif

					// 全螢幕模式
					FullScreenMode? fullScreenMode = null;
					int? fullScreenModeCode = Config.main.GetInt(Option.FileName_Display, "fullScreenMode");
					if (fullScreenModeCode != null) fullScreenMode = Option.getFullScreenMode((int) fullScreenModeCode);

					if (fullScreenMode != null) {
						Option.SetFullScreenMode((FullScreenMode) fullScreenMode, isSaveConfig_no);
					} else {
						Option.SetFullScreenMode(FullScreenMode.FullScreenWindow, true);
						fullScreenMode = FullScreenMode.FullScreenWindow;
					}

					// 解析度
					DictSO resData = Config.main.GetObj(Option.FileName_Display, "resolution");
					bool isSetResFromData = false;
					if (resData != null) {
						float? width = null, height = null;
						resData.TryGetInt("width", (res)=>{width = res;});
						resData.TryGetInt("height", (res)=>{height = res;});
						if (width != null && height != null) {
							isSetResFromData = Option.SetResolution((int) width, (int) height, isSaveConfig_no);
						}
					}
					
					if (!isSetResFromData) {
						Resolution curRes = Screen.currentResolution;
						Option.SetResolution(curRes.width, curRes.height, true);
					}

					#if UNITY_EDITOR
						next(true);
					#endif

				},

				// 音效 =============
				// (done)=>{
				// 	// 音量
				// 	if (Config.main.IsExist(Option.FileName_Audio, "volumes")) {
				// 		// 由 AudioUtil 自行讀取
				// 	}
				// 	done();
				// },

				// 操作 =============
				// (done)=>{
				// 	// 按鍵綁定
				// 	if (ProfileSave.main.IsExist(Option.FileName_Keybinding, null)) {
				// 		// 由 InputSetting 自行讀取
				// 	}
				// 	done();
				// },

				
				// 遊戲 =============
				(next)=>{

					// 語言
					string lang = ProfileSave.main.GetStr(Option.FileName_Game, "language");
					if (lang != null && lang != "") {
						Option.SetLanguage(lang, isSaveConfig_no);
					}

					// 是否於背景執行
					bool? isRunInBackground = ProfileSave.main.GetBool(Option.FileName_Game, "isRunInBackground");
					if (isRunInBackground != null) {
						Option.SetRunInBackground((bool) isRunInBackground, isSaveConfig_no);
					}

					next(true);
				}

			},
			()=>{

				// 清除 Unity 多餘寫入
				DeleteUnityGarbage.Inst().Clear();

				if (onLoaded != null) onLoaded();
			}
		);

	}

	/** 設置語言 */
	public static void SetLanguage (string targetLang, bool isSaveConfig = true) {
		string langCode = i18n.StrReplacer.Inst().FindLanguageCode(targetLang);
		if (langCode != null) {
			Option.SetLanguageCode(langCode, isSaveConfig);
		}
	}

	public static void SetLanguageCode (string langCode, bool isSaveConfig = true) {
		i18n.StrReplacer.Inst().SetLanguageCode(langCode);

		if (isSaveConfig) {
			ProfileSave.main.SetStr(Option.FileName_Game, "language", langCode);
		}
	}

	/** 取得語言 */
	public static string GetLanguage () {
		return i18n.StrReplacer.Inst().GetLanguage();
	}

	/** 取得語言資訊列表 */
	public static List<LanguageInfo> GetLanguageInfos () {
		return i18n.StrReplacer.Inst().GetLanguageInfos();
	}

	/** 是否於背景執行 */
	public static bool IsRunInBackground () {
		return Application.runInBackground;
	}

	/** 設置 是否於背景執行 */
	public static void SetRunInBackground (bool isRunInBackground, bool isSaveConfig = true) {

		Application.runInBackground = isRunInBackground;

		if (isSaveConfig) {
			ProfileSave.main.SetBool(Option.FileName_Game, "isRunInBackground", Application.runInBackground);
		}

		// 清除 Unity 多餘寫入
		DeleteUnityGarbage.Inst().Clear(); // TODO: 不確定 Application.runInBackground 會不會保存到Registry
	}
		
	/** 設置音量 */
	public static void SetVolumeLinear (string mixerName, float volume, bool isSaveConfig = true) {

		AudioUtil.SetVolumeLinear(mixerName, volume);
		
		// 若 要保存到設定檔
		if (isSaveConfig) {
			DictSO volumeCfg = Config.main.GetObj(Option.FileName_Audio, "volumes");
			if (volumeCfg == null) volumeCfg = new DictSO();

			volumeCfg.Set(mixerName, volume);

			Config.main.SetObj(Option.FileName_Audio, "volumes", volumeCfg);
		}
	}

	/** 取得音量 */
	public static float GetVolumeLinear (string mixerName) {
		return AudioUtil.GetVolumeLinear(mixerName);
	}

	/** 取得 綁定 */
	public static Dictionary<string, List<int>> GetKeyBindings () {		
		DictSO data = ProfileSave.main.GetObj(Option.FileName_Keybinding, null);
		if (data == null) return null;
		
		Dictionary<string, List<int>> id2SrcKeys = new Dictionary<string, List<int>>();
		foreach (KeyValuePair<string, object> pair in data) {
			DictSO bindingData = DictSO.Json(pair.Value);
			if (bindingData.ContainsKey("src") == false) continue;
			List<int> srcKeys = bindingData.GetList<int>("src");
			id2SrcKeys.Add(pair.Key, srcKeys);
		}

		return id2SrcKeys;
	}

	/** 取得 綁定 */
	public static List<int> GetKeyBinding (string id) {
		DictSO data = ProfileSave.main.GetObj(Option.FileName_Keybinding, id);
		if (data == null) return null;
		
		if (data.ContainsKey("src") == false) return null;

		return data.GetList<int>("src");
	}

	/** 設置 綁定 */
	public static void SetKeyBinding (string handlerID, List<int> srcKeys, bool isSaveConfig = true) {
		if (srcKeys == null) return;

		DictSO toSetData = new DictSO();
		toSetData.Add("src", srcKeys);

		// 改寫設定
		InputSetting.GetDefault().OverrideSetting(handlerID, toSetData);

		// 改寫 當前 InputMgr 的內容
		List<InputHandler> handlers = InputMgr.Inst().GetHandlerInLayers(handlerID);
		foreach (InputHandler each in handlers) {
			each.SetData(toSetData);
		}

		// 若 要保存到設定檔
		if (isSaveConfig) {		
			DictSO data = ProfileSave.main.GetObj(Option.FileName_Keybinding, handlerID);
			if (data == null) data = new DictSO();

			data.Set("src", srcKeys);

			ProfileSave.main.SetObj(Option.FileName_Keybinding, handlerID, data);
		}
	}

	
	/** 取得 解析度 */
	public static DictSO GetResolution () {
		return new DictSO().Set("width", Screen.width)
						   .Set("height", Screen.height);
	}

	/** 設置 解析度 */
	public static bool SetResolution (int width, int height, bool isSaveConfig = true) {
		Resolution? targetRes = null;
		foreach (Resolution res in Screen.resolutions) {
			if (res.width == width && res.height == height) {
				targetRes = res;
				break;
			}
		}

		// 若沒有對應 解析度 則 返回
		if (targetRes == null) return false;

		// 設置 解析度
		bool isSuccess = WindowStyle.Inst().SetResolution(width, height);
		if (!isSuccess) return false;
		
		// 資料 : 設置資料
		DictSO resData = new DictSO().Set("width", width).Set("height", height);
		
		// 若 要保存到設定檔
		if (isSaveConfig) {
			Config.main.SetObj(Option.FileName_Display, "resolution", resData);
		}

		// 發送事件
		EventBus.Inst().Post("onResolutionChanged", resData);

		return true;
	}

	/** 取得 可支援的解析度 */
	public static List<DictSO> GetSupportResolutions () {
		List<Resolution> resolutions = new List<Resolution>();
		foreach (Resolution res in Screen.resolutions) {
			
			bool isExist = false;
			foreach (Resolution exist in resolutions) {
				if (exist.width == res.width && exist.height == res.height) {
					isExist = true;
					break;
				}
			}
			if (isExist) continue;
			
			resolutions.Add(new Resolution{width = res.width, height = res.height, refreshRate = 0});
		}

		List<DictSO> resolutionDatas = new List<DictSO>();
		foreach (Resolution res in resolutions) {
			resolutionDatas.Add(new DictSO().Set("width", res.width).Set("height", res.height));
		}
		return resolutionDatas;
	}

	/** 取得 視窗模式 */
	public static int GetFullScreenMode () {
		return Option.getFullScreenModeCode(Screen.fullScreenMode);
	}

	/** 設置 視窗模式 */
	public static void SetFullScreenMode (int fullScreenMode, bool isSaveConfig = true) {
		FullScreenMode target;
		switch (fullScreenMode) {
			case 1:
				target = FullScreenMode.FullScreenWindow;
				break;
			case 2:
				target = FullScreenMode.Windowed;
				break;
			default:
				return;
		}
		Option.SetFullScreenMode(target, isSaveConfig);
	}
	public static void SetFullScreenMode (FullScreenMode fullScreenMode, bool isSaveConfig = true) {
		
		WindowStyle.Inst().SetFullScreenMode(fullScreenMode);

		// 若 要保存到設定檔		
		if (isSaveConfig) {
			Config.main.SetNum(Option.FileName_Display, "fullScreenMode", Option.getFullScreenModeCode(fullScreenMode));
		}

		// 清除 Unity 多餘寫入
		DeleteUnityGarbage.Inst().Clear();
	}

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/
	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	
	protected static int getFullScreenModeCode (FullScreenMode fullScreenMode) {
		switch (fullScreenMode) {
			case FullScreenMode.FullScreenWindow:
				return 1;
			case FullScreenMode.Windowed:
				return 2;
		}
		return 0;
	}

	protected static FullScreenMode? getFullScreenMode (int code) {
		switch (code) {
			case 1:
				return FullScreenMode.FullScreenWindow;
			case 2:
				return FullScreenMode.Windowed;
		}
		return null;
	}

	
	/*====================================Private Function=======================================*/

}



}