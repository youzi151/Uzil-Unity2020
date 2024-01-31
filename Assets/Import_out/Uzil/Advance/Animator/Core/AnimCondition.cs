using System.Collections.Generic;

using Uzil;
using Uzil.Util;

namespace Uzil.Anim {

public enum CompareType {
	NA, STRING, NUMBER, BOOL
}

public class AnimCondition : IMemoable {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 要比較的變數 */
	public string parameterKey = "";

	/** 比較運算 */
	public string comparer = Comparer.EQUAL;

	/** 比較類型 */
	public CompareType compareType = CompareType.NA;

	/** 比較數值 */
	public object toCompare {
		get {
			return this._toCompare;
		}
		set {
			if (value is string) {
				this.compareType = CompareType.STRING;
				this._toCompare = value.ToString();
			} else if (value is bool) {
				this.compareType = CompareType.BOOL;
				this._toCompare = DictSO.Bool(value);
			} else if (DictSO.IsNumeric(value)) {
				this.compareType = CompareType.NUMBER;
				this._toCompare = DictSO.Float(value);
			} else {
				this.compareType = CompareType.NA;
				this._toCompare = null;
			}
		}
	}
	private object _toCompare = null;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** [IMemoable] 紀錄為Json格式 */
	public object ToMemo () {
		DictSO data = new DictSO();

		/** 要比較的變數 */
		data.Set("key", this.parameterKey);

		/** 比較運算 */
		data.Set("comparer", this.comparer);

		/** 比較數值 */
		data.Set("value", this.toCompare);

		return data;
	}

	/** [IMemoable] 讀取Json格式 */
	public void LoadMemo (object memoJson) {

		DictSO data = DictSO.Json(memoJson);
		
		/** 要比較的變數 */
		if (data.ContainsKey("key")) {
			this.parameterKey = data.GetString("key");
		}
		
		/** 比較運算 */
		if (data.ContainsKey("comparer")) {
			this.comparer = data.GetString("comparer");
		}
		
		/** 比較數值 */
		if (data.ContainsKey("value")) {
			System.Type type = data.GetType("value");
			if (type == typeof(string)) {
				this.toCompare = data.GetString("value");
			} else if (type == typeof(bool)) {
				this.toCompare = data.GetBool("value");
			} else if (DictSO.IsNumericType(type)) {
				this.toCompare = data.GetFloat("value");
			} else {
				this.toCompare = null;
			}
		}
	}

	/*=====================================Public Function=======================================*/

	/**
	 * 是否通過
	 * @param parameter 要被比較的參數集
	 */
	public bool isPass (DictSO parameter) {
		string key = this.parameterKey;
		if (key == null || key == "") return false;
		if (parameter.ContainsKey(key) == false) return false;

		bool res = false;

		if (this.compareType == CompareType.NA) {
			return false;
		} else if (this.compareType == CompareType.STRING) {
			string param = parameter.GetString(key);
			res = CompareUtil.Compare(param, (string) this.toCompare, this.comparer);
		} else if (this.compareType == CompareType.BOOL) {
			bool param = parameter.GetBool(key);
			res = CompareUtil.Compare(param, (bool) this.toCompare, this.comparer);
		} else if (this.compareType == CompareType.NUMBER) {
			float param = parameter.GetFloat(key);
			res = CompareUtil.Compare(param, (float) this.toCompare, this.comparer);
		}
		
		return res;
	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/


}

}