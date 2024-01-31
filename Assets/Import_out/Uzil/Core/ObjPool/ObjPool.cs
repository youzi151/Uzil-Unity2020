using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Uzil {

public class ObjPool<T> {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/** 使用中 */
	public List<T> usedList = new List<T>();

	/** 物件池 */
	public Stack<T> pool = new Stack<T>();


	/*==========================================Event============================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 請求 */
	public virtual T Reuse () {
		T newOne;
		if (this.pool.Count == 0) {
			newOne = this.create();
		} else {
			newOne = this.pool.Pop();
		}
		
		this.init(newOne);

		return newOne;
	}

	/** 請求 */
	public virtual void Recovery (T toRecovery) {
		if (this.usedList.Contains(toRecovery)) {
			this.usedList.Remove(toRecovery);
		}
		if (this.pool.Contains(toRecovery)) return;

		this.pool.Push(toRecovery);

		this.uninit(toRecovery);
	}

	/** 清除 */
	public void Clear () {

	}

	/*===================================Protected Function======================================*/
	
	/** 建立 */
	protected virtual T create () {
		return default(T);
	}

	/** 初始化 */
	protected virtual void init (T target) {

	}

	/** 反初始化 */
	protected virtual void uninit (T target) {
		
	}


	/*====================================Private Function=======================================*/
}

}