using System;
using System.IO;
using System.Collections.Generic;

using UnityEngine;

using Uzil.Lua;
using Uzil.Res;
using Uzil.Util;

namespace Uzil.Mod {

public class ModLoader_LuaLoader : ModLoader {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	public static string[] sortFileNames = { "sort.json", "sortList.json", "sortList" };

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	public virtual string GetLoaderName () {
		return "Lua";
	}

	public virtual string GetRootPath () {
		return "";
	}

	public virtual void LoadLua (string luaScript, string fileName) {
		
	}

	/*=====================================Public Function=======================================*/

	/** 讀取 */
	public override void Load (ModInfo modInfo) {
		if (modInfo.GetTags().Contains(ModInfo.TAG_FileOnly)) return;

		string modPath = modInfo.GetModPath();

		string sortFilePath = null;
		string sortFileName = null;
		List<string> sortedList = null;
		Dictionary<string, string> fileName2Path = new Dictionary<string, string>();

		// 讀取 每個文件中的內容
		Dictionary<string, byte[]> files = ModViewer.GetModFiles(modPath, (filePath) => {
			
			// 若 不在資料夾內 則 略過
			if (!filePath.StartsWith(this.GetRootPath())) return false;

			FileInfo info = new FileInfo(filePath);

			// 若 不為Lua檔名
			if (Array.IndexOf(ResUtil.luaFormat, info.Extension) == -1) {

				//若 為 排序檔
				if (sortFilePath == null && Array.IndexOf(ModLoader_LuaLoader.sortFileNames, info.Name) != -1) {
					sortFilePath = filePath;
					sortFileName = PathUtil.GetFilePathWithoutExtension(new FileInfo(filePath).Name);
					return true;
				}

				return false;
			}

			return true;
		});

		// 加入到
		foreach (KeyValuePair<string, byte[]> pair in files) {
			FileInfo info = new FileInfo(pair.Key);
			string fileName = PathUtil.GetFilePathWithoutExtension(PathUtil.Combine(PathUtil.GetRelativePath(this.GetRootPath(), info.DirectoryName)) + info.Name);
			fileName2Path.Add(fileName, pair.Key);
		}

		// 若 有讀取到 排序檔
		if (sortFilePath != null) {

			// 取出 排序檔
			byte[] sortFileByte = files[sortFilePath];
			fileName2Path.Remove(sortFileName);
			files.Remove(sortFilePath);

			// 解析 為 json、List
			string json = ResUtil.text.Create(sortFileByte);

			if (json != null) {
				sortedList = DictSO.List<string>(json);
			}

		}

		// 取得所有要讀取的檔案Key
		string[] toAddKeys_array = new string[fileName2Path.Keys.Count];
		fileName2Path.Keys.CopyTo(toAddKeys_array, 0);

		List<string> toAddKeys = new List<string>(toAddKeys_array);

		// 先使用有排序過的檔案
		if (sortedList != null) {

			foreach (string eachToUse in sortedList) {
				
				// 取得 要用的檔案
				string toUseName = null;
				// 以 各種Lua檔案格式 嘗試取得
				foreach (string format in ResUtil.luaFormat) {
					if (fileName2Path.ContainsKey(eachToUse + format)) {
						toUseName = eachToUse + format;
						break;
					}
				}
				// 若 無法從排序中取得指定的檔案 則 換下一個檔案
				if (toUseName == null) continue;
				string toUsePath = fileName2Path[toUseName];

				byte[] toUseFile_Byte = files[toUsePath];

				string luaString = ResUtil.text.Create(toUseFile_Byte);
				this.tryDoLua(toUseName, luaString);

				// Debug.Log("Do Sorted lua:"+toUseName);

				// 移除 (因為已經先行使用)
				toAddKeys.RemoveAt(toAddKeys.IndexOf(toUseName));
			}
		}


		// 執行沒有排序的Lua檔案
		foreach (string key in toAddKeys) {
			string toUsePath = fileName2Path[key];
			string luaString = ResUtil.text.Create(files[toUsePath]);
			this.tryDoLua(key, luaString);
		}

	}

	/*===================================Protected Function======================================*/

	protected void tryDoLua (string fileName, string luaString) {
		try {
			this.LoadLua(fileName, luaString);
		} catch (Exception e) {
			Debug.Log("[ModuleLoader_" + this.GetLoaderName() + "] : load err in " + fileName);
			Debug.Log(e);
		}
	}

	// 提供 ScriptDB類 Lua檔 方便用
	protected void importToDB (string dbName, string fileName, string luaScript) {

		string luaString = "local temp = (function () \n "+luaScript+" \n end)(); \n ScriptDB."+dbName+".import(\""+fileName+"\", temp)";

		LuaUtil.DoString(luaString);
		
	}

	/*====================================Private Function=======================================*/

}

}