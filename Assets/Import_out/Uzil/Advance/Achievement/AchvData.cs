using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Uzil.Achievement {

public class AchvData {
	
	/*======================================Constructor==========================================*/

	public AchvData () {

	}

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 識別 */
	public string id = "";

	/** API用ID */
	public string apiID = "";

	/** 統計用ID */
	public string statID = null;

	/** 是否解鎖 */
	public bool isUnlock {
		get;
		protected set;
	} = false;

	/** 是否隱藏 */
	public bool isHide = false;

	/** 進度 */
	protected int stat = 0;

	/** 總進度 */
	protected int? statMax = null;

	/** 圖片路徑-解鎖 */
	public string iconPath_unlock = null;

	/** 圖片路徑-鎖住 */
	public string iconPath_locked = null;

	/** 是否需要同步 */
	public bool isNeedSync = false;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*========================================Interface==========================================*/
	
	/*=====================================Public Function=======================================*/
	
    /** 讀取資料 */
	public void LoadData (object memoJson) {
		DictSO data = DictSO.Json(memoJson);

		data.TryGetString("api_id", (res)=>{
			this.apiID = res;
		});

		data.TryGetString("stat_id", (res)=>{
			this.statID = res;
		});

		data.TryGetBool("isHide", (res)=>{
			this.isHide = res;
		});

		data.TryGetInt("statMax", (res)=>{
			this.statMax = res;
		});

		data.TryGetString("icon_unlock", (res)=>{
			this.iconPath_unlock = res;
		});

		data.TryGetString("icon_locked", (res)=>{
			this.iconPath_locked = res;
		});

	}

	public void SetStat (int stat) {
		if (this.stat == stat) return;
		this.stat = stat;
		this.onStatChanged();
		this.isNeedSync = true;
	}

	public int GetStat () {
		return this.stat;
	}

	public int? GetStatMax () {
		return this.statMax;
	}

	/*===================================Protected Function======================================*/

	protected void onStatChanged () {
		if (this.statMax != null) {
			this.isUnlock = this.stat >= this.statMax;
		} else {
			this.isUnlock = this.stat > 0;
		}
	}
	
	/*====================================Private Function=======================================*/
}


}
