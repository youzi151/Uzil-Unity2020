using System.Collections.Generic;

using Uzil;
using Uzil.PostProc;

namespace UZAPI {

public class PostProc {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/** 設置 優先 */
	public static void SetPriority (string instID, string userName, float priority) {
		PostProcMgr ppMgr = PostProcMgr.Inst(instID);
		ppMgr.SetPriority(userName, priority);
	}

	/** 設置 效果 */
	public static void SetEffects (string instID, string userName, string effects_str) {
		PostProcMgr ppMgr = PostProcMgr.Inst(instID);

		DictSO effects = DictSO.Json(effects_str);

		foreach (KeyValuePair<string, object> pair in effects) {
			ppMgr.SetEffect(userName, pair.Key, DictSO.Json(pair.Value));
		}
	}

	/** 移除 效果 */
	public static void RemoveEffects (string instID, string userName) {
		PostProcMgr ppMgr = PostProcMgr.Inst(instID);
		ppMgr.RemoveEffects(userName);
	}

	/** 更新 效果 */
	public static void UpdateEffects (string instID) {
		PostProcMgr ppMgr = PostProcMgr.Inst(instID);
		ppMgr.UpdateEffect();
	}

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/
	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}
