using System.Collections.Generic;

using Uzil;
using Uzil.RNG;

namespace UZAPI {

public class RNG {
	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/**== 池隨機 ===============*/

	/** 設置 池隨機 */
	public static void SetPoolRandom (string instKey, string id, string datajson) {
		PoolRandom poolRandom = RNGUtil.Inst(instKey).GetPoolRandom(id);
		poolRandom.LoadMemo(datajson);
	}

	/** 設置 池隨機 種子 */
	public static void SetPoolRandomSeed (string instKey, string id, string seed) {
		PoolRandom poolRandom = RNGUtil.Inst(instKey).GetPoolRandom(id);
		poolRandom.SetSeed(seed);
	}

	/** 設置 池隨機 範圍 */
	public static void SetPoolRandomRange (string instKey, string id, int min, int max) {
		PoolRandom poolRandom = RNGUtil.Inst(instKey).GetPoolRandom(id);
		poolRandom.SetRange(min, max);
	}

	/** 設置 池隨機 比率 */
	public static void SetPoolRandomRate (string instKey, string id, int num, int rate) {
		PoolRandom poolRandom = RNGUtil.Inst(instKey).GetPoolRandom(id);
		poolRandom.SetRate(num, rate);
	}

	/** 取得 池隨機 隨機值 */
	public static float GetPoolRandomNum (string instKey, string id) {
		PoolRandom poolRandom = RNGUtil.Inst(instKey).GetPoolRandom(id);
		return poolRandom.GetFloat();
	}
	
	/** 取得 池隨機 隨機整數 */
	public static int GetPoolRandomInt (string instKey, string id) {
		PoolRandom poolRandom = RNGUtil.Inst(instKey).GetPoolRandom(id);
		return poolRandom.GetInt();
	}

	/** 重置 */
	public static void ResetPoolRandom (string instKey, string id) {
		PoolRandom poolRandom = RNGUtil.Inst(instKey).GetPoolRandom(id);
		poolRandom.ResetPool();
	}

	/**== 加權隨機 ===============*/

	/** 取得加權隨機 */
	public static int GetRateRandom (string rateList_json, string seed = null) {
		List<float> rateList = DictSO.List<float>(rateList_json);
		return RateRandom.Get(rateList.ToArray(), seed);
	}


	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}

}