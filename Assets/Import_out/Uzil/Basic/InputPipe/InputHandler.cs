using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 輸入Input處理
 * 1. 接收 實際Input 並轉換為 虛擬Input
 * 2. 依序呼叫Listener
 */

namespace Uzil.InputPipe {
public class InputHandler {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 識別 */
	public string id;

	/** 來源 KeyCode */
	public List<int> srcKeyCodes = new List<int>();

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 處理信號 */
	public virtual void HandleSignal (InputSignal signal) {

	}

	/** 設置 資料 */
	public virtual void SetData (DictSO data) {
		if (data == null) return;

		data.TryGetList<int>("src", (res)=>{
			if (res != null) this.srcKeyCodes = res;
		});
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}
