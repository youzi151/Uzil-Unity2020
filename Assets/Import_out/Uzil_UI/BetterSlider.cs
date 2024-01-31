using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;

public class BetterSlider : Slider {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/


	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	[SerializeField]
	public UnityEvent onPointerUp;
	public override void OnPointerUp(PointerEventData eventData){
		base.OnPointerUp(eventData);
		this.onPointerUp.Invoke();
	}

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/




}