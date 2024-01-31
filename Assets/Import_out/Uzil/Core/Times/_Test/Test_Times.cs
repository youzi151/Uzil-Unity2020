using UnityEngine;

using Uzil;

namespace Uzil.Test {

public class Test_Times : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		TimeInstance defaultTime = TimeUtil.Inst();
		TimeInstance altTime = TimeUtil.Inst("alt");
		altTime.SetTimeScale(0.5f);

		Debug.Log(altTime.timeScale);

		float time = defaultTime.time;
		float time_alt = altTime.time;

		float dt = defaultTime.deltaTime;
		float dt_alt = altTime.deltaTime;

		Debug.Log("[real]dt("+Time.deltaTime+") | [default]:time("+time+") dt("+dt+")"+" | [alt]:time("+time_alt+") dt("+dt_alt+")");
	}

	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}
