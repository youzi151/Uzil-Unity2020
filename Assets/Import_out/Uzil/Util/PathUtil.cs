using System;
using System.IO;

using UnityEngine;

namespace Uzil.Util {

public class PathUtil {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/


	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	private static string _rootPath = null;

	private static string _dataPath = null;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/
	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*== 取得資訊 =====================*/

	/* 取得根目錄 */
	public static string GetRootPath () {
		if (PathUtil._rootPath != null) {
			return PathUtil._rootPath;
		}

		// 根目錄
		string rootPath;

		/* 編輯中 */
#if   UNITY_EDITOR

		// 與 Assets 同層級的資料夾
		DirectoryInfo info = new DirectoryInfo(Application.dataPath);
		rootPath = info.Parent.FullName;

		/* MAC */
#elif UNITY_STANDALONE_OSX

		// 與 執行檔.app 同層級的資料夾
		DirectoryInfo info = new DirectoryInfo(Application.dataPath);
		rootPath = info.Parent.Parent.FullName;//content -out-> xxx.app -out-> root

		/* Windows */
#elif UNITY_STANDALONE_WIN

		// Application.dataPath的上一層
		// 與 執行檔.exe 同層級的資料夾

		DirectoryInfo info = new DirectoryInfo(Application.dataPath);
		if (info == null) return Application.dataPath;
		if (info.Parent == null) return Application.dataPath;
		if (info.Parent.Parent == null) return Application.dataPath;
		rootPath = info.Parent.Parent.FullName;

#else
		// 若為 移動平台 則 為 Application.persistentDataPath
		rootPath = Application.isMobilePlatform ? Application.persistentDataPath : Application.dataPath;	

#endif

		string res = rootPath+"/";
		res = res.Replace("\\", "/");

		PathUtil._rootPath = res;

		return res;

	}

	/* 取得檔案路徑 */
	public static string GetDataPath () {
		if (PathUtil._dataPath != null) {
			return PathUtil._dataPath;
		}

		// 根目錄
		string rootPath = PathUtil.GetRootPath();
		string dataPath;

		/* 編輯中 */
#if   UNITY_EDITOR

		// Assets上一層
		dataPath = PathUtil.Combine(rootPath, PathUtil.GetRelativeDataPath());
		
		// Uzil測試用
		// dataPath = PathUtil.Combine(rootPath, "Import_out/Uzil/_DataInEditor");

		/* MAC */
#elif UNITY_STANDALONE_OSX

		dataPath = rootPath;

		/* Windows */
#elif UNITY_STANDALONE_WIN

		dataPath = rootPath;

#else
		// 若為移動平台 則 為 Application.persistentDataPath
		dataPath = Application.isMobilePlatform ? Application.persistentDataPath : rootPath;

#endif

		string res = dataPath+"/";

		res = PathUtil.GetAbsolutePath(res);

		res = res.Replace("\\", "/");

		PathUtil._dataPath = res;
		return res;
	}

	public static string GetAssetBundlePath () {
		return PathUtil.Combine(PathUtil.GetDataPath(), "bundles");
	}

	/** 取得 Data資料夾路徑 */
	public static string GetRelativeDataPath () {
		
		string relativeDataPath = "";

		/* 編輯中 */
#if   UNITY_EDITOR

		// Assets上一層
		relativeDataPath = "./AppData";
		
		// Uzil測試用
		// dataPath = PathUtil.Combine(Application.dataPath, "Import_out/Uzil/_DataInEditor");

		/* MAC */
#elif UNITY_STANDALONE_OSX

		/* Windows */
#elif UNITY_STANDALONE_WIN


#else

#endif
		return relativeDataPath;
	}

	/*== 路徑處理 =====================*/

	/* 串聯 */
	public static string Combine (params string[] paths) {
		if (paths.Length == 0) return null;
	
		string res = paths[0];
		if (res.EndsWith("/") == false) {
			res = res + "/";
		}

		for (int idx = 1; idx < paths.Length; idx++) {
			string nextPath = paths[idx];

			if (nextPath == "" || nextPath == null) continue;

			while (nextPath.StartsWith("/")) {
				nextPath = nextPath.Substring(1, nextPath.Length-1);
			}
			while (nextPath.EndsWith("/")) {
				nextPath = nextPath.Substring(0, nextPath.Length-1);
			}

			res += nextPath;
			
			if (idx != (paths.Length-1)) {
				res += "/";
			}
		}

		if (res == "/") return "";

		res = res.Replace("\\", "/");

		return res;
	}

	/* 取得檔案路徑 (忽略副檔名) */
	public static string GetFilePathWithoutExtension(string filePath){
		string dirPath = System.IO.Path.GetDirectoryName(filePath);
		if (dirPath != "" && dirPath != null) dirPath += "/";
		return dirPath + System.IO.Path.GetFileNameWithoutExtension(filePath);
	}

	/* 取得檔案路徑 副檔名 */
	public static string GetExtension(string filePath){
		return System.IO.Path.GetExtension(filePath);
	}

	/* 取得相對路徑 */
	public static string GetRelativePath(string parentPath, string path){
		string relativePath = path;

		relativePath = relativePath.Replace("\\", "/");
		parentPath = parentPath.Replace("\\", "/");
		
		if (parentPath.EndsWith("/")){
			parentPath = parentPath.Substring(0, parentPath.Length-1);
		}

		string[] strs= relativePath.Split(new string[] {parentPath}, StringSplitOptions.None);

		if (strs.Length == 0) {
			return relativePath;
		}

		relativePath = strs[strs.Length-1];

		while (relativePath.StartsWith("/")) {
			relativePath = relativePath.Substring(1, relativePath.Length-1);
		}
		
		if (relativePath.EndsWith(parentPath)) return "";
		if (relativePath == "/") return "";

		return relativePath;
	}

	/** 取得絕對路徑 */
	public static string GetAbsolutePath (string path) {
		return Path.GetFullPath(path);
	}
	public static string GetAbsolutePath (string parentPath, string path) {
		// 斜線防呆
		parentPath = parentPath.Replace("\\", "/");
		path = path.Replace("\\", "/");

		// 完整路徑 為 父路徑+子路徑
		string finalPath = PathUtil.Combine(parentPath, path);

		// 若 子路徑 本身的Root 就是 :/ 則 視為 完整路徑
		string pathRootName = Path.GetPathRoot(path);
		if (pathRootName.EndsWith(":/")) {
			finalPath = path;
		}
		
		// 斜線防呆
		string res = Path.GetFullPath(finalPath).Replace("\\", "/");
		return res;
	}


	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}
