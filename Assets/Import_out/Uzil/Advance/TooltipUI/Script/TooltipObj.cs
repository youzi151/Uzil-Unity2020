using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Uzil.BuiltinUtil;
using UzEvent = Uzil.Event;

namespace Uzil.TooltipUI {

public enum TooltipDirection {
	Up, Down, Both
}


/**
 * 提示視窗物件
 * 此物件建議直接放置在Canvas底下 或 等同Canvas矩形之容器
 */
public class TooltipObj : MonoBehaviour, IMemoable {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	public const string PREFAB_TOOLTIPOBJ = "TooltipUI/Prefab/ToolTipObj";

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*== 設置 ==========*/

	/** 預設方向 */
	public TooltipDirection defaultDirection = TooltipDirection.Both;

	/** 是否保留在視窗內 */
	public bool isKeepInView = true;
	
	/** 指標 到 邊界 的 保持距離 */
	public float keepDistance_PointerToBorder = 20f;

	/** 指標中心點 */
	public float pointerPivotX = 0.5f;
	
	/** 內容外距 */
	public RectOffset contentPadding = new RectOffset();

	/** 視窗偏移 */
	public Vector2 up_windowOffset;
	public Vector2 down_windowOffset;

	/** 內容 */
	protected RectTransform content;

	/*== 屬性 ==========*/

	/** 視窗向上 */
	protected bool isWindowUp = true;

	/** 內容content初始的位置 */
	protected Vector2 orinContentOffset;

	/** Canvas */
	protected Canvas _canvas;
	protected Canvas canvas {
		get {
			if (this._canvas == null) {
				this._canvas = RectTransformUtil.FindParentCanvas(this.gameObject);
			}
			return this._canvas;
		}
	}

	/** 內容視窗 */
	protected RectTransform contentContainer;
	
	/** 內容位移 */
	protected RectTransform contentLocator;

	/** 視窗邊界 */
	protected RectTransform windowBorder;

	/*========================================Components=========================================*/

	/** 要設置的內容 */
	public RectTransform toSetContent;

	/** 動畫 */
	public Animator animator;

	/** 背景-視窗 */
	public List<Image> backgroundImgs = new List<Image>();
	/** 背景-箭頭 */
	public List<Image> outlineImgs = new List<Image>();

	[Header("Up")]
	public GameObject up_root;
	public RectTransform up_offsetTrans;
	public RectTransform up_contentContainer;
	public RectTransform up_windowBorder;
	private Vector2 up_orinContentOffset;

	[Header("Down")]
	public GameObject down_root;
	public RectTransform down_offsetTrans;
	public RectTransform down_contentContainer;
	public RectTransform down_windowBorder;
	private Vector2 down_orinContentOffset;


	/*==========================================Event============================================*/

	public UzEvent onHide = new UzEvent();

	/*======================================Unity Function=======================================*/

	void Awake () {
		// 設置 上下偏移
		this.up_orinContentOffset = this.up_contentContainer.localPosition;
		this.down_orinContentOffset = this.down_contentContainer.localPosition;

		// 切換 視窗上下
		this.SwitchUpDown(this.isWindowUp);
		
		// 顯示
		this.Show();
	}

	void LateUpdate () {
		if (this.canvas == null) return;

		if (this.content != null) {

			if (this.content.parent != this.contentContainer) {
				this.content.SetParent(this.contentContainer, false);
				// this.content.anchorMin = this.contentContainer.anchorMin;
				// this.content.anchorMax = this.contentContainer.anchorMax;
			}

			// 尺寸相符
			Vector2 size = this.content.sizeDelta;
			size.x = size.x + this.contentPadding.horizontal;
			size.y = size.y + this.contentPadding.vertical;
			this.up_contentContainer.sizeDelta = size;
			this.down_contentContainer.sizeDelta = size;

			// 中心點對位置
			this.content.pivot = this.contentContainer.pivot;
			this.content.localPosition = new Vector2(
				this.contentPadding.left - (this.contentPadding.horizontal / 2),
				this.isWindowUp ? this.contentPadding.bottom : -this.contentPadding.top
			);
		}

		this.up_offsetTrans.localPosition = this.up_windowOffset;
		this.down_offsetTrans.localPosition = this.down_windowOffset;

		// 更新 邊界
		if (this.isKeepInView) {
			this.UpdateYBorder();
			this.UpdateXBorder();
		} 
	}
	
	/** 當 隱藏後 */
	// 由動畫呼叫
	public void Call_onHided () {
		this.onHide.Call();
	}

	/*========================================Interface==========================================*/

	/** [IMemoable] 紀錄為Json格式 */
	public object ToMemo () {
		DictSO data = DictSO.New();

		Color backgroundColor = Color.black;
		if (this.backgroundImgs.Count > 0) {
			backgroundColor = this.backgroundImgs[0].color;
		}
		data.Set("backgroundColor", backgroundColor);

		Color outlineColor = Color.white;
		if (this.outlineImgs.Count > 0) {
			outlineColor = this.outlineImgs[0].color;
		}
		data.Set("outlineColor", outlineColor);

		data.Set("defaultDirection", DictSO.EnumTo<TooltipDirection>(this.defaultDirection));

		data.Set("keepDistance_PointerToBorder", this.keepDistance_PointerToBorder);

		data.Set("pointerPivotX", this.pointerPivotX);
		
		data.Set("contentPadding", this.contentPadding);

		data.Set("windowOffset_up", this.up_windowOffset);
		data.Set("windowOffset_down", this.down_windowOffset);
		
		return data;
	}
	
    /** [IMemoable] 讀取Json格式 */
	public void LoadMemo (object memoJson) {
		DictSO data = DictSO.Json(memoJson);

		data.TryGetColor("backgroundColor", (res)=>{
			this.SetBackgroundColor(res);
		});

		data.TryGetColor("outlineColor", (res)=>{
			this.SetOutlineColor(res);
		});

		data.TryGetEnum<TooltipDirection>("defaultDirection", (res)=>{
			this.defaultDirection = res;
		});

		data.TryGetFloat("keepDistance_PointerToBorder", (res)=>{
			this.keepDistance_PointerToBorder = res;
		});

		data.TryGetFloat("pointerPivotX", (res)=>{
			this.pointerPivotX = res;
		});
		
		data.TryGetRectOffset("contentPadding", (res)=>{
			this.contentPadding = res;
		});
		
		data.TryGetVector2("windowOffset_up", (res)=>{
			this.up_windowOffset = res;
		});

		data.TryGetVector2("windowOffset_down", (res)=>{
			this.down_windowOffset = res;
		});
	}

	/*=====================================Public Function=======================================*/

	/** 設置 內容 */
	public void SetContent (RectTransform content) {
		this.content = content;
		if (this.content != null) {
			this.content.SetParent(this.contentContainer, false);
		}
	}

	/** 切換 視窗上下 */
	public void SwitchUpDown (bool isUp) {
		this.contentContainer = isUp ? this.up_contentContainer : this.down_contentContainer;

		this.contentLocator = isUp ? this.up_contentContainer : this.down_contentContainer;
		this.windowBorder = isUp ? this.up_windowBorder : this.down_windowBorder;
		this.orinContentOffset = isUp ? this.up_orinContentOffset : this.down_orinContentOffset;

		this.up_root.SetActive(isUp);
		this.down_root.SetActive(!isUp);

		this.isWindowUp = isUp;

	}

	/** 顯示 */
	public void Show () {
		this.animator.Play("Show");
	}

	/** 關閉 */
	public void Hide () {
		this.animator.Play("Hide");
	}
	public void Close () {
		this.Hide();
	}

	/** 刷新邊界 */
	public void UpdateYBorder () {
		// 畫布大小
		Vector2 canvasSize = (this.canvas.transform as RectTransform).sizeDelta;

		bool isCheckUp = this.isWindowUp;

		RectTransform border = this.windowBorder;
		switch (this.defaultDirection) {
			case TooltipDirection.Up:
				border = this.up_windowBorder;
				isCheckUp = true;
				break;
			case TooltipDirection.Down:
				border = this.down_windowBorder;
				isCheckUp = false;
				break;
		}

		// 取得 此視窗在 螢幕中的 四周 (以左下角為原點)
		Rect borderRect = RectTransformUtil.GetRectInCanvas(this.canvas, border);
		float top = borderRect.y + borderRect.height;
		float bottom = borderRect.y;

		// 上方超出的距離
		float topOutDistance = top - canvasSize.y;
		// 下方超出的距離
		float bottomOutDistance = -bottom;

		// 若 當前視窗在上
		if (this.isWindowUp) {
			// 若 要檢查上方是否可行 且 上方視窗有超出
			if (isCheckUp && topOutDistance > 0) {
				this.SwitchUpDown(false);
			}
			// 若 要檢查下方是否可行 且 下方視窗沒超出
			else if (!isCheckUp && bottomOutDistance <= 0) {
				this.SwitchUpDown(false);
			}
		}  
		// 若 當前視窗在下
		else {
			// 若 要檢查上方是否可行 且 上方視窗沒超出
			if (isCheckUp && topOutDistance <= 0) {
				this.SwitchUpDown(true);
			}
			// 若 要檢查下方是否可行 且 下方視窗有超出
			else if (!isCheckUp && bottomOutDistance > 0) {
				this.SwitchUpDown(true);
			}
		}

	}
	public void UpdateXBorder () {
		// 畫布
		RectTransform canvasTrans = (this.canvas.transform as RectTransform);
		Vector2 canvasSize = (this.canvas.transform as RectTransform).sizeDelta;

		// 主體
		RectTransform mainTrans = (this.transform as RectTransform);

		// 要設置的新位置
		Vector2 newPos = this.contentContainer.localPosition;

		// 回歸原始位移
		newPos = this.orinContentOffset;
		float width = this.contentContainer.rect.width - (this.keepDistance_PointerToBorder*2);
		newPos.x = - (width * (-0.5f + Mathf.Clamp01(this.pointerPivotX)));
		this.contentLocator.localPosition = newPos;

		// 還原為原始位置後
		// 取得 此視窗在 螢幕中的 四周 (以左下角為原點)
		Rect borderRect = RectTransformUtil.GetRectInCanvas(this.canvas, this.windowBorder);
		float left = borderRect.x;
		float right = borderRect.x + borderRect.width;

		// 左邊超出的距離
		float leftOutDistance = -left;
		// 右邊超出的距離
		float rightOutDistance = right - canvasSize.x;

		// 主體 X位置
		Rect mainRect = RectTransformUtil.GetRectInCanvas(this.canvas, mainTrans);

		// scale影響的係數 (父物件 以及 螢幕大小)
		float scale_x = (this.contentLocator.lossyScale.x / canvasTrans.localScale.x);

		float keepDistance = this.keepDistance_PointerToBorder * scale_x;

		// 指標是否在連接在內容視窗上
		bool isPointerAttatch = mainRect.x > (0 + keepDistance) && // 大於 左側
								mainRect.x < (canvasSize.x - keepDistance); // 小於 右側

		// 若 右邊有超出 則 把新位置的位置 左縮
		if (rightOutDistance > 0) {
			newPos.x -= rightOutDistance;
			if (!isPointerAttatch) {
				float fix = /*超出的距離*/(mainRect.x - (canvasSize.x - keepDistance));
				newPos.x += fix;
			}
		}
		// 若 左邊有超出 則 把新位置的位置 右縮
		else if (leftOutDistance > 0) {
			newPos.x += leftOutDistance;
			if (!isPointerAttatch) {
				float fix = /*超出的距離*/(0 + keepDistance) - mainRect.x;
				newPos.x -= fix;
			}
		}
		else {
			return;
		}

		// 排除 Scale 影響
		newPos.x = newPos.x / scale_x;

		// 將視窗內容 設置為新位置
		this.contentLocator.localPosition = newPos;

	}

	/** 設置 背景顏色 */
	public void SetBackgroundColor (Color color) {
		color.a = Mathf.Clamp(color.a, 0.004f, 1f);// 0.004f > 1/255
		foreach (Image each in this.backgroundImgs) {
			each.color = color;
		}
	}
	
	/** 設置 背景透明度 */
	public void SetBackgroundAlpha (float alpha) {
		alpha = Mathf.Clamp(alpha, 0.004f, 1f);// 0.004f > 1/255
		foreach (Image each in this.backgroundImgs) {
			Color color = each.color;
			color.a = alpha;
			each.color = color;
		}
	}

	/** 設置 外框顏色 */
	public void SetOutlineColor (Color color) {
		foreach (Image each in this.outlineImgs) {
			each.color = color;
		}
	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/


}



}