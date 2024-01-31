using System;

using UnityEngine;

using UzEvent = Uzil.Event;

namespace Uzil.Video {

public class VideoCtrlr : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/** 當準備好 */
	public UzEvent onPrepared = new UzEvent();

	/** 當播放 */
	public UzEvent onPlayBegin = new UzEvent();

	/** 當播放完畢 */
	public UzEvent onPlayEnd = new UzEvent();

	/*======================================Unity Function=======================================*/
	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/
	
	/** 準備 */
	public virtual void Prepare (string videoPath, float width, float height, Action onDone = null) {

	}

	/** 播放 */
	public virtual void Play (Action onPlayBegin = null) {
		
	}

	/** 停止 */
	public virtual void Stop () {

	}

	/** 設置循環 */
	public virtual void SetLoop (bool isLoop) {
		
	}

	/** 取得長度 */
	public virtual double GetDuration () {
		return 0f;
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}