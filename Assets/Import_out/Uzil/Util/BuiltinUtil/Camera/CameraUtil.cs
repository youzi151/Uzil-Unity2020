using System.Collections.Generic;

using UnityEngine;
using UnityCamera = UnityEngine.Camera;

namespace Uzil.BuiltinUtil {

public class CameraUtil {
	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	private static CameraObj _main;

	private static Dictionary<string, CameraObj> key2Camera = new Dictionary<string, CameraObj>();

	/*=====================================Static Funciton=======================================*/
	
	/* 取得主要攝影機 */
	public static UnityCamera GetMain () {
		if (CameraUtil._main == null){
			return UnityCamera.main;
		}
		return CameraUtil._main.targetCamera;
	}

	/* 取得攝影機 */
	public static CameraObj Inst (string key = null) {
		if (key == null) { 
			if (CameraUtil._main == null) {

				GameObject gObj = new GameObject("MainCameraObj");
				gObj.transform.SetParent(UnityCamera.main.transform, false);
				gObj.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
				gObj.transform.localScale = Vector3.one;
				gObj.transform.SetParent(UnityCamera.main.transform.parent, false);
				UnityCamera.main.transform.SetParent(gObj.transform, true);

				CameraUtil._main = gObj.AddComponent<CameraObj>();
				CameraUtil._main.targetCamera = UnityCamera.main;
				CameraUtil._main.key = null;

			}
			return CameraUtil._main;
		}
		
		if (CameraUtil.key2Camera.ContainsKey(key) == false) {
			return null;
		}
		return CameraUtil.key2Camera[key];
	}

	/* 註冊 */
	public static void Reg (string key, CameraObj camera) {
		if (CameraUtil.key2Camera.ContainsKey(key)) {
			Debug.Log("[CameraUtil]: register fail Camera key["+key+"] is exist.");
			return;
		}
		CameraUtil.key2Camera[key] = camera;

		if (CameraUtil._main == null) {
			CameraUtil.RegMain(camera);
		}
	}
	/* 取消註冊 */
	public static void UnReg (string key) {
		if (CameraUtil.key2Camera.ContainsKey(key) == false) return;
		
		CameraObj camObj = CameraUtil.key2Camera[key];
		
		CameraUtil.key2Camera.Remove(key);
		
		if (CameraUtil._main == camObj) {
			CameraUtil._main = null;
		}
	}

	/* 註冊主要 */
	public static void RegMain (CameraObj camera) {
		CameraUtil._main = camera;
	}

	/* 取得 視區 */
	public static List<float> GetViewBorder2D (string key, float distance) {
		Camera cam;
		if (key != null) {
			CameraObj camera = CameraUtil.Inst(key);
			if (camera == null) return null;
			cam = camera.targetCamera;
		} else {
			cam = CameraUtil.GetMain();
		}

		Vector3 orinPos = cam.transform.localPosition;
		Quaternion orinRot = cam.transform.localRotation;
		Transform orinParent = cam.transform.parent;

		cam.transform.rotation = Quaternion.identity;
		cam.transform.position = Vector3.zero;
		cam.transform.SetParent(null, false);

		Vector3 fdl = cam.ViewportToWorldPoint(new Vector3(0f, 0f, distance));
		fdl -= cam.transform.localPosition;
		Vector3 fur = cam.ViewportToWorldPoint(new Vector3(1f, 1f, distance));
		fur -= cam.transform.localPosition;

		float fu = fur.y;
		float fd = fdl.y;
		float fl = fdl.x;
		float fr = fur.x;

		List<float> border = new List<float>{fl, fr, fu, fd};

		cam.transform.localRotation = orinRot;
		cam.transform.localPosition = orinPos;
		cam.transform.SetParent(orinParent, false);

		return border;
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
