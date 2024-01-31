using Uzil;
using Uzil.Lua;

namespace UZAPI {

public class Callback {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** 執行Callback */
	public static void Call (string tag) {
		Callbacks cbs = Callbacks.main;
		cbs.Call(tag);
	}

	public static void CallWith (string tag, string data_json) {
		Callbacks cbs = Callbacks.main;
		cbs.Call(tag, DictSO.Json(data_json));
	}

	public static bool IsExist (string tag) {
		Callbacks cbs = Callbacks.main;
		return cbs.IsExist(tag);
	}


	/** 執行LuaCallback */
	/** for Lua */
	public static void CallLua (int luaCallbackID, string data_json) {
		LuaUtil.CallFunction("Callback.call", luaCallbackID, data_json);
	}
	/** for C# */
	public static T CallLua_cs<T> (int luaCallbackID, DictSO data = null) {
		return LuaUtil.CallFunction<T>("Callback.call", luaCallbackID, (data == null) ? null : data.ToJson());
	}

	public static void CallLua_cs (int luaCallbackID, DictSO data = null) {
		LuaUtil.CallFunction("Callback.call", luaCallbackID, (data == null) ? null : data.ToJson());
	}

	public static void CallLua_cs_arg (int luaCallbackID, object arg1) {
		LuaUtil.CallFunction("Callback.call", luaCallbackID, arg1);
	}
	public static void CallLua_cs_arg (int luaCallbackID, object arg1, object arg2) {
		LuaUtil.CallFunction("Callback.call", luaCallbackID, arg1, arg2);
	}
	public static void CallLua_cs_arg (int luaCallbackID, object arg1, object arg2, object arg3) {
		LuaUtil.CallFunction("Callback.call", luaCallbackID, arg1, arg2, arg3);
	}

	public static void Remove (int luaCallbackID) {
		LuaUtil.CallFunction("Callback.removeID", luaCallbackID);
	}

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}

}