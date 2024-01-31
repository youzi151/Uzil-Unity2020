using System;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using UzEvent = Uzil.Event;

namespace Uzil.BlockPage {

[ExecuteInEditMode]
public class Block_Slider : BlockObj {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	public Slider slider = null;

	/*==========================================Event============================================*/

	public UzEvent onEndEdit = new UzEvent();

	/*======================================Unity Function=======================================*/

	public override void Awake () {
		UnityAction<float> onValueChanged = (value)=>{
			this.onValueChanged.Call(DictSO.New().Set("value", value));	
		};
		this.slider.onValueChanged.AddListener(onValueChanged);
	}

	public void Call_OnEndEdit () {
		this.onEndEdit.Call(new DictSO().Set("value", this.slider.value));
	}

	/*========================================Interface==========================================*/
	
	/** 設置資料 */
	public override void _SetData (DictSO data) {
		base._SetData(data);
		
		// 當 數值改變
		this.handleValueChange<int>(data, (evtData, cb)=>{
			evtData.TryGetInt("value", (res)=>{
				cb(res);
			});
		});

		
		// 當 完成改變
		if (data.ContainsKey("onEndEdit")) {
			var scriptOrCBID = data.Get("onEndEdit");
			Action<int> cb = this.createCBwithData<int>(scriptOrCBID);
			if (cb != null) {
				this.onEndEdit.AddListener(new EventListener((data)=>{
					int idx = data.GetInt("value")+1;
					cb(idx);
				}).ID("_onEndEdit"));
			}
		}

		// 最小值
		data.TryGetFloat("min", (res)=>{
			this.slider.minValue = res;
		});
		// 最大值
		data.TryGetFloat("max", (res)=>{
			this.slider.maxValue = res;
		});

		// 數值
		data.TryGetFloat("value", (res)=>{
			this.slider.SetValueWithoutNotify(res);
		});

	}

	/*=====================================Public Function=======================================*/

	public void Next () {
		this.slider.value = Mathf.Clamp(this.slider.value+1, this.slider.minValue, this.slider.maxValue);
		this.Call_OnEndEdit();
	}

	public void Previous () {
		this.slider.value = Mathf.Clamp(this.slider.value-1, this.slider.minValue, this.slider.maxValue);
		this.Call_OnEndEdit();
	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/



}

}