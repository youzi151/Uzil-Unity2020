using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace Uzil.UI {

/* 20170818 v0.1 */
public class ButtonImageTransition : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler, IDragHandler{


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*目標*/
	public Selectable target;

	/*要額外作效果的文字*/
	public Image Image;
	public List<Image> Images = new List<Image>();

	/*游標是否放在按鈕上*/
	private bool isOver = false;

	/*按鈕是否按下*/
	private bool isPressed = false;

	/*是否可按下*/
	private bool isInteractable = true;


	//位移=======================
	/*原始位置*/
	private Dictionary<Image, Vector2> offsetPos_Orin = new Dictionary<Image, Vector2>();

	/*關閉時 位移效果*/
	public Vector2 offsetPos_Disabled;

	/*按下時 位移效果*/
	public Vector2 offsetPos_Pressed;

	/*滑過時 位移效果*/
	public Vector2 offsetPos_Highlighted;

	//顏色=======================
	/*原始顏色*/
	private Dictionary<Image, Color> color_Orin = new Dictionary<Image, Color>();

	/*關閉時 顏色效果*/
	public Color color_Disabled = Color.black;

	/*按下時 顏色效果*/
	public Color color_Pressed = Color.black;

	/*滑過時 顏色效果*/
	public Color color_Highlighted = Color.black;


	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	void Awake () {
		if (this.target == null) {
			this.target = this.gameObject.GetComponent<Selectable>();
		}
	}

	void Start () {
		
		doToImage((each)=>{
			this.color_Orin.Add(each, each.color);
			this.offsetPos_Orin.Add(each, ((RectTransform)each.GetComponent<Transform>()).anchoredPosition);
		});

		this.updateTransition();
	}

	void Update () {

		if (this.target.interactable != this.isInteractable){
			this.isInteractable = this.target.interactable;
			this.updateTransition();
		}
		
	}

	/*========================================Interface==========================================*/
	/*當游標進入*/
	public void OnPointerEnter(PointerEventData eventData){
		if (this.isButtonEnabled() == false) return;
		this.isOver = true;
		this.updateTransition();
	}

	/*當游標離開*/
	public void OnPointerExit(PointerEventData eventData){
		if (this.isButtonEnabled() == false) return;
		this.isOver = false;
		this.updateTransition();
	}

	/*當按下*/
	public void OnPointerDown(PointerEventData eventData){
		if (this.isButtonEnabled() == false) return;
		this.isPressed = true;
		this.updateTransition();
	}

	/*當彈起*/
	public void OnPointerUp(PointerEventData eventData){
		if (this.isButtonEnabled() == false) return;
		this.isPressed = false;
		this.updateTransition();
	}

	/*當點擊*/
	public void OnPointerClick(PointerEventData eventData){

	}

	/*防止PointerUp誤觸發的bug*/
	public void OnDrag(PointerEventData eventData){}

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
		doToImage((each)=>{
			this.color_Orin[each] = each.color;
			this.offsetPos_Orin[each] = ((RectTransform)each.GetComponent<Transform>()).anchoredPosition;
		});
	}

	private void updateTransition(){

		if (this.target.interactable == false){
			
			//關閉
			this.doToImage((each)=>{
				((RectTransform)each.GetComponent<Transform>()).anchoredPosition = this.offsetPos_Orin[each] + this.offsetPos_Disabled;
				each.color = this.color_Disabled;
			});

		}else{

			if (this.isOver){

				//按下
				if (this.isPressed){
					this.doToImage((each)=>{
						((RectTransform)each.GetComponent<Transform>()).anchoredPosition = this.offsetPos_Orin[each] + this.offsetPos_Pressed;
						each.color = this.color_Pressed;
					});
				}
				
				//高亮
				else{
					this.doToImage((each)=>{
						((RectTransform)each.GetComponent<Transform>()).anchoredPosition = this.offsetPos_Orin[each] + this.offsetPos_Highlighted;
						each.color = this.color_Highlighted;
					});
				}

			}else{

				//原本
				this.doToImage((each)=>{
					((RectTransform)each.GetComponent<Transform>()).anchoredPosition = this.offsetPos_Orin[each];
					each.color = this.color_Orin[each];
				});

			}
	
		}

	}

	private void doToImage(Action<Image> toDo){
		toDo(this.Image);
		for (int i = 0; i < this.Images.Count; i ++) {
			Image each = this.Images[i];
			toDo(each);
		}
	}

}



}