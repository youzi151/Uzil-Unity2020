using System;
using System.Collections;
using UnityEngine;

using UzCoroutine = Uzil.Misc.Coroutine;

namespace Uzil.Macro {

/* 即時監聽麥克風音訊 */
[RequireComponent(typeof(AudioSource))]
public class MicrophoneListener : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/
	
	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/* 是否啟用 */
	public bool isRecording = false;
	private bool _isRecording;

	/* 是否即時播放 */
	public bool isFeedbackOutput = true;
	private bool _isFeedbackOuput;

	/* 音源 */
	private AudioSource _audioSource;

	/* 音量 */
	private float _lastVolume = -1;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	void Start() {
		
		this._audioSource = this.gameObject.GetComponent<AudioSource>();

		if (this.isRecording) {
			UzCoroutine.Start(this.recordStart());
		}

	}

	void Update() {
		this.CheckIfisRecordingChanged();
		this.CheckIfFeedbackOutputChanged();
	}

	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/


	/* 檢查是否改變 作用中 */
	public void CheckIfisRecordingChanged () {
		
		if (this._isRecording == this.isRecording) return;
		this._isRecording = this.isRecording;

		if (this.isRecording) {
			UzCoroutine.Start(this.recordStart());
		} else {
			this.recordStop();
		}
		
	}

	/* 檢查是否改變 即時反饋 */
	public void CheckIfFeedbackOutputChanged () {
		if (this._isFeedbackOuput == this.isFeedbackOutput) return;
		this._isFeedbackOuput = this.isFeedbackOutput;
		
		this.SetFeedbackOutput(this.isFeedbackOutput);
		
	}

	/* 設置 即時反饋 */
	public void SetFeedbackOutput (bool isFeedback) {
		// 若啟用
		if (isFeedback) {

			if (this._lastVolume == -1) {
				// this._audioSource.volume = 1f;
			} else {
				// 復原 原本音量
				this._audioSource.volume = this._lastVolume;
			}

		}
		// 若關閉
		else {
			// 記錄 原本音量
			this._lastVolume = this._audioSource.volume;
			this._audioSource.volume = 0f;
		}
	}

	/* 錄音開始 */
	private IEnumerator recordStart () {
		this._audioSource.clip = Microphone.Start(null, true, 1, 44100);
		this._audioSource.loop = true;
		while (!(Microphone.GetPosition(null) > 0)) {
			yield return new WaitForEndOfFrame();
		}
		this._audioSource.Play();
	}

	/* 錄音結束 */
	private void recordStop () {
		Microphone.End(null);
		this._audioSource.Stop();
	}
}

}