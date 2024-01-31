using UnityEngine;
using System.Collections;

namespace Uzil.Misc {


public class Coroutine : MonoBehaviour {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	private static Coroutine instance;

	/*=====================================Static Funciton=======================================*/

	public static Coroutine Inst () {
		if (Coroutine.instance == null){
			Coroutine.instance = RootUtil.GetMember("Coroutine").AddComponent<Coroutine>();
		}
		return Coroutine.instance;
	}
	public static void Clear () {
		Coroutine.instance = null;
		Coroutine.Inst();
	}


	/*=======Coroutine========*/

	public static UnityEngine.Coroutine Start (IEnumerator routine) {
		return Coroutine.Inst().StartCoroutine(routine);
	}

	public static void Stop (IEnumerator routine) {
		Coroutine.Inst().StopCoroutine(routine);
	}

	public static void StopAll () {
		Coroutine.Inst().StopAllCoroutines();	
	}

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/


}


}