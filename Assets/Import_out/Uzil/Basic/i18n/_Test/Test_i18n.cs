using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Uzil.i18n;

namespace Uzil.Test {

public class Test_i18n : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Start () {
		StrReplacer strReplacer = StrReplacer.Inst();
		
		// 註冊 關鍵字 (直接替換)
		strReplacer.SetKeyword("testKeyword", "keyword");
		// 註冊 關鍵字 (異步函式替換)
        strReplacer.SetKeyword("testAction", (data, cb)=>{
            cb(data.GetString("msg"));
        });

		// 設置 自訂變數
		CustomVarUtil.Inst().SetStr("testVar", "var");

		// 準備 計算用時間
        float startTime, endTime;


		/*== 立即替換 ===========*/

		// 起始時間
        startTime = Time.realtimeSinceStartup;

		// 立即替換 (僅限 <$字典替換$>, <%變數替換%>)
        string res_immediate = StrReplacer.RenderNow("<$testKeyword$> | <%testVar%>");

		// 結束時間
        endTime = Time.realtimeSinceStartup;

		// 印出 結果 與 概略耗時
		Debug.Log(res_immediate+" | cost:"+(endTime - startTime));


		/*== 異步替換 ===========*/

		// 起始時間
        startTime = Time.realtimeSinceStartup;

		// 異步替換
        StrReplacer.Render("<$testKeyword$> | <%testVar%> | <!testAction{\"msg\":\"action\"}!>", (res)=>{
			
			// 結束時間
            endTime = Time.realtimeSinceStartup;

			// 印出 結果 與 概略耗時
            Debug.Log(res + " | cost:" + (endTime - startTime));
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
