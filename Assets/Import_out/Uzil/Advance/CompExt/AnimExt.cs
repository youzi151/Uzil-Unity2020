using System.Collections.Generic;

using UnityEngine;

namespace Uzil.CompExt {

public class AnimExt : MonoBehaviour {
	protected bool isDebug = false;
	protected void log(object log){
		if (isDebug) Debug.Log(log);
	}

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/* 當前片段名稱 */
	public string currentClipName = null;

	/* 是否動畫中 */
	public bool isAniming = false;

	/* 什麼時機要觸發什麼事件的List */
	public List<TimeEventListener> timeListenerList = new List<TimeEventListener>();

	/*========================================Components=========================================*/

	/* 殼的動畫 */
	public Animator animator {
		get {
			if (this._animator != null) return this._animator;
			this._animator = this.gameObject.GetComponent<Animator>();
			return this._animator;
		}
	}
	private Animator _animator;
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Update is called once per frame
	void Update () {
		this.CheckTiming();
	}

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/
	
	/* 播放演出 */
	public void Play (string clipName, float sinceTime_percent = 0f) {
	
		//清除所有偵聽者
		this.ClearTimeEventListener();

		//播放動畫
		if (this.animator != null) {
			this.animator.Play(clipName, /*layer*/-1, /*time*/ sinceTime_percent);
			this.animator.Update(Time.deltaTime);
		}

		//當前片段名稱
		this.currentClipName = clipName;

		//加入事件 (改為CheckTiming內檢查執行)
		// this.AddTimeEventListener(new TimeEventListener(/*time*/1f, /*action*/onSingleAnimEnd));

		this.isAniming = true;
	}

	/* 終止動作 */
	public void Stop () {
		//設置 正在播放動畫 為 否
		this.isAniming = false;

		//停止
		this.animator.StopPlayback();

		//當前片段名稱
		this.currentClipName = null;

		//清除所有偵聽者
		this.ClearTimeEventListener();
	}

	/* 新增偵聽者  */
	public void AddTimeEventListener (TimeEventListener listener) {
		this.timeListenerList.Add(listener);
		this.SortListener();
	}

	/* 移除偵聽者 */
	public void RemoveTimeEventListener (TimeEventListener listener) {
		this.timeListenerList.Remove(listener);
	}

	/* 清除偵聽者 */
	public void ClearTimeEventListener () {
		this.timeListenerList.Clear();
	}

	/* 排序 */
	public void SortListener () {
		//根據各Listener的time(時機)排序
		this.timeListenerList.Sort();
	}

	/* 取得當前角色動畫播放時間 */
	public float GetAnimTime () {
		return this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
	}

	/* 檢查角色動畫播放時間，並呼叫TimeEventListener */
	public void CheckTiming () {

		//防呆
		if (this.isAniming == false) return;
			
		//當前動畫時間
		float time;

		//取得當前動畫時間
		if (this.animator != null) {
			time = this.GetAnimTime();
		} else {
			time = 1f;
		}

		// string clipName = this.currentClipName;

		//要檢查的listner (從Listener複製)
		TimeEventListener[] toCheckArray = this.timeListenerList.ToArray(); 
		//要移除的listener
		TimeEventListener[] removeArray = new TimeEventListener[this.timeListenerList.Count];

		// log("=====================================");
		//依序Listener
		for(int i = 0; i < toCheckArray.Length; i++) {
			TimeEventListener eachListener = toCheckArray[i];

			//若已經到達時機
			if (time >= eachListener.time) {
				//呼叫
				eachListener.Call();
				//加入移除清單
				removeArray[i] = eachListener;
			}
			//若未達時機
			else {
				
				//若動作已結束 則強制呼叫剩餘Listener
				if (time >= 1f) {
					eachListener.Call();
					removeArray[i] = eachListener;
				} else {
					break;
				}

			}
		}
		// log("=====================================");
		//全部移除
		foreach (TimeEventListener eachRemove in removeArray) {
			this.RemoveTimeEventListener(eachRemove);
		}

	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/


}


}