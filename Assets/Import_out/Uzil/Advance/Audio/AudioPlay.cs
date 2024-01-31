using System.Collections.Generic;

using UnityEngine;


namespace Uzil.Audio {

public class AudioPlay : MonoBehaviour {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/* 音效ID */
	public string audioID;
	/* 音效來源 */
	public string audioPathOrKey;
	/* Mixer名稱 */
	public string mixer = "SFX";
	/* 是否重複 */
	public bool isLoop = false;
	/* 是否在啟用時播放 */
	public bool isPlayOnAwake = true;

	/* 是否呼叫播放 */
	public bool isCallPlay = false;
	private bool isCalled = false;//是否已經播放

	/* 是否在此位置撥放 */
	public bool isPlayOnThisPosition = true;

	/* 音效音層 */
	public List<string> layers = new List<string>();


	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	void Awake () {

		DictSO args = new DictSO().Set("mixer", this.mixer)
								  .Set("isLoop", this.isLoop);

		AudioObj audio;
		if (this.audioID == "") {
			audio = AudioMgr.Inst().Preload(this.audioPathOrKey, this.audioPathOrKey, args);
		}
		else {
			audio = AudioMgr.Inst().Preload(this.audioID, this.audioPathOrKey, args);
		}


		foreach (string eachLayerID in this.layers) {
			audio.Layer(eachLayerID);
		}

		if (audio != null)
			this.audioID = audio.id;

		if (this.isPlayOnAwake)
			this.Play();
	}

	void Update () {
		if (this.isCallPlay && !this.isCalled) {
			this.isCalled = true;
			this.Play();
		}

		if (this.isCalled && !this.isCallPlay) {
			this.isCalled = false;
		}
	}

	void OnDestroy () {
		AudioMgr.Inst().Release(this.audioID);
	}

	public void PlayAudio () {
		this.Play();
	}
	public void Play () {
		AudioObj audio = AudioMgr.Inst().Request(this.audioID, this.audioPathOrKey, this.mixer, this.isLoop);
		if (audio == null) {
			return;
		} 

		audio.Play();

		//若是 "要在此位置撥放"
		if (this.isPlayOnThisPosition) {
			audio.transform.SetParent(this.transform);
			audio.transform.localPosition = Vector3.zero;
		}

		this.audioID = audio.id;
	}

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}



}