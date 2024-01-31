using Uzil;

using UzConfig = Uzil.UserData.Config;


namespace UZAPI {

public class Config {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** 是否存在 */
	public static bool IsExist (string path, string key) {
		return UzConfig.main.IsExist(path, key);
	}

	/** 設置 字串 */
	public static bool SetStr (string path, string key, string val) {
		return UzConfig.main.SetStr(path, key, val);
		// Debug.Log("SetStr["+key+"] = "+val);
	}

	/** 設置 數字 */
	public static bool SetNum (string path, string key, float val) {
		return UzConfig.main.SetNum(path, key, val);
		// Debug.Log("SetNum["+key+"] = "+val);
	}

	/** 設置 物件 */
	public static bool SetObj (string path, string key, string val_json) {
		return UzConfig.main.SetObj(path, key, DictSO.Json(val_json));
	}

	/** 設置 物件 */
	public static bool SetList (string path, string key, string val_json) {
		return UzConfig.main.SetList(path, key, DictSO.List<object>(val_json));
	}
	
	/** 取得 字串 */
	public static string GetStr (string path, string key) {
		return UzConfig.main.GetStr(path, key);
	}

	/** 取得 數字 */
	public static int? GetInt (string path, string key) {
		return UzConfig.main.GetInt(path, key);
	}

	/** 取得 數字 */
	public static double? GetFloat (string path, string key) {
		return UzConfig.main.GetFloat(path, key);
	}

	/** 取得 布林 */
	public static bool? GetBool (string path, string key) {
		return UzConfig.main.GetBool(path, key);
	}

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}

}