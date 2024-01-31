using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UzEvent = Uzil.Event;

/**
 * 
 */

namespace Uzil.InputPipe {

public class InputLayer {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 識別 */
	public string id;

	/** 是否啟用 */
	public bool isActive { get; private set; } = true;

	/** 處理器 */
	public Dictionary<string, InputHandler> id2Handlers = new Dictionary<string, InputHandler>();
	protected List<InputHandler> handlers = new List<InputHandler>();

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/** 虛擬輸入 對應 持有/當前的訊號 */
	protected Dictionary<int, InputSignal> vkey2Signal = new Dictionary<int, InputSignal>();

	/** 虛擬輸入 對應 事件 */
	protected Dictionary<int, UzEvent> vkey2Event = new Dictionary<int, UzEvent>();

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/	

	/** 啟用 */
	public void Active () {
		this.isActive = true;
	}

	/** 關閉 */
	public void Deactive () {
		this.isActive = false;
	}


	/** 處理信號 */
	public void HandleSignal (InputSignal signal) {
		// 每個 Handler
		foreach (KeyValuePair<string, InputHandler> pair in this.id2Handlers) {
			InputHandler handler = pair.Value;
			InputSignal copySignal = signal.GetCopy();

			// 把 信號 的 來源key 處理為 虛擬key
			handler.HandleSignal(copySignal);

			// 轉紀錄 虛擬key
			if (this.vkey2Signal.ContainsKey(copySignal.virtualKeyCode)) {
				this.vkey2Signal[copySignal.virtualKeyCode] = copySignal;
			} else {
				this.vkey2Signal.Add(copySignal.virtualKeyCode, copySignal);
			}
		}
	}

	/** 取得信號 */
	public InputSignal GetSignal (int virtualKeyCode) {
		if (this.vkey2Signal.ContainsKey(virtualKeyCode) == false) return null;
		InputSignal signal = this.vkey2Signal[virtualKeyCode];
		return signal;
	}

	/** 清空信號 */
	public void ClearSignals () {
		this.vkey2Signal.Clear();
	}

	/** 取得 處理器 */
	public List<InputHandler> GetHandlers () {
		return this.handlers;
	}

	/** 取得 處理器 */
	public InputHandler GetHandler (string handlerID) {
		if (this.id2Handlers.ContainsKey(handlerID) == false) return null;
		return this.id2Handlers[handlerID];
	}

	/** 新增 處理器 */
	public void AddHandler (InputHandler handler) {
		if (this.id2Handlers.ContainsKey(handler.id)) {
			this.RemoveHandler(handler.id);
		}
		this.id2Handlers.Add(handler.id, handler);
		this.handlers.Add(handler);
	}

	/** 新增 處理器 */
	public void AddHandler (List<InputHandler> toAddHandlers) {
		for (int idx = 0; idx < toAddHandlers.Count; idx++) {
			this.AddHandler(toAddHandlers[idx]);
		}
	}

	/** 移除 處理器 */
	public void RemoveHandler (InputHandler handler) {
		this.RemoveHandler(handler.id);
	}

	/** 移除 處理器 */
	public void RemoveHandler (string handlerID) {
		if (this.id2Handlers.ContainsKey(handlerID) == false) return; 
		this.handlers.Remove(this.id2Handlers[handlerID]);
		this.id2Handlers.Remove(handlerID);
	}

	/** 查詢 */
	public InputSignal GetInput (int virtualKeyCode) {
		if (this.vkey2Signal.ContainsKey(virtualKeyCode) == false) return null;
		return this.vkey2Signal[virtualKeyCode];
	}

	/** 註冊 當 輸入 */
	public void AddOnInput (int virtualKeyCode, EventListener eventListener) {
		UzEvent onInput;
		if (this.vkey2Event.ContainsKey(virtualKeyCode) == false) {
			onInput = new UzEvent();
			this.vkey2Event.Add(virtualKeyCode, onInput);
		} else {
			onInput = this.vkey2Event[virtualKeyCode];
		}
		onInput.AddListener(eventListener);
	}

	/** 註銷 當 輸入 */
	public void RemoveOnInput (int virtualKeyCode, EventListener eventListener) {
		if (this.vkey2Event.ContainsKey(virtualKeyCode) == false) return;
		UzEvent onInput = this.vkey2Event[virtualKeyCode];
	}
	/** 註銷 當 輸入 */
	public void RemoveOnInput (int virtualKeyCode, string eventListenerID) {
		if (this.vkey2Event.ContainsKey(virtualKeyCode) == false) return;
		UzEvent onInput = this.vkey2Event[virtualKeyCode];
		onInput.Remove(eventListenerID);
	}

	/** 呼叫 信號 */
	public void CallInput (InputSignal signal) {
		if (signal.IsAlive() == false) return;
		if (this.vkey2Event.ContainsKey(signal.virtualKeyCode) == false) return;
		this.vkey2Event[signal.virtualKeyCode].Call(new DictSO().Set("signal", signal));
	}

	public void CallAllInput () {
		foreach (KeyValuePair<int, InputSignal> pair in this.vkey2Signal) {
			this.CallInput(pair.Value);
		}
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}
