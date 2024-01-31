using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

using Uzil;
using Uzil.Util;
using Uzil.Macro;
using UzEvent = Uzil.Event;

namespace Uzil.Video {

public class VideoCtrlr_VideoPlayer : VideoCtrlr {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 貼圖 */
	private RenderTexture _renderTexture;

	/*========================================Components=========================================*/
	
	/** 影片播放組件 */
	public VideoPlayer videoPlayer;

	/** 影片播放畫面  */
	public RawImage videoImage;

	/** 音源 組件 */
	public AudioSource audioSource;

	/*==========================================Event============================================*/

	/** 當影格準備完畢 */
	public UzEvent onFrameReady = new UzEvent();

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Awake () {
		this.onPlayEnd.Add(()=>{
			this.onPlayEnd.Call();
		});
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 準備 */
	public override void Prepare (string videoPath, float width, float height, Action onDone = null) {
		
		// 停止現有
		if (this.videoPlayer.isPlaying) {
			this.videoPlayer.Stop();
		}
		
		// 設置檔案
		this.videoPlayer.url = PathUtil.GetAbsolutePath(PathUtil.GetDataPath(), videoPath);

		// 建立新貼圖
		this._renderTexture = new RenderTexture((int)width, (int)height, 1, UnityEngine.Experimental.Rendering.DefaultFormat.LDR);

		// 設置貼圖至 播放器
		this.videoPlayer.targetTexture = this._renderTexture;

		// 設置貼圖至 播放畫面
		this.videoImage.texture = this._renderTexture;
		(this.videoImage.transform as RectTransform).sizeDelta = new Vector2(width, height);

		// 設置音源
		if (this.audioSource != null) {
			this.videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
			this.videoPlayer.SetTargetAudioSource(0, this.audioSource);
		} else {
			this.videoPlayer.audioOutputMode = VideoAudioOutputMode.Direct;
		}

		
		if (onDone != null) {
			this.onPrepared.AddListener(new EventListener(onDone).Once());
		}

		// 準備
		this.videoPlayer.Prepare();
		
	}

	/** 播放 */
	public override void Play (Action onPlayBegin = null) {
		// 播放
		this.videoPlayer.Play();

		this.videoPlayer.sendFrameReadyEvents = true;

		if (onPlayBegin != null) {
			this.onPlayBegin.AddListener(new EventListener(()=>{
				onPlayBegin();
			}).Once());
		}

		this.onFrameReady.AddListener(new EventListener(()=>{
			this.onPlayBegin.Call();
			this.videoPlayer.sendFrameReadyEvents = false;
		}).Once().ID("onPlayBegin"));
	}

	/** 停止 */
	public override void Stop () {
		this.videoPlayer.Stop();
	}

	/** 設置 循環 */
	public override void SetLoop (bool isLoop) {
		this.videoPlayer.isLooping = isLoop;
	}

	/** 取得長度 */
	public override double GetDuration () {
		return this.videoPlayer.length;
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}
