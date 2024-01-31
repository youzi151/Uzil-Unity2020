using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Uzil.PageCard {

public class Card : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 識別 */
	public string id = "_anonymous";
	
	/** 目標物件 */
	public GameObject targetGameObject;

	/** 是否啟用 */
	public bool isActive {
		get {
			if (this.targetGameObject == null) return false;
			return this.targetGameObject.activeSelf;
		}
	}

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Awake () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/**
	 * 啟用
	 * @param isForceReactive 若已經啟用，是否強制重新啟用
	 */
	public void active (bool isForceReactive = false) {
		if (!isForceReactive && this.isActive) return;

		this.targetGameObject.SetActive(true);
		
		this._onActive();
	}

	/**
	 * 關閉
	 * @param isForceReDeactive 若已經關閉，是否強制重新關閉
	 */
	public void deactive (bool isForceReDeactive = false) {
		if (!isForceReDeactive && !this.isActive) return;

		this.targetGameObject.SetActive(false);

		this._onDeactive();
	}

	/*===================================Protected Function======================================*/

	/** 當啟用 */
	protected virtual void _onActive () {

	}

	/** 當關閉 */
	protected virtual void _onDeactive () {

	}
	
	/*====================================Private Function=======================================*/
}


}
