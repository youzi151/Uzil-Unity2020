#if STEAM

using System.Collections.Generic;
using UnityEngine;

using Uzil.ThirdParty.Steam;


namespace Uzil.Achievement {

public class AchvLink_Steam : AchvLink {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	protected bool isCurrentStatsRequested = false;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*========================================Interface==========================================*/
	
	/** 初始化 */
	public override void Init () {
		if (SteamInst.Inst().isInitialized == false) return;
		SteamStats steamStats = SteamInst.Inst().Stats();
		this.isCurrentStatsRequested = steamStats.RequestCurrentStats();
	}

	/** 同步 到 */
	public override void SyncTo (AchvMgr mgr) {
		if (SteamInst.Inst().isInitialized == false) return;

		SteamStats steamStats = SteamInst.Inst().Stats();

		if (this.isCurrentStatsRequested == false) {
			this.isCurrentStatsRequested = steamStats.RequestCurrentStats();
		}

		bool isAnyChanged = false;

		Dictionary<string, AchvData> id2Achv = mgr.GetAchvs();
		foreach (KeyValuePair<string, AchvData> pair in id2Achv) {

			AchvData data = pair.Value;

			if (data.isNeedSync == false) continue;

			if (data.statID != null) {
				steamStats.SetStat(data.statID, data.GetStat());
				isAnyChanged = true;
				// Debug.Log("Set steam stat: ["+data.statID+"] = "+data.GetStat());
			}

			bool isSteamUnlock = steamStats.GetAchv(data.apiID);

			if (data.isUnlock && isSteamUnlock == false) {
				bool isSuccess = steamStats.SetAchv(data.apiID);
				isAnyChanged = true;
				// Debug.Log("Unlock steam achv: ["+pair.Key+"] = "+isSuccess);
			}
		}

		if (isAnyChanged) {
			steamStats.StoreStats();
		}
	}

	/** 同步 自 */
	public override void SyncFrom (AchvMgr mgr) {
		if (SteamInst.Inst().isInitialized == false) return;
		// empty
	}

	/** 清除 */
	public override void Clear (AchvMgr mgr) {
		if (SteamInst.Inst().isInitialized == false) return;

#if UNITY_EDITOR

		SteamStats steamStats = SteamInst.Inst().Stats();

		List<string> list_str = new List<string>();

		Dictionary<string, AchvData> id2Achv = mgr.GetAchvs();
		foreach (KeyValuePair<string, AchvData> pair in id2Achv) {
			AchvData data = pair.Value;
			steamStats.ClearAchv(data.apiID);
			list_str.Add(pair.Key);
		}
		steamStats.StoreStats();
		Debug.Log("Clear Steam Achvs:"+DictSO.ToJson(list_str));
#endif
	}

	/*=====================================Public Function=======================================*/
	
	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}

#endif