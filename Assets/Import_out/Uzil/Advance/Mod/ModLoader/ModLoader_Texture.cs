using System;
using System.IO;
using System.Collections.Generic;

using UnityEngine;

using Uzil.Res;
using Uzil.Util;

namespace Uzil.Mod {

public class ModLoader_Texture : ModLoader {


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

		string modPath = modInfo.GetModPath();

		// 每個文件中的
		List<string> texturePathList = this.getAllTexturePath(modPath);
		foreach (string eachPath in texturePathList) {
			ModMgr.AddRes(new ModRes(modInfo.id, eachPath));
		}
	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

	private List<string> getAllTexturePath (string modPath) {
		// string path = modPath + "/" + RConst.TEXTURE_FOLDERNAME;
		// string fullPath = ResUtil.GetDataPath() + "/" + path; 

		List<string> result = new List<string>();

		List<string> array = ModViewer.GetModFilesName(modPath);

		foreach (string filePath in array) {

			if (!filePath.StartsWith(ModUtil.TEXTURE_FOLDERNAME)) continue;

			try {
				FileInfo info = new FileInfo(filePath);

				// 若 不為 合法檔名 則 忽略
				if (Array.IndexOf(ResUtil.textureFormat, info.Extension) == -1) {
					continue;
				}

				// 加入路徑清單
				result.Add(filePath);
				// Debug.Log("[ModLoader_Texture]: loading "+filePath);
				// Debug.Log("Done");
				// Debug.Log(RConst.TEXTURE_FOLDERNAME + "/" + info.Name);

			}
			catch (Exception e) {
				Debug.Log(e);
			}
		}

		return result;
	}

}



}