using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Uzil.Values;

namespace Uzil.Test {

public class Test_Values : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Start () {
		Vals vals = new Vals("default");

		// 首個 A(2) 顯示 A
		vals.Set("testA", /* priority */2, /* value */"A");
		Debug.Log(vals.GetCurrent());

		// B(3) 優先度晚於 A(3) 故 依舊顯示 A
		vals.Set("testB", /* priority */3, /* value */"B");
		Debug.Log(vals.GetCurrent());

		// 改B(3)為B(1)，B(1) 優先度先於 A(3) 故 改顯示 B
		vals.SetPriority("testB", 1);
		Debug.Log(vals.GetCurrent());

		// 改 B(1)值 為 BB 故 改顯示 BB
		vals.SetValue("testB", "BB");
		Debug.Log(vals.GetCurrent());
		
		object memo = vals.ToMemo();

		// 移除 B， 剩下 A(2) 為最優先 故 顯示 A
		vals.Remove("testB");
		Debug.Log(vals.GetCurrent());


		// 移除所有 故 顯示 預設default
		vals.Clear();
		Debug.Log(vals.GetCurrent());

		Debug.Log(DictSO.ToJson(memo));

		vals.LoadMemo(memo);
		Debug.Log(vals.GetCurrent());

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
