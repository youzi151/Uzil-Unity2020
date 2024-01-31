using System;
using System.IO;
using System.Collections.Generic;

using UnityEngine;

using Uzil.Util;

namespace Uzil.Mod {

public class ModLoader_File : ModLoader {


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

		DirectoryInfo info = new DirectoryInfo(modPath);

		// 每個文件中的
		List<string> dataPathList = this.getAllFilePath(modPath);
		foreach (string each in dataPathList) {
			ModMgr.AddRes(new ModRes(modInfo.id, each));
		}

	}
	
	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

	private List<string> getAllFilePath (string modPath) {
		List<string> result = new List<string>();


		List<string> array = ModViewer.GetModFilesName(modPath);
		foreach (string filePath in array) {
			try {
				// FileInfo info = new FileInfo(filePath);

				// 加入路徑清單
				result.Add(filePath);

				// Debug.Log("[ModLoader_File]: loading "+filePath);
				// Debug.Log("Done");
				// Debug.Log(RConst.DATA_FOLDERNAME + "/" + info.Name);

			}
			catch (Exception e) {
				Debug.Log(e);
			}
		}

		return result;
	}

}



}