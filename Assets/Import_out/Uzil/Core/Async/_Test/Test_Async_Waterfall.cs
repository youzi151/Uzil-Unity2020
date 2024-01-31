using System;
using System.Collections.Generic;

using UnityEngine;

namespace Uzil.Test {

public class Test_Async_Waterfall : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Start () {

		// 異步依序執行 <傳遞參數類型>
		Async.Waterfall<DictSO>(
			new List<Action<DictSO, Action<DictSO>>>(){
				// 建立
				(data, next)=>{
					Debug.Log("1. new data");
					DictSO newData = new DictSO();
					next(newData);
				},
				// 添加資料
				(data, next)=>{
					Debug.Log("2. set msg");
					data.Set("msg", "Hello");
					next(data);
				},

				// 修改內容 並 等候2秒
				(data, next)=>{
					Debug.Log("3. fix msg and wait 2 sec");
					data.Set("msg", "Hello Fixed");
					Invoker.main.Once(()=>{
						next(data);
					}, 2);
				},

			}, 
			(data)=>{
				// 最後顯示
				Debug.Log("4. msg :");
				Debug.Log(data.GetString("msg"));
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
