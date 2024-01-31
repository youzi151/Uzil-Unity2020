using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Uzil.State {

public class State : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 狀態名稱 */
	public string stateName = "";
	
	/** 使用者 */
	public object user = null;


	/** 是否啟用 */
	public bool isActive {
		get {
			return this._isActive;
		}
	}
	/** 是否啟用 */
	private bool _isActive = false;
	
	
	/** 是否已經初始化 */
	public bool isInited {
		get {
			return this._isInited;
		}
	} 
	/** 是否已經初始化 */
	private bool _isInited = false;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/
	
	// Update is called once per frame
	void Update () {
		if (!this.isActive) return;

		this.OnUpdate(Time.deltaTime);
	}

	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*== 基本功能 =================*/

	/**
	 * 初始化
	 * @param user 使用者
	 */
	public void Init (object user) {
		if (this._isInited) return;
		this.user = user;
		this._init(user);
		this._isInited = true;
	}

	/** 進入狀態 */
	public void OnEnter () {
		this._isActive = true;
		this._onEnter();
	}

	/**
	 * 更新
	 * @param dt 每幀時間
	 */
	public void OnUpdate (float dt) {
		this._onUpdate(dt);
	}

	/** 離開狀態 */
	public void onExit () {
		this._isActive = false;
		this._onExit();
	}

	/*===================================Protected Function======================================*/

	
	/**
	 * 初始化
	 * @param user 使用者
	 */
	protected virtual void _init (object user) {
		
	}

	/** 進入狀態 */
	protected virtual void _onEnter () {
		
	}

	/**
	 * 更新
	 * @param dt 每幀時間
	 */
	protected virtual void _onUpdate (float dt) {
		
	}

	/** 離開狀態 */
	protected virtual void _onExit () {
		
	}
	
	/*====================================Private Function=======================================*/
}


}
