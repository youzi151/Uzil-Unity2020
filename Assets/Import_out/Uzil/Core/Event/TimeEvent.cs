using System;
using System.Collections.Generic;

// TimeEvent
// Callback擴充版，用於增加對Call時，傳入當前時間機制的檢查來呼叫已達時間的Callback
// NOTE: 因為有管理機制 所以 不從Callback繼承

namespace Uzil {

public class TimeEvent {

	/*======================================Constructor==========================================*/

	public TimeEvent (string id) {
		this.id = id;
		TimeEvent.callbackList.Add(this);
	}

	public TimeEvent () {
		TimeEvent.callbackList.Add(this);
	}

	~TimeEvent() {
		if (TimeEvent.callbackList.Contains(this)) {
			TimeEvent.callbackList.Remove(this);
		}
	}

	/*=====================================Static Members========================================*/

	/** 所有TimeEvent */
	public static List<TimeEvent> callbackList = new List<TimeEvent>();

	/*=====================================Static Funciton=======================================*/

	/** 取得 */
	public static TimeEvent Get (string id) {
		for (int i = 0; i < TimeEvent.callbackList.Count; i++){
			TimeEvent each = TimeEvent.callbackList[i];
			if (each.id == id) return each;
		}
		return null;
	}

	/** 呼叫 */
	public static void Call (string id, float time, DictSO param = null) {
		for (int i = 0; i < TimeEvent.callbackList.Count; i++){
			TimeEvent each = TimeEvent.callbackList[i];
			if (each.id == id) each.Call(time, param);
		}
	}

	/** 銷毀 */
	public static void Destroy (string id) {
		List<TimeEvent> toRemove = new List<TimeEvent>();
		for (int i = 0; i < TimeEvent.callbackList.Count; i++) {
			TimeEvent each = TimeEvent.callbackList[i];
			if (each.id == id) {
				toRemove.Add(each);
			}
		}	

		for (int i = 0; i < toRemove.Count; i++) {
			TimeEvent.callbackList.Remove(toRemove[i]);
		}

	}
	public static void Destroy (TimeEvent toDestroy) {
		int times = 50;
		while (TimeEvent.callbackList.Contains(toDestroy)) {
			TimeEvent.callbackList.Remove(toDestroy);
			if (times-- < 0) break;
		}	
	}


	/*==運算元==============*/

	public static TimeEvent operator + (TimeEvent a, TimeEventListener b) {
		a.AddListener(b);
		return a;
	}
	public static TimeEvent operator - (TimeEvent a, TimeEventListener b) {
		a.RemoveListener(b);
		return a;
	}


	/*=========================================Members===========================================*/

	/** ID */
	public string id;

	/** 偵聽者列表 */
	public List<TimeEventListener> listeners = new List<TimeEventListener>();

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	public void Destroy() {
		TimeEvent.Destroy(this);
	}

	public TimeEvent AddListener (TimeEventListener cbListener) {
		if (this.listeners.Contains(cbListener)) return this;
		//若已有同名的存在 則 覆蓋
		if (this.ContainsListener(cbListener.name)) {
			this.RemoveListener(cbListener.name);	
		}
		this.listeners.Add(cbListener);
		this.SortListener();
		cbListener.observers.Add(this);

		return this;
	}

	public TimeEvent AddListener(float time, Action<DictSO> action) {
		TimeEventListener newListener = new TimeEventListener(time, action);
		this.listeners.Add(newListener);
		this.SortListener();
		newListener.observers.Add(this);

		return this;
	}

	public void RemoveListener (TimeEventListener cbListener) {
		this.listeners.Remove(cbListener);
		cbListener.observers.Remove(this);	
	}

	public void RemoveListener (string name) {
		foreach (TimeEventListener each in this.listeners) {
			if (each.name == name) {
				this.RemoveListener(each);
				return;
			}
		}
	}

	public bool ContainsListener (string name) {
		if (name == "" || name == null) return false;
		foreach (TimeEventListener each in this.listeners) {
			if (each.name == name) {
				return true;
			}
		}
		return false;
	}

	/** 排序 */
	public void SortListener () {
		//根據各Listener的priority(優先度)排序(越小越先)
		this.listeners.Sort();
	}

	/** 呼叫 */
	public void Call (float time, DictSO args = null) {
		List<TimeEventListener> cloneList = new List<TimeEventListener>(this.listeners);

		foreach (TimeEventListener each in cloneList){
			// 若 未到達時機 則 忽略
			if (time < each.time) continue;

			// 呼叫
			each.Call(args);
			
			// 移除
			this.listeners.Remove(each);
		}
	}

	/** 呼叫所有(無視時間) */
	public void CallAll (DictSO param = null) {
		foreach (TimeEventListener each in this.listeners){
			// 呼叫
			each.Call(param);
		}
		this.listeners.Clear();
	}

	/** 清空 */
	public void Clear(){
		this.listeners.Clear();
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

}



}