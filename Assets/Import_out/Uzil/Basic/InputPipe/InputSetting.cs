using System.Collections.Generic;

using Uzil.Util;
using Uzil.UserData;

namespace Uzil.InputPipe {
public class InputSetting {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	public static InputSetting defaultSetting = null;

	/*=====================================Static Funciton=======================================*/

	public static InputSetting GetDefault () {
		if (InputSetting.defaultSetting != null) return InputSetting.defaultSetting;

		InputSetting setting = new InputSetting();

		DictSO handlerID2Setting = ProfileSave.main.GetObj("keybinding.cfg", null);
		if (handlerID2Setting == null) return null;

		foreach (KeyValuePair<string, object> pair in handlerID2Setting) {
			setting.handlerID2Setting.Add(pair.Key, DictSO.Json(pair.Value));
		}

		InputSetting.defaultSetting = setting;

		return setting;
	}

	/*=========================================Members===========================================*/
	
	/** 處理器ID 對應 設定 */
	public Dictionary<string, DictSO> handlerID2Setting = new Dictionary<string, DictSO>();

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	public DictSO GetSetting (string handlerID) {
		if (this.handlerID2Setting.ContainsKey(handlerID) == false) return null;
		return this.handlerID2Setting[handlerID];
	}

	public void SetSetting (string handlerID, DictSO data) {
		if (this.handlerID2Setting.ContainsKey(handlerID)) {
			this.handlerID2Setting[handlerID] = data;
		} else {
			this.handlerID2Setting.Add(handlerID, data);
		}
	}

	public void OverrideSetting (string handlerID, DictSO data) {
		if (this.handlerID2Setting.ContainsKey(handlerID)) {
			this.handlerID2Setting[handlerID].Merge(data);
		} else {
			this.handlerID2Setting.Add(handlerID, data);
		}
	}


	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}
