using System.Collections.Generic;

using Uzil;

namespace Uzil.Anim {

public class AnimTransition : IMemoable {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 名稱 */
	public string id = null;

	/** 目標狀態 */
	public string nextState = null;

	/** 條件 */
	public List<AnimCondition> conditions = new List<AnimCondition>();
	
	/** 前一動畫最少需要播放過多久 (0~1) */
	public float exitTime = 1;


	/** 混合時間 (有限支援) */
	public float mixTime = 0;


	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** [IMemoable] 紀錄為Json格式 */
	public object ToMemo () {
		DictSO data = new DictSO();

		/** 名稱 */
		data.Set("id", this.id);

		/** 目標狀態 */
		data.Set("nextState", this.nextState);

		/** 條件 */
		List<object> conditionDatas = new List<object>();
		foreach (AnimCondition each in this.conditions) {
			conditionDatas.Add(each.ToMemo());
		}
		data.Set("conditions", conditionDatas);
		
		/** 前一動畫最少需要播放過多久 (0~1) */
		data.Set("exitTime", this.exitTime);

		/** 混合時間 (有限支援) */
		data.Set("mixTime", this.mixTime);
	

		return data;
	}

	/** [IMemoable] 讀取Json格式 */
	public void LoadMemo (object memoJson) {

		DictSO data = DictSO.Json(memoJson);

		/** 名稱 */
		if (data.ContainsKey("id")) {
			this.id = data.GetString("id");
		}
		
		/** 目標狀態 */
		if (data.ContainsKey("nextState")) {
			this.nextState = data.GetString("nextState");
		}
		
		/** 條件 */
		if (data.ContainsKey("conditions")) {
			List<DictSO> conditions = data.GetList<DictSO>("conditions");
			if (conditions != null) {
				foreach (DictSO each in conditions) {
					AnimCondition clip = AnimFactory.CreateCondition(each);
					this.AddCondition(clip);
				}
			}
		}

		/** 前一動畫最少需要播放過多久 (0~1) */
		if (data.ContainsKey("exitTime")) {
			this.exitTime = data.GetFloat("exitTime");
		}

		/** 混合時間 (有限支援) */
		if (data.ContainsKey("mixTime")) {
			this.mixTime = data.GetFloat("mixTime");
		}
		

	}

	/*=====================================Public Function=======================================*/

	/**
	 * 是否通過
	 * @param parameter 
	 */
	public bool isPass (DictSO parameter) {
		foreach (AnimCondition each in this.conditions) {
			if (each.isPass(parameter) == false) {
				return false;
			}
		}
		return true;
	}

	/**
	 * 加入 條件
	 * @param condition 要加入的條件
	 */
	public AnimTransition AddCondition (AnimCondition condition) {
		if (this.conditions.Contains(condition)) return this;
		this.conditions.Add(condition);
		return this;
	}

	/**
	 * 設置 下一個狀態
	 * @param nextState 下一個要轉移至的狀態
	 */
	public AnimTransition Next (AnimState nextState) {
		this.nextState = nextState.id;
		return this;
	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/


}

}