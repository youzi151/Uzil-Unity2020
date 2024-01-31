using System.Collections.Generic;

using UnityEngine;

using Uzil;
using Uzil.UI;
using Uzil.BuiltinUtil;

using Uzil.ObjInfo;

namespace UZAPI {

public class UI {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/** 建立 */
	public static void Inst (string instID) {
		UIMgr.Inst(instID);
	}

	/** 銷毀 */
	public static void DestroyInst (string instID) {
		UIMgr.DestroyInst(instID);
	}

	/** 排序 */
	public static void SortInst (string instID, float sort) {
		UIMgr.SortInst(instID, sort);
	}

	/** 設置 渲染模式 */
	public static void SetRenderMode (string instID, string renderMode) {
		if (renderMode == null) return;

		renderMode = renderMode.ToLower();
		
		RenderMode mode;
		
		switch (renderMode) {
			case "camera":
				mode = RenderMode.ScreenSpaceCamera;
				break;
			case "overlay":
				mode = RenderMode.ScreenSpaceOverlay;
				break;
			case "world":
				mode = RenderMode.WorldSpace;
				break;
			default:
				return;
		}
			
		UIMgr ui = UIMgr.Inst(instID);
		ui.SetRenderMode(mode);
	}

	/** 取得 Canvas 尺寸 */
	public static string GetCanvasSize (string instID) {
		CanvasObj canvas = CanvasUtil.Inst(instID);
		return DictSO.ToJson(DictSO.Vector2To(canvas.GetSize()));
	}

	/** 取得 Canvas 參考尺寸 */
	public static string GetCanvasRefSize (string instID) {
		CanvasObj canvas = CanvasUtil.Inst(instID);
		return DictSO.ToJson(DictSO.Vector2To(canvas.GetRefSize()));
	}

	/** 建立 */
	public static void Create (string instID, string id, string json) {
		UIMgr uiMgr = UIMgr.Inst(instID);
		DictSO data = DictSO.Json(json);
		uiMgr.Create(id, data);
	}

	/** 銷毀 */
	public static void Remove (string instID, string id) {
		UIMgr uiMgr = UIMgr.Inst(instID);
		uiMgr.Remove(id);
	}

	/** 清空 */
	public static void Clear (string instID) {
		UIMgr uiMgr = UIMgr.Inst(instID);
		uiMgr.Clear();
	}

	/** 強制更新 */
	public static void ForceUpdate (string instID, string id) {
		UIMgr uiMgr = UIMgr.Inst(instID);
		uiMgr.ForceUpdate(id);
	}

	/** 排序 */
	public static void Sort (string instID, string id, float sort) {
		UIMgr uiMgr = UIMgr.Inst(instID);
		uiMgr.Sort(id, sort);
	}

	/** 設置 資料 */
	public static void SetData (string instID, string id, string json) {
		try {
			UIMgr uiMgr = UIMgr.Inst(instID);
			DictSO data = DictSO.Json(json);
			uiMgr.SetData(id, data);
		} catch (System.Exception e) {
			Debug.LogError(e);
		}
	}

	/** 取得 資料 */
	public static string GetData (string instID, string id, string porps_json) {
		UIMgr uiMgr = UIMgr.Inst(instID);
		List<string> requests = DictSO.List<string>(porps_json);
		DictSO data = uiMgr.GetData(id, requests);
		return DictSO.ToJson(data);
	}

	/** 設置 屬性 */
	public static void SetProp (string instID, string id, string key, object value) {
		UIMgr uiMgr = UIMgr.Inst(instID);
		uiMgr.SetProp(id, key, value);
	}

	/** 設置 圖像資料 */
	public static void SetImageData (string instID, string id, string json) {
		UIMgr uiMgr = UIMgr.Inst(instID);
		ImageInfo imgInfo = new ImageInfo(json);
		DictSO data = (DictSO) imgInfo.ToMemo();
		uiMgr.SetImageData(id, data);
	}

	/** 設置 圖像屬性 */
	public static void SetImageProp (string instID, string id, string key, object value) {
		UIMgr uiMgr = UIMgr.Inst(instID);
		uiMgr.SetImageProp(id, key, value);
	}

	/** 設置 文字資料 */
	public static void SetTextData (string instID, string id, string json) {
		UIMgr uiMgr = UIMgr.Inst(instID);
		DictSO data = DictSO.Json(json);
		uiMgr.SetTextData(id, data);
	}

	/** 設置 文字屬性 */
	public static void SetTextProp (string instID, string id, string key, object value) {
		UIMgr uiMgr = UIMgr.Inst(instID);
		uiMgr.SetTextProp(id, key, value);
	}

	/** 設置 動畫參數 */
	public static void SetAnimParam (string instID, string id, string paramName, object value) {
		UIMgr uiMgr = UIMgr.Inst(instID);
		uiMgr.SetAnimParam(id, paramName, value);
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
