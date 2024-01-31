using System.Collections.Generic;

using UnityEngine;


namespace Uzil.Audio {

public class AudioMgrInspector : MonoBehaviour {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	public List<AudioClip> clips = new List<AudioClip>();

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	void Update () {
		List<UnityEngine.Object> list = new List<UnityEngine.Object>(Resources.FindObjectsOfTypeAll<AudioClip>());
		this.clips.Clear();
		for (int idx = 0; idx < list.Count; idx++) {
			this.clips.Add((AudioClip) list[idx]);
		}
	}

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}



}