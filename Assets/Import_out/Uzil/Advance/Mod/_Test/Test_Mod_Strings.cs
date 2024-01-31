using UnityEngine;

using Uzil.i18n;
using Uzil.Mod;

namespace Uzil.Test {

public class Test_Mod_Strings : MonoBehaviour {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Start () {

		ModMgr modMgr = ModMgr.Inst();
		modMgr.AddBaseLoader();
		modMgr.ReloadAll();

		Debug.Log(StrReplacer.RenderNow("<$testKeyword$>"));

		// StrReplacer.Language("zh-tw");

		Debug.Log(StrReplacer.RenderNow("<$testKeyword$>"));
		
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
