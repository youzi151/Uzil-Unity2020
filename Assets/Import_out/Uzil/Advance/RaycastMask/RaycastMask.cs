#if SOFTMASK

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using SoftMasking;

namespace Uzil.RaycastMask {

public enum RaycastMaskType {
	Target, Rect
}

public class RaycastMask : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	protected static Dictionary<string, RaycastMask> _id2inst = new Dictionary<string, RaycastMask>();

	/*=====================================Static Funciton=======================================*/

	public static RaycastMask Get (string key = "default") {
		if (RaycastMask._id2inst.ContainsKey(key)) {
			return RaycastMask._id2inst[key];
		} else {
			return null;
		}
	}

	public static void Reg (string key, RaycastMask inst) {
		if (RaycastMask._id2inst.ContainsKey(key)) {
			RaycastMask._id2inst[key] = inst;
		} else {
			RaycastMask._id2inst.Add(key, inst);
		}
	}

	/*=========================================Members===========================================*/

	public string key = "default";

	public RaycastMaskType maskType = RaycastMaskType.Rect;
	protected RaycastMaskType _lastMaskType = RaycastMaskType.Rect;

	public RectTransform maskTarget = null;
	protected RectTransform _lastMaskTarget = null;

	protected Image maskTarget_img = null;

	public Image maskTargetSelf = null;

	public Rect maskRect = Rect.zero;
	protected Rect _lastMaskRect = Rect.zero;

	/*========================================Components=========================================*/

	public SoftMask softMask_out = null;

	public SoftMask softMask_in = null;
	
	public Image block_inner = null;

	public Image block_outter = null;

	/*==========================================Event============================================*/
	
	/*========================================Interface==========================================*/
	
	/*======================================Unity Function=======================================*/

	void Awake () {
		RaycastMask.Reg(this.key, this);
	}

	void Update() {
		if (this.maskRect != this._lastMaskRect) {
			this.SetRect(this.maskRect);
		}
		if (this.maskType != this._lastMaskType) {
			this.SetType(this.maskType);
		}
		if (this.maskTarget != this._lastMaskTarget) {
			this.SetTarget(this.maskTarget);
		}
		
		this.softMask_in.separateMask = this.softMask_in.separateMask;
		this.softMask_out.separateMask = this.softMask_out.separateMask;
	}

	/*=====================================Public Function=======================================*/

	public void SetType (RaycastMaskType maskType) {
		this.maskType = maskType;
		this._lastMaskType = maskType;
		if (maskType == RaycastMaskType.Target) {
			this.maskTargetSelf.gameObject.SetActive(false);
			this.softMask_out.separateMask = this.maskTarget;
			this.softMask_in.separateMask = this.maskTarget;
		} else {
			this.maskTargetSelf.gameObject.SetActive(true);
			this.softMask_out.separateMask = this.maskTargetSelf.rectTransform;
			this.softMask_in.separateMask = this.maskTarget;
		}
	}

	public void SetInnerBlock (bool isShow) {
		this.softMask_in.gameObject.SetActive(isShow);
	}

	public void SetTarget (RectTransform rectTrans) {
		this.maskTarget = rectTrans;
		this._lastMaskTarget = rectTrans;

		if (rectTrans != null) {
			this.maskTarget_img = rectTrans.GetComponent<Image>();
		} else {
			this.maskTarget_img = null;
		}

		this.SetType(this.maskType);
	}

	public void SetRect (Rect rect) {
		this.maskRect = rect;
		this._lastMaskRect = rect;
		this.maskTargetSelf.rectTransform.anchoredPosition = rect.position;
		this.maskTargetSelf.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, rect.height);
		this.maskTargetSelf.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width);
	}

	public void SetImage (Sprite sprite) {
		this.maskTargetSelf.sprite = sprite;
		if (this.maskTarget_img != null) {
			this.maskTarget_img.sprite = sprite;
		}
	}

	public void SetInnerColor (Color color) {
		this.block_inner.color = color;
	}

	public void SetOutterColor (Color color) {
		this.block_outter.color = color;
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}

}

#endif