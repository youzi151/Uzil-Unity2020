using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

using Uzil.UI.UGUI;

using Uzil.i18n;

namespace Uzil.BlockPage {

[ExecuteInEditMode]
public class Block_Select : BlockObj {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	protected List<string> options = new List<string>();

	/*========================================Components=========================================*/

	/** 下拉選單 */
	public Dropdown dropdown;

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	public override void Awake () {
		UnityAction<int, DropdownOptionData, DropdownItem> onValueChanged = (optionIdx, optionData, item)=>{
			this.onValueChanged.Call(DictSO.New().Set("value", optionIdx));	
		};
		this.dropdown.onValueChanged.AddListener(onValueChanged);
	}

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 設置資料 */
	public override void _SetData (DictSO data) {
		base._SetData(data);

		// 當 數值改變
		this.handleValueChange<int>(data, (evtData, cb)=>{
			evtData.TryGetInt("value", (res)=>{
				cb(res+1);
			});
		});;

		// 選項
		data.TryGetList<string>("options", (res)=>{
			this.options = res;
			this.updateOptions();
		});
		
		// 當前
		data.TryGetInt("currentIdx", (res)=>{
			this.dropdown.SetValueWithoutNotify(res);
		});

	}

	/** 當刷新頁面 */
	public override void OnPageUpdate (DictSO pageData) {
		this.updateOptions();
	}

	protected void updateOptions () {
		this.dropdown.ClearOptions();
		List<string> rendered = new List<string>();
		for (int idx = 0; idx < this.options.Count; idx++) {
			rendered.Add(StrReplacer.RenderNow(this.options[idx]));
		}
		this.dropdown.AddOptions(rendered);
	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/



}

}