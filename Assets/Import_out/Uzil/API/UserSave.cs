using Uzil;

using UzUserSave = Uzil.UserData.UserSave;


namespace UZAPI {

public class UserSave {

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
		return UzUserSave.main.IsExist(path, key);
	}

	/** 設置 字串 */
	public static bool SetStr (string path, string key, string val) {
		return UzUserSave.main.SetStr(path, key, val);
	}

	/** 設置 數字 */
	public static bool SetNum (string path, string key, float val) {
		return UzUserSave.main.SetNum(path, key, val);
	}

	/** 設置 布林 */
	public static bool SetBool (string path, string key, bool val) {
		return UzUserSave.main.SetBool(path, key, val);
	}

	/** 設置 物件 */
	public static bool SetObj (string path, string key, string val_json) {
		return UzUserSave.main.SetObj(path, key, DictSO.Json(val_json));
	}

	/** 設置 物件 */
	public static bool SetList (string path, string key, string val_json) {
		return UzUserSave.main.SetList(path, key, DictSO.List<object>(val_json));
	}
	
	/** 取得 字串 */
	public static string GetStr (string path, string key) {
		return UzUserSave.main.GetStr(path, key);
	}

	/** 取得 數字 */
	public static int? GetInt (string path, string key) {
		return UzUserSave.main.GetInt(path, key);
	}

	/** 取得 數字 */
	public static double? GetFloat (string path, string key) {
		return UzUserSave.main.GetFloat(path, key);
	}

	/** 取得 布林 */
	public static bool? GetBool (string path, string key) {
		return UzUserSave.main.GetBool(path, key);
	}

	/** 移除 */
	public static bool Remove (string path, string key) {
		return UzUserSave.main.Remove(path, key);
	}

	/** 刪除 */
	public static bool Delete (string path) {
		return UzUserSave.main.Delete(path);
	}

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}

}