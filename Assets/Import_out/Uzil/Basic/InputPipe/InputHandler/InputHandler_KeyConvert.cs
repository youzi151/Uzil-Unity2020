using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Uzil.InputPipe {
public class InputHandler_KeyConvert : InputHandler {

	
	/*======================================Constructor==========================================*/

	public InputHandler_KeyConvert () {

	}

	public InputHandler_KeyConvert (int dstKeyCode, List<int> srcKeyCodes) {
		this.dstKeyCode = dstKeyCode;
		this.srcKeyCodes = srcKeyCodes;
	}

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 目標 KeyCode */
	public int dstKeyCode;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 處理 */
	public override void HandleSignal (InputSignal signal) {
		if (this.srcKeyCodes.Contains(signal.realKeyCode) == false) return;
		signal.virtualKeyCode = this.dstKeyCode;
	}

	/** 設置 資料 */
	public override void SetData (DictSO data) {
		if (data == null) return;

		base.SetData(data);

		data.TryGetInt("dst", (res)=>{
			this.dstKeyCode = res;
		});
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}
