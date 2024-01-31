using System.Collections.Generic;

using Uzil;

namespace Uzil.Anim {

public class AnimState : IMemoable {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 識別 */
	public string id = "";

	/** 動畫Clip */
	public List<string> clips = new List<string>();

	/** 連接通道 */
	public List<string> transitions = new List<string>();

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** [IMemoable] 紀錄為Json格式 */
	public object ToMemo () {
		DictSO data = new DictSO();

		/* 名稱 */
		data.Set("id", this.id);

		/* 動畫Clip */
		data.Set("clips", this.clips);

		/* 連接通道 */
		data.Set("transitions", this.transitions);

		return data;
	}

	/** [IMemoable] 讀取Json格式 */
	public void LoadMemo (object memoJson) {

		DictSO data = DictSO.Json(memoJson);

		// 名稱
		this.id = data.GetString("id");
		
		// 片段
		if (data.ContainsKey("clips")) {
			List<string> clips = data.GetList<string>("clips");
			if (clips != null) this.clips = clips;
		}

		// 轉換通道
		if (data.ContainsKey("transitions")) {
			List<string> transitions = data.GetList<string>("transitions");
			if (transitions != null) this.transitions = transitions;
		}

	}

	/*=====================================Public Function=======================================*/

	/*== 基本功能 =================*/

	/**
	 * 加入 轉換通道
	 * @param transition 轉換通道
	 */
	public AnimState AddTransition (string transition) {
		this.transitions.Add(transition);
		return this;
	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/


}

}