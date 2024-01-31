using System;
using System.Collections.Generic;

using Uzil;
using Uzil.Util;

namespace Uzil.Anim {

public class AnimFactory {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	public static Type DEFAULT_CLIPTYPE = typeof(AnimClip_Custom);

	/*=====================================Static Funciton=======================================*/

	/*== 基本功能 =================*/

	/**
	 * 建立 層級
	 * @param data 層級資料
	 */
	public static AnimLayer CreateLayer (DictSO data) {
		// 驗證
		if (AnimFactory.validate(data, new string[]{"id"}) == false) return null;

		AnimLayer layer = new AnimLayer();
		layer.LoadMemo(data);

		return layer;
	}

	/**
	 * 建立 片段
	 * @param data 片段資料
	 */
	public static AnimClip CreateClip (DictSO data) {
		
		// 驗證
		if (AnimFactory.validate(data, new string[]{"id"}) == false) return null;
		
		Type type = typeof (AnimClip);

		// 若 指定類型 存在
		if (data.ContainsKey("type")) {
			// 尋找 指定類型
			Type targetType = TypeUtil.FindType("AnimClip_"+data.GetString("type"));
			// 若 指定類型存在 則 覆蓋
			if (targetType != null) {
				type = targetType;
			}
		} else {
			type = AnimFactory.DEFAULT_CLIPTYPE;
		}

		// 建立
		AnimClip clip = (AnimClip) Activator.CreateInstance(type);
		clip.LoadMemo(data);

		return clip;
	}

	/**
	 * 建立 狀態
	 * @param data 狀態資料
	 */
	public static AnimState CreateState (DictSO data) {
		// 驗證
		if (AnimFactory.validate(data, new string[]{"id"}) == false) return null;

		AnimState state = new AnimState();
		state.LoadMemo(data);

		return state;
		
	}

	/**
	 * 建立 轉場
	 * @param data 轉場資料
	 */
	public static AnimTransition CreateTransition (DictSO data) {
		// 驗證
		if (AnimFactory.validate(data, new string[]{"nextState"}) == false) return null;

		AnimTransition transition = new AnimTransition();
		transition.LoadMemo(data);

		return transition;

	}

	/**
	 * 建立 條件
	 * @param data 條件資料
	 */
	public static AnimCondition CreateCondition (DictSO data) {
		// 驗證
		if (AnimFactory.validate(data, new string[]{"key", "comparer", "value"}) == false) return null;

		AnimCondition condition = new AnimCondition();
		condition.LoadMemo(data);
		

		return condition;
	}

	/*== 其他功能 =================*/

	/*== Private Function =========================================*/

	/**
	 * 檢查 資料 是否具有 這些鍵值
	 * @param data 資料
	 * @param keys 要具有的鍵值
	 */
	private static bool validate (DictSO data, string[] keys) {
		foreach (string key in keys) {
			if (data.ContainsKey(key) == false) return false;
		}
		return true;
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