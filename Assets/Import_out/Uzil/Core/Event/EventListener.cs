using System;
using System.Collections.Generic;

namespace Uzil {

public class EventListener : IComparable {

	/*======================================Constructor==========================================*/

	public EventListener () {}

	public EventListener (Action cb) {
		this.callback = (data, ctrlr)=>{
			cb();
		};
	}
	public EventListener (Action<DictSO> cb) {
		this.callback = (data, ctrlr)=>{
			cb(data);
		};
	}

	public EventListener (Action<DictSO, EventCtrlr> cb) {
		this.callback = (data, ctrlr)=>{
			cb(data, ctrlr);
		};
	}

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/* 建立新的偵聽者 (執行內容不帶參數) */
	public static EventListener New (Action cb, float priority = 0f) {
		EventListener listener = new EventListener();
		listener.callback = (data, ctrlr)=>{
			cb();
		};
		listener.priority = priority;
		return listener;
	}

	/* 建立新的偵聽者 (執行內容帶參數) */
	public static EventListener New (Action<DictSO> cb, float priority = 0f) {
		EventListener listener = new EventListener();
		listener.callback = (data, ctrlr)=>{
			cb(data);
		};
		listener.priority = priority;
		return listener;
	}

	/* 建立新的偵聽者 (執行內容帶參數 與 控制器) */
	public static EventListener New (Action<DictSO, EventCtrlr> cb, float priority = 0f) {
		EventListener listener = new EventListener();
		listener.callback = (data, ctrlr)=>{
			cb(data, ctrlr);
		};
		listener.priority = priority;
		return listener;
	}



	/*=========================================Members===========================================*/

	/* 識別 */
	public string id;

	/* 優先度 (越小越先) */
	public float priority = 0f;

	/* 呼叫次數 */
	public int callTimes = -1;

	/* 呼叫內容 */
	public Action<DictSO, EventCtrlr> callback = null;

	/* 所屬Event */
	public List<Event> observers = new List<Event>();

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/
	
	/* 實作比較器，以供排序 */
	int IComparable.CompareTo (object b) {
		return this.priority.CompareTo( ((EventListener)b).priority);
	}

	/*=====================================Public Function=======================================*/

	/* 是否可以再呼叫 */
	public bool IsCallAgain () {
		return (this.callTimes != 0);
	}

	/* 呼叫 */
	public void Call (EventCtrlr ctrlr = null) {
		this.Call(null, ctrlr);
	}
	public void Call (DictSO data, EventCtrlr ctrlr = null) {
		if (this.IsCallAgain() == false) return;

		if (this.callTimes != -1){
			int newCallTimes = this.callTimes - 1;
			this.callTimes = newCallTimes <= 0? 0 : newCallTimes;
		}

		this.callback(data, ctrlr);
	}

	/* 一次性事件 */
	public EventListener Once () {
		this.callTimes = 1;
		return this;
	}

	/* 優先度 */
	public EventListener Sort (float priority) {
		this.priority = priority;
		return this;
	}
	
	/* 限定呼叫次數 */
	public EventListener CallTimes (int i) {
		this.callTimes = i;
		return this;
	}

	/* 事件名稱 */
	public EventListener ID (string id) {
		this.id = id;
		return this;
	}

	/* 移除自己 */
	public void RemoveSelf () {
		List<Event> toRemove = new List<Event>(this.observers);
		foreach (Event observer in toRemove) {
			observer.RemoveListener(this);	
		}
	}
	


	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

}


}