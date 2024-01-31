
using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Audio;

using Uzil.Res;
using Uzil.BuiltinUtil;

namespace Uzil.Audio {

public class AudioObj : MonoBehaviour, IMemoable {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	public static GameObject prefab;

	/*=====================================Static Funciton=======================================*/

	/** 建立 (非同步) */
	public static void CreateAsync (string pathOrKey, Action<AudioObj> onDone) {
		ResReq req = ResReq.Parse(pathOrKey);
		req.user = req;

		ResMgr.HoldAudioAsync(req, (resInfo)=>{
			
			if (resInfo == null) {
				onDone(null);
				return;
			}

			AudioClip clip = resInfo.GetResult<AudioClip>();
			if (clip == null) {
				onDone(null);
				return;
			}

			AudioObj audioObj = AudioObj.clip2AudioObj(clip, resInfo);
			if (audioObj == null) {
				onDone(null);
				return;
			}
			
			resInfo.ReplaceUser(req, audioObj);
			ResMgr.UnholdOnDestroy(resInfo, audioObj, audioObj.gameObject);

			onDone(audioObj);
			
		});
	}

	/** 建立 */
	public static AudioObj Create (string pathOrKey) {

		ResReq req = ResReq.Parse(pathOrKey);
		req.user = req;

		ResInfo resInfo = ResMgr.Hold<AudioClip>(req);
		if (resInfo == null) return null;

		AudioClip audioClip = resInfo.GetResult<AudioClip>();
		if (audioClip == null) return null;

		AudioObj audioObj = AudioObj.clip2AudioObj(audioClip, resInfo);
		if (audioObj == null) return null;

		resInfo.ReplaceUser(req, audioObj);
		ResMgr.UnholdOnDestroy(resInfo, audioObj, audioObj.gameObject);

		return audioObj;
	}

	
	/** 建立 */
	protected static AudioObj clip2AudioObj (AudioClip clip, ResInfo resInfo) {
		
		if (clip == null) return null;

		ResInfo info = ResMgr.Hold<GameObject>(new ResReq(Audio_Resource.PREFAB_AUDIOOBJ).User("AudioObj.create"));
		GameObject audioPrefab = info.GetResult<GameObject>();
		GameObject instance = GameObject.Instantiate(audioPrefab);

		AudioObj audioObj = instance.GetComponent<AudioObj>();

		// 設置音效來源
		audioObj.SetAudioClip(clip);

		// 設置銷毀時 回收
		audioObj.onDestroy.AddListener(new EventListener(() => {
			AudioMgr audioMgr = AudioMgr.Inst();
			audioMgr.Release(audioObj);
		}));

		// 設置預設位置
		Transform parent = null;
		if (CameraUtil.GetMain() != null) {
			parent = CameraUtil.GetMain().transform;
		}
		else {
			parent = Camera.main.transform;
		}
		audioObj.transform.SetParent(parent, false);
		audioObj.transform.position = Vector3.zero;

		audioObj.transform.SetParent(AudioMgr.Inst().root);

		return audioObj;
	}

	/*=========================================Members===========================================*/

	/** 識別 */
	public string id;

	/** 路徑或關鍵字 */
	public string pathOrKey;

	/** 是否正在撥放 */
	public bool isPlaying {
		get {
			if (this._isDestroyed) return false;
			return this.audioSource.isPlaying;
		}
	}
	private bool lastIsPlaying = false;

#if UNITY_EDITOR
	public bool debug_isPlaying = false;
#endif

	/** 是否循環 */
	public bool isLoop {
		get {
			if (this._isDestroyed) return false;
			return this.audioSource.loop;
		}
		set {
			if (this._isDestroyed) return;
			this.audioSource.loop = value;
		}
	}

	/** 當前時間 */
	public float currentTime {
		get {
			if (this._isDestroyed) return 0;
			return this.audioSource.time;
		}
	}

	/** Mixer */
	private string _mixer;
	public string mixer {
		get {
			return this._mixer;
		}
	}
	
	/** 音效Mixer (Master) */
	public AudioMixer audioMixer;


	/** 目標音量 */
	[Range(0f, 1f)]
	public float targetVolume = 1f;
	/** 音量漸變時間 */
	public float fadeVolume_sec = 2f;

	/** 所在群組 */
	public List<string> groups = new List<string>();


	/** 是否撥放完後 即 銷毀 */
	public bool isReleaseOnEnd = true;

	/** 是否已通知關閉(用於淡出Loop) */
	public bool isCallStop = false;

	/** 是否已經銷毀 */
	public bool isDestroyed {
		get {
			return this._isDestroyed;
		}
	}
	private bool _isDestroyed = false;

	/*========================================Components=========================================*/

	/** 音效 */
	public AudioSource audioSource;

	/*==========================================Event============================================*/

	/** 當播放結束 */
	public Event onEnd = new Event();

	/** 當銷毀 */
	public Event onDestroy = new Event();

	/*======================================Unity Function=======================================*/

	void Update () {
#if UNITY_EDITOR
		this.debug_isPlaying = this.isPlaying;
#endif

		// 比對 當前音量 與 處理過音量
		float currentVolume = this.audioSource.volume;
		float targetVolume = this.getLayeredVolume();

		// 若不同 則 當前音量 漸變 為 處理過音量
		if (currentVolume != targetVolume) {
			this.audioSource.volume = Mathf.MoveTowards(currentVolume, targetVolume, this.fadeVolume_sec * TimeUtil.Inst().deltaTime);
		}


		if (this.audioSource.clip == null) return;

		// 計算當前播放時間
		float currentTime = this.audioSource.time;
		float totalTime = this.audioSource.clip.length;


		bool isPlaying;
		// 若 當前時間 為 0 則
		if (currentTime == 0f) {
			// 以 本身是否播放動畫 為判斷
			isPlaying = this.isPlaying;
		}
		// 不為零 則
		else {
			// 以 當前時間 是否 仍在 總時間 內 為判斷
		 	isPlaying = currentTime < totalTime;
		}
		
		// 若 上一個是否正在播放 為 在播放 且 目前判斷 為 非播放
		// 則 視為 停止播放
		if (this.lastIsPlaying && isPlaying == false) {
			// 更新狀態
			this.lastIsPlaying = false;
			// 呼叫事件
			this.onEnd.Call();
			EventBus.Inst().Post("onAudioPlayEnd", DictSO.New().Set("audio", this.id));
		} 
		// 若 上一個是否正在播放 為 非播放 且 目前判斷 為 在播放
		else if (!this.lastIsPlaying && isPlaying) {
			// 更新狀態
			this.lastIsPlaying = true;
		}

	}

	void OnDestroy () {
		this._isDestroyed = true;
		this.onDestroy.Call();
	}

	/*========================================Interface==========================================*/

	/** [IMemoable] 紀錄為Json格式 */
	public object ToMemo () {
		DictSO data = new DictSO();

		/* ID */
		data.Set("id", this.id);

		/* 來源 */
		data.Set("src", this.pathOrKey);

		/* 是否正在播放 */
		data.Set("isPlaying", this.isPlaying);
		/* 是否循環 */
		data.Set("isLoop", this.isLoop);
		/* 當前時間 */
		data.Set("currentTime", this.currentTime);
		/* Mixer */
		data.Set("mixer", this.mixer);
		/* 目標音量 */
		data.Set("volume", this.targetVolume);

		/* pitch */
		data.Set("pitch", this.audioSource.pitch);

		/* 音量漸變時間 */
		data.Set("volumeFadeTime", this.fadeVolume_sec);

		/* 所在圖層 */
		data.Set("layers", this.groups);

		/* 是否撥放完後 即 銷毀 */
		data.Set("isReleaseOnEnd", this.isReleaseOnEnd);

		/* 是否已通知關閉(用於淡出Loop) */
		data.Set("isCallStop", this.isCallStop);

		return data;
	}

	/** [IMemoable] 讀取Json格式 */
	public void LoadMemo (object memoJson) {
		DictSO data = DictSO.Json(memoJson);

		/* 先暫停 */
		this.audioSource.Pause();

		/* ID */
		this.id = data.GetString("id");

		/* 來源 */
		this.pathOrKey = data.GetString("src");

		/* 是否正在播放 */
		this.SetIsPlaying(data.GetBool("isPlaying"));

		/* 是否循環 */
		this.isLoop = data.GetBool("isLoop");

		/* 當前時間 */
		float currentTime = data.GetFloat("currentTime");
		this.SetTime(currentTime);

		/* Mixer */
		this._mixer = data.GetString("mixer");

		/* pitch */
		this.audioSource.pitch = data.GetFloat("pitch");

		/* 目標音量 */
		this.targetVolume = data.GetFloat("volume");

		/* 音量漸變時間 */
		this.fadeVolume_sec = data.GetFloat("volumeFadeTime");

		/* 所在圖層 */
		this.groups = data.GetList<string>("layers");

		/* 是否撥放完後 即 銷毀 */
		this.isReleaseOnEnd = data.GetBool("isReleaseOnEnd");

		/* 是否已通知關閉(用於淡出Loop) */
		this.isCallStop = data.GetBool("isCallStop");

	}

	/*=====================================Public Function=======================================*/

	/** 播放 */
	public void Play () {
		if (this._isDestroyed) return;

		this.audioSource.Play();
		this.isCallStop = false;
		this.lastIsPlaying = true;

		this.UpdateLayered();
	}

	/** 停止 */
	public void Stop (bool isForce = true) {
		if (this._isDestroyed) return;

		if (this.audioSource == null) return;
		if (this.audioSource.loop && isForce == false) {
			this.audioSource.loop = false;
			this.isCallStop = true;
		}
		else {
			this.lastIsPlaying = false;
			this.audioSource.Stop();
			this.onEnd.Call();
		}
	}

	/** 復原 */
	public void Resume () {
		if (this._isDestroyed) return;

		this.audioSource.UnPause();
	}

	/** 暫停 */
	public void Pause () {
		if (this._isDestroyed) return;

		this.audioSource.Pause();
	}

	/** 設置播放中 */
	public void SetIsPlaying (bool isPlaying) {
		this.lastIsPlaying = isPlaying;
		if (isPlaying) {
			this.audioSource.Play();
		} else {
			this.audioSource.Pause();
		}
	}

	/** 設置時間 */
	public void SetTime (float currentTime) {
		this.audioSource.time = currentTime;
	}

	/** 設置位置 */
	public void SetPosition (Vector3 positionOrLocalPosition, Transform root = null) {
		if (this._isDestroyed) return;

		if (root != null) {
			this.transform.SetParent(root);
			this.transform.localPosition = positionOrLocalPosition;
		}
		else {
			this.transform.position = positionOrLocalPosition;
		}
	}

	/** 設置音效 */
	public void SetAudioClip (AudioClip audioClip) {
		if (this._isDestroyed) return;

		this.audioSource.clip = audioClip;
	}

	/** 設置輸出mixer */
	public void SetMixer (string mixer) {
		if (this._isDestroyed) return;

		this._mixer = mixer;
		AudioMixerGroup[] mixers = this.audioMixer.FindMatchingGroups(mixer);
		if (mixers.Length != 0) {
			this.audioSource.outputAudioMixerGroup = mixers[0];
		}
	}

	/** 設置pitch */
	public void SetPitch (float pitch) {
		this.audioSource.pitch = pitch;
	}

	/** 設置音量 */
	public void SetVolume (float volume, bool isImmediately = true) {
		if (this._isDestroyed) return;

		// 設置目標音量
		this.targetVolume = volume;

		// 若為立刻 則 立即設置
		if (isImmediately) {
			this.audioSource.volume = this.getLayeredVolume();
		}

		this.UpdateLayered();
	}

	public AudioObj Layer (string layerID) {
		this.AddLayer(layerID);
		return this;
	}

	/** 加入圖層 */
	public void AddLayer (string layerID) {
		List<string> toAdd = new List<string>();
		toAdd.Add(layerID);
		this.AddLayers(toAdd);
	}
	/** 加入圖層 */
	public void AddLayers (List<string> layerIDs) {

		foreach (string layerID in layerIDs) {

			if (this.groups.Contains(layerID)) continue;

			this.groups.Add(layerID);

			// 保證互相設置
			AudioMgr.Inst().SetLayer(this.id, layerID);

		}
		
		this.sortLayer();

		this.UpdateLayered();
	}

	/** 移除圖層 */
	public void RemoveLayer (string layerID) {
		if (this.groups.Contains(layerID) == false) return;
		this.groups.Remove(layerID);

		// 保證互相移除
		AudioLayer layer = AudioMgr.Inst().GetLayer(layerID);
		if (layer != null) {
			layer.RemoveAudio(this.id);
		}

		this.UpdateLayered();
	}

	/** 清除圖層 */
	public void ClearLayers () {
		this.groups.Clear();
		this.UpdateLayered();
	}

	/** 刷新圖層處理部分 */
	public void UpdateLayered () {
		// 取得 處理過的播放狀態

		bool isPlaying = !this.getLayeredIsPause();

		if (this.isPlaying != isPlaying) {
			if (isPlaying) {
				this.Resume();
			}
			else {
				this.Pause();
			}
		}

	}

	/*===================================Protected Function======================================*/

	/** 排序 */
	protected void sortLayer () {

		AudioMgr audioMng = AudioMgr.Inst();

		this.groups.Sort((a, b) => {
			AudioLayer aLayer = audioMng.GetLayer(a);
			AudioLayer bLayer = audioMng.GetLayer(b);
			return (int) (bLayer.priority - aLayer.priority);
		});
	}

	/** 取得圖層處理過的音量 */
	protected float getLayeredVolume () {

		float layeredVol = this.targetVolume;

		// 若沒有圖層 則 直接返回
		if (this.groups.Count == 0) return layeredVol;

		// 每個圖層
		foreach (string eachLayerID in this.groups) {

			AudioLayer layer = AudioMgr.Inst().GetLayer(eachLayerID);

			// 若為不指定 則 繼續下一個圖層
			if (layer.layerVolume < 0) continue;

			layeredVol *= layer.layerVolume;
			break;
		}

		return layeredVol;

	}


	/** 取得圖層處理過播放狀態 */
	protected bool getLayeredIsPause () {
		bool isPause = !this.isPlaying;

		// 若沒有圖層 則 直接返回
		if (this.groups.Count == 0) return isPause;

		// 每個圖層
		foreach (string eachLayerID in this.groups) {

			AudioLayer layer = AudioMgr.Inst().GetLayer(eachLayerID);

			// 若為不指定 則 繼續下一個圖層
			if (layer.pauseState == AudioLayer_PauseState.UNDEFINED) {
				continue;
			}

			isPause = layer.pauseState == AudioLayer_PauseState.PAUSE;
			break;
		}

		return isPause;
	}

	/*====================================Private Function=======================================*/

}

}