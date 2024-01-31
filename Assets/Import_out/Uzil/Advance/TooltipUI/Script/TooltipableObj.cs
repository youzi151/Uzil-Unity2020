using UnityEngine;

using Uzil.Res;
using Uzil.Misc;
using Uzil.BuiltinUtil;

namespace Uzil.TooltipUI {

public class TooltipableObj : MonoBehaviour {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	public const string PREFAB_UI_TOOLTIPOBJ = "TooltipUI/Prefab/TooltipObj";

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 是否為UI */
	public bool isInWorldSpace = false;

	/** 提示需要秒數 */
	public float hintTime = 0.6f;
	protected float hintTimeCount = -99f;

	/** 追蹤目標 */
	protected Transform followTarget;

	/** 提示物件 */
	protected TooltipObj tooltipObj;

	/** 是否在關閉後 仍然持有TooltipObj */
	public bool isKeepAfterClose = false;

	/** 預設內容 原先的父物件 */
	protected Transform _contentOrinParent = null;

	/*========================================Components=========================================*/

	/** 
	 * 提示要顯示的地方
	 * 若 無 則以 此物件 為 目標
	 */
	public Transform hintLocator;

	/** 預設內容 */
	public RectTransform content = null;
	
	/** 攝影機 (若目標在世界空間時使用) */
	public Camera worldMode_camera = null;

	/** 提示要待的UI空間Canvas */
	public Canvas worldMode_canvas = null;


	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	public void Awake () {
		if (this.content != null) this.content.gameObject.SetActive(false);
	}

	public void Update () {
		
		if (this.hintTimeCount == -99f) {
			//do nothing
		}
		else if (this.hintTimeCount > 0) {
			this.hintTimeCount -= Time.unscaledDeltaTime;
		}
		else if (this.hintTimeCount < 0 && this.hintTimeCount != -99f) {
			this.hintTimeCount = -99f;
			this._showHint();
		}
	}

	public void OnDestroy () {
		this.Close();
	}

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	//=====================

	/** 設置追蹤目標 (當目標在3D 此物件在2D) */
	public void SetTarget (Transform targetIn3D) {
		this.followTarget = targetIn3D;
	}

	/** 設置位置 (在Canvas中) */
	public void SetPosition2D (Vector2 pos) {
		// 確認是 RectTransform
		if ((this.transform is RectTransform) == false) return;

		// 設置位置
		(this.transform as RectTransform).anchoredPosition = pos;
	}

	/** 設置位置 (在3D空間中) */
	public void SetPosition3D (Vector3 pos) {
		this.transform.position = pos;
	}

	//======================

	/** 顯示提示 */
	public void Show () {
		this.hintTimeCount = this.hintTime;
	}
	protected void _showHint () {

		// 取得 Canvas
		Canvas canvas;
		// 若指定 在 世界空間
		if (this.isInWorldSpace) {
			// 從指定中取得
			canvas = this.worldMode_canvas;
		}
		// 否則 從 自身父層尋找
		else {
			canvas = RectTransformUtil.FindParentCanvas(this.gameObject);
		}
		// 若仍然沒有則 使用 Canvas.main
		if (canvas == null) {
			canvas = CanvasUtil.GetMain().canvas;
		}

		// 若 tooltip物件 為 空
		if (this.tooltipObj == null) {
			
			// 建立、取得
			GameObject prefab = ResMgr.Get<GameObject>(new ResReq(TooltipableObj.PREFAB_UI_TOOLTIPOBJ));
			GameObject hintGObj = GameObject.Instantiate(prefab);
			hintGObj.transform.SetParent(canvas.transform, false);
			hintGObj.transform.localPosition = Vector2.zero;
		
			this.tooltipObj = hintGObj.GetComponent<TooltipObj>();
		}

		// 若 內容存在
		if (this.content != null) {
			// 紀錄 原先 內容 父物件
			this._contentOrinParent = this.content.parent;
			// 設置 tooltip 內容為 內容
			this.tooltipObj.SetContent(this.content);
			// 開啟 內容
			this.content.gameObject.SetActive(true);
		}

		// 取得 UI跟隨
		UIFollow follow = this.tooltipObj.gameObject.GetComponent<UIFollow>();
		// 設置
		follow.SetFollower((this.tooltipObj.gameObject.transform as RectTransform));
		follow.SetTarget(this.hintLocator == null ? this.transform : this.hintLocator);

		// 若指定 在 世界空間
		if (this.isInWorldSpace) {
			// 若有指定 Camera 則 取用
			if (this.worldMode_camera != null) {
				follow.worldMode_camera = this.worldMode_camera;
			}
			// 否則 以 canvas 取用
			else {
				follow.worldMode_camera = canvas.worldCamera;
			}
		}
		// 若沒有指定 則 設Camera為 空
		else {
			follow.worldMode_camera = null;
		}
	}

	/** 關閉提示 */
	public void Close () {
		this.hintTimeCount = -99f;
		
		TooltipObj tooltip = this.tooltipObj;

		if (tooltip != null) {
			
			// 關閉 tooltip
			tooltip.Close();

			// 若 不要在關閉後繼續持有
			if (!this.isKeepAfterClose) {

				// 當關閉後
				tooltip.onHide.Add(()=>{
					
					// 設置內容為空
					tooltip.SetContent(null);
					// 回復 內容 父物件
					this.content.SetParent(this._contentOrinParent, false);
					// 銷毀 tooltip物件
					GameObject.Destroy(tooltip.gameObject);
				});
				
				// 清空
				this.tooltipObj = null;
			}

			// 當 關閉後 關閉內容
			tooltip.onHide.Add(()=>{		
				this.content.gameObject.SetActive(false);
			});
		}

	}

	/*===================================Protected Function======================================*/

	/*====================================protected Function=======================================*/

}



}