using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Uzil;
using Uzil.Res;

namespace Uzil.ObjInfo {


public class ObjInfo : IMemoable {

	/*======================================Constructor==========================================*/

	public ObjInfo () {

	}

	public ObjInfo (object jsonOrPath) {
		this.raw = jsonOrPath;

		DictSO data = DictSO.Json(jsonOrPath);
		
		if (data == null) return;

		this.LoadMemo(data);

	}

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 原始資料 */
	public object raw = null;

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** [IMemoable] 紀錄為Json格式 */
	public virtual object ToMemo () {
		DictSO data = DictSO.New();

		return data;
	}

	/** [IMemoable] 讀取Json格式 */
	public virtual void LoadMemo (object memoJson) {
		
		DictSO data = DictSO.Json(memoJson);
		if (data == null) return;

		
	}

	/** 應用在 */
	// public virtual void ApplyOn (T target) {

	// }

    /** 從實體複製 */
	// public void CopyFrom (T target) {

	// }

	/*=====================================Public Function=======================================*/


	/** 取得原始資料 */
	public object GetRaw () {
		DictSO rawData = DictSO.Json(this.raw);
		if (rawData == null) return this.raw;
		else return rawData;
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

}

}