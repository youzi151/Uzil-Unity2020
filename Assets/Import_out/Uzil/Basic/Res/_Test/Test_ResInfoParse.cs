using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Uzil.Res;

namespace Uzil.Test {

public class Test_ResInfoParse : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Start () {
		ResInfo info = ResInfo.Parse("test/folder/file.ext:[tagA, tagA.alter, ab:testbundle]");
		Debug.Log(info.path);
		Debug.Log(info.GetBundleName());
		foreach (string each in info.tags) {
			Debug.Log(each);
		}
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
