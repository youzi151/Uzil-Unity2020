using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Uzil {

/**
 * 對 陣列
 *   全部 : Each
 *   依序 : EachSeries
 * 對 呼叫 (Action)
 *   全部 : Parallel
 *   依序 : WaterFall
 */

public class Async {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	
	/* 每個 (依序) */
	public static void EachSeries<T> (List<T> list, Action<T, Action<bool>> eachFn, Action onDone = null) {
		if (onDone == null) onDone = ()=>{};
		
		// 總數
		int count = list.Count;
		if (count == 0) { onDone(); return; }
		
		// 當前序號
		int currentIdx = list.Count-1;

		// 是否中止
		bool isStop = false;

		Action<int> nextTask = null;
		nextTask = (idx)=>{
			// 該序號的執行內容
			T each = list[idx];

			// 下一個序號
			int nextIdx = idx+1;

			// 是否已經結束
			bool isEnd = (nextIdx >= count);

			// 是否已經呼叫
			bool isCalled = false;

			// 加入next
			Action<bool> toNext = (isContinue)=>{
				// 若 已經呼叫過 則 忽略
				if (isCalled) return;
				isCalled = true;

				// 若已經中止 則 返回
				if (isStop) return;

				// 若 不繼續 則 結束
				if (!isContinue) {
					onDone();
					isStop = true;
					return;
				}

				// 若 還沒結束 則 執行下一個序號
				if (!isEnd) {
					nextTask(nextIdx);
				}
				// 否則 呼叫 結束 並 返回
				else {
					onDone();
				}
			};

			// 執行內容
			eachFn(each, toNext);
		};

		nextTask(0);
	}

	/* 每個 (一齊) */
	public static void Each<T> (List<T> list, Action<T, Action<bool>> eachFn, Action onDone = null) {
		if (onDone == null) onDone = ()=>{};
		
		// 是否中止
		bool isStop = false;
		// 剩餘數量
		int leftCount = list.Count;

		if (leftCount == 0) { onDone(); return; }

		// 準備 每個項目的執行完畢後
		Action<bool> eachDone = (isContinue)=>{
			// 若已經中止 則 返回
			if (isStop) return;

			// 若 不繼續 則 結束
			if (!isContinue) {
				onDone();
				isStop = true;
				return;
			}

			// 減少剩餘數量 若 已經歸零 則 結束
			leftCount--;
			if (leftCount == 0) {
				onDone();
			}
		};

		// 每個項目
		for (int idx = 0; idx < list.Count; idx++) {
			T each = list[idx];

			// 是否已完成 該項目 (避免重複呼叫next)
			bool isEachDone = false;
			
			// 準備 下一項目
			Action<bool> eachNext = (isContinue)=>{
				if (isEachDone) return;
				isEachDone = true;
				// 每個項目執行完畢後
				eachDone(isContinue);
			};

			// 執行 每個項目 要執行的內容
			eachFn(each, eachNext);
		}
	}

	
	public static void Filter<T> (List<T> list, Action<T, Action<T>> eachFn, Action<List<T>> onDone) {
		Async.Filter<T, T>(list, eachFn, onDone);
	}
	public static void Filter<T1, T2> (List<T1> list, Action<T1, Action<T2>> eachFn, Action<List<T2>> onDone) {
		List<T2> results = new List<T2>();

		// 是否完成
		bool isDone = false;

		// 剩餘數量
		int leftCount = list.Count;
		// 若 剩餘數量 為 零 則 直接結束
		if (leftCount == 0) { onDone(results); return; }

		// 要回傳的結果陣列 (用陣列避免非同步導致結果排序混亂)
		T2[] array = new T2[leftCount];

		// 要輪詢的列表 副本
		List<T1> copy = new List<T1>(list.ToArray());
		
		// 每個要輪詢的
		for (int idx = 0; idx < list.Count; idx++) {
			
			// 紀錄 序號
			int _idx = idx;

			// 執行 該項目
			eachFn(
				/* each 項目 */
				copy[_idx],
				/* next 當項目完成 */
				(res)=> {
					// 避免重複呼叫
					if (isDone) return;
					
					// 若回傳結果 不為空 則 設置 到 結果陣列 中
					if (res != null) {
						array[_idx] = res;
					}
				
					// 減少剩餘項目
					leftCount = leftCount-1;
					// 若 剩餘項目數量 歸零
					if (leftCount == 0) {		
						// 每個陣列成員加入到結果中
						for (int arrayIdx = 0; arrayIdx < array.Length; arrayIdx++) {
							results.Add(array[arrayIdx]);
						}
						
						// 設為已經完成
						isDone = true;

						// 呼叫完成 並回傳結果
						onDone(results);
					}
				}
			);
		}
	}

	/** 非同步 依序 */
	public static void Waterfall(List<Action<Action<bool>>> list, Action onDone = null) {
		if (onDone == null) onDone = ()=>{};
		if (list.Count == 0) { onDone(); return; }

		Action<int> nextTask = null;
		nextTask = (idx)=>{
			
			// 該序號的執行內容
			Action<Action<bool>> task = list[idx];
			// 下一個序號
			int nextIdx = idx+1;
			// 是否已經結束
			bool isEnd = nextIdx >= list.Count;

			// 執行內容
			task((isContinue)=>{
				
				// 若 已經結束 則 呼叫 結束 並 返回
				if (isEnd || !isContinue) {
					onDone();
					return;
				}
				// 執行下一個序號
				nextTask(nextIdx);
			});

		};

		nextTask(0);
	}
	public static void Waterfall<T> (List<Action<T, Action<T>>> list, Action<T> onDone = null) {
		if (onDone == null) onDone = (res)=>{};
		if (list.Count == 0) { onDone(default(T)); return; }

		Action<int, T> nextTask = null;
		nextTask = (idx, arg)=>{
			
			// 該序號的執行內容
			Action<T, Action<T>> task = list[idx];
			// 下一個序號
			int nextIdx = idx+1;
			// 是否已經結束
			bool isEnd = nextIdx >= list.Count;

			// 執行內容
			task(arg, (taskArg)=>{
				// 若已經結束 則 呼叫 結束 並 返回
				if (isEnd) {
					onDone(taskArg);
					return;
				}

				// 執行下一個序號
				nextTask(nextIdx, taskArg);
			});

		};

		nextTask(0, default(T));
	}

	/** 非同步 (一齊) */
	public static void Parallel (List<Action<Action>> list, Action<int> eachFn, Action onDone) {
		if (onDone == null) onDone = ()=>{};
		if (list.Count == 0) { onDone(); return; }

		bool isStop = false;

		int leftTask = list.Count;

		Action<int> eachDone = (idx)=>{
			if (isStop) return;

			if (eachFn != null) {
				eachFn(idx);
			}

			leftTask--;
			if (leftTask <= 0) {
				onDone();
			}
		};

		for (int idx = 0; idx < list.Count; idx++) {
			
			Action<Action> eachTask = list[idx];
			
			bool isDone = false;
			
			int toIdx = idx;
			
			eachTask(()=>{
				if (isDone) return;
				isDone = true;
				eachDone(toIdx);
			});
		}

	}

	/* 非同步平行 */
	public static void Parallel<T> (List<Action<Action<T>>> list, Action<int, List<T>> eachFn, Action<List<T>> onDone) {
		if (onDone == null) onDone = (results)=>{};

		List<T> results = new List<T>();
		for (int idx = 0; idx < list.Count; idx++) {
			results.Add(default(T));
		}
		
		if (list.Count == 0) { onDone(results); return; }
		
		bool isStop = false;

		int leftTask = list.Count;

		Action<int> eachDone = (idx)=>{
			if (isStop) return;

			if (eachFn != null) {
				eachFn(idx, results);
			}

			leftTask--;
			if (leftTask <= 0) {
				onDone(results);
			}
		};

		for (int idx = 0; idx < list.Count; idx++) {
			
			Action<Action<T>> eachTask = list[idx];
			
			bool isDone = false;
			
			int toIdx = idx;
			
			eachTask((result)=>{
				if (isDone) return;
				isDone = true;

				Debug.Log(toIdx);

				results[toIdx] = result;
				eachDone(toIdx);
			});
		}

	}

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

}



}