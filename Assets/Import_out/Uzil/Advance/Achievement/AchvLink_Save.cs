using System.Collections;
using System.Collections.Generic;

using Uzil.UserData;

namespace Uzil.Achievement {

public class AchvLink_Save : AchvLink {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 存檔 檔名 */
	public string saveFileName = "player.sav";

	/** 存檔 鍵 */
	public string saveFileKey = "achv";

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*========================================Interface==========================================*/

	/** 初始化 */
	public override void Init () {
		
	}
	
	/** 同步 到 */
	public override void SyncTo (AchvMgr mgr) {
		DictSO id2Stat = new DictSO();
		Dictionary<string, AchvData> id2Achv = mgr.GetAchvs();
		foreach (KeyValuePair<string, AchvData> pair in id2Achv) {
			AchvData data = pair.Value;

			// 因為是存檔 重新建立 所以 不用檢查是否需要同步
			// if (data.isNeedSync == false) continue;

			int stat = data.GetStat();

			// 若是初始狀態 則 忽略
			if (data.isUnlock == false && stat == 0) continue;
			
			id2Stat.Add(pair.Key, stat);
		}
		UserSave.main.SetObj(this.saveFileName, this.saveFileKey, id2Stat);
	}

	/** 同步 自 */
	public override void SyncFrom (AchvMgr mgr) {
		DictSO achv2Data = UserSave.main.GetObj(this.saveFileName, this.saveFileKey);
		if (achv2Data == null) return;

		foreach (KeyValuePair<string, object> pair in achv2Data) {
			AchvData data = mgr.GetAchv(pair.Key);
			if (data == null) continue;
			data.SetStat(DictSO.Int(pair.Value));
		}
	}

	/** 清除 */
	public override void Clear (AchvMgr mgr) {	
#if UNITY_EDITOR

		UserSave.main.Remove(this.saveFileName, this.saveFileKey);
		
#endif
	}

	/*=====================================Public Function=======================================*/
	
	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}
