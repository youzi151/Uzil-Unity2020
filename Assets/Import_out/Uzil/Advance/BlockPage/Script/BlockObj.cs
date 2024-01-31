using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Uzil;
using Uzil.Lua;
using Uzil.ObjInfo;
using UzEvent = Uzil.Event;

namespace Uzil.BlockPage {


[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(LayoutElement))]
public class BlockObj : UIBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/
	
	/** 辨識 */
	public string id;

	/** 所屬頁面 */
	public BlockPageObj page;
	public BlockRowObj row;

	/** 排版 */
	public LayoutElement layout {
		get {
			return this.GetComponent<LayoutElement>();
		}
	}

	/** 內容 */
	public RectTransform contentRectTrans;

	/** 背景 */
	public Image backgroundImg;

	/** 外距 */
	public RectOffset padding = new RectOffset();

	/** */
	public List<ContentSizeFitter> sizeFitters = new List<ContentSizeFitter>();

	/** 是否反序更新 */
	public virtual bool isReverseUpdateSort {
		get {
			return false;
		}
	}

	/** 適應 */
	
	[SerializeField]
	private bool _isFitWidth = false;
	public bool isFitWidth { 
		get { return this._isFitWidth; }
		set {
			this._isFitWidth = value;
			this.isDirty = true;
		}
	}
	
	[SerializeField]
	private bool _isFitHeight = false;
	public bool isFitHeight { 
		get { return this._isFitHeight; }
		set {
			this._isFitHeight = value;
			this.isDirty = true;
		}
	}

	public bool isDirty = false;


	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	public UzEvent onClick = new UzEvent();
	public UzEvent onValueChanged = new UzEvent();
	public UzEvent onPointerEnter = new UzEvent();
	public UzEvent onPointerExit = new UzEvent();

	/*======================================Unity Function=======================================*/

	protected override void OnEnable() {
		base.OnEnable();
		SetDirty();
	}

	public virtual new void Awake () {
		
	}
	
	// Use this for initialization
	public virtual new void Start () {
		
	}
	
	// Update is called once per frame
	public virtual void Update () {

	}

	/** 當 點擊 */
	public void Call_OnClick () {
		this.onClick.Call();
	}

	/** 當 游標 進入 */
	public void Call_OnPointerEnter () {
		this.onPointerEnter.Call();
	}

	/** 當 游標 離開 */
	public void Call_OnPointerExit () {
		this.onPointerExit.Call();
	}
	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 設置資料 */
	public void SetData (DictSO data) {
		this._SetData(data);
		this.SetDirty();
	}
	public virtual void _SetData (DictSO data) {

		//== 尺寸 ====================

		RectTransform thisTrans = this.transform as RectTransform;
		Vector2 size = thisTrans.sizeDelta;

		// 高度
		if (data.ContainsKey("height")) {
			object _height = data.Get("height");
			string _heightStr = _height.ToString();
			bool isFit = false;

			this.layout.minHeight = -1;
			this.layout.preferredHeight = -1;
			this.layout.flexibleHeight = -1;

			switch (_heightStr) {
				case "fit":
					isFit = true;
					break;

				case "fill":
					size.y = this.row.GetHeight();
					this.layout.flexibleHeight = 1;
					break;

				default:
					_heightStr = null;
					break;
			}

			this.isFitHeight = isFit;
			
			if (_heightStr == null && !this.isFitHeight) {
				float height = data.GetFloat("height");
				this.layout.minHeight = height;
				size.y = height;
			}
		}

		// 寬度
		if (data.ContainsKey("width")) {
			object _width = data.Get("width");
			string _widthStr = _width.ToString();
			bool isFit = false;

			this.layout.minWidth = -1;
			this.layout.preferredWidth = -1;
			this.layout.flexibleWidth = -1;

			switch (_widthStr) {
				case "fit":
					isFit = true;
					break;

				case "fill":
					size.x = this.row.GetWidth();
					this.layout.flexibleWidth = 1;
					break;

				default:
					_widthStr = null;
					break;
			}

			this.isFitWidth = isFit;
			
			if (_widthStr == null && !this.isFitWidth) {
				float width = data.GetFloat("width");
				this.layout.minWidth = width;
				size.x = width;
			}
		}

		thisTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
		thisTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);

		// ======================

		// 內距
		data.TryGetRectOffset("padding", (res)=>{
			this.padding = res;
		});

		// 背景 
		if (this.backgroundImg != null) {
			if (data.ContainsKey("background")) {

				object bgObj = data.Get("background");
				if (bgObj == null) {
					this.backgroundImg.gameObject.SetActive(false);
				} else {
					this.backgroundImg.gameObject.SetActive(true);
					ImageInfo info = new ImageInfo(bgObj);
					info.ApplyOn(this.backgroundImg);
				}
			}
		}


		// if (data.ContainsKey("onValueChanged")) {
		// 	var scriptOrCBID = data.Get("onValueChanged");
		// 	Action<string> cb = this.createCBwithData<string>(scriptOrCBID);
		// 	if (cb != null) {
		// 		this.onValueChanged.AddListener(new EventListener((data)=>{
		// 			cb(data.GetString("value"));
		// 		}).ID("_onValueChanged"));
		// 	}
		// }

		if (data.ContainsKey("onClick")) {
			var scriptOrCBID = data.Get("onClick");
			Action cb = this.createCB(scriptOrCBID);
			if (cb != null) {
				this.onClick.AddListener(new EventListener(cb).ID("_onClick"));
			}
		}

		if (data.ContainsKey("onPointerEnter")) {
			var scriptOrCBID = data.Get("onPointerEnter");
			Action cb = this.createCB(scriptOrCBID);
			if (cb != null) {
				this.onPointerEnter.AddListener(new EventListener(cb).ID("_onPointerEnter"));
			}
		}

		if (data.ContainsKey("onPointerExit")) {
			var scriptOrCBID = data.Get("onPointerExit");
			Action cb = this.createCB(scriptOrCBID);
			if (cb != null) {
				this.onPointerExit.AddListener(new EventListener(cb).ID("_onPointerExit"));
			}
		}

	}

	/** 取得寬度 */
	public virtual float GetWidth () {
		return LayoutUtility.GetPreferredWidth((this.transform as RectTransform));
	}

	/** 取得高度 */
	public virtual float GetHeight () {
		return LayoutUtility.GetPreferredHeight((this.transform as RectTransform));
	}

	/** 重建 */
	public virtual void Rebuild () {
		
		
		if (this.contentRectTrans != null) {

			RectTransform thisTrans = (this.gameObject.transform as RectTransform);
			Vector2 thisSize = thisTrans.rect.size;
			
			// 調整 內容 Transform ==============

			// 位置
			Vector2 contentPos = this.contentRectTrans.anchoredPosition;

			// 依照 內距 調整 位置
			contentPos.x = this.padding.left;
			contentPos.y = -this.padding.top;

			this.contentRectTrans.anchoredPosition = contentPos;


			// 內容適應大小

			// 改變錨點
			// 若 菲容器符合內容寬度 && 菲容器符合內容高度
			if (!this.isFitWidth && !this.isFitHeight) {
				this.contentRectTrans.anchorMin = Vector2.zero;
				this.contentRectTrans.anchorMax = Vector2.one;
			// 若 為 容器符合內容寬度 與 容器符合內容高度
			} else if (this.isFitWidth && this.isFitHeight) {
				this.contentRectTrans.anchorMin = Vector2.up;
				this.contentRectTrans.anchorMax = Vector2.up;
			// 若 為 容器符合內容寬度
			} else if (this.isFitWidth) {
				this.contentRectTrans.anchorMin = Vector2.zero;
				this.contentRectTrans.anchorMax = Vector2.up;
			// 若 為 容器符合內容高度
			} else if (this.isFitHeight) {
				this.contentRectTrans.anchorMin = Vector2.up;
				this.contentRectTrans.anchorMax = Vector2.one;
			}


			Vector2 contentSize = this.contentRectTrans.rect.size;

			// 若 為 非容器符合內容寬度 則 內容尺寸 為 容器尺寸 減去 內距
			if (!this.isFitWidth) {
				contentSize.x = thisSize.x - this.padding.horizontal;
			}

			// 若 為 非容器符合內容高度 則 內容尺寸 為 容器尺寸 減去 內距
			if (!this.isFitHeight) {
				contentSize.y = thisSize.y - this.padding.vertical;
			}

			// 設置大小
			this.contentRectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, contentSize.x);
			this.contentRectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, contentSize.y);
			
			// 調整 自身 Transform ==============
			
			// 若 為 容器符合內容寬度 則
			if (this.isFitWidth) {

				// 設置 各內容之Fitter自適應內容
				this.SetFitter((each)=>{
					each.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
					each.SetLayoutHorizontal();
				});

				// 設置 容器寬度 為 內容寬度 加上 內距
				float minWidth = this.contentRectTrans.rect.width + this.padding.horizontal;
				thisSize.x = minWidth;
				this.layout.preferredWidth = minWidth;

			} else {

				// 取消 各內容之Fitter自適應內容
				this.SetFitter((each)=>{
					each.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
				});
			}

			// 若 為 容器符合內容寬度 則
			if (this.isFitHeight) {

				// 設置 各內容之Fitter自適應內容
				this.SetFitter((each)=>{
					each.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
					each.SetLayoutVertical();
				});

				// 設置 容器高度 為 內容高度 加上 內距
				float minHeight = this.contentRectTrans.rect.height + this.padding.vertical;
				thisSize.y = minHeight;
				this.layout.preferredHeight = minHeight;
				
			} else {
				// 取消 各內容之Fitter自適應內容
				this.SetFitter((each)=>{
					each.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
				});
			}

			// 設置大小
			thisTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, thisSize.x);
			thisTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, thisSize.y);
			// thisTrans.sizeDelta = thisSize;

		}

	}
	
	/** 當刷新頁面 */
	public virtual void OnPageUpdate (DictSO data) {

	}


	/*===================================Protected Function======================================*/
	protected void SetFitter (Action<ContentSizeFitter> act) {
		foreach (ContentSizeFitter each in this.sizeFitters) {
			act(each);
		}
	}


	/// <summary>
	/// Mark the LayoutGroup as dirty.
	/// </summary>
	protected void SetDirty()
	{
		if (!IsActive())
			return;

		this.Rebuild();

		if (!CanvasUpdateRegistry.IsRebuildingLayout()) {
			LayoutRebuilder.MarkLayoutForRebuild(this.transform as RectTransform);
		} else {
			StartCoroutine(DelayedSetDirty(this.transform as RectTransform));
		}

	}

	IEnumerator DelayedSetDirty(RectTransform rectTransform) {
		yield return null;
		LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
	}

#if UNITY_EDITOR
	protected override void OnValidate() {
		SetDirty();
	}

#endif

	/*====================================Private Function=======================================*/

	protected Action<T> createCBwithData<T> (object scriptOrCBID) {
		Action<T> cb = null;
		if (scriptOrCBID is string) {
			cb = (data)=>{
				LuaUtil.DoString((string)scriptOrCBID);
			};
		} else if (DictSO.IsNumeric(scriptOrCBID)) {
			cb = (data)=>{
				int cbID = DictSO.Int(scriptOrCBID);
				UZAPI.Callback.CallLua_cs_arg(cbID, data);
			};
		}
		return cb;
	}

	protected Action createCB (object scriptOrCBID) {
		Action cb = null;
		if (scriptOrCBID is string) {
			cb = ()=>{
				LuaUtil.DoString((string)scriptOrCBID);
			};
		} else if (DictSO.IsNumeric(scriptOrCBID)) {
			cb = ()=>{
				int cbID = DictSO.Int(scriptOrCBID);
				UZAPI.Callback.CallLua_cs(cbID);
			};
		}
		return cb;
	}

	protected void handleValueChange<T> (DictSO data, Action<DictSO, Action<T>> handler) {
		if (data.ContainsKey("onValueChanged")) {
			var scriptOrCBID = data.Get("onValueChanged");
			Action<T> cb = this.createCBwithData<T>(scriptOrCBID);
			if (cb != null) {
				this.onValueChanged.AddListener(new EventListener((evtData)=>{
					handler(evtData, cb);
				}).ID("_onValueChanged"));
			}
		}
	}
}


}
