using System;
using System.Collections.Generic;

using UnityEngine;

namespace Uzil.Test {

public class Test_Async_Parallel : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Start () {

		// 異步執行 <結算類別>
		Async.Parallel<int>(
			new List<Action<Action<int>>>() {
				(each)=>{
					Invoker.main.Once(()=>{
						Debug.Log("B: add 2");
						each(2);
					}, 2);
				}
				,			
				(each)=>{
					Debug.Log("A: add 1");
					each(1);
				}
			},
			/* eachDone */
			(idx, results)=>{
				Debug.Log("Add idx["+idx+"], results count: "+results.Count);
			},
			/* done */
			(results)=>{
				int total = 0;
				foreach (int num in results) {
					total += num;
				}
				Debug.Log("total: "+total);
			}
		);
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
