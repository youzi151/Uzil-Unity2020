using UnityEngine;

using Uzil;
using Uzil.Achievement;

namespace UZAPI {

public class Achv {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** 是否 已解鎖 */
	public static bool? IsUnlock (string id) {
		AchvData data = AchvMgr.Inst().GetAchv(id);
		if (data == null) return null;
		return data.isUnlock;
	}

	/** 取得進度 */
	public static int? GetStat (string id) {
		AchvData data = AchvMgr.Inst().GetAchv(id);
		if (data == null) return null;
		return data.GetStat();
	}

	/** 設置 進度 */
	public static void SetStat (string id, int stat) {
		AchvMgr.Inst().SetStat(id, stat);
	}

	/** 設置 完成 */
	public static void SetDone (string id) {
		AchvMgr.Inst().SetDone(id);
	}

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}

}