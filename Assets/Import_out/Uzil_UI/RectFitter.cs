using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Uzil.UI {
	
[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class RectFitter : UIBehaviour, ILayoutSelfController {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*Rect*/
	private RectTransform m_Rect;
	private RectTransform rectTransform{
		get{
			if (m_Rect == null)
				m_Rect = GetComponent<RectTransform>();
			return m_Rect;
		}
	}
	public RectTransform targetRect;

	/*選項*/
	public bool isFit_Left = false;
	public bool isFit_Right = false;
	public bool isFit_Top = false;
	public bool isFit_Bottom = false;

	public float paddingLeft;
	public float paddingRight;
	public float paddingTop;
	public float paddingBottom;

	public float minWidth;
	public float minHeight;
	



	private DrivenRectTransformTracker m_Tracker;

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	public void Update(){
		this.HandleSelfFitting();
	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

	#region Unity Lifetime calls

	protected override void OnEnable(){
		base.OnEnable();
		SetDirty();
	}

	protected override void OnDisable(){
		LayoutRebuilder.MarkLayoutForRebuild(this.rectTransform);
		base.OnDisable();
	}

	#endregion

	protected override void OnRectTransformDimensionsChange(){
	   SetDirty();
	}

	private void HandleSelfFitting(){
		if (this.targetRect == null) return;

		Vector2 orinSize = this.rectTransform.sizeDelta;
		Vector2 targetSize = orinSize;

		
		//TODO:Left
		//TODO:Bottom
		//TODO:Top
		if (this.isFit_Right){
			float right = this.targetRect.anchoredPosition.x + this.targetRect.sizeDelta.x;
			float newWidth = right - this.rectTransform.anchoredPosition.x + this.paddingRight;

			if (newWidth < this.minWidth) newWidth = this.minWidth;
			
			targetSize.x = newWidth;
		}

		this.rectTransform.sizeDelta = targetSize;
	}

	public virtual void SetLayoutHorizontal(){
		m_Tracker.Clear();
		HandleSelfFitting();
	}

	public virtual void SetLayoutVertical(){
		HandleSelfFitting();
	}

	protected void SetDirty(){
		if (!this.gameObject.activeSelf)
			return;

		LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
	}

	#if UNITY_EDITOR
	protected override void OnValidate(){
		SetDirty();
	}
   #endif
}

}