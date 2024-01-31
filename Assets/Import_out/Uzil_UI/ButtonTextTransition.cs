using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;

namespace Uzil.UI {

/* 20170818 v0.1 */
public class ButtonTextTransition : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler, IDragHandler{


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 目標 */
	public Selectable target;

	/** 要額外作效果的文字 */
	public TMP_Text text;
	public List<TMP_Text> texts = new List<TMP_Text>();

	/** 游標是否放在按鈕上 */
	private bool isOver = false;

	/** 按鈕是否按下 */
	private bool isPressed = false;

	/** 是否可按下 */
	private bool isInteractable = true;


	// 位移=======================
	/** 原始位置 */
	private Dictionary<TMP_Text, Vector2> offsetPos_Orin = new Dictionary<TMP_Text, Vector2>();

	/** 關閉時 位移效果 */
	public Vector2 offsetPos_Disabled;

	/** 按下時 位移效果 */
	public Vector2 offsetPos_Pressed;

	/** 滑過時 位移效果 */
	public Vector2 offsetPos_Highlighted;

	//顏色=======================
	/** 原始位置 */
	private Dictionary<TMP_Text, Color> color_Orin = new Dictionary<TMP_Text, Color>();

	/** 關閉時 顏色效果 */
	public Color color_Disabled = Color.black;

	/** 按下時 顏色效果 */
	public Color color_Pressed = Color.black;

	/** 滑過時 顏色效果 */
	public Color color_Highlighted = Color.black;


	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	void Awake () {
		// 初期設置 ===============

		if (this.target == null) {
			this.target = this.gameObject.GetComponent<Selectable>();
		}
		
		doToText((each)=>{
			this.color_Orin.Add(each, each.color);
			this.offsetPos_Orin.Add(each, ((RectTransform)each.GetComponent<Transform>()).anchoredPosition);
		});

		// 首次 刷新
		if (this.target.interactable != this.isInteractable) {
			this.isInteractable = this.target.interactable;
		}

		this.updateTransition();
	}

	void Update () {
		if (this.target.interactable != this.isInteractable) {
			this.isInteractable = this.target.interactable;
			this.updateTransition();
		}
	}


	/*========================================Interface==========================================*/
	/** 當游標進入 */
	public void OnPointerEnter (PointerEventData eventData) {
		if (this.isButtonEnabled() == false) return;
		this.isOver = true;
		this.updateTransition();
	}

	/** 當游標離開 */
	public void OnPointerExit (PointerEventData eventData) {
		if (this.isButtonEnabled() == false) return;
		this.isOver = false;
		this.updateTransition();
	}

	/** 當按下 */
	public void OnPointerDown (PointerEventData eventData) {
		if (this.isButtonEnabled() == false) return;
		this.isPressed = true;
		this.updateTransition();
	}

	/** 當彈起 */
	public void OnPointerUp (PointerEventData eventData) {
		if (this.isButtonEnabled() == false) return;
		this.isPressed = false;
		this.updateTransition();
	}

	/** 當點擊 */
	public void OnPointerClick (PointerEventData eventData) {

	}

	/*防止PointerUp誤觸發的bug*/
	public void OnDrag (PointerEventData eventData) {}

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

	private bool isButtonEnabled(){
		if (this.target == null){
			this.target = this.GetComponent<Selectable>();
		}
		if (this.target == null) return false;

		return this.target.enabled;
	}

	private void checkAndSetOrin(){
		if (this.isOver || this.isPressed || this.isInteractable == false) return;
		doToText((each)=>{
			this.offsetPos_Orin[each] = ((RectTransform)each.GetComponent<Transform>()).anchoredPosition;
			this.color_Orin[each] = each.color;
		});
	}

	private void updateTransition(){
		if (this.isButtonEnabled() == false) return;

		if (this.target.interactable == false){
			
			//關閉
			this.doToText((each)=>{
				((RectTransform)each.GetComponent<Transform>()).anchoredPosition = this.offsetPos_Orin[each] + this.offsetPos_Disabled;
				each.color = this.color_Disabled;
			});

		}else{

			if (this.isOver){

				//按下
				if (this.isPressed){
					this.doToText((each)=>{
						((RectTransform)each.GetComponent<Transform>()).anchoredPosition = this.offsetPos_Orin[each] + this.offsetPos_Pressed;
						each.color = this.color_Pressed;
					});
				}
				
				//高亮
				else{
					this.doToText((each)=>{
						((RectTransform)each.GetComponent<Transform>()).anchoredPosition = this.offsetPos_Orin[each] + this.offsetPos_Highlighted;
						each.color = this.color_Highlighted;
					});
				}

			}else{

				//原本
				this.doToText((each)=>{
					((RectTransform)each.GetComponent<Transform>()).anchoredPosition = this.offsetPos_Orin[each];
					each.color = this.color_Orin[each];
				});

			}
	
		}

		
	}

	private void doToText(Action<TMP_Text> toDo){
		toDo(this.text);
		for (int i = 0; i < this.texts.Count; i ++) {
			TMP_Text each = this.texts[i];
			toDo(each);
		}
	}


}



}