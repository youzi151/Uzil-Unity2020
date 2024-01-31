using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Uzil.Audio {

/*
 * 音效代號 與實際對應的 音效檔案路徑
 */

public class AudioSet {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	public static Dictionary<string, string> sets = new Dictionary<string, string>();

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	public static void Set (string key, string path) {
		if (AudioSet.sets.ContainsKey(key)) {
			AudioSet.sets[key] = path;
		} else {
			AudioSet.sets.Add(key, path);
		}
	}

	public static string Get (string key) {
		if (AudioSet.sets.ContainsKey(key) == false) {
			return null;
		} else {
			return AudioSet.sets[key];
		}
	}


	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/


}

}