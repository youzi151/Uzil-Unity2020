using System;

using UnityEngine;
using UnityEngine.Events;

using TMPro;

using Uzil.CompExt;
using UzEvent = Uzil.Event;

namespace Uzil.BlockPage {

[ExecuteInEditMode]
public class Block_InputField : BlockObj {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/** 尺寸適應 */
	public TMP_InputField inputField;

	/** 提示文字 */
	public TextExt placeholder;

	/*==========================================Event============================================*/

	public UzEvent onEndEdit = new UzEvent();

	/*======================================Unity Function=======================================*/

	public override void Awake () {
		if (this.inputField.onValueChanged.GetPersistentEventCount() == 0) {
			UnityAction<string> onValueChanged = (value)=>{
				this.onValueChanged.Call(DictSO.New().Set("value", value));	
			};
			this.inputField.onValueChanged.AddListener(onValueChanged);
		}

		if (this.inputField.onEndEdit.GetPersistentEventCount() == 0) {
			UnityAction<string> onEndEdit = (value)=>{
				this.onEndEdit.Call(DictSO.New().Set("value", value));	
			};
			this.inputField.onEndEdit.AddListener(onEndEdit);
		}
	}

	/*========================================Interface==========================================*/

	/** 設置資料 */
	public override void _SetData (DictSO data) {
		base._SetData(data);

		// 文字
		if (data.ContainsKey("text")) {
			this.inputField.text = data.GetString("text");
		}

		// 提示文字
		if (data.ContainsKey("placeholder")) {
			this.placeholder.SetText(data.GetString("placeholder"));
		}

		// 可否互動
		if (data.ContainsKey("isInteractable")) {
			this.inputField.interactable = data.GetBool("isInteractable");
		}

		// 當數值改變
		this.handleValueChange<string>(data, (evtData, cb)=>{
			evtData.TryGetString("value", (res)=>{
				cb(res);
			});
		});

		// 當結束編輯
		if (data.ContainsKey("onEndEdit")) {
			var scriptOrCBID = data.Get("onEndEdit");
			Action<string> cb = this.createCBwithData<string>(scriptOrCBID);
			if (cb != null) {
				this.onEndEdit.AddListener(new EventListener((data)=>{
					cb(data.GetString("value"));
				}).ID("_onEndEdit"));
			}
		}

	}

	/** 當刷新頁面 */
	public override void OnPageUpdate (DictSO pageData) {
		
	}

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/



}

}