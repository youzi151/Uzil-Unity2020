using System;
using System.Collections.Generic;

using UnityEngine;

using Uzil.Util;
using Uzil.Macro;
using Uzil.Indices;
using Uzil.BuiltinUtil;

namespace Uzil.Audio {

public class AudioMgr : IMemoable {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/** 單例 */
	public static AudioMgr _instance;
	public static AudioMgr Inst () {

		if (AudioMgr._instance == null) {

			AudioMgr._instance = new AudioMgr();
			
#if UNITY_EDITOR
			AudioMgr._instance.inspector = AudioMgr._instance.root.gameObject.AddComponent<AudioMgrInspector>();
#endif

			AudioUtil.Init();

			// 內建音層
			AudioMgr._instance.AddLayer(Audio_BuiltinLayer.AUDIOLAYER_ALLBASE).SetPriority(0);
			AudioMgr._instance.AddLayer(Audio_BuiltinLayer.AUDIOLAYER_INGAME).SetPriority(5);
			AudioMgr._instance.AddLayer(Audio_BuiltinLayer.AUDIOLAYER_ALLFORCE).SetPriority(100);

			AudioMgr._instance.AddLayer(Audio_BuiltinLayer.AUDIOLAYER_PLAYONPAUSE).SetPause(AudioLayer_PauseState.PLAY).SetPriority(10);

			// 預設
			AudioMgr._instance.defaultLayers = new List<string>{
				Audio_BuiltinLayer.AUDIOLAYER_ALLBASE,
				Audio_BuiltinLayer.AUDIOLAYER_INGAME,
				Audio_BuiltinLayer.AUDIOLAYER_ALLFORCE,
			};

		}
		return AudioMgr._instance;
	}

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 根物件 */
	public Transform _root;
	public Transform root {
		get {
			if (this._root == null) {
				GameObject gObj = RootUtil.GetMember("Audio");
				if (gObj != null) this._root = gObj.transform;
			}
			return this._root;
		}
	}

	public AudioMgrInspector inspector = null;

	/** ID對應音效物件 */
	private Dictionary<string, AudioObj> id2InstanceDict = new Dictionary<string, AudioObj>();
	private List<string> audioIDs = new List<string>();

	/** 音層ID對應音層 */
	private Dictionary<string, AudioLayer> id2LayerDict = new Dictionary<string, AudioLayer>();
	private List<string> layerIDs = new List<string>();

	/** 預設音層 */
	public List<string> defaultLayers = new List<string>();

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** [IMemoable] 紀錄為Json格式 */
	public object ToMemo () {
		DictSO data = new DictSO();

		List<object> audioList = SortDict.GetSortValuesTo<string, AudioObj, object>(this.id2InstanceDict, this.audioIDs, (audio)=>{
			return (audio as IMemoable).ToMemo();
		});
		data.Set("audios", audioList);

		List<object> layerList = SortDict.GetSortValuesTo<string, AudioLayer, object>(this.id2LayerDict, this.layerIDs, (layer)=>{
			return (layer as IMemoable).ToMemo();
		});
		data.Set("layers", layerList);

		return data;
	}

	/** [IMemoable] 讀取Json格式 */
	public void LoadMemo (object memoJson) {
		DictSO data = DictSO.Json(memoJson);

		// 先擋掉所有音效繼續播放
		this.GetLayer(Audio_BuiltinLayer.AUDIOLAYER_ALLFORCE).SetPause(AudioLayer_PauseState.PAUSE);

		List<DictSO> audioList = data.GetList<DictSO>("audios");
		foreach (DictSO each in audioList) {

			string id = each.GetString("id");
			string pathOrKey = each.GetString("src");
			string mixer = each.GetString("mixer");

			// 實體還原
			AudioObj audio = this.GetAudioObj(id);
			if (audio == null) {
				audio = this.Preload(id, pathOrKey, DictSO.New().Ad("mixer", mixer));
			}
			if (audio == null) {
				continue;
			}

			// 屬性還原
			(audio as IMemoable).LoadMemo(each);
		}


		List<DictSO> layerList = data.GetList<DictSO>("layers");
		foreach (DictSO each in layerList) {

			string id = each.GetString("id");

			// 實體還原
			AudioLayer layer = this.GetLayer(id);

			// 屬性還原
			(layer as IMemoable).LoadMemo(each);
		}

		// 復原
		this.GetLayer(Audio_BuiltinLayer.AUDIOLAYER_ALLFORCE).SetPause(AudioLayer_PauseState.UNDEFINED);

	}

	/*=====================================Public Function=======================================*/

	/*==主要功能===============*/

	/**
	 * 預載 (適合 短音效)
	 * 明定 從集合中的key中取得, 並以key作為audioID
	 */
	public AudioObj PreloadBySet (string keyInSet, string mixer, bool isLoop = false, bool isReleaseOnEnd = true, bool isOnCamera = true) {
		DictSO args = new DictSO().Set("mixer", mixer)
								  .Set("isLoop", isLoop)
								  .Set("isReleaseOnEnd", isReleaseOnEnd)
								  .Set("isOnCamera", isOnCamera);
		return this.Preload(keyInSet, AudioSet.Get(keyInSet), args);
	}

	/**
	 * 預載 非同步 適合長音效
	 * 明定 從集合中的key中取得, 並以key作為audioID
	 */
	public void PreloadAsyncBySet (string keyInSet, string mixer, bool isLoop = false, bool isReleaseOnEnd = true, bool isOnCamera = true, Action<AudioObj> cb = null) {
		DictSO args = new DictSO().Set("mixer", mixer)
								  .Set("isLoop", isLoop)
								  .Set("isReleaseOnEnd", isReleaseOnEnd)
								  .Set("isOnCamera", isOnCamera);
		this.PreloadAsync(keyInSet, AudioSet.Get(keyInSet), args, cb);
	}

	/**
	 * 預載
	 * 附帶不特定參數
	 */
	public AudioObj Preload (string id, string pathOrKey, DictSO args = null) {
		if (pathOrKey == null) return null;

		AudioObj audioObj = this.GetAudioObj(id);

		// 若 存在 則設置
		if (audioObj != null) {
			// 設置
			this.SetAudioObj(audioObj, args);
		}
		// 若 不存在 則 建立
		else {

			// 嘗試 Path
			audioObj = this.Request(id, pathOrKey, args);

			// 嘗試 SetKey
			if (audioObj == null) {
				audioObj = this.Request(id, AudioSet.Get(pathOrKey), args);
			}
		}

		return audioObj;
	}

	/**
	 * 預載 (非同步)
	 * 附帶不特定參數
	 */
	public void PreloadAsync (string id, string pathOrKey, Action<AudioObj> cb) {
		this.PreloadAsync(id, pathOrKey, null, cb);
	}
	public void PreloadAsync (string id, string pathOrKey, DictSO args, Action<AudioObj> cb) {
		if (pathOrKey == null || pathOrKey == "") {
			cb(null);
			return;
		}

		AudioObj audioObj = null;

		// 若 不存在 則 建立
		Action<AudioObj, Action> check = (audioObj, next)=>{
			if (audioObj == null) {
				next();
				return;
			}	
			// 設置
			this.SetAudioObj(audioObj, args);
			cb(audioObj);
		};

		Async.Waterfall(new List<Action<Action<bool>>>(){
			// 試著 取得現有
			(next)=>{
				audioObj = this.GetAudioObj(id);
				check(audioObj, ()=>{ next(true); });
			},
			// 嘗試 Path
			(next)=>{
				this.RequestAsync(id, pathOrKey, args, (audioObj)=>{
					check(audioObj, ()=>{ next(true); });
				});
				// audioObj = this.Request(id, pathOrKey, args);
				// check(audioObj, ()=>{ next(true); });
			},
			// 嘗試 SetKey
			(next)=>{
				this.RequestAsync(id, AudioSet.Get(pathOrKey), args, (audioObj)=>{
					check(audioObj, ()=>{ next(true); });
				});
				// audioObj = this.Request(id, AudioSet.Get(pathOrKey), args);
				// check(audioObj, ()=>{ next(true); });
			},
		}, ()=>{
			check(audioObj, ()=>{
				cb(null);
			});
		});

	}

	/**
	 * 申請 音效物件
	 * 附帶固定選項參數
	 */
	public AudioObj Request (string id, string pathOrKey, string mixer, bool isLoop = false, bool isReleaseOnEnd = true, bool isOnCamera = true) {
		DictSO args = new DictSO().Set("mixer", mixer)
								  .Set("isLoop", isLoop)
								  .Set("isReleaseOnEnd", isReleaseOnEnd)
								  .Set("isOnCamera", isOnCamera);
		return this.Request(id, pathOrKey, args);
	}
	public AudioObj Request (string id, string pathOrKey, DictSO args = null) {
		if (pathOrKey == null) return null;
		
		id = this.handleID_inRequest(id);
		
		AudioObj audioObj = null;
		
		audioObj = AudioObj.Create(pathOrKey);

		if (audioObj == null) {
			audioObj = AudioObj.Create(AudioSet.Get(pathOrKey));
		}

		if (audioObj == null) {
			return null;
		}

		this.setAudioObj_inRequest(audioObj, id, pathOrKey, args);

		return audioObj;

	}


	/**
	 * 申請 音效物件 (非同步)
	 * 附帶不特定參數
	 */
	public void RequestAsync (string id, string pathOrKey, DictSO args, Action<AudioObj> onDone) {
		if (pathOrKey == null) {
			onDone(null);
			return;
		}

		id = this.handleID_inRequest(id);
		
		string pathOrKeyToFind = pathOrKey;

		AudioObj audioObj = null;

		// 建立音效物件
		Async.Waterfall(
			new List<Action<Action<bool>>> {
				(next)=>{
					AudioObj.CreateAsync(pathOrKeyToFind, (res)=>{
						if (res == null) {
							next(true);
							return;
						}

						audioObj = res;

						next(false);
					});
				},
				(next)=>{

					pathOrKeyToFind = AudioSet.Get(pathOrKey);

					if (pathOrKeyToFind == null || pathOrKeyToFind == "") {
						next(true);
						return;
					}

					AudioObj.CreateAsync(pathOrKeyToFind, (res)=>{
						if (res == null) {
							next(true);
							return;
						}

						audioObj = res;

						next(false);
					});
				},
			},
			()=>{
				if (audioObj == null) {
					onDone(null);
					return;
				}
				
				// 設置
				this.setAudioObj_inRequest(audioObj, id, pathOrKey, args);

				onDone(audioObj);
			}
		);

		
	}


	/** 釋放 */
	public void Release (string id) {
		AudioObj audio = this.GetAudioObj(id);
		if (audio == null) {
			// Debug.Log("audio["+id+"] not exist");
			return;
		}
		this.release(audio);
	}
	public void Release (AudioObj audio) {
		if (this.id2InstanceDict.ContainsValue(audio) == false) {
			// Debug.Log("audio["+id+"] not exist");
			return;
		}
		this.release(audio);
	}
	protected void release (AudioObj audio) {
		// 從註冊中移除
		this.id2InstanceDict.Remove(audio.id);
		// 若尚未銷毀 則 銷毀
		if (audio.isDestroyed == false) {
			GameObject.Destroy(audio.gameObject);
		}
		// Debug.Log("[AudioMgr] Release:"+id);
	}


	/** 以參數設置音效物件 */
	public AudioObj Set (string id, DictSO args = null) {

		AudioObj audioObj = this.GetAudioObj(id);
		if (audioObj == null) return null;

		this.SetAudioObj(audioObj, args);

		return audioObj;
	}

	/** 播放 */
	public void Play (string id) {
		AudioObj audio = this.GetAudioObj(id);
		if (audio == null) {
			Debug.Log("[AudioMgr] play audio failed, audio["+id+"] is not exist");
			return;
		}
		audio.Play();
	}

	/**
	 * 播放
	 * 附帶固定選項參數
	 */
	public AudioObj Play (string id, string mixer, bool isLoop = false, bool isReleaseOnEnd = true, bool isOnCamera = true) {
		DictSO args = new DictSO().Set("mixer", mixer)
								  .Set("isLoop", isLoop)
								  .Set("isReleaseOnEnd", isReleaseOnEnd)
								  .Set("isOnCamera", isOnCamera);
		return this.Play(id, args);
	}
	/**
	 * 播放
	 * 附帶不特定參數
	 */
	public AudioObj Play (string id, DictSO args = null) {
		// Debug.Log("[AudioMgr]: Play, id:"+id+" path:"+path);

		AudioObj audioObj = this.GetAudioObj(id);

		if (audioObj == null) return null;

		// 設置
		this.SetAudioObj(audioObj, args);

		audioObj.Play();
		return audioObj;
	}

	/** 播放(自動指派ID) */
	public AudioObj Emit (string path, DictSO args = null) {
		if (args == null) args = DictSO.New();
		AudioObj audio = this.Request(null, path, args);
		audio.Play();
		return audio;
	}

	/** 停止 */
	public void Stop (string id) {
		AudioObj audio = this.GetAudioObj(id);
		if (audio == null) return;
		audio.Stop();
	}

	/** 停止所有 */
	public void StopAll () {
		foreach (KeyValuePair<string, AudioObj> pair in this.id2InstanceDict) {
			pair.Value.Stop();
		}
	}

	/** 復原 */
	public void Resume (string id) {
		AudioObj audio = this.GetAudioObj(id);
		if (audio == null) return;
		audio.Resume();
	}

	/** 復原所有 */
	public void ResumeAll () {
		foreach (KeyValuePair<string, AudioObj> pair in this.id2InstanceDict) {
			pair.Value.Resume();
		}
	}

	/** 暫停 */
	public void Pause (string id) {
		AudioObj audio = this.GetAudioObj(id);
		if (audio == null) return;
		audio.Pause();
	}

	/** 暫停所有 */
	public void PauseAll () {
		foreach (KeyValuePair<string, AudioObj> pair in this.id2InstanceDict) {
			pair.Value.Pause();
		}
	}

	/** 取得音效物件 */
	public AudioObj GetAudioObj (string id) {
		if (id == null) return null;
		if (this.id2InstanceDict.ContainsKey(id) == false) {
			return null;
		}
		return this.id2InstanceDict[id];
	}

	/** 音效物件是否存在 */
	public bool IsExist (string id) {
		return (this.GetAudioObj(id) != null);
	}

	/*== 音層功能 ===========*/

	/** 設置音層 */
	public void SetLayer (string audioID, string layerID) {
		AudioLayer layer = this.GetLayer(layerID);
		if (layer != null) {
			layer.AddAudio(audioID);
		}

		AudioObj audio = this.GetAudioObj(audioID);
		if (audio != null) {
			audio.AddLayer(layerID);
		}
		// Debug.Log("SetAudio:"+audioID+" to layer:"+layerID);
	}

	public void UnsetLayer (string audioID, string layerID) {
		AudioLayer layer = this.GetLayer(layerID);
		if (layer != null) {
			layer.AddAudio(audioID);
		}

		AudioObj audio = this.GetAudioObj(audioID);
		if (audio != null) {
			audio.AddLayer(layerID);
		}
	}

	/** 新增音層 */
	public AudioLayer AddLayer (string layerID, DictSO args = null) {
		AudioLayer layer = new AudioLayer(layerID, args);
		this.id2LayerDict.Add(layerID, layer);
		this.layerIDs.Add(layerID);
		return layer;
	}

	/** 移除音層 */
	public void RemoveLayer (string layerID) {
		this.id2LayerDict.Remove(layerID);
		this.layerIDs.Remove(layerID);
	}

	/** 取得音層 */
	public AudioLayer GetLayer (string layerID) {
		AudioLayer layer = null;

		if (this.id2LayerDict.ContainsKey(layerID)) {
			layer = this.id2LayerDict[layerID];
		}
		else {
			layer = new AudioLayer(layerID);
			this.id2LayerDict.Add(layerID, layer);
		}

		return layer;

	}

	/*== 選項功能 ===========*/

	/** 設置音效物件 */
	public void SetAudioObj (AudioObj audioObj, DictSO args) {
		if (audioObj == null) return;
		if (args == null) args = DictSO.New();

		// == 解析參數 ==============

		// Mixer
		if (args.ContainsKey("mixer")) {
			audioObj.SetMixer(args.GetString("mixer"));
		}

		// 重複
		if (args.ContainsKey("isLoop")) {
			audioObj.isLoop = args.GetBool("isLoop");
		}

		// pitch
		if (args.ContainsKey("pitch")) {
			audioObj.SetPitch(args.GetFloat("pitch"));
		}

		// 音層
		if (args.ContainsKey("layers")) {
			audioObj.ClearLayers();
			audioObj.AddLayers(args.GetList<string>("layers"));
		}

		// 是否在結束時釋放
		if (args.ContainsKey("isReleaseOnEnd")) {

			bool isReleaseOnEnd = args.GetBool("isReleaseOnEnd");

			audioObj.isReleaseOnEnd = isReleaseOnEnd;

			if (isReleaseOnEnd) {
				audioObj.onEnd.Remove("ReleaseOnEnd");
				audioObj.onEnd.AddListener(new EventListener(() => {
					this.Release(audioObj.id);
				}).ID("ReleaseOnEnd"));
			}
			else {
				audioObj.onEnd.Remove("ReleaseOnEnd");
			}
		}

		// 位置
		bool isOnCamera = true;
		if (args.ContainsKey("isOnCamera")) {
			isOnCamera = args.GetBool("isOnCamera");
		}

		Vector3 position = new Vector3();
		args.TryGetVector3("position", (res)=>{
			isOnCamera = false;
			position = res;
			audioObj.transform.SetParent(this.root);
			audioObj.transform.position = position;
		});

		if (isOnCamera) {
			Camera mainCam = CameraUtil.GetMain();
			if (mainCam != null) {
				LinkParent linkParent = audioObj.transform.gameObject.AddComponent<LinkParent>();
				linkParent.child = audioObj.transform;
				linkParent.Link(mainCam.transform, audioObj.transform);
			}
		}

	}

	/** 設置音量 */
	public void SetVolume (string id, float volume) {
		AudioObj audio = this.GetAudioObj(id);
		if (audio == null) return;
		audio.SetVolume(volume);
	}

	/** 設置當前播放時間 */
	public void GetCurrentTime (string id) {

	}

	/** 設置當前播放時間 */
	public void GetDuration (string id) {

	}

	/** 是否播放中 */
	public bool IsPlaying (string id) {
		AudioObj audio = this.GetAudioObj(id);
		if (audio == null) return false;
		return audio.isPlaying;
	}


	/*===================================Protected Function======================================*/

	protected string handleID_inRequest (string id) {
		if (id == null) {
			id = "_anonymous_";
			id = UniqID.Fix(id, (newID) => {
				return this.id2InstanceDict.ContainsKey(newID) == false;
			});
		}

		// 若 已存在 則 釋放 之前的
		if (this.id2InstanceDict.ContainsKey(id)) {
			this.Release(id);
		}

		return id;
	}

	protected void setAudioObj_inRequest (AudioObj audioObj, string id, string pathOrKey, DictSO args) {
		// ID
		audioObj.id = id;
		audioObj.gameObject.name = id;

		// 路徑或關鍵字
		audioObj.pathOrKey = pathOrKey;

		// 預設 音層
		audioObj.AddLayers(this.defaultLayers);

		// 設置
		this.SetAudioObj(audioObj, args);

		// 加入到 紀錄
		this.id2InstanceDict.Add(id, audioObj);
		this.audioIDs.Add(id);
	}

	/*====================================Private Function=======================================*/

}

}