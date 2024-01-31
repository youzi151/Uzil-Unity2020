using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Audio;

using Uzil.UserData;

namespace Uzil.Audio {

public class AudioUtil {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/* 是否已經初始化 */
	public static bool isInited = false;

	/* 混合器路徑 */
	public static string AUDIOMIXER_PATH = "Audio/AudioMixer";

	/* 混和器 */
	public static AudioMixer mixer;

	public static Dictionary<string, Event> onValChanged = new Dictionary<string, Event>();

	/*=====================================Static Funciton=======================================*/

	/* 
	 * 初始化
	 * 讀取設定
	 */
	public static void Init () {
		// 若 已經初始化 則 返回
		if (AudioUtil.isInited) return;
		
		// 設為 已經初始化完成
		AudioUtil.isInited = true;

		// 讀取 混和器
		AudioUtil.mixer = Resources.Load<AudioMixer>(AudioUtil.AUDIOMIXER_PATH);
		if (AudioUtil.mixer == null) {
			Debug.Log("[AudioUtil]: initial fail! not found AudioMixer in : " + AudioUtil.AUDIOMIXER_PATH);
			return;
		}

		// 取得音量
		DictSO volumeCfg = Config.main.GetObj("audio.cfg", "volumes");
		if (volumeCfg == null) return;
		foreach (KeyValuePair<string, object> pair in volumeCfg) {
			string name = pair.Key;
			float val = DictSO.Float(pair.Value);
			
			// 設置 音量 (若以同步方式執行 可能因為mixer剛建立 而 設置參數失效)
			Invoker.Inst().Once(()=>{
				AudioUtil.SetVolumeLinear(name, val);
			});
		}
	}


	/*== 基本 ================*/

	/* 設置音量 */
	public static void SetVolumeLinear (string paramName, float val) {
		AudioUtil.SetVolume(paramName, AudioUtil.LinearToDecibel(val));
	}

	/* 設置音量 */
	public static void SetVolume (string paramName, float val) {
		if (paramName == null) return;

		val = Mathf.Clamp(val, -144f, 0f);
		AudioUtil.Set("Volume_"+paramName, val);
	}

	/* 取得音量 */
	public static float GetVolumeLinear (string paramName) {
		if (AudioUtil.isInited == false) AudioUtil.Init();
		float val = AudioUtil.Get("Volume_"+paramName);
		val = AudioUtil.DecibelToLinear(val);
		return val;
	}

	/* 註冊 當 音量改變 */
	public static void OnVolumeChange (string paramName, EventListener listener) {
		if (AudioUtil.onValChanged.ContainsKey(paramName) == false) {
			AudioUtil.onValChanged.Add(paramName, new Event());
		}
		AudioUtil.onValChanged[paramName].AddListener(listener);
	}

	/* 註銷 當 音量改變 */
	public static void OffVolumeChange (string paramName, EventListener listener) {
		if (AudioUtil.onValChanged.ContainsKey(paramName) == false) return;
		AudioUtil.onValChanged[paramName].RemoveListener(listener);
	}

	/*== 底層 ================*/

	public static void Set (string paramName, float val) {
		if (AudioUtil.isInited == false) AudioUtil.Init();
		AudioUtil.mixer.SetFloat(paramName, val);

		if (AudioUtil.onValChanged.ContainsKey(paramName)) {
			AudioUtil.onValChanged[paramName].Call(DictSO.New().Set("val", val));
		}
	}

	public static float Get (string paramName) {
		if (AudioUtil.isInited == false) AudioUtil.Init();

		float val;
		bool isGot = AudioUtil.mixer.GetFloat(paramName, out val);
		if (!isGot) return -1;
		return val;
	}


	/*== 公用 ================*/

	public static float LinearToDecibel (float linear) {
		float dB;
		if (linear != 0) {
			dB = 20.0f * Mathf.Log10(linear);
		} else { dB = -144.0f; }

		return dB;
	}

	public static float DecibelToLinear (float dB) {
		float linear = Mathf.Pow(10.0f, dB / 20.0f);
		return linear;
	}

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}



}