using System;
using System.Collections.Generic;

using UnityEngine;

using Uzil.Audio;

namespace Uzil.Anim {

public class Animator {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 所有層級 */
	public List<AnimLayer> layers = new List<AnimLayer>();
	
	/** 所有片段 */
	public List<AnimClip> clips = new List<AnimClip>();

	/** 變數 */
	public DictSO parameter = new DictSO();

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/** 當事件 */
	public Event onEvent = new Event();

	/** 當關鍵幀 */
	public Event onTime = new Event();

	/** 當播放完畢 */
	public Event onComplete = new Event();

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** [IMemoable] 紀錄為Json格式 */
	public virtual object ToMemo () {
		DictSO data = new DictSO();

		/* 層級 */
		List<object> layerDatas = new List<object>();
		foreach (AnimLayer each in this.layers) {
			layerDatas.Add(each.ToMemo());
		}
		data.Set("layers", layerDatas);

		/* 片段 */
		List<object> clipDatas = new List<object>();
		foreach (AnimClip each in this.clips) {
			clipDatas.Add(each.ToMemo());
		}
		data.Set("clips", clipDatas);

		/* 變數 */
		data.Set("parameter", this.parameter);

		return data;
	}

	/** [IMemoable] 讀取Json格式 */
	public virtual void LoadMemo (object memoJson) {

		DictSO data = DictSO.Json(memoJson);

		this.Load(data);

	}

	/*=====================================Public Function=======================================*/

	/*== 基本功能 =================*/

	/** 初始化 */
	public void Init () {
		
		// 當播放完畢時檢查
		this.onComplete.Add(()=>{

			this.eachLayer((layer)=>{
				this.checkTransitions(layer, /* force */1);
			});
		});
	
		this.eachLayer((layer)=>{

			// 播放預設狀態
			if (layer.defaultState != null) {
				this.Play(layer, layer.defaultState);
			} else if (layer.states.Count > 0) {
				this.PlayState(layer, layer.states[0]);
			} else {}
		});
	}

	

	/**
	 * 讀取資料
	 * @param data 讀取描述檔
	 */
	public Animator Load (DictSO data) {

		// 片段列表
		if (data.ContainsKey("clips")) {
			List<DictSO> clips = data.GetList<DictSO>("clips");
			if (clips != null) {
				foreach (DictSO each in clips) {
					AnimClip clip = AnimFactory.CreateClip(each);
					this.AddClip(clip);
				}
			}
		}

		// 層級列表
		if (data.ContainsKey("layers")) {
			List<DictSO> layers = data.GetList<DictSO>("layers");
			if (layers != null) {
				foreach (DictSO each in layers) {
					AnimLayer layer = AnimFactory.CreateLayer(each);
					this.AddLayer(layer);
				}
			}
		}

		// 變數
		if (data.ContainsKey("parameter")) {
			DictSO parameter = data.GetDictSO("parameter");
			if (parameter != null) {
				this.parameter = parameter.GetCopy();
			}
		}

		this.load(data);

		return this;
	}

	/** 更新 */
	public void Update (float deltaTime) {

		this.eachLayer((layer)=>{
			
			if (layer.isPlaying == false) return;

			this.update(layer, deltaTime);
			
			this.checkTransitions(layer);

		});

		this.layersUpdated(deltaTime);
	}


	
	/*== 播放控制 =================*/

	/** 播放 */
	public void Play (string layerName, string stateName) {
		AnimLayer layer = this.GetLayer(layerName);
		this.Play(layer, stateName);
	}

	public void Play (AnimLayer layer, string stateName) {
		foreach (AnimState each in layer.states) {
			if (each.id == stateName) {
				this.PlayState(layer, each);
				break;
			}
		}
	}

	/** 播放 */
	public void PlayState (AnimLayer layer, AnimState state) {
		if (state == null) {
			this.Stop(layer);
			return;
		}

		// 正在播放中
		layer.isPlaying = true;

		// 改變狀態
		layer.currentState = state;

		// 片段列表
		List<string> clips = state.clips;
		if (clips.Count == 0) return;

		// 播放 主要片段
		AnimClip mainAnim = this.GetClip(clips[0]);
		this.play(layer, mainAnim);

		// 疊加播放 次要片段
		if (clips.Count > 1) {
			for (int idx = 1; idx < clips.Count; idx++) {
				this.playAdditive(layer, this.GetClip(clips[idx]));
			}
		}
	}

	/** 停止 */
	public void Stop (AnimLayer layer) {
		layer.isPlaying = false;
		this.stop(layer);
	}

	/** 暫停 */
	public void Pause (AnimLayer layer) {
		layer.isPlaying = false;
		this.pause(layer);
	}
	
	/** 復原 */
	public void Resume (AnimLayer layer) {
		layer.isPlaying = true;
		this.resume(layer);
	}

	/*== 時間 =================*/
	
	/**
	 * 設置 百分比時間
	 * @param normalizedTime 百分比時間
	 */
	public void SetNormalizedTime (AnimLayer layer, float normalizedTime) {
		this.setNormalizedTime(layer, normalizedTime);
	}
	
	/** 取得 當前播放進度 */
	public float GetNormalizedTime (AnimLayer layer) {
		return this.getNormalizedTime(layer);
	}

	/*== 其他功能 =================*/

	/**
	 * 加入層級
	 * @param layer 層級
	 */
	public Animator AddLayer (AnimLayer layer) {
		if (this.layers.Contains(layer)) return this;
		this.layers.Add(layer);
		return this;
	}

	/**
	 * 移除層級
	 * @param name 欲移除層級的名稱
	 */
	public void RemoveLayer (string name) {
		foreach (AnimLayer each in this.layers) {
			if (each.id != name) continue;
			this.layers.Remove(each);
			break;
		}
	}

	/**
	 * 取得狀態
	 * @param name 欲取得狀態的名稱
	 */
	public AnimLayer GetLayer (string name) {
		foreach (AnimLayer each in this.layers) {
			if (each.id == name) return each;
		}
		return null;
	}

	/**
	 * 加入片段
	 * @param clip 片段
	 */
	public Animator AddClip (AnimClip clip) {
		if (this.clips.Contains(clip)) return this;
		this.clips.Add(clip);
		return this;
	}

	/**
	 * 移除片段
	 * @param name 欲移除片段的名稱
	 */
	public void RemoveClip (string name) {
		foreach (AnimClip each in this.clips) {
			if (each.id != name) continue;
			this.clips.Remove(each);
			break;
		}
	}

	/** 取得片段 */
	public AnimClip GetClip (string name) {
		foreach (AnimClip each in this.clips) {
			if (each.id == name) return each;
		}
		return null;
	}


	/** 設置變數 */
	public void Param (string key, object value) {
		this.parameter.Set(key, value);
	}

	public object GetParam (string key) {
		return this.parameter.Get(key);
	}

	/*===================================Protected Function======================================*/

	/**
	 * 進入轉換 
	 * @param transition 轉場
	 */
	protected void enter (AnimLayer layer, AnimTransition transition) {
		
		// 取得下一個 狀態
		AnimState nextState = layer.GetState(transition.nextState);
		layer.currentState = nextState;

		// 播放狀態
		this.PlayState(layer, layer.currentState);
	}

	/** 檢查並轉場 */
	protected void checkTransitions (AnimLayer layer, float forceNormalizedTime = -1) {

		if (layer.currentState == null) return;

		// 計算 當前播放時間
		float normalizedTime = this.GetNormalizedTime(layer);
		if (forceNormalizedTime != -1){
			normalizedTime = forceNormalizedTime;
		}

		foreach (string each in layer.anyTransitions) {

			AnimTransition transition = layer.GetTransition(each);
			if (transition == null) continue;

			// 若 當前狀態 為 該連接通道的下一狀態 則 忽略
			if (layer.currentState != null) {
				if (transition.nextState == layer.currentState.id) {
					continue;
				}
			}

			bool isEnter = this.checkTransition(layer, transition, normalizedTime);
			if (isEnter) return;
		}

		// 檢查 轉換通道
		foreach (string each in layer.currentState.transitions) {

			AnimTransition transition = layer.GetTransition(each);
			if (transition == null) continue;

			bool isEnter = this.checkTransition(layer, transition, normalizedTime);
			if (isEnter) return;
		}
	}
	protected bool checkTransition (AnimLayer layer, AnimTransition transition, float normalizedTime = -1) {
		// 若 有離開時間限制
		if (transition.exitTime != -1) {
			// 若尚未到達離開時間 則 忽略此通道
			if (normalizedTime < transition.exitTime) {
				return false;
			}
		}

		// 若 該通道條件符合 則 進入通道
		if (transition.isPass(this.parameter)) {
			this.enter(layer, transition);
			return true;
		}

		return false;
	}

	/** 對每個層級 */
	protected void eachLayer (Action<AnimLayer> act, List<string> layerNames = null) {
		
		bool isSpecifyLayer = false;
		if (layerNames != null) {
			if (layerNames.Count > 0) {
				isSpecifyLayer = true;
			}
		}

		foreach (AnimLayer each in this.layers) {

			if (isSpecifyLayer) {
				
				if (layerNames.Contains(each.id)) {
					act(each);
				}

			} else {
				act(each);
			}

		}
	}
	
	/*== 子類別實作 ============================ */

	/** 讀取 */
	protected virtual void load (DictSO data) {}

	/** 每幀更新 */
	protected virtual void update (AnimLayer layer, float deltaTime) {}
	
	/** 每幀更新後 */
	protected virtual void layersUpdated (float deltaTime) {}

	/**
	 * 播放
	 * @param anim 要播放的動畫片段
	 */
	protected virtual void play (AnimLayer layer, AnimClip anim) {}

	/**
	 * 疊加播放
	 * @param anim 要播放的動畫片段
	 */
	protected virtual void playAdditive (AnimLayer layer, AnimClip anim) {}

	/** 停止 */
	protected virtual void stop (AnimLayer layer) {}

	/** 暫停 */
	protected virtual void pause (AnimLayer layer) {}

	/** 復原 */
	protected virtual void resume (AnimLayer layer) {}

	/**
	 * 設置 百分比時間
	 * @param normalizedTime 百分比時間
	 */
	protected virtual void setNormalizedTime (AnimLayer layer, float normalizedTime) {}

	/** 取得當前播放進度 */
	protected virtual float getNormalizedTime (AnimLayer layer) {return 0f;}
	
	/**
	 * 設置時間
	 * @param time 時間
	 */
	protected virtual void setTime (AnimLayer layer, float time) {}

	/**
	 * 取得時間
	 */
	protected virtual float getTime (AnimLayer layer) {return 0f;}

	/**
	 * 設置時間倍率
	 * @param timeScale 時間倍率
	 */
	protected virtual void setTimeScale (AnimLayer layer, float timeScale) {}

	/**
	 * 取得時間倍率
	 */
	protected virtual float getTimeScale (AnimLayer layer) {return 0;}


	/*====================================Private Function=======================================*/


}

}