using System;
using System.Collections.Generic;

using Uzil;
using Uzil.Audio;

namespace UZAPI {

public class Audio {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** 設置音量 */
	public static void SetVolumeLinear (string mixerName, float volume) {
		AudioUtil.SetVolumeLinear(mixerName, volume);
	}

	/** 取得音量 */
	public static float GetVolumeLinear (string mixerName) {
		float aa = AudioUtil.GetVolumeLinear(mixerName);
		return aa;
	}

	/** 是否存在 */
	public static bool IsExist (string id) {
		return AudioMgr.Inst().IsExist(id);
	}

	/** 預載 */
	public static string Preload (string id, string pathOrKey, string argsJson) {
		DictSO args = DictSO.Json(argsJson);
		if (args == null) args = new DictSO();

		if (pathOrKey == null) return null;

		AudioObj audio = AudioMgr.Inst().Preload(id, pathOrKey, args);
		if (audio == null) audio = AudioMgr.Inst().Preload(id, AudioSet.Get(pathOrKey), args);
		if (audio == null) return null;

		return audio.id;
	}
	public static void PreloadAsync (string id, string pathOrKey, string argsJson, int callbackID) {
		DictSO args = DictSO.Json(argsJson);
		if (args == null) args = new DictSO();

		Action<string> cb = (audioID)=>{
			Callback.CallLua_cs_arg(callbackID, audioID);
		};

		Action<AudioObj, Action> check = (audio, next)=>{
			if (audio != null) {
				cb(audio.id);
				return;
			}
			if (next != null) next();
		};

		if (pathOrKey == null) {
			check(null, null);
			return;
		}

		AudioMgr audioMgr = AudioMgr.Inst();
		Async.Waterfall(
			new List<Action<Action<bool>>>(){
				(next)=>{
					audioMgr.PreloadAsync(id, pathOrKey, args, (audioObj)=>{
						check(audioObj, ()=>{
							next(true);
						});
					});
				},
				(next)=>{
					audioMgr.PreloadAsync(id, AudioSet.Get(pathOrKey), args, (audioObj)=>{
						check(audioObj, ()=>{
							next(true);
						});
					});
				},
			},
			()=>{
				check(null, ()=>{
					cb(null);
				});
			}
		);
	}

	/** 申請 */
	public static string Request (string id, string pathOrKey, string argsJson) {
		DictSO args = DictSO.Json(argsJson);
		if (args == null) args = new DictSO();

		AudioObj audio = AudioMgr.Inst().Request(id, pathOrKey, args);
		if (audio == null) audio = AudioMgr.Inst().Request(id, AudioSet.Get(pathOrKey), args);
		if (audio == null) return null;

		return audio.id;
	}
	
	/** 設置 */
	public static void Set (string id, string argsJson) {
		DictSO args = DictSO.Json(argsJson);
		if (args == null) args = new DictSO();

		AudioMgr.Inst().Set(id, args);
	}

	/** 播放 */
	public static void Play (string id) {
		AudioMgr.Inst().Play(id);
	}

	/** 播放音效(方便使用) */
	public static string PlaySFX (string pathOrKey, string argsJson = null) {
		DictSO args = DictSO.Json(argsJson);
		if (args == null) args = new DictSO();

		args.Set("mixer", "SFX").Set("isLoop", false).Set("isReleaseOnEnd", true);

		AudioObj audio = AudioMgr.Inst().Emit(pathOrKey, args);
		if (audio == null) audio = AudioMgr.Inst().Emit(AudioSet.Get(pathOrKey), args);
		if (audio == null) return null;

		return audio.id;
	}

	/** 釋放 */
	public static void Release (string id) {
		AudioMgr.Inst().Release(id);
	}

	/** 停止 */
	public static void Stop (string id) {
		AudioMgr.Inst().Stop(id);
	}

	/** 停止所有 */
	public static void StopAll () {
		AudioMgr.Inst().StopAll();
	}

	/** 復原 */
	public static void Resume (string id) {
		AudioMgr.Inst().Resume(id);
	}
	/** 復原所有 */
	public static void ResumeAll () {
		AudioMgr.Inst().ResumeAll();
	}

	/** 暫停 */
	public static void Pause (string id) {
		AudioMgr.Inst().Pause(id);
	}
	/** 暫停所有 */
	public static void PauseAll () {
		AudioMgr.Inst().PauseAll();
	}

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}

}