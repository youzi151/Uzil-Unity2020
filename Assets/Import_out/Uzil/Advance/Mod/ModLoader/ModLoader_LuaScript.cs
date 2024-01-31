using System;
using System.IO;
using System.Collections.Generic;

using UnityEngine;

using Uzil.Res;
using Uzil.Util;

namespace Uzil.Mod {

public class ModLoader_LuaScript : ModLoader {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	public readonly string[] LUAFILE_EXTENSION = { ".lua" };

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 讀取 */
	public override void Load (ModInfo modInfo) {

		string modPath = modInfo.GetModPath();

		// 每個文件中的
		List<string> luaPathList = this.getAllLuaPath(modPath);
		foreach (string each in luaPathList) {
			ModMgr.AddRes(new ModRes(modInfo.id, each));
		}

	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

	private List<string> getAllLuaPath (string modPath) {
		// string path = modPath + "/" + Const_Mod.LUASCRIPT_FOLDERNAME;
		// string fullPath = ResUtil.GetDataPath() + "/" + path; 

		List<string> result = new List<string>();

		List<string> array = ModViewer.GetModFilesName(modPath);

		foreach (string filePath in array) {
			if (!filePath.StartsWith(ModUtil.LUASCRIPT_FOLDERNAME)) continue;

			try {
				FileInfo info = new FileInfo(filePath);

				// 若不為 合法檔名
				if (Array.IndexOf(ResUtil.luaFormat, info.Extension) == -1) {
					continue;
				}

				// 加入路徑清單
				result.Add(filePath);

				// Debug.Log("[ModLoader_LuaScript]: loading "+filePath);
				// Debug.Log("Done");
				// Debug.Log(Const_Mod.LUASCRIPT_FOLDERNAME + "/" + info.Name);

			}
			catch (Exception e) {
				Debug.Log(e);
			}
		}

		return result;
	}

}



}