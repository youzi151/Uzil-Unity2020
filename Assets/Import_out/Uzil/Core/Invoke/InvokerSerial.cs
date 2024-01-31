using System;
using System.Collections.Generic;

using UnityEngine;

namespace Uzil {


/*
	依序呼叫器
	以指定的長度、及指定序號的任務成員來依序執行
	用於若 需要先加入 後執行的任務 之後再加入 要先執行的任務
	並在 先執行的任務 執行前進行等待


	加入順序
	Add(2.)
	Add(1.)

	任務佇列：
	Run 1.
	Run 2.
	Run End

 */

public class InvokerSerial {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/* key:實例 */
	public static Dictionary<string, InvokerSerial> key2Instance = new Dictionary<string, InvokerSerial>();

	/*=====================================Static Funciton=======================================*/

	public static InvokerSerial Inst (string key) {
		InvokerSerial instance;

		if (InvokerSerial.key2Instance.ContainsKey(key)) {
			instance = InvokerSerial.key2Instance[key];
		}
		else {
			instance = new InvokerSerial();
			InvokerSerial.key2Instance.Add(key, instance);
		}

		return instance;
	}

	/*=========================================Members===========================================*/

	/* 名稱 */
	public List<string> tags;

	/* 當前執行序號 */
	public int currentIdx = 0;

	/* 是否暫停 */
	private bool isPauseDoAll = false;

	/* 是否正在執行中 */
	private bool isDoingAll = false;

	/* 是否呼叫執行所有 */
	private bool isCallDoAll = false;

	/* 執行佇列 */
	public List<InvokeTask> taskList = new List<InvokeTask>();

	/* 當結束 */
	public Event onEnd = new Event();

	public bool isDebug = false;

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/
	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/* 加入任務 */
	public InvokeTask Set (Action act, int index = -1) {
		
		InvokeTask task = null;

		// 若 有指定 執行內容
		if (act != null) {
			// 建立 任務
			task = new InvokeTask();
			task.SetAct(act);
		}

		// 若 序號 為 預設 則 視為 加在最後面
		if (index < 0) {
			index = this.taskList.Count;
		}

		// 若超出已有的數量，則建立空的直到index
		if (index >= this.taskList.Count) {
			for (int i = this.taskList.Count; i <= index; i++) {
				this.taskList.Add(null);
			}
		}

		// 設置 index位置 的 任務
		this.taskList[index] = task;

		return task;
	}

	/* 取得任務 */
	public InvokeTask Get (int index) {
		if (index < 0 || index >= this.taskList.Count) return null;
		return this.taskList[index];
	}

	/* 移除任務 */
	public void Remove (int index) {
		if (index < 0 || index >= this.taskList.Count) return;

		// 是否為 不必要的(多出來的)
		bool isOutRange = true;

		// 從 末端 到 指定位置
		for (int idx = this.taskList.Count-1; idx >= index; idx--) {
			InvokeTask task = this.taskList[idx];
			
			// 若 任務存在 且 不是指定的位置 則 視為 已經不是多出來的
			if (task != null && idx != index) {
				isOutRange = false;
			}

			// 若 是多出來的空位 則 移除
			if (isOutRange) {
				this.taskList.RemoveAt(idx);
			}
			// 否則
			else {
				// 若 是指定位置
				if (idx == index) {
					// 設為空
					this.taskList[idx] = null;
				}
			}

		}
	}

	/* 清空 */
	public void Clear () {
		// 歸零 當前執行序號
		this.currentIdx = 0;
		// 清空 任務列表
		this.taskList.Clear();
		// 清空 結束事件
		this.onEnd.Clear();
		
		// 關閉 呼叫執行全部
		this.isCallDoAll = false;
		// 關閉 暫停執行全部
		this.isPauseDoAll = false;
		// 關閉 正在執行全部
		this.isDoingAll = false;
	}

	/* 執行下個 */
	public void DoNext () {
		if (this.isDebug) Debug.Log(this.currentIdx);

		InvokeTask task = null;
		// 在序列中
		while (this.currentIdx < this.taskList.Count) {
			// 取得下一個任務
			task = this.taskList[this.currentIdx];
			this.currentIdx++;

			// 若任務存在 則 執行 並 跳出
			if (task != null) {
				task.Do();
				break;
			}

			// 否則繼續尋找下一個序號的任務
		}

		// 若 已達到 最後一個
		if (this.currentIdx >= this.taskList.Count) {
			// 關閉 正在執行全部
			if (this.isDoingAll) this.isDoingAll = false;
			// 呼叫結束
			this.callOnEnd();
		}
	}

	/* 執行全部 */
	public void DoAll (int taskEachFrame = -1) {
		if (this.taskList.Count == 0) return;

		// 若 正在執行全部 則 返回
		if (this.isDoingAll) return;
		// 開啟 正在執行全部
		this.isDoingAll = true;

		// 執行次數
		int count = 0;
		int limit = 99999; // 防呆
		// 當 任務序號 尚未抵達最後
		while (this.currentIdx < this.taskList.Count) {
			if (limit--<0) return;

			// 若中途被呼叫暫停 則
			if (this.isPauseDoAll) {
				// 關閉 正在執行全部
				this.isDoingAll = false;
				// 返回
				return;
			}

			// 若 執行次數 超過 每幀可執行次數
			if (taskEachFrame > 0 && count > taskEachFrame) {
				
				// 關閉 正在執行全部
				this.isDoingAll = false;
				
				// 打開 呼叫執行全部
				this.isCallDoAll = true;
				// 隔幀再次呼叫
				Invoker.main.Once(()=>{
					// 若 呼叫執行全部 還是 打開狀態
					if (this.isCallDoAll == false) return;
					// 執行全部
					this.DoAll(taskEachFrame);
				});
				return;
				
			} else {
				// 執行下個
				this.DoNext();
				count++;
			}
		}
	}

	/* 暫停 */
	public void Pause () {
		// 若 正在執行全部
		if (this.isDoingAll) {
			// 打開 暫停執行全部
			this.isPauseDoAll = true;
		}
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

	/* 當結束 */
	private void callOnEnd () {
		this.onEnd.Call();
	}


}

}
