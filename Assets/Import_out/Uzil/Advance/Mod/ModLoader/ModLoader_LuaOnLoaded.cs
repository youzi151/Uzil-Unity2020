using System;

using UnityEngine;

using Uzil.Res;
using Uzil.Lua;

namespace Uzil.Mod {

public class ModLoader_LuaOnLoaded : ModLoader {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/
	
	/** 讀取 */
	public override void Load (ModInfo modInfo) {
		if (modInfo.GetTags().Contains(ModInfo.TAG_FileOnly)) return;

		string modPath = modInfo.GetModPath();

		// 執行 模組 的 讀取 腳本====================
		// 備註: 因為模組檔可能是壓縮包，所以要 用ModuleViewr來取得資源
		string script = ResUtil.text.Create(ModViewer.GetModFile(modPath, "onLoaded.lua"));
		if (script != null) {
			try {
				LuaUtil.DoString(script);
			} catch(Exception e) {
				Debug.Log("[ModLoader_LuaOnUnloaded]: Run onLoaded Script fail"+" in "+modPath);
				Debug.Log(e);
			}
		}

	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}

}