using UnityEngine;
using UnityEngine.UI;

using Uzil.Res;

namespace Uzil.BuiltinUtil {

public class CanvasObj : MonoBehaviour {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/* 識別 */
	public string key = "_default";

	/* Canvas */
	public Canvas canvas {
		get {
			if (this._canvas == null) this._canvas = this.GetComponent<Canvas>();
			return this._canvas;
		}
	}
	private Canvas _canvas;

	public bool isRegisterToCanvasUtil = false;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/* 當銷毀 */
	public Event onDestroyed = new Event();

	/*======================================Unity Function=======================================*/

	void Awake () {
		if (this.isRegisterToCanvasUtil) CanvasUtil.Reg(this.key, this);
	}

	void Update () {

	}

	void OnDestroy () {
		this.onDestroyed.Call();
	}

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/* 建立UI物件 */
	public GameObject CreateUIObj (string resourcePath) {
		GameObject prefab = ResMgr.Get<GameObject>(new ResReq(resourcePath));
		if (prefab == null) return null;

		GameObject gObj = (GameObject) GameObject.Instantiate(prefab, this.transform, false);

		this.AddUIObj(gObj);

		return gObj;
	}

	/* 加入UI物件 */
	public void AddUIObj (GameObject gObj) {
		gObj.transform.SetParent(this.transform, false);
	}

	
	/** 取得 尺寸 */
	public Vector2 GetSize () {
		CanvasScaler scaler = this.GetComponent<CanvasScaler>();
		if (scaler != null) {
			if (scaler.uiScaleMode == CanvasScaler.ScaleMode.ScaleWithScreenSize && scaler.screenMatchMode == CanvasScaler.ScreenMatchMode.MatchWidthOrHeight) {

				Vector2 refRes = scaler.referenceResolution;
				Vector2 scrRes = new Vector2(Screen.width, Screen.height);
				float match = scaler.matchWidthOrHeight;

				float logW = Mathf.Log(scrRes.x / refRes.x, 2);
				float logH = Mathf.Log(scrRes.y / refRes.y, 2);
				float avg = Mathf.Lerp(logW, logH, match);

				float scaleFactor = Mathf.Pow(2, avg);
				return scrRes / scaleFactor;
			}
		}

		return this.canvas.GetComponent<RectTransform>().rect.size;
	}

	/** 取得 參考尺寸 */
	public Vector2 GetRefSize () {
		return this.GetComponent<CanvasScaler>().referenceResolution;
	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/
}


}
