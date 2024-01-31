using System.Collections.Generic;

using UnityEngine;

namespace Uzil.BuiltinUtil {

public class CanvasUtil : MonoBehaviour {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	public static CanvasUtil instance;

	/*=====================================Static Funciton=======================================*/

	private static CanvasObj _main;

	private static Dictionary<string, CanvasObj> key2Canvas = new Dictionary<string, CanvasObj>();

	/*=====================================Static Funciton=======================================*/

	/** 取得主要Canvas */
	public static CanvasObj GetMain () {
		if (CanvasUtil._main == null) {
			GameObject prefab = Resources.Load<GameObject>(Const_Canvas.PREFAB_CANVAS);
			CanvasObj instance = GameObject.Instantiate(prefab).GetComponent<CanvasObj>();
			CanvasUtil.RegMain(instance);
		}
		return CanvasUtil._main;
	}

	/** 取得Canvas */
	public static CanvasObj Inst (string key = null) {
		if (key == null) key = "_default";
		
		if (CanvasUtil.key2Canvas.ContainsKey(key)) return CanvasUtil.key2Canvas[key];
		
		GameObject prefab = Resources.Load<GameObject>(Const_Canvas.PREFAB_CANVAS);
		CanvasObj instance = GameObject.Instantiate(prefab).GetComponent<CanvasObj>();
		instance.key = key;
		
		GameObject canvasGObj = RootUtil.GetMember("Canvas");
		instance.transform.SetParent(canvasGObj.transform, false);
		
		CanvasUtil.Reg(key, instance);

		return instance;
	}

	/** 銷毀Canvas */
	public static void Destroy (string key = null) {
		if (key == null) key = "_default";
		if (CanvasUtil.key2Canvas.ContainsKey(key) == false) return;

		CanvasObj inst = CanvasUtil.key2Canvas[key];

		GameObject.DestroyImmediate(inst.gameObject);	

		CanvasUtil.key2Canvas.Remove(key);

	}

	/** 註冊 */
	public static void Reg (string key, CanvasObj canvas) {
		if (CanvasUtil.key2Canvas.ContainsValue(canvas)) {
			return;
		}
		if (CanvasUtil.key2Canvas.ContainsKey(key)) {
			Debug.Log("[CanvasUtil]: register fail Canvas key[" + key + "] is exist.");
			return;
		}

		CanvasUtil.key2Canvas[key] = canvas;

		canvas.onDestroyed.AddListener(new EventListener(() => {
			CanvasUtil.key2Canvas.Remove(key);
		}).Once());

		if (CanvasUtil._main == null) {
			CanvasUtil.RegMain(canvas);
		}
	}

	/** 註冊主要 */
	public static void RegMain (CanvasObj canvas) {
		CanvasUtil._main = canvas;
		canvas.onDestroyed.AddListener(new EventListener(() => {
			CanvasUtil._main = null;
		}).Once());
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