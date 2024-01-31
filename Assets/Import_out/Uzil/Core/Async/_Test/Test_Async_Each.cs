using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Uzil;

namespace Uzil.Test {

public class Test_Async_Each : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Start () {

		List<float> eles = new List<float>{5.1f, 3.2f, 4.3f};
		
		Uzil.Async.EachSeries<float>(eles, (each, next)=>{
			Uzil.Invoker.Inst().Once(()=>{
				Debug.Log(each);
				next(true);
			});

		}, ()=>{
			Debug.Log("Done");
		});
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
