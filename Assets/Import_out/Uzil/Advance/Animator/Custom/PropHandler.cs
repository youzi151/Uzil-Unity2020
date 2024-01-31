using System.Collections.Generic;

using UnityEngine;

using Uzil.Audio;

namespace Uzil.Anim {

public class PropHandler {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	private static bool _isInited = false;

	private static Dictionary<string, PropHandler> name2Handler = new Dictionary<string, PropHandler>();

	public static PropHandler defaultHandler = new PropHandler();

	/*=====================================Static Funciton=======================================*/

	/** 初始化 */
	public static void Initilize () {

		// 屬性處理器 列表 ==================

		// 啟用
		PropHandler.SetHandler("active", new PropHandler_Bool("active"));

		// 位置
		PropHandler.SetHandler("position.x", new PropHandler_Float("position.x"));
		PropHandler.SetHandler("position.y", new PropHandler_Float("position.y"));
		PropHandler.SetHandler("position.z", new PropHandler_Float("position.z"));
		
		// 縮放
		PropHandler.SetHandler("scale.x", new PropHandler_Float("scale.x"));
		PropHandler.SetHandler("scale.y", new PropHandler_Float("scale.y"));
		PropHandler.SetHandler("scale.z", new PropHandler_Float("scale.z"));

		// 旋轉
		PropHandler.SetHandler("rotation.x", new PropHandler_Float("rotation.x"));
		PropHandler.SetHandler("rotation.y", new PropHandler_Float("rotation.y"));
		PropHandler.SetHandler("rotation.z", new PropHandler_Float("rotation.z"));

		// 網格
		PropHandler.SetHandler("mesh", new PropHandler_Vector2List());
		PropHandler.SetHandler("uv", new PropHandler_Vector2List());
		PropHandler.SetHandler("pixelUV", new PropHandler_Vector2List());

		// 貼圖
		PropHandler.SetHandler("texture", new PropHandler_Texture("texture"));

		// 音效\
		PropHandler.SetHandler("audio", new PropHandler_Audio("audio"));

		// 腳本
		PropHandler.SetHandler("script", new PropHandler_Script("script"));

		// 顏色
		PropHandler.SetHandler("color.r", new PropHandler_Float("color.r"));
		PropHandler.SetHandler("color.g", new PropHandler_Float("color.g"));
		PropHandler.SetHandler("color.b", new PropHandler_Float("color.b"));
		PropHandler.SetHandler("color.a", new PropHandler_Float("color.a"));

		// 填滿 (Image.FillAmount)
		PropHandler.SetHandler("fillAmount", new PropHandler_Float("fillAmount"));

		// 呼叫
		PropHandler.SetHandler("call", new PropHandler_Script("call"));
		


		//====================================

		// 標示 已初始化
		PropHandler._isInited = true;
	}

	/** 取得 屬性處理器 */
	public static PropHandler GetHandler (string propName) {
		if (propName == null) return null;
		if (PropHandler._isInited == false) PropHandler.Initilize();
		if (PropHandler.name2Handler.ContainsKey(propName) == false) return null;
		return PropHandler.name2Handler[propName];
	}
	
	/** 設置 屬性處理器 */
	public static void SetHandler (string propName, PropHandler handler) {
		if (PropHandler.name2Handler.ContainsKey(propName)) {
			PropHandler.name2Handler[propName] = handler;
		} else {
			PropHandler.name2Handler.Add(propName, handler);
		}

		handler.propName = propName;
	}

	/*=========================================Members===========================================*/

	/** 屬性名稱 */
	public string propName = null;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 設置至目標 */
	public virtual void SetToTarget (PropTarget target, object value, bool isAddtive = false) {
		target.SetTo(this.propName, value, isAddtive);
	}

	/** 取得 插值 */
	public virtual object CalculateValue (PropKeyframe keyframe, PropKeyframe keyframe_next, float percent, int frameRate = 60) {
		return keyframe.value;
	}

	/** 附加 */
	public virtual object Addtive (object value, object value_add) {
		return value;
	}

	/** 轉換為可記錄格式 */
	public virtual object ValueTo (object value) {
		// 僅可適用 ValueType 數值型別
		return value;
	}
	public virtual object EaseInTo (object easeIn) {
		return DictSO.Vector2To((Vector2) easeIn);
	}
	public virtual object EaseOutTo (object easeOut) {
		return DictSO.Vector2To((Vector2) easeOut);
	}

	/** 讀取從可記錄格式 */
	public virtual object ValueFrom (object value) {
		// 僅可適用 ValueType 數值型別
		return value;
	}
	public virtual object EaseInFrom (object easeIn) {
		if (easeIn == null) return null;
		if (easeIn is Vector2) return easeIn;
		return DictSO.Vector2(easeIn);
	}
	public virtual object EaseOutFrom (object easeOut) {
		if (easeOut == null) return null;
		if (easeOut is Vector2) return easeOut;
		return DictSO.Vector2(easeOut);
	}


	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/


}

}