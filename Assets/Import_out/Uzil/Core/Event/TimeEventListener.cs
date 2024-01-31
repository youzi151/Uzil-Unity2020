using System;
using System.Collections.Generic;

namespace Uzil {

public class TimeEventListener : IComparable {

	/*======================================Constructor==========================================*/

	/* 建構子 */
	public TimeEventListener (float time, Action<DictSO> onTime) {
		this.time = time;
		this.onTime += onTime;
	}

	public TimeEventListener (float time, Action onTime) {
		this.time = time;
		this.onTime += (args)=>{
			onTime();
		};
	}


	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/* 名稱(可用於debug) */
	public string name;

	/* 預定的時間 */
	public float time;

	/* 所屬Callback */
	public List<TimeEvent> observers = new List<TimeEvent>();

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	public event Action<DictSO> onTime = delegate{};

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/
	
	/* 實作比較器，以供排序 */
	public int CompareTo (object b) {
		return this.time.CompareTo( ((TimeEventListener)b).time);
	}

	/*=====================================Public Function=======================================*/

	/* 呼叫 */
	public void Call (DictSO args = null) {
		this.onTime(args);
	}

	/* 命名 */
	public TimeEventListener Name (string name) {
		this.name = name;
		return this;
	}

	/* 設定時間 */
	public TimeEventListener Time (float time) {
		this.time = time;
		return this;
	}

	/* 移除自己 */
	public void RemoveSelf () {
		List<TimeEvent> toRemove = new List<TimeEvent>(this.observers);
		foreach (TimeEvent observer in toRemove){
			observer.RemoveListener(this);	
		}
	}

	/*====================================Private Function=======================================*/


}



}