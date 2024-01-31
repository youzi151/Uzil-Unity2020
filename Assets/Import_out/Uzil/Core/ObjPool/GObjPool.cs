using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Uzil {

public class GObjPool<T> : ObjPool<GameObject> {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	public GameObject original = null;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	public T2 ReuseComp<T2> () {
		GameObject gObj = this.Reuse();
		return gObj.GetComponent<T2>();
	}

	public void RecoveryComp<T2> (T2 targetComp) where T2 : MonoBehaviour  {
		T2 comp = targetComp.GetComponent<T2>();
		GameObject gObj = comp.gameObject;
		this.Recovery(gObj);
	}

	/*===================================Protected Function======================================*/
	
	/** 建立 */
	protected override GameObject create () {
		if (this.original == null) return null;

		GameObject newOne = GameObject.Instantiate(this.original);
		return newOne;
	}

	/** 初始化 */
	protected override void init (GameObject target) {
		if (target == null) return;
		target.SetActive(true);
	}

	/** 反初始化 */
	protected override void uninit (GameObject target) {
		if (target == null) return;
		target.SetActive(false);
	}


	/*====================================Private Function=======================================*/
}

}