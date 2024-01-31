using System;
using System.Collections.Generic;

using UnityEngine;

using Uzil.CompExt;
using UzEvent = Uzil.Event;

namespace Uzil.BlockPage {

[ExecuteInEditMode]
public class Block_Switch : BlockObj {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	public int currentIdx = 0;

	public List<string> options = new List<string>();

	/*========================================Components=========================================*/

	public TextExt text = null;

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	public override void Awake () {
		
	}

	/*========================================Interface==========================================*/
	
	/** 設置資料 */
	public override void _SetData (DictSO data) {
		base._SetData(data);

		// 選項
		data.TryGetList<string>("options", (res)=>{
			this.options = res;
		});
		
		// 當前
		data.TryGetInt("currentIdx", (res)=>{
			this.Switch(res);
		});

		// 當 數值改變
		this.handleValueChange<int>(data, (evtData, cb)=>{
			evtData.TryGetInt("value", (res)=>{
				cb(res+1);
			});
		});
	}

	/*=====================================Public Function=======================================*/

	public void Switch (int idx) {
		if (idx < 0 || idx >= this.options.Count) {
			idx = -1;
		}

		this.currentIdx = idx;

		string text;
		if (idx == -1) {
			text = "";
		} else {
			text = this.options[idx];
		}
		this.text.SetText(text);

	}

	public void Next () {
		int next = this.currentIdx+1;
		if (next >= this.options.Count) next = 0;
		this.Switch(next);
	}

	public void Previous () {
		int previous = this.currentIdx-1;
		if (previous < 0) previous = this.options.Count-1;
		this.Switch(previous);
	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/



}

}