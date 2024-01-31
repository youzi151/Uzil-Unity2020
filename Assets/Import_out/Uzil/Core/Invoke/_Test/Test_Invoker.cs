using UnityEngine;

namespace Uzil.Test {

public class Test_Invoker : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Start () {
		
		// 註冊 "不能呼叫的"
		Invoker.main.Once(()=>{
			Debug.Log("do not call");
		}, 0f).Tag("do not call");

		// 取消 "不能呼叫的"
		Invoker.main.Cancel("do not call");

		// 建立 第一、二項任務(內容錯誤)
		InvokeTask task1 = Invoker.main.Once(()=>{
			Debug.Log("secondCall");
		}, 1f).Tag("secondCall");

		InvokeTask task2 = Invoker.main.Once(()=>{
			Debug.Log("firstCall");
		}, 2f).Tag("firstCall");

		// 在 第一任務即將執行前 呼叫修正
		Invoker.main.Once(()=>{
			// 修正 兩者執行內容
			task2.SetAct(()=>{
				Debug.Log("secondCall");
			});

			task1.SetAct(()=>{
				Debug.Log("firstCall");
			});
		}, 0.9999999f);
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
