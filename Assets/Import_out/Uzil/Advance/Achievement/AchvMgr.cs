using System;
using System.Collections.Generic;

using Uzil.Res;

namespace Uzil.Achievement {

public class AchvMgr {

	
	/*======================================Constructor==========================================*/

	private AchvMgr () {}

	/*=====================================Static Members========================================*/

	private static AchvMgr inst = null;
	public static AchvMgr Inst () {
		if (AchvMgr.inst != null) {
			return AchvMgr.inst;
		}
		
		AchvMgr inst = new AchvMgr();
		inst.Init();

		AchvMgr.inst = inst;
		return AchvMgr.inst;
	}

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 成就資料路徑 */
	public string achvDataPath = "Data/achievement.json";

	/** 成就名冊 */
	protected Dictionary<string, AchvData> id2Achv = new Dictionary<string, AchvData>();

	/** 成就連結 */
	public List<AchvLink> links = new List<AchvLink>();

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	public void Init () {

		AchvLink_Save saveLink = new AchvLink_Save();
		saveLink.Init();

		this.links.Add(saveLink);
	
#if STEAM
		AchvLink_Steam steamLink = new AchvLink_Steam();
		steamLink.Init();

		this.links.Add(steamLink);
#endif

		// 讀取資料
		this.LoadAchvData();

		// 從檔案 讀取進度
		saveLink.SyncFrom(this);

		// 同步 到 檔案以外的
		this.SyncToLinks(excepts: new AchvLink[]{saveLink});
	}

	/** 讀取 成就 資料 (id:achvData_json)*/
	public void LoadAchvData (DictSO id2Achvs = null) {
		if (id2Achvs == null) {
			string fileStr = ResMgr.Get<string>(new ResReq().Path(this.achvDataPath));
			if (fileStr != null) {
				id2Achvs = DictSO.Json(fileStr);
			}
		}

		if (id2Achvs == null) return;

		foreach (KeyValuePair<string, object> pair in id2Achvs) {
			AchvData data = this.GetAchv(pair.Key);
			if (data == null) {
				data = new AchvData();
				data.id = pair.Key;
				this.id2Achv.Add(pair.Key, data);
			}
			data.LoadData((DictSO) pair.Value);
		}
	}

	/** 與 已連結對象 同步 */
	public void SyncToLinks (AchvLink[] targets = null, AchvLink[] excepts = null, bool isForceSync = false) {
		foreach (AchvLink each in this.links) {
			// 若 在排除中 則 忽略
			if (excepts != null) {
				if (Array.IndexOf(excepts, each) != -1) continue;
			}

			// 若 有指定
			if (targets != null) {
				// 若 不包含在指定中 則 忽略
				if (Array.IndexOf(targets, each) == -1) continue;
			}

			each.SyncTo(this);
		}

		foreach (KeyValuePair<string, AchvData> pair in this.id2Achv) {
			pair.Value.isNeedSync = false;
		}
	}

	/** 完成 進度 */
	public void SetDone (string id) {
		AchvData data = this.GetAchv(id);
		if (data == null) return;

		int? statMax = data.GetStatMax();
		int stat = statMax == null ? 1 : (int)statMax;

		this.SetStat(id, stat);
	}

	/** 設置 進度 */
	public void SetStat (string id, int stat) {
		AchvData data = this.GetAchv(id);
		if (data == null) return;

		int? statMax = data.GetStatMax();
		if (statMax == null) {
			if (stat > 1) {
				stat = 1;
			}
		}
		data.SetStat(stat);

		if (data.isNeedSync) {
			this.SyncToLinks();
		}
	}

	/** 取得 成就資料 */
	public AchvData GetAchv (string id) {
		if (id == null) return null;
		if (this.id2Achv.ContainsKey(id) == false) return null;
		return this.id2Achv[id];
	}

	/** 取得 成就資料表 */
	public Dictionary<string, AchvData> GetAchvs () {
		return this.id2Achv;
	}

	public void ClearAchv () {
#if UNITY_EDITOR
		foreach (AchvLink each in this.links) {
			each.Clear(this);
		}
#endif
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}
