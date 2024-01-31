using UnityEngine;

using Uzil.Res;
using Uzil.Lua;
using Uzil.Util;

using Uzil.Mod;

namespace UZAPI {

public class Util {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	public static string LUAREQUIRE_ROOT = ModUtil.LUASCRIPT_FOLDERNAME;

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** 供 Lua層 Require 使用 */
	public static void LuaRequire (string path) {
		
		string scriptPath = path;
		string luaScript;

		if (scriptPath.EndsWith(".lua") == false) {
			scriptPath = scriptPath + ".lua";
		}
		luaScript = ResMgr.Get<string>(new ResReq(scriptPath));

		if (luaScript == null) {
			scriptPath = PathUtil.Combine(Util.LUAREQUIRE_ROOT, scriptPath);
		}
		luaScript = ResMgr.Get<string>(new ResReq(scriptPath));

		if (luaScript == null) return;

		LuaUtil.DoString("Global.RequireResTemp = (function()\n"+luaScript+"\nend)();");
	}


	/** 取得 字串 Hash */
	public static int GetStringHashCode (string str) {
		return str.GetHashCode();
	}

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}

}