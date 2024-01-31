using System.Collections.Generic;

using UnityEngine;

using Uzil.Audio;

namespace Uzil.Test {

public class Test_AudioMgr : MonoBehaviour {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Start () {

		DictSO args = new DictSO().Set("mixer", "AudioMixer")
								  .Set("isLoop", false)
								  .Set("isReleaseOnEnd", true)
								  .Set("isOnCamera", true);

		// 預載
		AudioMgr.Inst().Preload(/* ID */"test", /* Path */"Folder1/Test_ResAudio", args);

		// 播放
		AudioMgr.Inst().Play("test");
		
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
