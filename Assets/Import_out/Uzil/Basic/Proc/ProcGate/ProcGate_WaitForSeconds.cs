using UnityEngine;

namespace Uzil.Proc {

public class ProcGate_WaitForSeconds : ProcGate {

	/*======================================Constructor==========================================*/

	/*=========================================Members===========================================*/

	// 要保存的變數盡量以string, int, float等基本型別 在 WriteToMemo, FromMemo中自行實作
	// 若有其他型別 則需 自行設計轉換為基本型別做保存

	/* 要等的時間 */
	public float timeToWait = 1f;

	/* 剩餘的時間 */
	private float leftTime = 0f;

	/* 是否正在倒數 */
	public bool isCounting = false;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Update is called once per frame
	public void Update () {

		// 若 正在執行
		if (this.isCounting) {

			// 時間倒數
			this.leftTime -= Time.deltaTime;

			// 檢查條件
			this.CheckIfComplete();
		}


		// 如果要完成後重置狀態、繼續檢查 (例如條件為:每五秒時觸發，然後重置五秒)
		// this.check_always();
	}

	/*========================================Interface==========================================*/

	/* 自行決定呼叫時機 */
	public override void CheckIfComplete () {
		if (!this.isListening) return;

		// 若 時間已用完
		if (this.leftTime <= 0) {
			// 呼叫完成
			this.complete();
			// 暫停檢查
			this.Pause();
		}

		// 若 通過

	}

	/* 開始偵聽 */
	protected override void _begin () {
		// 內容繼承後自定義
	}

	/* 暫停偵聽 */
	protected override void _pause () {
		// 關閉倒數
		this.isCounting = false;
	}

	/* 繼續偵聽 */
	protected override void _resume () {
		// 開啟倒數
		this.isCounting = true;
	}

	/* 暫停偵聽 */
	protected override void _end () {
		// 關閉倒數
		this.isCounting = false;
	}

	/* 重置此條件 */
	protected override void _reset () {
		// 重置時間
		this.leftTime = this.timeToWait;
		// 開啟倒數
		this.isCounting = true;
	}


	/* 紀錄為Json格式 */
	protected override DictSO WriteToMemo () {
		DictSO data = base.WriteToMemo();

		/* 總需時間 */
		data.Set("time", this.timeToWait);

		/* 剩餘時間 */
		data.Set("leftTime", this.leftTime);

		/* 是否倒數中 */
		data.Set("isCounting", this.isCounting ? 1 : 0);

		return data;
	}

	/* 讀取Json格式 */
	protected override void FromMemo (DictSO data) {
		base.FromMemo(data);

		/* 總需時間 */
		if (data.ContainsKey("time")) {
			this.timeToWait = data.GetFloat("time");
		}

		/* 剩餘時間 */
		if (data.ContainsKey("leftTime")) {
			this.leftTime = data.GetFloat("leftTime");
		}

		/* 是否倒數中 */
		if (data.ContainsKey("isCounting")) {
			this.isCounting = data.GetInt("isCounting") > 0 ? true : false;
		}

		return;
	}


	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}



}