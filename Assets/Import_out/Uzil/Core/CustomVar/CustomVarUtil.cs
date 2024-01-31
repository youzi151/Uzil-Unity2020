using System.Collections.Generic;

using UnityEngine;

namespace Uzil {

public class CustomVarUtil {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	public const string defaultKey = "_default";

	/* key:實體 */
	private static Dictionary<string, CustomVar> key2Instance = new Dictionary<string, CustomVar>();


	/*=====================================Static Funciton=======================================*/

	public static CustomVar Inst (string key = CustomVarUtil.defaultKey) {
		CustomVar instance = null;

		// 若 實體存在 則 取用
		if (CustomVarUtil.key2Instance.ContainsKey(key)) {

			instance = CustomVarUtil.key2Instance[key];

		}
		// 否則 建立
		else {

			// 取得 實體 根物件
			GameObject customVarGObj = RootUtil.GetMember("CustomVars");
			GameObject gObj = RootUtil.GetChild(/* name */key, /* parent */customVarGObj);
			if (gObj == null) return null;

			instance = gObj.AddComponent<CustomVar>();
			CustomVarUtil.key2Instance.Add(key, instance);

			instance.Init(key);

		}
		return instance;
	}

	/* 銷毀 */
	public static void Del (string key) {
		if (CustomVarUtil.key2Instance.ContainsKey(key) == false) return;
		CustomVar instance = CustomVarUtil.key2Instance[key];
		CustomVarUtil.key2Instance.Remove(key);
		GameObject.Destroy(instance.gameObject);
	}

	/* 以Key尋找實體 */
	public static CustomVar Find (string key) {
		foreach (KeyValuePair<string, CustomVar> pair in CustomVarUtil.key2Instance) {
			if (pair.Value.Contains(key)) {
				return pair.Value;
			}
		}
		return null;
	}

	
	/* 紀錄為Json格式 */
	public static object ToMemo () {
		DictSO data = new DictSO();

		foreach (KeyValuePair<string, CustomVar> pair in CustomVarUtil.key2Instance) {
			data.Add(pair.Key, pair.Value.ToMemo());
		}
		
		return data;
	}
	
	/* 讀取Json格式 */
	public static void LoadMemo (object memoJson) {
		DictSO data = DictSO.Json(memoJson);

		// 剩餘名單(未被使用的)
		List<string> left = new List<string>(CustomVarUtil.key2Instance.Keys);

		if (data.ContainsKey("key2Instance")) {

			DictSO key2Instance_data = data.GetDictSO("key2Instance");

			foreach (KeyValuePair<string, object> pair in key2Instance_data) {
				string key = pair.Key;
				object instanceMemo = pair.Value;

				// 取得實例 並 讀取
				CustomVar instance = CustomVarUtil.Inst(key);
				instance.LoadMemo(instanceMemo);

				// 從剩餘名單中移除
				if (left.Contains(key)) {
					left.Remove(key);
				}

			}
		}
		// 移除未被使用的實例
		foreach (string key in left) {
			CustomVarUtil.Del(key);
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