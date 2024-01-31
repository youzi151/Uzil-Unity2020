using System.Collections.Generic;

using UnityEngine;

using Uzil;
using Uzil.Lua;

using UzInvoker = Uzil.Invoker;

namespace UZAPI {

public class Invoker {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	private static Dictionary<InvokeTask, int> _task2cbID = new Dictionary<InvokeTask, int>();

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** Delay呼叫 */
	public static void Once (string instName, int callbackID, float time, string tags = null) {
		
		UzInvoker invoker = UzInvoker.Inst(instName);

		// 註冊任務
		InvokeTask_Delay task = null;
		task = invoker.Once(() => {
			
			// 移除任務
			invoker.Cancel(task);
			
			// 呼叫LuaCB
			UZAPI.Callback.CallLua_cs(callbackID);

		}, time);

		// 設置 任務 標籤
		List<string> tagList = DictSO.List<string>(tags);
		if (tagList != null) {
			foreach (string tag in tagList) {
				task.Tag(tag);
			}
		}

		// 註冊 任務 與 CBID 對應
		Invoker._task2cbID.Add(task, callbackID);
	}

	/** 取消 */
	public static void Cancel (string instName, string tags) {
		
		UzInvoker inst = UzInvoker.Inst(instName);

		List<string> tagList = DictSO.List<string>(tags);
		if (tagList != null) {
			foreach (string tag in tagList) {
				List<InvokeTask_Delay> toRm = inst.Cancel(tag);
				foreach (InvokeTask_Delay task in toRm) {
					Invoker.removeCallbackID(task);
				}
			}
		}
	}

	/** 清空 */
	public static void Clear (string instName) {
		UzInvoker.Inst(instName).Clear();
		InvokerUpdate.Inst(instName).Clear();
	}

	/** 加入呼叫佇列 */
	public static void Queue (string instName, int callbackID) {
		InvokerSerial queue = InvokerSerial.Inst(instName);
		//註冊事件
		queue.Set(() => {
			UZAPI.Callback.CallLua_cs(callbackID);
		});
	}

	/** 設定佇列最後一個呼叫 */
	public static void QueueFinal (string instName, int callbackID) {
		InvokerSerial queue = InvokerSerial.Inst(instName);
		// 註冊事件
		queue.onEnd.Add(() => {
			UZAPI.Callback.CallLua_cs(callbackID);
		});
	}

	/** 呼叫 */
	public static void QueueCall (string instName) {
		InvokerSerial queue = InvokerSerial.Inst(instName);
		queue.DoAll();
	}

	/** 每幀 */
	public static void Update (string instName, int callbackID, string tags) {
		InvokerUpdate inst = InvokerUpdate.Inst(instName);

		// 註冊事件
		InvokeTask task = inst.Add(()=>{
			UZAPI.Callback.CallLua_cs_arg(callbackID, TimeUtil.deltaTime);
		});

		List<string> tagList = DictSO.List<string>(tags);
		if (tagList != null) {
			foreach (string tag in tagList) {
				task.Tag(tag);
			}
		}

		Invoker._task2cbID.Add(task, callbackID);
	}

	/** 取消每幀 */
	public static void UpdateCancel (string instName, string tags) {
		InvokerUpdate inst = InvokerUpdate.Inst(instName);

		List<string> tagList = DictSO.List<string>(tags);
		if (tagList != null) {
			foreach (string tag in tagList) {
				List<InvokeTask_Priority> toRm = inst.Cancel(tag);
				foreach (InvokeTask_Priority task in toRm) {
					Invoker.removeCallbackID(task);
				}
			}
		}
	}

	private static void removeCallbackID (InvokeTask task) {
		if (Invoker._task2cbID.ContainsKey(task) == false) return;

		int callbackID = Invoker._task2cbID[task];

		Invoker._task2cbID.Remove(task);
		
		Callback.Remove(callbackID);
	}

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}

}