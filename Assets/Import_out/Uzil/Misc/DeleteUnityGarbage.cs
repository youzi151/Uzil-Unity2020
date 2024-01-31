using System;
using System.IO;

#if UNITY_STANDALONE_WIN
using Microsoft.Win32;
#endif

using UnityEngine;

namespace Uzil.Misc {

public class DeleteUnityGarbage : MonoBehaviour {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/
	
	/** 實例 */
	protected static bool _isInstInit = false;
	protected static DeleteUnityGarbage _inst = null;
	public static DeleteUnityGarbage Inst () {
		if (DeleteUnityGarbage._inst == null) {
			DeleteUnityGarbage._isInstInit = true;
			DeleteUnityGarbage._inst = RootUtil.GetChild("DeleteUnityGarbage", RootUtil.GetMember("Misc")).AddComponent<DeleteUnityGarbage>();
			DeleteUnityGarbage._isInstInit = false;
		}
		return DeleteUnityGarbage._inst;
	}

	/** 是否已經在離開時刪除 */
	protected static bool _isAppQuitAndClear = false;

	/*=====================================Static Funciton=======================================*/

	/** 是否存在 必要資訊 */
	public static bool IsCompanyProductExist () {
		if (Application.companyName == "" || Application.companyName == null) return false;
		if (Application.productName == "" || Application.productName == null) return false;
		return true;
	}

	/** 在離開時刪除 */
	public static void QuitDelete () {
		if (DeleteUnityGarbage._isAppQuitAndClear) return;
		DeleteUnityGarbage._isAppQuitAndClear = true;
		DeleteUnityGarbage.Delete();
	}

	/** 刪除 所有 */
	public static void Delete () {
		DeleteUnityGarbage.DeleteWindowsRegistry();
		DeleteUnityGarbage.DeleteWindowsTempFile();
	}

	/** 刪除 暫存檔案 */
	protected static void DeleteWindowsTempFile () {
#if UNITY_STANDALONE_WIN

		// 防呆
		if (DeleteUnityGarbage.IsCompanyProductExist() == false) return;

		string tempPath = Application.persistentDataPath;
		tempPath = tempPath.Replace("\\", "/");

		// 防呆
		if (tempPath.Contains("AppData/LocalLow") == false) return;
		if (tempPath.Contains(Application.companyName) == false) return;
		if (tempPath.Contains(Application.productName) == false) return;
		
		// 產品 資料夾
		DirectoryInfo productDirInfo = new DirectoryInfo(tempPath);
		if (productDirInfo == null) return;

		// 若 資料夾內有 檔案 或 資料夾 則 返回
		FileInfo[] filesInProduct = productDirInfo.GetFiles();
		DirectoryInfo[] dirsInProduct = productDirInfo.GetDirectories();
		if (filesInProduct.Length > 0 || dirsInProduct.Length > 0) return;

		// 移除 產品 資料夾
		Directory.Delete(productDirInfo.FullName);

		// 單位 資料夾
		DirectoryInfo companyDirInfo = productDirInfo.Parent;
		if (companyDirInfo == null) return;

		// 若 資料夾內有 檔案 或 資料夾 則 返回
		FileInfo[] filesInCompany = companyDirInfo.GetFiles();
		DirectoryInfo[] dirsInCompany = companyDirInfo.GetDirectories();
		if (filesInCompany.Length > 0 || dirsInCompany.Length > 0) return;

		// 移除 單位 資料夾
		Directory.Delete(companyDirInfo.FullName);
#endif
	}

	/** 刪除 登錄檔資訊 */
	protected static void DeleteWindowsRegistry () {
#if UNITY_STANDALONE_WIN

		// 防呆
		if (DeleteUnityGarbage.IsCompanyProductExist() == false) return;

		string companyName = Application.companyName;
		string productName = Application.productName;
		string companyKeyName = @"Software\"+companyName;
		string productKeyName = companyKeyName+@"\"+productName;

		// 取得 登錄檔
		RegistryKey subKey = Registry.CurrentUser.OpenSubKey(productKeyName);
		if (subKey == null) return;
		
		// 是否 為 Unity所屬
		bool isUnityApp = false;

		string[] valNames = subKey.GetValueNames();
		for (int idx = 0; idx < valNames.Length; idx++) {
			string each = valNames[idx];

			// 安全檢查
			if (each.Contains("unity") || each.Contains("Screenmanager Fullscreen mode_")) {
				isUnityApp = true;
				break;
			}
		}

		if (!isUnityApp) return;
		
		Registry.CurrentUser.DeleteSubKeyTree(productKeyName);

		if (Registry.CurrentUser.OpenSubKey(companyKeyName).GetSubKeyNames().Length == 0) {
			Registry.CurrentUser.DeleteSubKey(companyKeyName);
		}
#endif
	}

	/*=========================================Members===========================================*/

	/** 是否 呼叫任何 */
	protected bool _isCallAny = false;

	/** 是否 呼叫刪除所有 */
	protected bool _isCall_Delete = false;

	/** 是否 呼叫刪除暫存檔案 */
	protected bool _isCall_DeleteWindowsTempFile = false;

	/** 是否 呼叫刪除登錄檔 */
	protected bool _isCall_DeleteWindowsRegistry = false;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	void Awake () {
		// 禁用 非 Inst() 建立
		if (DeleteUnityGarbage._isInstInit == false) {
			UnityEngine.Object.Destroy(this);
			return;
		}
	}

	void Update () {
		if (this._isCallAny == false) return;
		this._isCallAny = false;

		if (this._isCall_Delete) {
			this._isCall_Delete = false;
			this.Clear(false);
			return;
		}

		if (this._isCall_DeleteWindowsTempFile) {
			this._isCall_DeleteWindowsTempFile = false;
			this.ClearWindowsTempFile(false);
		}

		if (this._isCall_DeleteWindowsRegistry) {
			this._isCall_DeleteWindowsRegistry = false;
			this.ClearWindowsRegistry(false);
		}
	}

	void OnApplicationQuit () {
		DeleteUnityGarbage.QuitDelete();
	}
	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 初始化 */
	public void Init () {
		this.Clear(false);
	}

	public void Clear (bool isSafeMode = true) {
		if (isSafeMode) {
			this._isCallAny = true;
			this._isCall_Delete = true;
			return;
		}

		DeleteUnityGarbage.Delete();
	}

	/** 刪除 Windows暫存檔 */
	public void ClearWindowsTempFile (bool isSafeMode = true) {
#if UNITY_STANDALONE_WIN
		if (isSafeMode) {
			this._isCallAny = true;
			this._isCall_DeleteWindowsTempFile = true;
			return;
		}
		
		DeleteUnityGarbage.DeleteWindowsTempFile();
#endif
	}

	/** 刪除 Windows登錄檔 */
	public void ClearWindowsRegistry (bool isSafeMode = true) {
#if UNITY_STANDALONE_WIN
		if (isSafeMode) {
			this._isCallAny = true;
			this._isCall_DeleteWindowsRegistry = true;
			return;
		}

		DeleteUnityGarbage.DeleteWindowsRegistry();
#endif
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}

}