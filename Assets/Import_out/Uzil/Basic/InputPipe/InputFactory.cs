using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 輸入Input處理
 * 1. 接收 實際Input 並轉換為 虛擬Input
 * 2. 依序呼叫Listener
 */

namespace Uzil.InputPipe {

public enum InputHandlerType {
	KeyConvert
}

public class InputFactory {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/** 建立 處理器 */
	public static InputHandler CreateHandler (string id, InputHandlerType type, DictSO data) {

		InputHandler handler;

		switch (type) {

			case InputHandlerType.KeyConvert:
			default:
				handler = new InputHandler_KeyConvert();
				break;

		}

		// 從 設定檔 設置
		DictSO setting = InputSetting.GetDefault().GetSetting(id);
		if (setting != null) {
			handler.SetData(setting);
		}

		// 設置 資料
		handler.SetData(data);

		handler.id = id;

		return handler;
	}

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}
