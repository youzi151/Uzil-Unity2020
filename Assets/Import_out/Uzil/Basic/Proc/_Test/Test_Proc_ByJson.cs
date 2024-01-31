using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Uzil.Proc;

namespace Uzil.Test {

public class Test_Proc_ByJson : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/
    
    [TextArea(0, 10)]
    public string json = "";

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Start () {
		
		ProcMgr pMgr = ProcMgr.Inst();

        pMgr.LoadMemo(this.json);

        pMgr.StartFirstNode();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}
