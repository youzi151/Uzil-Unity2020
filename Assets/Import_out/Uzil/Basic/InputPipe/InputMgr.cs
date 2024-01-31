using System;
using System.Collections.Generic;

using UnityEngine;

using Uzil.Util;
using UzEvent = Uzil.Event;

/**
 * 
 * 1. Mgr每幀會從每個Layer中的Handler取得要偵聽的srcKeyCode
 * 2. Mgr對每個要偵聽的srcKeyCode，會產生Signal
 * 2. Mgr依序傳遞Signal給每個Layer、Layers再發Signal給所有Handler。
 * 4-1. Handler會將Signal中實際的Input轉換為綁定/其他的對應Input。
 * 4-2. 依序傳遞Signal給各個負責執行邏輯內容的onInputListener並夾帶參數。
 * 5. Listener可自行決定繼續傳遞，或者取用操作而停止傳遞給下一個Listener。也可中斷Signal來停止傳給下一個Layer。
 * 
 * InputSignal(key17, key23) →
 * 
 *                    1.1. InputLayer1 (Domain)
 *                       └ [Handler1] 把 key17 轉換/處理 成 兩倍值 添加成新key17 到 Layer
 *                       └ [Handler2] 把 key23 轉換/處理 成 key87 添加成新key87 到 Layer
 *                    2.1 [key17] → Listener1 ——→ Listener2 
 *                        [key87] → Listener1 —x→ Listener2 (可利用Event.Call時的ctrlr來進行中斷傳遞)
 * 
 *                    1.2. InputLayer2 (Domain)
 *                       └ [Handler1] 把 key17 轉換/處理 成 兩倍值 添加成新key17 到 Layer
 *                       └ [Handler2] 把 key23 轉換/處理 成 key99 添加成新key99 到 Layer
 *                    2.1 [key17] → Listener1 ——→ Listener2 
 *                        [key99] → Listener1 —x→ Listener2 (可利用Event.Call時的ctrlr來進行中斷傳遞)
 * 
 */

namespace Uzil.InputPipe {

public class InputMgr : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/** 預設Key */
	public const string defaultKey = "_default";

	/** key:實體 */
	private static Dictionary<string, InputMgr> key2Instance = new Dictionary<string, InputMgr>();

	/** 特殊處理 */
	public static Dictionary<int, Func<int, InputSignal>> specialSignalCreate = new Dictionary<int, Func<int, InputSignal>>();

	/*=====================================Static Funciton=======================================*/

	/** 取得實體 */
	public static InputMgr Inst (string key = null) {
		if (key == null) key = InputMgr.defaultKey;
		
		InputMgr instance = null;

		// 若 實體存在 則 取用
		if (InputMgr.key2Instance.ContainsKey(key)) {
			instance = InputMgr.key2Instance[key];
		}
		// 否則 建立
		else {
			// 取得根物件
			GameObject root = RootUtil.GetMember("InputPipe");
			
			// 建立
			GameObject instanceGObj = RootUtil.GetChild(key, root);
			instance = instanceGObj.AddComponent<InputMgr>();
			instance.key = key;

			InputMgr.key2Instance.Add(key, instance);

			instance.Active();
		}

		return instance;
	}

	/*=========================================Members===========================================*/

	/** 鍵 */
	public string key;

	/** 是否啟用 */
	public bool isActive { get; private set; }

	/** 層級 */
	public List<InputLayer> layers = new List<InputLayer>();

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 更新 */
	public void Update () {

		InputUtil.UpdateOnce();

		if (!this.isActive) return;

		List<int> toListenSrcKeyCodes = new List<int>();

		// 每個層級
		for (int layerIdx = 0; layerIdx < this.layers.Count; layerIdx++) {
			
			InputLayer layer = this.layers[layerIdx];

			// 清空所有信號
			layer.ClearSignals();

			// 蒐集 要偵測的來源KeyCode
			List<InputHandler> handlers = layer.GetHandlers();
			foreach (InputHandler handler in handlers) {
				foreach (int srcKeyCode in handler.srcKeyCodes) {
					if (toListenSrcKeyCodes.Contains(srcKeyCode) == false) {
						toListenSrcKeyCodes.Add(srcKeyCode);
					}
				}
			}
		}

		// 每一個 偵聽的 KeyCode
		foreach (int srcKeyCode in toListenSrcKeyCodes) {

			InputSignal signal = null;

			// 試著取得/產生信號==========

			// 若 為 特殊處理 則 特殊處理
			if (InputMgr.specialSignalCreate.ContainsKey(srcKeyCode)) {
				signal = InputMgr.specialSignalCreate[srcKeyCode](srcKeyCode);
			}
			// 否則 一般處理
			else {
				signal = this.getSignal(srcKeyCode);
			}

			// 若 查無訊號 則 忽略
			if (signal == null) continue;

			// 處理信號==================

			// 交給 每個層級 處理
			for (int layerIdx = 0; layerIdx < this.layers.Count; layerIdx++) {
				InputLayer layer = this.layers[layerIdx];

				// 若 關閉中 則 忽略
				if (layer.isActive == false) continue;

				// 處理信號
				layer.HandleSignal(signal);
			}

		}

		// 每個層級
		for (int layerIdx = 0; layerIdx < this.layers.Count; layerIdx++) {
			InputLayer layer = this.layers[layerIdx];

			// 若 關閉中 則 忽略
			if (layer.isActive == false) continue;

			// 呼叫所有信號
			layer.CallAllInput();
		}
		
	}

	public void LateUpdate () {
		InputUtil.ReadyNextUpdate();
	}

	/** 啟用 */
	public void Active () {
		this.isActive = true;
	}

	/** 關閉 */
	public void Deactive () {
		this.isActive = false;
	}

	/** 清空 */
	public void Clear () {
		this.layers.Clear();
	}
	
	/**== 建構 =======================*/

	/** 新增 輸入處理 */
	public void AddHandler (string layerID, InputHandler handler) {
		InputLayer layer = this.getLayer(layerID);
		layer.AddHandler(handler);
	}

	/** 取得 層級中 所有 處理 */
	public List<InputHandler> GetHandlerInLayers (string handlerID) {
		List<InputHandler> res = new List<InputHandler>();
		foreach (InputLayer layer in this.layers) {
			InputHandler handler = layer.GetHandler(handlerID);
			if (handler != null) {
				res.Add(handler);
			}
		}
		return res;
	}

	/** 移除 輸入處理 */
	public void RemoveHandler (string layerID, string handlerID) {
		InputLayer layer = this.getLayer(layerID);
		layer.RemoveHandler(handlerID);
	}


	/**== 取得輸入 ===================*/

	/** 查詢 */
	public InputSignal GetInput (string layerID, int virtualKeyCode) {
		InputLayer layer = this.getLayer(layerID);
		return layer.GetInput(virtualKeyCode);
	}

	/** 註冊 當 輸入 */
	public void AddOnInput (string layerID, int virtualKeyCode, EventListener eventListener) {
		InputLayer layer = this.getLayer(layerID);
		layer.AddOnInput(virtualKeyCode, eventListener);
	}

	/** 註銷 當 輸入 */
	public void RemoveOnInput (string layerID, int virtualKeyCode, string eventListenerID) {
		InputLayer layer = this.getLayer(layerID);
		layer.RemoveOnInput(virtualKeyCode, eventListenerID);
	}
	public void RemoveOnInput (string layerID, int virtualKeyCode, EventListener eventListener) {
		InputLayer layer = this.getLayer(layerID);
		layer.RemoveOnInput(virtualKeyCode, eventListener);
	}

	/** 
	 * 停止輸入 
	 * 以最終的虛擬KeyCode，向該層級取得訊號後執行停止，追溯到源訊號將實際KeyCode關閉
	 */
	public void StopInput (string layerID, int virtualKeyCode, bool isStream) {
		InputLayer layer = this.getLayer(layerID);
		if (layer == null) return;
		InputSignal signal = layer.GetSignal(virtualKeyCode);
		if (signal == null) return;
		signal.Stop(isStream);
	}

	/*===================================Protected Function======================================*/

	/** 取得 層級 */
	protected InputLayer getLayer (string layerID) {
		for (int layerIdx = 0; layerIdx < this.layers.Count; layerIdx++) {
			InputLayer each = this.layers[layerIdx];
			if (each.id == layerID) return each;
		}

		InputLayer layer = new InputLayer();
		layer.id = layerID;
		this.layers.Add(layer);
		return layer;
	}

	/** 取得/產生 信號 */
	protected InputSignal getSignal (int keyCode) {

		InputSignal signal = new InputSignal();
		signal.Init(keyCode);

		InputType valType = InputUtil.GetInputType(keyCode);

		switch (valType) {
			case InputType.Key:
				
				if (InputUtil.GetKeyUp(keyCode)) {
					signal.SetValue((int) InputKeyState.Up);
				} else if (InputUtil.GetKeyDown(keyCode)) {
					signal.SetValue((int) InputKeyState.Down);
				} else if (InputUtil.GetKey(keyCode)) {
					signal.SetValue((int) InputKeyState.Pressed);
				} else {
					return null;
				}

				break;

			case InputType.Axis:

				float val = InputUtil.GetAxis(keyCode);
				if (val == 0) return null;
				
				signal.SetValue(val);
				break;
		}



		return signal;
	}
	
	/*====================================Private Function=======================================*/
}


}
