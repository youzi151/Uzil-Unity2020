using System.Collections.Generic;

using UnityEngine;

using Uzil.Res;
using Uzil.Lua;

namespace Uzil.Anim {

public class PropTarget {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	private static Dictionary<string, PropHandler> name2Handler = new Dictionary<string, PropHandler>();

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 暫存 要應用到目標屬性的值 */
	public Dictionary<string, object> prop2value = new Dictionary<string, object>();

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 設置 */
	public virtual void SetTo (string propName, object value, bool isAddtive = false) {

		// 若 暫存 尚未含有該屬性 則 新增
		if (this.prop2value.ContainsKey(propName) == false) {

			this.prop2value.Add(propName, value);

		}
		// 否則
		else {
			
			// 已經存在的
			object exist = this.prop2value[propName];

			// 若 不是附加
			if (!isAddtive) {
				this.prop2value[propName] = value;
			}
			// 否則 若為 附加
			else {

				// 試著取得 屬性處理器
				PropHandler handler = PropHandler.GetHandler(propName);
				// 若 屬性處理器 存在 則 將 已經存在的+附加值 設入 暫存
				if (handler != null) {
					this.prop2value[propName] = handler.Addtive(exist, value);
				}

			}

		}
	}

	/** 應用 */
	protected virtual void applyTo (Animator animator, string propName, object value) {

	}

	/** 應用 */
	public void Apply (Animator animator) {
		
		// 應用 所有暫存值 至 目標屬性
		foreach (KeyValuePair<string, object> pair in this.prop2value) {
			this.applyTo(animator, pair.Key, pair.Value);
		}

		// 清除暫存
		this.prop2value.Clear();
	}

	/** 當關鍵幀改變 */
	public virtual void OnKeyframeChanged () {
		
	}

	/*===================================Protected Function======================================*/

	/** 設置 向量類 */
	protected void setFloats (string propName, int dimensionCount, int dimensionIdx, float value, bool isAddtive) {
		float?[] res;
		if (this.prop2value.ContainsKey(propName)) {
			res = (float?[]) this.prop2value[propName];
		} else {
			res = new float?[dimensionCount];
		}

		if (isAddtive && res[dimensionIdx] != null) {
			res[dimensionIdx] += value;
		} else {
			res[dimensionIdx] = value;
		}

		this.SetTo(propName, res, false /* 已經處理過 不需後續處理 */);
	}

	/** 設置 二維向量類 */
	protected void setFloat2 (string propName, int dimensionIdx, float value, bool isAddtive) {
		this.setFloats(propName, 2, dimensionIdx, value, isAddtive);
	}

	/** 設置 三維向量類 */
	protected void setFloat3 (string propName, int dimensionIdx, float value, bool isAddtive) {
		this.setFloats(propName, 3, dimensionIdx, value, isAddtive);
	}
	
	/** 設置 四維向量類 */
	protected void setFloat4 (string propName, int dimensionIdx, float value, bool isAddtive) {
		this.setFloats(propName, 4, dimensionIdx, value, isAddtive);
	}
	
	/** 執行腳本 */
	protected void doScript (string script, DictSO args = null) {
		string argStr = "";
		if (args != null) {
			argStr = "Json.decode(\""+args.ToJson().Replace("\"", "\\\"")+"\")";
		}
		string str = "(function (args) "+script+" end)("+argStr+");";
		LuaUtil.DoString(str);
	}


	/*====================================Private Function=======================================*/


}

}