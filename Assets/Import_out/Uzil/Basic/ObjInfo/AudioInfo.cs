using System.Collections.Generic;

using Uzil;
using Uzil.Audio;

namespace Uzil.ObjInfo {

public class AudioInfo : ObjInfo {

	/*======================================Constructor==========================================*/

	public AudioInfo () {

	}

	public AudioInfo (object jsonOrPath) {
		this.raw = jsonOrPath;

		DictSO data = DictSO.Json(jsonOrPath);
		
		if (data == null) {
			this.src = jsonOrPath.ToString();
			return;
		} else {
			this.LoadMemo(data);
		}

	}

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** ID */
	public string id = null;

	/** 預設設置 */
	public string audioSet = null;

	/**== 設定屬性 ===========================*/

	/** 音源路徑 */
	public string src = null;

	/** 是否迴圈 */
	public bool? isLoop = null;

	/** 是否於播放玩釋放 */
	public bool? isReleaseOnEnd = null;

	/**== 實時屬性 ===========================*/

	/** Mixer */
	public string mixer = null;

	/** 是否正在播放 */
	public bool? isPlaying = null;

	/** 當前時間 */
	public float? currentTime = null;

	/** 目標音量 */
	public float? volume = null;

	/** 所在圖層 */
	public List<string> groups = null;

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** [IMemoable] 紀錄為Json格式 */
	public override object ToMemo () {
		DictSO data = (DictSO) base.ToMemo();

		if (this.id != null) {
			data.Set("id", this.id);
		}

		if (this.audioSet != null) {
			data.Set("set", this.audioSet);
		}
		
		/**== 設定屬性 ===========================*/

		if (this.src != null) {
			data.Set("src", this.src);
		}

		if (this.isLoop != null) {
			data.Set("isLoop", this.isLoop);
		}

		if (this.isReleaseOnEnd != null) {
			data.Set("isReleaseOnEnd", this.isReleaseOnEnd);
		}
		
		/**== 實時屬性 ===========================*/

		if (this.mixer != null) {
			data.Set("mixer", this.mixer);
		}

		if (this.isPlaying != null) {
			data.Set("isPlaying", this.isPlaying);
		}

		if (this.currentTime != null) {
			data.Set("time", this.currentTime);
		}

		if (this.volume != null) {
			data.Set("volume", this.volume);
		}

		if (this.groups != null) {
			data.Set("layers", this.groups);
		}

		return data;
	}

	/** [IMemoable] 讀取Json格式 */
	public override void LoadMemo (object memoJson) {
		DictSO data = DictSO.Json(memoJson);
		if (data == null) return;

		base.LoadMemo(data);
		
		/**== 設定屬性 ===========================*/

		data.TryGetString("id", (res)=>{
			this.id = res;
		});

		data.TryGetString("src", (res)=>{
			this.src = res;
		});

		data.TryGetString("set", (res)=>{
			this.audioSet = res;
		});
		
		data.TryGetBool("isLoop", (res)=>{
			this.isLoop = res;
		});

		data.TryGetBool("isReleaseOnEnd", (res)=>{
			this.isReleaseOnEnd = res;
		});
		
		/**== 實時屬性 ===========================*/
		
		data.TryGetString ("mixer", (res)=>{
			this.mixer = res;
		});
		
		data.TryGetBool("isPlaying", (res)=>{
			this.isPlaying = res;
		});
		
		data.TryGetFloat ("time", (res)=>{
			this.currentTime = res;
		});
		
		data.TryGetFloat ("volume", (res)=>{
			this.volume = res;
		});
		
		data.TryGetList<string>("layers", (res)=>{
			this.groups = res;
		});

	}

	/*=====================================Public Function=======================================*/

    /** 應用在 */
	public virtual void ApplyOn (AudioObj target, AudioInfo lastInfo) {
		if (target == null) return;

		if (lastInfo == null) lastInfo = new AudioInfo();

		if (this.currentTime != null) {
			if (this.currentTime != lastInfo.currentTime) {
				target.SetTime((float) this.currentTime);
			}
		}

		if (this.volume != null) {
			if (this.volume != lastInfo.volume) {
				target.SetVolume((float) this.volume);
			}
		}

		if (this.mixer != null) {
			if (this.mixer != lastInfo.mixer) {
				target.SetMixer(this.mixer);
			}
		}

		if (this.groups != null) {
			target.ClearLayers();
			target.AddLayers(this.groups);
		}
		
		if (this.isPlaying != null) {
			if (this.isPlaying != lastInfo.isPlaying) {
				if ((bool) this.isPlaying) {
					target.Play();
				} else {
					target.Stop();
				}
			}
		}

	}


	/** 應用 */
	public virtual void Apply (AudioInfo lastInfo) {
		AudioMgr mgr = AudioMgr.Inst();

		AudioObj audio = null;

		string id = null;
		string src = null;

		if (this.audioSet != null) {
			src = AudioSet.Get(this.audioSet);
			if (src != null) id = this.audioSet;
		}

		if (id == null) id = this.id;
		if (src == null) src = this.src;
		
		if (id != null && src == null) {
			
			audio = mgr.GetAudioObj(id);

		} else {
			
			DictSO args = DictSO.New();

			if (this.isLoop != null) args.Set("isLoop", this.isLoop);
			if (this.isReleaseOnEnd != null) args.Set("isReleaseOnEnd", this.isReleaseOnEnd);

		 	audio = mgr.Preload(id, src, args);
		}

		if (audio != null) {
			this.ApplyOn(audio, lastInfo);
		}
	}


	/** 是否等同 */
	public bool IsEqual (AudioInfo other) {

		if (this == other) return true;
		
		/**== 設定屬性 ===========================*/

		/** ID */
		if (this.id != null && other.id != null) {
			if (this.id != other.id) return false;
		}

		/** 音源路徑 */
		if (this.src != null && other.src != null) {
			if (this.src != other.src) return false;
		}

		/** 是否迴圈 */
		if (this.isLoop != null && other.isLoop != null) {
			if (this.isLoop != other.isLoop) return false;
		}

		/** 是否於播放玩釋放 */
		if (this.isReleaseOnEnd != null && other.isReleaseOnEnd != null) {
			if (this.isReleaseOnEnd != other.isReleaseOnEnd) return false;
		}

		/**== 實時屬性 ===========================*/

		/** Mixer */
		if (this.mixer != null && other.mixer != null) {
			if (this.mixer != other.mixer) return false;
		}

		/** 是否正在播放 */
		if (this.isPlaying != null && other.isPlaying != null) {
			if (this.isPlaying != other.isPlaying) return false;
		}

		/** 當前時間 */
		if (this.currentTime != null && other.currentTime != null) {
			if (this.currentTime != other.currentTime) return false;
		}

		/** 目標音量 */
		if (this.volume != null && other.volume != null) {
			if (this.volume != other.volume) return false;
		}

		/** 所在圖層 */
		if (this.groups != null && other.groups != null) {
			if (this.groups != other.groups) return false;
		}

		return true;
	}

	/** 是否等同 */
	public AudioInfo GetCopy () {

		AudioInfo copy = new AudioInfo();

		
		/**== 設定屬性 ===========================*/

		/** ID */
		if (this.id != null) {
			copy.id = this.id;
		}

		/** 音源路徑 */
		if (this.src != null) {
			copy.src = this.src;
		}

		/** 是否迴圈 */
		if (this.isLoop != null) {
			copy.isLoop = this.isLoop;
		}

		/** 是否於播放玩釋放 */
		if (this.isReleaseOnEnd != null) {
			copy.isReleaseOnEnd = this.isReleaseOnEnd;
		}

		/**== 實時屬性 ===========================*/

		/** Mixer */
		if (this.mixer != null) {
			copy.mixer = this.mixer;
		}

		/** 是否正在播放 */
		if (this.isPlaying != null) {
			copy.isPlaying = this.isPlaying;
		}

		/** 當前時間 */
		if (this.currentTime != null) {
			copy.currentTime = this.currentTime;
		}

		/** 目標音量 */
		if (this.volume != null) {
			copy.volume = this.volume;
		}

		/** 所在圖層 */
		if (this.groups != null) {
			copy.groups = this.groups;
		}

		return copy;
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

}

}