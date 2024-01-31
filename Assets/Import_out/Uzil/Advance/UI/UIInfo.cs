using System.Collections.Generic;

using UnityEngine;

using Uzil.Misc;
using Uzil.Anim;

namespace Uzil.UI {

public class UIInfo {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/
	
	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	public string id = "_anony";

	/** 排序 */
	public float sort = 0f;

	/** 父物件 */
	public UIInfo parent = null;

	/** 銷毀偵聽 */
	public DestroyListener destroyListener = null;

	/** 變形  */
	public RectTransform trans;

	/** 類型組件 */
	protected List<Component> typeComps = new List<Component>();
	
	/** 動畫狀態機 */
	public Animator_Custom animator;	
	
	/** 動畫屬性目標 */
	public PropTarget_UI propTarget;


	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	public T RequestComp<T> () where T : Component {
		T comp = this.trans.gameObject.GetComponent<T>();
		if (comp != null) return comp;
		return this.trans.gameObject.AddComponent<T>();
	}

	public T GetComp<T> () where T : Component {
		T comp = this.trans.gameObject.GetComponent<T>();
		if (comp != null) return comp;
		else return null;
	}

	public void ClearComps () {
		for (int idx = this.typeComps.Count-1; idx >= 0; idx--) {
			Component comp = this.typeComps[idx];
			Component.DestroyImmediate(comp);
		}
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}