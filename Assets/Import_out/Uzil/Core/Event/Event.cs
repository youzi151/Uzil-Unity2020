using System;
using System.Collections.Generic;

namespace Uzil {

public class EventCtrlr {
	public Action doStop = null;
	public void Stop () { if (this.doStop != null) this.doStop(); }
}

public class Event {

	/*======================================Constructor==========================================*/

	public Event (string id) {
		this.id = id;
	}

	public Event () {

	}

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*==運算元==============*/

	public static Event operator + (Event a, EventListener b) {
		a.AddListener(b);
		return a;
	}
	public static Event operator - (Event a, EventListener b) {
		a.RemoveListener(b);
		return a;
	}

	public static Event operator + (Event a, Action b) {
		a.AddListener(new EventListener(b));
		return a;
	}

	/*=========================================Members===========================================*/

	/** 識別 */
	public string id;

	/** 偵聽者列表 */
	public List<EventListener> listenerList = new List<EventListener>();

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 銷毀 */
	public void Destroy () {

	}

	/** 取得偵聽者 */
	public EventListener GetListener (string id) {
		foreach (EventListener each in this.listenerList) {
			if (each.id == id) {
				return each;
			}
		}
		return null;
	}

	/** 加入偵聽者 */
	public Event AddListener (EventListener listener) {
		if (this.listenerList.Contains(listener)) return this;
		// 若 已有同名的存在 則 覆蓋
		if (this.Contains(listener.id)){
			this.Remove(listener.id);	
		}
		this.listenerList.Add(listener);
		this.Sort();
		listener.observers.Add(this);

		return this;
	}

	/** 加入偵聽者 */
	public EventListener Add (Action action) {
		EventListener newListener = EventListener.New(action);
		this.listenerList.Add(newListener);
		this.Sort();
		newListener.observers.Add(this);

		return newListener;
	}

	/** 加入偵聽者 */
	public EventListener Add (Action<DictSO> action) {
		EventListener newListener = EventListener.New(action);
		this.listenerList.Add(newListener);
		this.Sort();
		newListener.observers.Add(this);

		return newListener;
	}

	/** 移除偵聽者 */
	public void RemoveListener (EventListener cbListener) {
		this.listenerList.Remove(cbListener);
		cbListener.observers.Remove(this);	
	}

	/** 移除偵聽者 */
	public void Remove (string id) {
		List<EventListener> toRm = new List<EventListener>();
		foreach (EventListener each in this.listenerList) {
			if (each.id == id) {
				toRm.Add(each);
			}
		}
		foreach (EventListener each in toRm) {
			this.RemoveListener(each);
		}
	}

	/** 是否包含 */
	public bool Contains (string id) {
		if (id == "" || id == null) return false;
		foreach (EventListener each in this.listenerList) {
			if (each.id == id) {
				return true;
			}
		}
		return false;
	}

	/** 排序 */
	public void Sort () {
		// 根據各Listener的priority(優先度)排序(越小越先)
		this.listenerList.Sort();
	}

	/** 呼叫 */
	public void Call (DictSO args = null) {

		List<EventListener> cloneList = new List<EventListener>(this.listenerList);

		/* 是否停止呼叫 */
		bool isCallStop = false;

		EventCtrlr ctrlr = new EventCtrlr();
		ctrlr.doStop = ()=>{
			isCallStop = true;
		};

		foreach (EventListener each in cloneList) {

			each.Call(args, ctrlr);
			
			if (each.IsCallAgain() == false) {
				this.listenerList.Remove(each);
			}

			if (isCallStop) break;
		}

		
	}

	/** 清空偵聽者 */
	public void Clear () {
		this.listenerList.Clear();
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

}



}