using System.Collections.Generic;
using UnityEngine;

using Uzil.BuiltinUtil;

// namespace XXX.XXX {

public class TTTTTT : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	RectTransform rectTrans = null;

	public RectTransform targetParent = null;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Start () {
		this.rectTrans = this.GetComponent<RectTransform>();
		// this.canvas = RectTransformUtil.FindRootCanvas(this.rectTrans);
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log(RectTransformUtil.GetRectIn(this.rectTrans, targetParent));
	}

	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


//} //namespace XXX.XXX
