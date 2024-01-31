using System.Collections.Generic;

using UnityEngine;

namespace Uzil.RNG {


/** 
 * 隨機相關公用
 * 持有並管理 多個 隨機實例
 */
public class RNGUtil {
	public bool isDebug = false;

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	public const string defaultKey = "_default";

	public static Dictionary<string, RNG> key2Instance = new Dictionary<string, RNG>();

	/*=====================================Static Funciton=======================================*/

	/** 取得 實例 */
	public static RNG Inst (string key = RNGUtil.defaultKey) {
		RNG instance = null;

		// 若 實體存在 則 取用
		if (RNGUtil.key2Instance.ContainsKey(key)) {

			instance = RNGUtil.key2Instance[key];

		}
		// 否則 建立
		else {

			// 取得 實體 根物件
			GameObject customVarGObj = RootUtil.GetMember("RNG");
			GameObject gObj = RootUtil.GetChild(/* name */key, /* parent */customVarGObj);
			if (gObj == null) return null;

			instance = gObj.AddComponent<RNG>();
			RNGUtil.key2Instance.Add(key, instance);

			instance.Init(key);

		}
		return instance;
	}

	/** 銷毀 */
	public static void Del (string key) {
		if (RNGUtil.key2Instance.ContainsKey(key) == false) return;
		RNG instance = RNGUtil.key2Instance[key];
		RNGUtil.key2Instance.Remove(key);
		GameObject.Destroy(instance.gameObject);
	}

    /** 紀錄為Json格式 */
	public static object ToMemo () {
		DictSO memo = DictSO.New();

		DictSO key2Instance = DictSO.New();
		foreach (KeyValuePair<string, RNG> pair in RNGUtil.key2Instance) {
			key2Instance.Set(pair.Key, pair.Value.ToMemo());
		}

		memo.Set("key2Instance", key2Instance);

		return memo;
	}
	
	/** 讀取Json格式 */
	public static void LoadMemo (object memoJson) {
		DictSO data = DictSO.Json(memoJson);

		// 剩餘名單(未被使用的)
		List<string> left = new List<string>(RNGUtil.key2Instance.Keys);

		if (data.ContainsKey("key2Instance")) {

			DictSO key2Instance_data = data.GetDictSO("key2Instance");

			foreach (KeyValuePair<string, object> pair in key2Instance_data) {
				string key = pair.Key;
				object instanceMemo = pair.Value;

				// 取得實例 並 讀取
				RNG instance = RNGUtil.Inst(key);
				instance.LoadMemo(instanceMemo);

				// 從剩餘名單中移除
				if (left.Contains(key)) {
					left.Remove(key);
				}

			}
		}
		// 移除未被使用的實例
		foreach (string key in left) {
			RNGUtil.Del(key);
		}

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