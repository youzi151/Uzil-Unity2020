using UnityEngine;

using System.Collections.Generic;

namespace Uzil {

public class TimeUtil {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	public const string defaultKey = "_default";

	/* Key:實體 表 */
	public static Dictionary<string, TimeInstance> key2Instance = new Dictionary<string, TimeInstance>();

	/*=====================================Static Funciton=======================================*/

	/* 取得實體 */
	public static TimeInstance Inst (string key = TimeUtil.defaultKey) {
		TimeInstance instance = null;

		// 若 實體存在 則 取用
		if (TimeUtil.key2Instance.ContainsKey(key)) {

			instance = TimeUtil.key2Instance[key];

		}
		// 否則 建立
		else {

			// 取得 時間實體 根物件
			GameObject timesGObj = RootUtil.GetMember("Times");
			GameObject gObj = RootUtil.GetChild(/* name */key, /* parent */timesGObj);
			if (gObj == null) return null;

			instance = gObj.AddComponent<TimeInstance>();
			TimeUtil.key2Instance.Add(key, instance);

			instance.Init(key);

		}

		return instance;
	}

	/* 銷毀 */
	public static void Del (string key) {
		if (TimeUtil.key2Instance.ContainsKey(key) == false) return;
		TimeInstance instance = TimeUtil.key2Instance[key];
		TimeUtil.key2Instance.Remove(key);
		GameObject.Destroy(instance.gameObject);
	}

	/*== 快速 ===============*/

	/* 當幀時間差 */
	public static float deltaTime {
		get {
			return TimeUtil.Inst().deltaTime;
		}
	}

	/* [IMemoable] 紀錄為Json格式 */
	public static object ToMemo () {
		DictSO memo = DictSO.New();

		DictSO key2Instance = DictSO.New();
		foreach (KeyValuePair<string, TimeInstance> pair in TimeUtil.key2Instance) {
			key2Instance.Set(pair.Key, pair.Value.ToMemo());
		}

		memo.Set("key2Instance", key2Instance);

		return memo;
	}

	/* [IMemoable] 讀取Json格式 */
	public static void LoadMemo (object memoJson) {
		DictSO data = DictSO.Json(memoJson);

		// 剩餘名單(未被使用的)
		List<string> left = new List<string>(TimeUtil.key2Instance.Keys);

		if (data.ContainsKey("key2Instance")) {

			DictSO key2Instance_data = data.GetDictSO("key2Instance");

			foreach (KeyValuePair<string, object> pair in key2Instance_data) {
				string key = pair.Key;
				object instanceMemo = pair.Value;

				// 取得實例 並 讀取
				TimeInstance instance = TimeUtil.Inst(key);
				instance.LoadMemo(instanceMemo);

				// 從剩餘名單中移除
				if (left.Contains(key)) {
					left.Remove(key);
				}

			}
		}
		// 移除未被使用的實例
		foreach (string key in left) {
			TimeUtil.Del(key);
		}
	}

	/* 更新時間比例
	 * 若是被key為預設的TimeInstance更新，則依此更新遊戲引擎內建的TimeScale
	 */
	public static void UpdatedTimeScale (string key) {
		if (key == TimeUtil.defaultKey) {
			TimeInstance inst = TimeUtil.Inst();
			if (inst != null) {
				UnityEngine.Time.timeScale = inst.timeScale;
			}
		}
	}

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/
}


}
