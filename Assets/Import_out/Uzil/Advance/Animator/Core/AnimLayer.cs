using System.Collections.Generic;

using Uzil.Audio;

namespace Uzil.Anim {

public class AnimLayer {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 所屬動畫狀態機 */
	public Animator animator = null;

	/** 識別 */
	public string id = null;

	/** 預設狀態 */
	public string defaultState = "_default";

	/** 當前狀態 */
	public AnimState currentState;

	/** 所有狀態 */
	public List<AnimState> states = new List<AnimState>();

	/** 所有連接通道 */
	public List<AnimTransition> transitions = new List<AnimTransition>();

	/** 所有任意連接通道 */
	public List<string> anyTransitions = new List<string>();

	/** 是否播放中 */
	public bool isPlaying = false;
	
	/** 時間比率 */
	public float timeScale = 1f;


	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** [IMemoable] 紀錄為Json格式 */
	public virtual object ToMemo () {
		DictSO data = new DictSO();

		/* 名稱 */
		data.Set("id", this.id);

		/* 預設狀態 */
		data.Set("defaultState", this.defaultState);

		/* 狀態 */
		List<object> stateDatas = new List<object>();
		foreach (AnimState each in this.states) {
			stateDatas.Add(each.ToMemo());
		}
		data.Set("states", stateDatas);

		/* 連接通道 */
		List<object> transitionDatas = new List<object>();
		foreach (AnimTransition each in this.transitions) {
			transitionDatas.Add(each.ToMemo());
		}
		data.Set("transitions", transitionDatas);

		/* 任意連接通道 */
		data.Set("anyTransitions", this.anyTransitions);

		return data;
	}

	/* [IMemoable] 讀取Json格式 */
	public virtual void LoadMemo (object memoJson) {

		DictSO data = DictSO.Json(memoJson);

		this.Load(data);

	}

	/*=====================================Public Function=======================================*/

	/*== 基本功能 =================*/

	/**
	 * 讀取資料
	 * @param data 讀取描述檔
	 */
	public AnimLayer Load (DictSO data) {

		// 名稱
		if (data.ContainsKey("id")) {
			this.id = data.GetString("id");
		}

		// 預設 狀態
		if (data.ContainsKey("defaultState")) {
			string defaultState = data.GetString("defaultState");
			if (defaultState != null && defaultState != "") {
				this.defaultState = defaultState;
			}
		}

		// 任意連接通道
		if (data.ContainsKey("anyTransitions")) {
			this.anyTransitions = data.GetList<string>("anyTransitions");
		}

		// 連接通道
		if (data.ContainsKey("transitions"))	{
			List<DictSO> transitions = data.GetList<DictSO>("transitions");
			if (transitions != null) {
				foreach (DictSO each in transitions) {
					AnimTransition transition = AnimFactory.CreateTransition(each);
					this.AddTransition(transition);
				}
			}
		}

		// 狀態列表
		if (data.ContainsKey("states")) {
			List<DictSO> states = data.GetList<DictSO>("states");
			if (states != null) {
				foreach (DictSO each in states) {
					AnimState state = AnimFactory.CreateState(each);
					this.AddState(state);
				}
			}
		}

		return this;
	}

	/*== 其他功能 =================*/

	/**
	 * 加入狀態
	 * @param state 狀態
	 */
	public AnimLayer AddState (AnimState state) {
		if (this.states.Contains(state)) return this;
		this.states.Add(state);
		return this;
	}

	/**
	 * 移除狀態
	 * @param name 欲移除狀態的名稱
	 */
	public void RemoveState (string name) {
		foreach (AnimState each in this.states) {
			if (each.id != name) continue;
			this.states.Remove(each);
			break;
		}
	}

	/**
	 * 取得狀態
	 * @param name 欲取得狀態的名稱
	 */
	public AnimState GetState (string name) {
		foreach (AnimState each in this.states) {
			if (each.id == name) return each;
		}
		return null;
	}

	/**
	 * 加入片段
	 * @param transition 片段
	 */
	public AnimLayer AddTransition (AnimTransition transition) {
		if (this.transitions.Contains(transition)) return this;
		this.transitions.Add(transition);
		return this;
	}

	/**
	 * 移除片段
	 * @param id 欲移除片段的名稱
	 */
	public void RemoveTransition (string id) {
		foreach (AnimTransition each in this.transitions) {
			if (each.id != id) continue;
			this.transitions.Remove(each);
			break;
		}
	}

	/** 取得片段 */
	public AnimTransition GetTransition (string id) {
		foreach (AnimTransition each in this.transitions) {
			if (each.id == id) return each;
		}
		return null;
	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/


}

}