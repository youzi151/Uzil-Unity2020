using UnityEngine.UI;
using UnityEngine.Events;


namespace Uzil.BlockPage {

public class Block_Toggle : BlockObj {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/** Toggle開關 */
	public Toggle toggle;

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	public override void Awake () {
		UnityAction<bool> onValueChanged = (isOn)=>{
			this.onValueChanged.Call(DictSO.New().Set("value", isOn));	
		};
		this.toggle.onValueChanged.AddListener(onValueChanged);
	}

	/*========================================Interface==========================================*/

	/** 設置資料 */
	public override void _SetData (DictSO data) {
		base._SetData(data);

		// 可否互動
		if (data.ContainsKey("isInteractable")) {
			this.toggle.interactable = data.GetBool("isInteractable");
		}

		// 開關
		if (data.ContainsKey("isOn")) {
			this.toggle.isOn = data.GetBool("isOn");
		}

		// 當 數值改變
		this.handleValueChange<bool>(data, (evtData, cb)=>{
			evtData.TryGetBool("value", (res)=>{
				cb(res);
			});
		});

	}

	/** 當刷新頁面 */
	public override void OnPageUpdate (DictSO pageData) {
		
	}

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/



}

}