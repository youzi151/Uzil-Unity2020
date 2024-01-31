using UnityEngine;

using Uzil.Util;

/**
 * 輸入Input信號
 */

namespace Uzil.InputPipe {

public class InputSignal {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 是否存活 */
	public bool isAlive = true;

	/** 實際KeyCode */
	public int realKeyCode { get; protected set;} = 0;

	/** 虛擬KeyCode */
	public int virtualKeyCode = 0;

	/** 來源信號 */
	public InputSignal srcSignal = null;

	/** 值 */
	public string value_str = null;
	public float? value_num = null;

	/** 值類型 */
	// Uzil.Util.InputType
	public int valueType = 0;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*========================================Interface==========================================*/

	/** [IMemoable] 紀錄為Json格式 */
	public object ToMemo () {
		DictSO memo = DictSO.New();

		memo.Ad("isAlive", this.isAlive);
		memo.Ad("srcKey", this.realKeyCode);
		memo.Ad("dstKey", this.virtualKeyCode);

		memo.Ad("valueType", this.valueType);

		if (this.value_num != null) {
			memo.Set("value", this.value_num);
		} else if (this.value_str != null) {
			memo.Set("value", this.value_str);
		}
		
		return memo;
	}
	
    /** [IMemoable] 讀取Json格式 */
	public virtual void LoadMemo (object memoJson) {
		DictSO data = DictSO.Json(memoJson);

		data.TryGetBool("isAlive", (res)=>{
			this.isAlive = res;
		});

		data.TryGetInt("srcKey", (res)=>{
			this.realKeyCode = res;
		});

		data.TryGetInt("dstKey", (res)=>{
			this.virtualKeyCode = res;
		});

		data.TryGetInt("valueType", (res)=>{
			this.valueType = res;
		});

		if (data.ContainsKey("value")) {
			object value_obj = data.Get("value");
			if (DictSO.IsNumeric(value_obj)) {
				this.SetValue(DictSO.Float(value_obj));
			} else {
				this.SetValue(value_obj.ToString());
			}
		}
	}

	/*=====================================Public Function=======================================*/

	public InputSignal Init (int keyCode) {
		this.realKeyCode = keyCode;
		this.virtualKeyCode = keyCode;
		return this;
	}

	public virtual InputSignal GetCopy () {
		InputSignal copy = new InputSignal();
		copy.realKeyCode = this.realKeyCode;
		copy.virtualKeyCode = this.virtualKeyCode;
		copy.isAlive = this.isAlive;
		copy.srcSignal = this;
		copy.valueType = this.valueType;
		copy.value_str = this.value_str;
		copy.value_num = this.value_num;
		return copy;
	}
	
	public void SetValueType (int typeInt) {
		this.valueType = typeInt;
	}

	public void SetValue (string str) {
		this.value_str = str;
		this.value_num = null;
	}

	public void SetValue (float num) {
		this.value_str = null;
		this.value_num = num;
	}

	public int GetValueType () {
		return this.valueType;
	}

	public float? GetValueNum () {
		return this.value_num;
	}

	public string GetValueStr () {
		return this.value_str;
	}

	public bool IsAlive (bool isStream = true) {
		if (this.isAlive == false) return false;

		if (!isStream) {

			return true;

		} else {
			
			InputSignal current = this;
			
			// 輪詢來源 直到 來源為空
			int tryTime = 100; 
			while (current.srcSignal != null && tryTime-- > 0) {
				current = current.srcSignal;
				if (current.isAlive == false) return false;
			}

			return true;
		}
	}

	public void Stop (bool isStream = true) {
		InputSignal current = this;
		current.isAlive = false;

		// 若 為 整條訊號流
		if (isStream) {
			// 輪詢來源 直到 來源為空
			int tryTime = 100; 
			while (current.srcSignal != null && tryTime-- > 0) {
				current = current.srcSignal;
				// 停止
				current.isAlive = false;
			}
		}
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}
