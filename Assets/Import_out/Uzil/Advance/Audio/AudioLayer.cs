using System.Collections.Generic;

namespace Uzil.Audio {

public enum AudioLayer_PauseState {
	UNDEFINED, PAUSE, PLAY
}

public class AudioLayer : IMemoable {

	/*======================================Constructor==========================================*/

	/* 建構子 */
	public AudioLayer (string id) {
		this.layerID = id;
	}

	public AudioLayer (string id, DictSO args) {
		this.layerID = id;

		if (args == null) return;

		/* 音量 */
		if (args.ContainsKey("volume")) {
			this.SetVolume(args.GetFloat("volume"));
		}

		/* 優先度 */
		if (args.ContainsKey("priority")) {
			this.priority = args.GetFloat("priority");
		}

		/* 是否暫停/播放 */
		if (args.ContainsKey("isPause")) {
			this.SetPause(args.GetEnum<AudioLayer_PauseState>("pauseState"));
		}
	}

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/* 圖層名稱 */
	public string layerID = "_default";

	/* 圖層音量 */
	public float layerVolume = -1f;

	/* 是否暫停 */
	public AudioLayer_PauseState pauseState = AudioLayer_PauseState.UNDEFINED;

	/* 圖層優先度 */
	public float priority = 5f;

	/* 音效物件 */
	public List<string> objs = new List<string>();


	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/* [IMemoable] 紀錄為Json格式 */
	public object ToMemo () {
		DictSO data = new DictSO();

		/* 圖層名稱 */
		data.Set("id", this.layerID);

		/* 圖層音量 */
		data.Set("volume", this.layerVolume);

		/* 是否暫停 */
		data.Set("pauseState", DictSO.EnumTo(this.pauseState));

		/* 圖層優先度 */
		data.Set("priority", this.priority);

		/* 音效物件 */
		data.Set("audios", this.objs);

		return data;
	}

	/* [IMemoable] 讀取Json格式 */
	public void LoadMemo (object memoJson) {

		DictSO data = DictSO.Json(memoJson);

		/* 圖層名稱 */
		this.layerID = data.GetString("id");

		/* 圖層音量 */
		this.layerVolume = data.GetFloat("volume");

		/* 是否暫停 */
		this.pauseState = data.GetEnum<AudioLayer_PauseState>("pauseState");

		/* 圖層優先度 */
		this.priority = data.GetFloat("priority");

		/* 音效物件 */
		this.objs = data.GetList<string>("audios");

	}

	/*=====================================Public Function=======================================*/

	/* 加入音效 */
	public void AddAudio (string audioID) {
		if (this.objs.Contains(audioID)) return;
		this.objs.Add(audioID);

		// 保證互相設置
		AudioMgr.Inst().SetLayer(audioID, this.layerID);
	}

	/* 移除音效 */
	public void RemoveAudio (string audioID) {
		if (this.objs.Contains(audioID) == false) return;

		// 保證互相移除
		AudioObj audio = AudioMgr.Inst().GetAudioObj(audioID);
		if (audio != null) {
			audio.RemoveLayer(this.layerID);
		}
	}

	/* 設置音量 */
	public AudioLayer SetVolume (float vol) {

		this.layerVolume = vol;

		this.UpdateAudios();

		return this;
	}

	/* 設置音量 */
	public AudioLayer SetPause (AudioLayer_PauseState pauseState) {

		this.pauseState = pauseState;

		this.UpdateAudios();

		return this;
	}

	/* 設置優先 */
	public AudioLayer SetPriority (float priority) {

		this.priority = priority;

		this.UpdateAudios();

		return this;
	}

	/* 更新所有音效物件 */
	public void UpdateAudios () {
		AudioMgr audioMng = AudioMgr.Inst();
		foreach (string each in this.objs) {
			AudioObj audio = audioMng.GetAudioObj(each);
			if (!audio) continue;
			audio.UpdateLayered();
		}
	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/


}

}