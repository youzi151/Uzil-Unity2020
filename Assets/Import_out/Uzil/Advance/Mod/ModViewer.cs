using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

using Uzil.Util;

namespace Uzil.Mod {

public class ModViewer {

	public static bool isDebug = false;

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/* 取得所有檔名 */
	public static List<string> GetModFilesName (string modPath) {

		if (ModViewer.IsZip(modPath)) {
			return ModViewer.getFilesNameFromZip(modPath);
		}
		else {
			return ModViewer.getFilesNameFromDir(modPath);
		}
	}

	/* 取得所有檔案 */
	public static Dictionary<string, byte[]> GetModFiles (string modPath, Predicate<string> isExtract = null) {

		//結果
		Dictionary<string, byte[]> result = new Dictionary<string, byte[]>();


		// 若為zip 則 依照條件解壓縮
		if (ModViewer.IsZip(modPath)) {
			result = ZipUtil.UnZipToMemory(modPath, ModUtil.GetZipPassword(), (filePath) => {
				filePath = filePath.Replace("\\", "/");
				//若是資料夾則不解壓縮
				if (filePath.EndsWith("/")) return false;
				return isExtract(filePath);
			});
		}

		// 若為 資料夾 則 依照條件取得檔案
		else {

			if (!Directory.Exists(modPath)) {
				return null;
			}

			// 所有檔案名稱
			string[] array = Directory.GetFiles(modPath, "*", SearchOption.AllDirectories);

			// 每一個檔案名稱
			foreach (string eachFile in array) {
				string filePath = eachFile.Replace("\\", "/");
				// 若是 資料夾 則 不解壓縮
				if (filePath.EndsWith("/")) continue;


				string relativePath = ModViewer.getRelativePath(modPath, filePath);

				if (ModViewer.isDebug) {
					Debug.Log("[ModViewer]: isExtract " + relativePath + " ?");
				}
				// 不符條件 則 忽略
				if (isExtract != null) {
					if (!isExtract(relativePath)) { continue; }
				}

				// 讀進結果中
				try {
					FileInfo info = new FileInfo(filePath);
					if (info.Exists) {
						byte[] file = File.ReadAllBytes(filePath);
						result.Add(relativePath, file);
					}
				}
				catch (Exception e) {
					Debug.Log(e);
					continue;
				}
			}

		}


		return result;
	}

	/* 取得模組內的特定檔案 */
	public static byte[] GetModFile (string modPath, string filePath) {

		if (ModViewer.IsZip(modPath)) {

			Dictionary<string, byte[]> data = ZipUtil.UnZipToMemory(modPath, ModUtil.GetZipPassword(), (fileName) => {
				return (fileName == filePath);
			});

			if (data.Count <= 0) {
				return null;
			}
			else {
				foreach (KeyValuePair<string, byte[]> pair in data) {
					return pair.Value;
				}
			}

		}

		else {
			string fullPath = modPath + "/" + filePath;
			try {
				FileInfo info = new FileInfo(fullPath);
				if (info.Exists) {
					return File.ReadAllBytes(fullPath);
				}

			}
			catch (Exception e) {
				Debug.Log(e);
				return null;
			}
		}

		return null;

	}


	/* 判斷是否為Zip */
	public static bool IsZip (string modPath) {
		FileInfo info = new FileInfo(modPath);
		// Debug.Log("ModViewer: isZip ("+modPath+") info.Extension == "+info.Extension);
		return info.Extension == ".mod";
	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

	/* 取得所有檔名 */
	private static List<string> getFilesNameFromZip (string modPath) {
		List<string> list = ZipUtil.GetFilesName(modPath);
		List<string> removeList = new List<string>();
		for (int i = 0; i < list.Count; i++) {
			if (list[i].EndsWith("/") || list[i].EndsWith("\\")) {
				removeList.Add(list[i]);
			}
			else {
				list[i] = list[i].Replace("\\", "/");
			}
		}
		foreach (string each in removeList) {
			list.Remove(each);
		}

		return list;
	}

	private static List<string> getFilesNameFromDir (string modPath) {
		List<string> result = new List<string>();

		string[] array = Directory.GetFiles(modPath, "*", SearchOption.AllDirectories);
		foreach (string eachFile in array) {
			string filePath = eachFile.Replace("\\", "/");
			// 若是 資料夾 則 不解壓縮
			if (filePath.EndsWith("/")) continue;

			try {

				// 相對路徑
				string relativePath = ModViewer.getRelativePath(modPath, filePath);

				// 加入路徑清單
				result.Add(relativePath);

			}
			catch (Exception e) {
				Debug.Log(e);
			}
		}

		return result;
	}

	private static string getRelativePath (string parentPath, string path) {
		string relativePath = path;
		relativePath = relativePath.Replace(parentPath + "\\", "");
		relativePath = relativePath.Replace(parentPath + "/", "");
		return relativePath;
	}

}



}