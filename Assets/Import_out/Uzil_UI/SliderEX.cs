using UnityEngine;
using UnityEngine.UI;

namespace Uzil.UI {

[RequireComponent(typeof(Slider))]
public class SliderEX : MonoBehaviour {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*Slider*/
	private Slider slider;

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	public void AddValue(float i){
		if (this.getSlider() == null) return;

		this.getSlider().value += i;
	}

	public void SubValue(float i){
		if (this.getSlider() == null) return;
		this.getSlider().value -= i;
	}

	/*===================================Protected Function======================================*/
	
	protected Slider getSlider(){
		if (this.slider == null)
			this.slider = this.GetComponent<Slider>();
		return this.slider;
	}

	/*====================================Private Function=======================================*/




}

}