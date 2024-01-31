using Uzil;

namespace UZAPI {

public class Var {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** 是否存在 */
	public static bool IsExist (string instanceKey, string key) {
		CustomVar customVar = CustomVarUtil.Inst(instanceKey);
		if (customVar == null) return false;
		if (customVar.ContainsFloat(key) || customVar.ContainsStr(key) || customVar.ContainsBool(key)) return true;
		return false;
	}

	/** 設置變數 */
	public static void SetFloat (string instanceKey, string key, float val) {
		CustomVar customVar = CustomVarUtil.Inst(instanceKey);
		if (customVar == null) return;
		customVar.SetFloat(key, val);
		// Debug.Log("SetFloat["+key+"] = "+val);
	}
	public static void SetStr (string instanceKey, string key, string val) {
		CustomVar customVar = CustomVarUtil.Inst(instanceKey);
		if (customVar == null) return;
		customVar.SetStr(key, val);
		// Debug.Log("SetStr["+key+"] = "+val);
	}
	public static void SetBool (string instanceKey, string key, bool val) {
		CustomVar customVar = CustomVarUtil.Inst(instanceKey);
		if (customVar == null) return;
		customVar.SetBool(key, val);
		// Debug.Log("SetStr["+key+"] = "+val);
	}

	/** 取得變數 */
	public static double GetInt (string instanceKey, string key) {
		CustomVar customVar = CustomVarUtil.Inst(instanceKey);
		if (customVar == null) return 0f;
		return customVar.GetInt(key);
	}
	public static double GetFloat (string instanceKey, string key) {
		CustomVar customVar = CustomVarUtil.Inst(instanceKey);
		if (customVar == null) return 0f;
		return customVar.GetFloat(key);
	}
	public static string GetStr (string instanceKey, string key) {
		CustomVar customVar = CustomVarUtil.Inst(instanceKey);
		if (customVar == null) return null;
		return customVar.GetStr(key);
	}
	public static bool GetBool (string instanceKey, string key) {
		CustomVar customVar = CustomVarUtil.Inst(instanceKey);
		if (customVar == null) return false;
		return customVar.GetBool(key);
	}

	/** 移除變數 */
	public static void DelInt (string instanceKey, string key) {
		CustomVar customVar = CustomVarUtil.Inst(instanceKey);
		if (customVar == null) return;
		customVar.DelInt(key);
	}
	public static void DelFloat (string instanceKey, string key) {
		CustomVar customVar = CustomVarUtil.Inst(instanceKey);
		if (customVar == null) return;
		customVar.DelFloat(key);
	}
	public static void DelStr (string instanceKey, string key) {
		CustomVar customVar = CustomVarUtil.Inst(instanceKey);
		if (customVar == null) return;
		customVar.DelStr(key);
	}
	public static void DelBool (string instanceKey, string key) {
		CustomVar customVar = CustomVarUtil.Inst(instanceKey);
		if (customVar == null) return;
		customVar.DelBool(key);
	}
	public static void Del (string instanceKey, string key) {
		CustomVar customVar = CustomVarUtil.Inst(instanceKey);
		if (customVar == null) return;
		customVar.DelFloat(key);
		customVar.DelStr(key);
		customVar.DelBool(key);	
	}

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}

}