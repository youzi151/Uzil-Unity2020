// #define AVPROVIDEO_EXIST

#if !AVPROVIDEO_EXIST

#else

using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Uzil.Util;

using RenderHeads.Media.AVProVideo;

namespace Uzil.Video {

public class VideoCtrlr_AVProVideo : VideoCtrlr {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	public MediaPlayer mediaPlayer;

	public List<GameObject> reactiveGameObjects = new List<GameObject>();

	protected bool isFirstFrameReady = false;
	protected bool isStarted = false;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Awake () {
		MediaPlayer.s_CustomFilePath = PathUtil.GetDataPath();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnEvents (MediaPlayer player, MediaPlayerEvent.EventType eventType, ErrorCode errorCode) {
		if (errorCode != ErrorCode.None) {
			Debug.Log(errorCode);
		}
		switch (eventType) {
			case MediaPlayerEvent.EventType.ReadyToPlay:
				this.onPrepared.Call();
				break;
			case MediaPlayerEvent.EventType.FirstFrameReady:
				this.isFirstFrameReady = true;
				this.checkPlayBegin();
				break;
			case MediaPlayerEvent.EventType.Started:
				this.isStarted = true;
				this.checkPlayBegin();
				break;
			case MediaPlayerEvent.EventType.FinishedPlaying:
				this.onPlayEnd.Call();
				break;
		}
	}

	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 準備 */
	public override void Prepare (string videoPath, float width, float height, Action onDone = null) {

		
		this.isFirstFrameReady = false;
		this.isStarted = false;

		// 停止現有
		this.mediaPlayer.Stop();

		this.mediaPlayer.OpenVideoFromFile(MediaPlayer.FileLocation.Custom, videoPath, false);

		if (onDone != null) {
			this.onPrepared.AddListener(new EventListener(()=>{
				onDone();
			}).Once());
		}
		
	}

	/** 播放 */
	public override void Play (Action onPlayBegin = null) {		

		this.mediaPlayer.Play();

		this.onPlayBegin.AddListener(new EventListener(()=>{
			foreach (GameObject each in this.reactiveGameObjects) {
				each.SetActive(false);
				each.SetActive(true);
			}
			if (onPlayBegin != null) {
				onPlayBegin();
			}
		}).Once());

	}

	/** 停止 */
	public override void Stop () {
		this.mediaPlayer.Stop();
	}

	/** 設置 循環 */
	public override void SetLoop (bool isLoop) {
		this.mediaPlayer.m_Loop = isLoop;
	}

	/** 取得長度 */
	public override double GetDuration () {
		return this.mediaPlayer.Info.GetDurationMs() * 0.001f;
	}

	/*===================================Protected Function======================================*/

	protected void checkPlayBegin () {
		if (!this.isStarted || !this.isFirstFrameReady) return;
		this.onPlayBegin.Call();
	}
	
	/*====================================Private Function=======================================*/
}


} // namespace Uzil.Video

#endif