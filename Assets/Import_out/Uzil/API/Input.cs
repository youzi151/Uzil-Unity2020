using System.Collections.Generic;

using Uzil;
using Uzil.InputPipe;
using Uzil.Util;

namespace UZAPI {

public class Input {

	class InputRegInfo {
		public int callbackID;
		public EventListener eventListener;
	}

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	private static Dictionary<string, InputRegInfo> id2Info = new Dictionary<string, InputRegInfo>();

	/*=====================================Static Funciton=======================================*/

	/**== 取得輸入 ============*/

	/** 取得 輸入 */
	public static string GetInput (string instID, string layerID, int virtualKeyCode) {
		InputSignal signal = InputMgr.Inst(instID).GetInput(layerID, virtualKeyCode);
		if (signal == null) return null;
		return ((DictSO) signal.ToMemo()).ToJson();
	}

	/** 取得 輸入 值 數值 */
	public static float GetInputValueNum (string instID, string layerID, int virtualKeyCode) {
		InputSignal signal = InputMgr.Inst(instID).GetInput(layerID, virtualKeyCode);
		if (signal == null) return 0;
		return (float) signal.GetValueNum();
	}

	/** 取得 輸入 值 字串 */
	public static string GetInputValueStr (string instID, string layerID, int virtualKeyCode) {
		InputSignal signal = InputMgr.Inst(instID).GetInput(layerID, virtualKeyCode);
		if (signal == null) return null;
		return (string) signal.GetValueStr();
	}

	/**== 按鍵輸入 快捷 ========*/

	/** 按鍵按下 */
	public static bool GetKeyDown (string instID, string layerID, int virtualKeyCode) {
		InputSignal signal = InputMgr.Inst(instID).GetInput(layerID, virtualKeyCode);
		if (signal == null) return false;
		return signal.GetValueNum() == (int) InputKeyState.Down;
	}

	/** 按鍵按著 */
	public static bool GetKey (string instID, string layerID, int virtualKeyCode) {
		InputSignal signal = InputMgr.Inst(instID).GetInput(layerID, virtualKeyCode);
		if (signal == null) return false;
		return signal.GetValueNum() == (int) InputKeyState.Pressed;
	}

	/** 按鍵彈起 */
	public static bool GetKeyUp (string instID, string layerID, int virtualKeyCode) {
		InputSignal signal = InputMgr.Inst(instID).GetInput(layerID, virtualKeyCode);
		if (signal == null) return false;
		return signal.GetValueNum() == (int) InputKeyState.Up;
	}

	/**== 註冊 =================*/

	/** 註冊 當輸入 */
	public static void AddOnInput (string instID, string layerID, int virtualKeyCode, string eventListenerID, int luaCallbackID, float sort = 0) {
		
		// 建立 偵聽者
		EventListener listener = new EventListener((data) => {
			InputSignal signal = (InputSignal) data.Get("signal");
			UZAPI.Callback.CallLua_cs(luaCallbackID, (DictSO) signal.ToMemo());
		}).ID(eventListenerID).Sort(sort);
		
		// 若 該ID已經存在
		if (Input.id2Info.ContainsKey(eventListenerID)) {
			InputRegInfo exist = Input.id2Info[eventListenerID];
			Input.id2Info.Remove(eventListenerID);
			// 移除 已存在 Callback
			if (luaCallbackID != exist.callbackID) {
				UZAPI.Callback.Remove(exist.callbackID);
			}
		}

		// 註冊資訊
		InputRegInfo regInfo = new InputRegInfo(){
			callbackID = luaCallbackID,
			eventListener = listener
		};
		Input.id2Info.Add(eventListenerID, regInfo);

		// 註冊
		InputMgr.Inst(instID).AddOnInput(layerID, virtualKeyCode, listener);
	}

	/** 註銷 當輸入 */
	public static void RemoveOnInput (string instID, string layerID, int virtualKeyCode, string eventListenerID) {
		InputMgr.Inst(instID).RemoveOnInput(layerID, virtualKeyCode, eventListenerID);
		
		// 移除 Callback
		if (Input.id2Info.ContainsKey(eventListenerID)) {
			InputRegInfo exist = Input.id2Info[eventListenerID];
			Input.id2Info.Remove(eventListenerID);
			// 移除 已存在 Callback
			UZAPI.Callback.Remove(exist.callbackID);
		}
	}

	/**== 處理器 =================*/

	public static void AddHandler (string instID, string layerID, string id, string dataJson) {
		DictSO data = DictSO.Json(dataJson);
		if (data == null) return;
		
		if (data.ContainsKey("dst") == false) return;
		
		InputHandlerType handlerType = InputHandlerType.KeyConvert;
		data.TryGetEnum<InputHandlerType>("type", (res)=>{
			handlerType = res;
		});

		InputHandler handler = InputFactory.CreateHandler(id, handlerType, data);
		if (handler == null) return;
		InputMgr.Inst(instID).AddHandler(layerID, handler);
	}

	public static void RemoveHandler (string instID, string layerID, string id) {
		InputMgr.Inst(instID).RemoveHandler(layerID, id);
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
