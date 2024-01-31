using System;

using UnityEngine;

namespace Uzil.i18n {

public class ReadProcess {

	/*======================================Constructor==========================================*/

	public ReadProcess (string content) {
		this.content = content;
	}

	/*=====================================Static Members========================================*/

	/* 每幀 取代器 的 可用耗時 */
	public const float COSTTIME_MAX = 0.04f;

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/* 起始時間 */
	public float startTime = -1;


	/* 嘗試次數 */
	public int tryTime = 10;

	/* 最後一個Reader */
	public SubReplacer lastReader = null;

	/* 文字內容 */
	private string _content = "";
	public string content {
		get {
			if (this._content == null) return "";
			return this._content;
		}
		set {
			this._content = value;
		}
	}

	/* 下一個階段暫存 */
	public Action next;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/* 最終回覆 */
	public Action<string> onResponse;

	/* 每單字回覆 */
	public Action<string> onEachWord;

	/* 下一階段 */
	public Action<string> onNextSubReader;

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/* 下一個SubReader */
	public void NextSubReader () {
		if (this.tryTime < 0) {
			this.Finish();
			return;
		}
		else {
			this.tryTime--;
		}
		// Debug.Log("next: tryTime:"+this.tryTime);

		if (this.onNextSubReader != null) {
			this.onNextSubReader(this.content);
		}

		float cost = 0f;
		if (this.startTime != -1f) {
			cost = Time.realtimeSinceStartup - this.startTime;
		}

		if (cost > ReadProcess.COSTTIME_MAX) {
			this.startTime = Time.realtimeSinceStartup;
			Invoker.main.Once(this.next);
		}
		else {
			this.next();
		}
	}

	/* 當字詞被代換 */
	public void EachWord () {
		if (this.onEachWord == null) return;
		this.onEachWord(this.content);
	}

	/* 代換作業結束 */
	public void Finish () {
		if (this.onResponse == null) return;
		this.onResponse(this.content);
	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}


}