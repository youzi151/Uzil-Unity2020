using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Uzil.Util;

namespace Uzil.Res {

class AsbInfo {
	public string bundleName;
	public int usedCount = 0;
	public AssetBundle assetBundle;
}

/* 
 * 簡單提供Uzil.Res管理AssetBundle
 * 若有需要非同步預先加載，則可於其他地方自行處理
 */
public class ResAsbMgr {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	private static Dictionary<string, AsbInfo> name2AsbInfo = new Dictionary<string, AsbInfo>();

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/
	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/* 使用 */
	public static AssetBundle Use (string bundleName) {
		AsbInfo asbInfo;
		
		// 若 已註冊
		if (ResAsbMgr.name2AsbInfo.ContainsKey(bundleName)) {
			
			// 直接取用
			asbInfo = ResAsbMgr.name2AsbInfo[bundleName];

		}
		// 若 尚未註冊 則 建立註冊資訊
		else {
			
			asbInfo = new AsbInfo();
			asbInfo.bundleName = bundleName;
			asbInfo.usedCount = 0;
			
			// 讀取 AssetBundle
			asbInfo.assetBundle = AssetBundle.LoadFromFile(PathUtil.Combine(PathUtil.GetAssetBundlePath(), bundleName));
			if (asbInfo.assetBundle == null) return null;
			
			// 加入註冊表
			ResAsbMgr.name2AsbInfo.Add(bundleName, asbInfo);

		}

		// 增加 已經使用數量
		asbInfo.usedCount += 1;

		return asbInfo.assetBundle;
	}

	/* 棄用 */
	public static void Unuse (string bundleName) {
		if (ResAsbMgr.name2AsbInfo.ContainsKey(bundleName) == false) {
			return;
		}
		AsbInfo asbInfo = ResAsbMgr.name2AsbInfo[bundleName];
		asbInfo.usedCount -= 1;

		if (asbInfo.usedCount <= 0) {
			asbInfo.assetBundle.Unload(false);
			ResAsbMgr.name2AsbInfo.Remove(bundleName);
		}
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}
