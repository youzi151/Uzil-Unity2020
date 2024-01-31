using UnityEngine;

namespace Uzil.Test {

public class Test_InvokerOnFrame : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Start () {
		
		// 會被取消的呼叫
		InvokerOnFrame.Once("test", ()=>{
			Debug.Log("beCancelCall");
		});

		// 取消
		InvokerOnFrame.Cancel("test");

		// 第1次 呼叫
		InvokerOnFrame.Once("test", ()=>{
			Debug.Log("firstCall");
		});

		// 第2次 呼叫
		InvokerOnFrame.Once("test", ()=>{
			Debug.Log("secondCall");
		});

		// 第3次 呼叫
		InvokerOnFrame.Once("test", ()=>{
			Debug.Log("thirdCall");
		});

		// 該幀應該只會顯示 thirdCall
		// 因為 後面 同名的呼叫 會 覆蓋 前面的呼叫

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
