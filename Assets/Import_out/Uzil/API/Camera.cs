using System.Collections.Generic;

using UnityEngine;

using Uzil;
using Uzil.BuiltinUtil;

namespace UZAPI {

public class Camera {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** 取得 位置 */
	public static string GetPosition (string cameraID) {
		CameraObj camera = CameraUtil.Inst(cameraID);
		if (camera == null) return null;
		return DictSO.ToJson(DictSO.Vector3To(camera.gameObject.transform.position));
	}

	/** 取得 世界座標 在 視區位置 */
	public static string GetWorldToViewPort (string cameraID, float x, float y, float z) {
		CameraObj camera = CameraUtil.Inst(cameraID);
		if (camera == null) return null;
		Vector2 pos = camera.targetCamera.WorldToViewportPoint(new Vector3(x, y, z));
		return DictSO.ToJson(DictSO.Vector2To(pos));
	}

	/** 取得 視區範圍 2D */
	public static string GetViewBorder2D (string cameraID, float distance) {
		List<float> border = CameraUtil.GetViewBorder2D(cameraID, distance);
		return DictSO.ToJson(border);
	}

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}

}