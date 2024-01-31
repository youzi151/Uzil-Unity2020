using System;
using System.Collections.Generic;

using UnityEngine;

namespace Uzil.Anim {

public class Custom_ClipState {

	public AnimClip_Custom clip;
	
	/** 目標Track : 當前影格 */
	public Dictionary<PropTrack, PropKeyframe> track2CurrentKeyframe = new Dictionary<PropTrack, PropKeyframe>();

	/** 目標Track : 預計要應用的 影格 */
	public Dictionary<PropTrack, Queue<PropKeyframe>> track2ToApply = new Dictionary<PropTrack, Queue<PropKeyframe>>();

	/** 目標Track : 已經應用的 影格 */
	public Dictionary<PropTrack, List<PropKeyframe>> track2Applyed = new Dictionary<PropTrack, List<PropKeyframe>>();

	/** 取得該軌道的 預備要應用的關鍵幀 */
	public Queue<PropKeyframe> GetToApply (PropTrack track) {
		Queue<PropKeyframe> toApply;
		if (this.track2ToApply.ContainsKey(track) == false) {
			toApply = new Queue<PropKeyframe>();
			this.track2ToApply.Add(track, toApply);
		} else {
			toApply = this.track2ToApply[track];
		}
		return toApply;
	}

	/** 取得該軌道的 已經應用的關鍵幀 */
	public List<PropKeyframe> GetApplyed (PropTrack track) {
		List<PropKeyframe> applyed;
		if (this.track2Applyed.ContainsKey(track) == false) {
			applyed = new List<PropKeyframe>();
			this.track2Applyed.Add(track, applyed);
		} else {
			applyed = this.track2Applyed[track];
		}
		return applyed;
	}
	
}

public class Animator_Custom : Animator {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 當前時間 */
	protected Dictionary<AnimLayer, float> _layer2Time = new Dictionary<AnimLayer, float>();

	/** 總時間長度 */
	protected Dictionary<AnimLayer, float> _layer2TotalTime = new Dictionary<AnimLayer, float>();

	protected Dictionary<AnimLayer, Dictionary<AnimClip_Custom, Custom_ClipState>> layer2Clip2State = new Dictionary<AnimLayer, Dictionary<AnimClip_Custom, Custom_ClipState>>();

	/** 要更新的對象 */
	protected List<PropTarget> toUpdateTargets = new List<PropTarget>();

	/*========================================Components=========================================*/

	/** 屬性處理器 */
	public Func<string, PropTarget> getTarget = (name)=>{
		return null;
	};

	public Func<List<PropTarget>> getTargets = ()=>{
		return null;
	};

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/* [IMemoable] 讀取Json格式 */
	public override void LoadMemo (object memoJson) {

		DictSO data = DictSO.Json(memoJson);

		// 清除 暫存狀態
		foreach (KeyValuePair<AnimLayer, Dictionary<AnimClip_Custom, Custom_ClipState>> pair in this.layer2Clip2State) {
			pair.Value.Clear();
		}
		this.layer2Clip2State.Clear();

		base.LoadMemo(data);
	}

	/*=====================================Public Function=======================================*/

	/** 
	 * 應用 關鍵幀
	 * @param state 片段狀態
	 * @param track 屬性軌道
	 * @param keyframe 關鍵幀
	 * @param keyframe_next 下一關鍵幀
	 * @param percent 關鍵幀到下一關鍵幀的百分比
	 */
	public void ApplyKeyframe (Custom_ClipState state, PropTrack track, PropKeyframe keyframe, PropKeyframe keyframe_next, float percent = 0) {
		
		// 最後設置的關鍵幀
		PropKeyframe lastKeyframe = null;;

		// 設置 軌道:當前關鍵幀  若 不存在 則 建立
		if (state.track2CurrentKeyframe.ContainsKey(track) == false) {
			state.track2CurrentKeyframe.Add(track, keyframe);
		} else {
			lastKeyframe = state.track2CurrentKeyframe[track];
			state.track2CurrentKeyframe[track] = keyframe;
		}
		
		// 取得 該軌屬性的 處理器
		PropHandler propHandler = PropHandler.GetHandler(track.prop);
		if (propHandler == null) return;

		// 數值 插值與處理
		object value = propHandler.CalculateValue(keyframe, keyframe_next, percent, state.clip.frameRate);

		// 取得目標
		PropTarget target = this.getTarget(track.target);
		if (target == null) return;

		// 若 最後關鍵幀 與 目前要設置的關鍵幀 不同 則 呼叫
		if (keyframe != lastKeyframe) {
			target.OnKeyframeChanged();
		}

		// 設置 數值 到 目標
		propHandler.SetToTarget(target, value, track.isAddtive);
		if (this.toUpdateTargets.Contains(target) == false) {
			this.toUpdateTargets.Add(target);
		}
	}

	/** 
	 * 更新 所有片段的幀
	 * @param layer 指定層級
	 */
	public void UpdateFrameAllClip (AnimLayer layer) {
		
		Custom_ClipState mainClipState = null;

		// 該層級 當前狀態 的 所有片段
		foreach (string clipName in layer.currentState.clips) {
			
			AnimClip clip = this.GetClip(clipName);

			if ((clip is AnimClip_Custom) == false) continue;
			Custom_ClipState clipState = this.getClipState(layer, (AnimClip_Custom)clip);

			// 更新
			this._updateFrame(layer, clipState);

			// 若 主要片段狀態 尚未 指定 則 指定
			if (mainClipState == null) mainClipState = clipState;
		}

		if (mainClipState == null) return;

		float time = this.getTime(layer);
		float totalTime = this.getTotalTime(layer);

		// 若 該片段 為 循環
		if (mainClipState.clip.isLoop) {
			
			// 若 播放時間 超過 總時間
			if (time >= totalTime) {
				// 超出的時間
				float overTime = (time - totalTime) % totalTime;
				// UnityEngine.Debug.Log("time:"+time+" / total:"+totalTime);
				// 重置時間
				this.ResetTime(layer);
				// 設 重置後的時間 為 超出的時間
				this._setTime(layer, overTime);
				// 重新更新
				this.UpdateFrameAllClip(layer);
			}
		}

	}

	/**
	 * 更新 幀
	 * @param layer 指定層級
	 * @param clipState 片段狀態
	 */
	protected void _updateFrame (AnimLayer layer, Custom_ClipState clipState) {

		// 若 當前片段不存在 則 忽略
		if (clipState == null) return;

		AnimClip_Custom clip = clipState.clip;

		// 當前影格數
		float currentFrame = clip.frameRate * this.getTime(layer);

		// 每一軌
		for (int trackIdx = 0; trackIdx < clip.tracks.Count; trackIdx++) {
			PropTrack track = clip.tracks[trackIdx];

			// 預備要應用的關鍵幀
			Queue<PropKeyframe> toApply = clipState.GetToApply(track);
			// 應用過的關鍵幀
			List<PropKeyframe> applyed = clipState.GetApplyed(track);

			// 當前關鍵幀
			PropKeyframe currentKeyframe = null;
			// 下一關鍵幀
			PropKeyframe nextKeyframe = null;

			// 該軌的每一個關鍵幀
			for (int idx = 0; idx < track.keyframes.Count; idx++) {

				PropKeyframe keyframe = track.keyframes[idx];

				// 若 當前影格數 尚未達到 該關鍵幀影格數
				if (currentFrame < keyframe.frame) {
					// 視為下一個關鍵幀 並 跳出
					nextKeyframe = keyframe;
					break;
				} else {
					// 取代為 當前關鍵幀
					currentKeyframe = keyframe;
				}


				// 若 尚未應用過 且 尚未加入待應用關鍵幀佇列
				if (applyed.Contains(keyframe) == false && toApply.Contains(keyframe) == false) {
					// 加入待應用關鍵幀佇列
					toApply.Enqueue(keyframe);
				}

			}

			// 應用每一個有待應用的關鍵幀 (直到剩下一個)
			while (toApply.Count > 1) {

				// 取出該關鍵幀
				PropKeyframe keyframe = toApply.Dequeue();
				// 下一個關鍵幀
				PropKeyframe keyframe_next = toApply.Peek();
				// 設置 關鍵幀 (播放百分比為100%)
				this.ApplyKeyframe(clipState, track, keyframe, keyframe_next, 1);
				// 加入 已經應用過
				applyed.Add(keyframe);
			}
			// 清空 預備要應用的關鍵幀
			toApply.Clear();

			// 若 當前關鍵幀 存在
			if (currentKeyframe != null) {

				float percent = 0;

				// 若下一關鍵幀存在 (只有 最後一幀為關鍵幀、下一關鍵幀為初始幀 才會 不存在下一關鍵幀)
				if (nextKeyframe != null) {
					// 計算 播放百分比
					percent = ((float)(currentFrame - currentKeyframe.frame)) / ((float) (nextKeyframe.frame - currentKeyframe.frame));
				}

				// 設置關鍵幀
				this.ApplyKeyframe(clipState, track, currentKeyframe, nextKeyframe, percent);

				// 加入 已經應用過
				applyed.Add(currentKeyframe);
			}

		}
	}

	/** 應用 變更 至 目標 */
	public void ApplyTargets () {
		foreach (PropTarget target in this.toUpdateTargets) {
			target.Apply(this);
		}
	}

	/** 重設 時間 */
	public void ResetTime (AnimLayer layer) {

		this._setTime(layer, 0f);

		// 該 層級 的 每個片段
		Dictionary<AnimClip_Custom, Custom_ClipState> clip2State = this.getClip2State(layer);
		foreach (KeyValuePair<AnimClip_Custom, Custom_ClipState> pair2 in clip2State) {
		
			// 該片段 的 每個屬性軌道 與 已經應用的關鍵幀
			Custom_ClipState clipState = pair2.Value;
			foreach (KeyValuePair<PropTrack, List<PropKeyframe>> pair3 in clipState.track2Applyed) {
				// 清空
				pair3.Value.Clear();
			}

		}

		List<PropTarget> targets = this.getTargets();
		foreach (PropTarget target in targets) {
			target.OnKeyframeChanged();
		}
	}

	/*===================================Protected Function======================================*/

	
	/*== 子類別實作 ============================ */

	/** 讀取 */
	protected override void load (DictSO data) {

	}

	/** 每幀更新 */
	protected override void update (AnimLayer layer, float deltaTime) {
		float time = this.getTime(layer) + deltaTime * layer.timeScale;
		this._setTime(layer, time);
		this.UpdateFrameAllClip(layer);
	}

	/** 每幀更新後 */
	protected override void layersUpdated (float deltaTime) {
		this.ApplyTargets();
	}

	/**
	 * 播放
	 * @param anim 要播放的動畫片段
	 */
	protected override void play (AnimLayer layer, AnimClip anim) {
		this.playClip(layer, anim, true);
	}

	/**
	 * 疊加播放
	 * @param anim 要播放的動畫片段
	 */
	protected override void playAdditive (AnimLayer layer, AnimClip anim) {
		this.playClip(layer, anim, false);
	}

	protected void playClip (AnimLayer layer, AnimClip _clip, bool isMain) {
		if ((_clip is AnimClip_Custom) == false) return;

		// Debug.Log("Play Clip: "+layer.id+"/"+_clip.id);

		AnimClip_Custom clip = (AnimClip_Custom) _clip;
		Custom_ClipState clipState = this.getClipState(layer, clip);
		
		// 重置時間
		this.ResetTime(layer);
		
		// 清空 軌道資訊
		clipState.track2Applyed.Clear();
		clipState.track2ToApply.Clear();
		clipState.track2CurrentKeyframe.Clear();

		// 若 影格存在
		if (clip.tracks.Count > 0) {
			
			if (isMain) {

				// 設置 總時間
				float lastFrame = 0;
				foreach (PropTrack track in clip.tracks) {
					foreach (PropKeyframe keyframe in track.keyframes) {
						if (keyframe.frame > lastFrame) {
							lastFrame = keyframe.frame;
						}
					}
				}
				// Debug.Log("["+layer.id+"]  "+lastFrame+"/"+(float) clip.frameRate);
				this.setTotalTime(layer, lastFrame / (float) clip.frameRate);

			}

			// 回到 影格位置0
			foreach (PropTrack track in clip.tracks) {
				PropKeyframe keyframe0 = track.keyframes[0];
				if (keyframe0.frame > 0) continue;
				this.ApplyKeyframe(clipState, track, keyframe0, null, 0);
			}
		}
		
		this.ApplyTargets();
	}

	/** 停止 */
	protected override void stop (AnimLayer layer) {
		this.ResetTime(layer);
	}
	/** 暫停 */
	protected override void pause (AnimLayer layer) {
		// 預設 Animator行為
	}
	/** 復原 */
	protected override void resume (AnimLayer layer) {
		// 預設 Animator行為
	}

	/**
	 * 設置 百分比時間
	 * @param normalizedTime 百分比時間
	 */
	protected override void setNormalizedTime (AnimLayer layer, float normalizedTime) {
		this._setTime(layer, normalizedTime / this.getTotalTime(layer));
		this.UpdateFrameAllClip(layer);
	}

	/** 取得當前播放進度 */
	protected override float getNormalizedTime (AnimLayer layer) {
		return this.getTime(layer) / this.getTotalTime(layer);
	}

	/**
	 * 設置時間
	 * @param time 時間
	 */
	protected override void setTime (AnimLayer layer, float time) {
		this._setTime(layer, time);
		this.UpdateFrameAllClip(layer);
	}
	private bool isUpdatingAllFrame = false;

	protected void _setTime (AnimLayer layer, float time) {
		if (this._layer2Time.ContainsKey(layer)) {
			this._layer2Time[layer] = time;
		} else {
			this._layer2Time.Add(layer, time);
		}
	}

	/**
	 * 取得時間
	 */
	protected override float getTime (AnimLayer layer) {
		if (this._layer2Time.ContainsKey(layer)) {
			return this._layer2Time[layer];
		}
		return 0f;
	}

	/**
	 * 設置時間倍率
	 * @param timeScale 時間倍率
	 */
	protected override void setTimeScale (AnimLayer layer, float timeScale) {
		layer.timeScale = timeScale;
	}
	/**
	 * 取得時間倍率
	 */
	protected override float getTimeScale (AnimLayer layer) {
		return layer.timeScale;
	}

	protected Dictionary<AnimClip_Custom, Custom_ClipState> getClip2State (AnimLayer layer) {
		
		if (this.layer2Clip2State.ContainsKey(layer)) {
			return this.layer2Clip2State[layer];
		}
		
		Dictionary<AnimClip_Custom, Custom_ClipState> clip2State = new Dictionary<AnimClip_Custom, Custom_ClipState>();
		this.layer2Clip2State.Add(layer, clip2State);

		return clip2State;
	}

	protected Custom_ClipState getClipState (AnimLayer layer, AnimClip_Custom clip) {

		Dictionary<AnimClip_Custom, Custom_ClipState> clip2State = this.getClip2State(layer);

		if (clip2State.ContainsKey(clip)) {
			return clip2State[clip];
		} else {
			Custom_ClipState clipState = new Custom_ClipState();
			clipState.clip = clip;
			clip2State.Add(clip, clipState);
			return clipState;
		}
	}

	protected void setTotalTime (AnimLayer layer, float totalTime) {
		if (this._layer2TotalTime.ContainsKey(layer) == false) {
			this._layer2TotalTime.Add(layer, totalTime);
		} else {
			this._layer2TotalTime[layer] = totalTime;
		}
	}

	protected float getTotalTime (AnimLayer layer) {
		if (this._layer2TotalTime.ContainsKey(layer) == false) return 0f;
		return this._layer2TotalTime[layer];
	}


	/*====================================Private Function=======================================*/



}

}