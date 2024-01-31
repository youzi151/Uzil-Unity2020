using System.Collections;

using UnityEngine;

namespace Uzil.Proc {

public class ProcGate_Template : ProcGate {

	/*======================================Constructor==========================================*/

	/*=========================================Members===========================================*/
	
	// 要保存的變數盡量以string, int, float等基本型別 在 WriteToMemo, FromMemo中自行實作
	// 若有其他型別 則需 自行設計轉換為基本型別做保存

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Update is called once per frame
	public void Update () {
		// 如果要完成後重置狀態、繼續檢查 (例如條件為:每五秒時觸發，然後重置五秒)
		// this.check_always();
	}

	/*========================================Interface==========================================*/

	/* 自行決定呼叫時機 */
	public override void CheckIfComplete () {
		if (!this.isListening) return;

		// 內容繼承後自定義
		// 例如:
		
		// 作某種檢查
		
		// 若沒通過
		// return;

		// 若通過
		this.complete();
		
	}

	/* 開始偵聽 */
	protected override void _begin () {
		// 內容繼承後自定義
	}

	/* 暫停偵聽 */
	protected override void _pause () {
		// 內容繼承後自定義
	}

	/* 繼續偵聽 */
	protected override void _resume () {
		// 內容繼承後自定義
	}
	
	/* 暫停偵聽 */
	protected override void _end () {
		// 內容繼承後自定義
	}

	/* 重置此條件 */
	protected override void _reset () {
		// 內容繼承後自定義
	}


	/* 紀錄為Json格式 */
	protected override DictSO WriteToMemo () {
		DictSO data = base.WriteToMemo();
		// 內容繼承後自定義
		return data;
	}

	/* 讀取Json格式 */
	protected override void FromMemo (DictSO data) {
		base.FromMemo(data);
		// 內容繼承後自定義
		return;
	}


	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}

}