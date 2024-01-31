#if STEAM

using Steamworks;

namespace Uzil.ThirdParty.Steam {

public class SteamStats {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 設置 成就 */
	public bool SetAchv (string apiID) {
		if (SteamInst.Inst().isInitialized == false) return false;
		return SteamUserStats.SetAchievement(apiID);
	}

	/** 取得 成就 */
	public bool GetAchv (string apiID) {
		if (SteamInst.Inst().isInitialized == false) return false;

		bool res = false;
		SteamUserStats.GetAchievement(apiID, out res);
		return res;
	}

	/** 清除 成就 */
	public bool ClearAchv (string apiID) {
		if (SteamInst.Inst().isInitialized == false) return false;
#if UNITY_EDITOR
		return SteamUserStats.ClearAchievement(apiID);
#else
		return false;
#endif
	}

	/** 設置統計 */
	public bool SetStat (string apiID, int val) {
		if (SteamInst.Inst().isInitialized == false) return false;

		return SteamUserStats.SetStat(apiID, val);
	}

	/** 請求統計/成就 */
	public bool RequestCurrentStats () {
		if (SteamInst.Inst().isInitialized == false) return false;

		return SteamUserStats.RequestCurrentStats();
	}

	/** 儲存統計/成就 */
	public bool StoreStats () {
		if (SteamInst.Inst().isInitialized == false) return false;

		return SteamUserStats.StoreStats();
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}

#endif