using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UzEvent = Uzil.Event;

namespace Uzil.State {

public class StateCtrl : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	public bool isDebug = false;

	/** 使用者 */
	public object user = null;
	
	/** 是否鎖住狀態 */
	public bool isLockState = false;
	
	/** 下一個狀態 */
	private State _nextState = null;
	
	/** 預設狀態 */

    public string defaultState = "";

	/** 狀態 */
	public List<State> states = new List<State>();

    /** 狀態 */
	public State currentState = null;


	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/** 當狀態切換 */
	public UzEvent onStateChange = new UzEvent();

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/**
	 * 初始化
	 * @param user 使用者
	 */
	public void Init (object user = null) {
		
		this.user = user;

		foreach (State state in this.states) {
			state.Init(this.user);
		}

		// 預設狀態
        this.Go(this.defaultState);
	}

	/**
	 * 設置狀態
	 * @param stateName 狀態名稱
	 */
	public void Go (string stateName) {
		
		foreach (State each in this.states) {
			
			if (each.stateName != stateName) continue;

			this.GoState(each);
			break;

		}
	}


	/**
	 * 設置狀態
	 * @param newState 狀態
	 */
    public void GoState (State newState, bool isForce = false) {
		// 防呆
        if (!newState) return;
		if (this.currentState == newState) return;

		this._nextState = newState;

		if (this.isLockState && !isForce) {
			return;
		}

		State lastState = this.currentState;

		// 若 舊狀態存在 則 呼叫離開
        if (this.currentState) {
            this.currentState.onExit();
        }

		// 指定為新狀態
        this.currentState = newState;

        if (this.isDebug) {
			Debug.Log("[StateCtrl] goState : " + newState.stateName);
		}

        if (!newState.isInited) {
            newState.Init(this.user);
        }

		newState.OnEnter();
		
		// 事件
		this.onStateChange.Call(DictSO.New().Set("lastState", lastState).Set("newState", newState));

	}

	/** 鎖住狀態 */
	public void LockState () {
		this.isLockState = true;
	}

	/** 解鎖狀態 */
	public void UnlockState () {
		this.isLockState = false;

		// 依照選項 更新狀態
		this.UpdateState();

	}

	/** 依照選項更新狀態 */
	public void UpdateState () {
		this.GoState(this._nextState, true);
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}