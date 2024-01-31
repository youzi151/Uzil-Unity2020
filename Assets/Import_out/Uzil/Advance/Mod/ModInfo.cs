using System;
using System.IO;
using System.Collections.Generic;

using UnityEngine;

using Uzil.Res;
using Uzil.Util;

#if STEAM
using Uzil.ThirdParty.Steam;
#endif

namespace Uzil.Mod {

public class ModInfo {

	public const string TAG_FileOnly = "_fileonly";


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	protected static Dictionary<string, ModInfo> id2ModInfo = new Dictionary<string, ModInfo>();

	/*=====================================Static Funciton=======================================*/

	/** 初始化 */
	public static void Init () {
		ModInfo.UpdateModInfos();
	}

	/** 更新 模組資訊 */
	public static void UpdateModInfos () {

		DirectoryInfo modsFolder = new DirectoryInfo(ModUtil.MOD_ROOT_PATH);

		List<string> addedMod = new List<string>();
		List<ModInfo> modInfos = new List<ModInfo>();

		// Local 根目錄/Mod資料夾 =============
		// Local 根目錄/資料夾/封裝檔 =========
		// Local 根目錄/資料夾/Mod資料夾 ======

		// 每個資料夾
		DirectoryInfo[] modFolders = modsFolder.GetDirectories();
		for (int idx_folder = 0; idx_folder < modFolders.Length; idx_folder++) {

			DirectoryInfo modFolder = modFolders[idx_folder];

			ModInfo mainMod = null;
			List<ModInfo> dirMods = new List<ModInfo>();
			List<ModInfo> archiveMods = new List<ModInfo>();

			// 取得 描述檔 =====
			
			// 主要Mod ==
			string mainMetaInfoPath = PathUtil.Combine(modFolder.FullName, "meta.info");
			FileInfo mainMetaFileInfo = new FileInfo(mainMetaInfoPath);
			if (mainMetaFileInfo.Exists) {
				// 讀取 Mod資訊檔
				string txt = File.ReadAllText(mainMetaFileInfo.FullName);
				DictSO modMetaData = DictSO.Json(txt);
				// 若格式有誤 則 忽略
				if (modMetaData == null) continue;

				// 建立 Mod資訊
				mainMod = new ModInfo();
				mainMod.LoadMemo(modMetaData);
				mainMod.platformType = ModPlatformType.Local;
				mainMod.loadType = ModLoadType.Dir;
			}

			// 封裝Mod檔 ==

			// Mod安裝路徑中 每個檔案
			FileInfo[] filesInMod = modFolder.GetFiles();
			for (int idx_file = 0; idx_file < filesInMod.Length; idx_file++) {

				FileInfo fileInfo = filesInMod[idx_file];
				// 若 為 .Mod 
				if (Array.IndexOf(ModUtil.MOD_EXTENSIONS, fileInfo.Extension) == -1) continue;
				// 尋找 Mod資訊檔
				byte[] fileBytes = ModViewer.GetModFile(fileInfo.FullName, "meta.info");
				if (fileBytes == null) continue;

				// 讀取 Mod資訊檔
				string txt = ResUtil.text.Create(fileBytes);
				DictSO modMetaData = DictSO.Json(txt);
				// 若格式有誤 則 忽略
				if (modMetaData == null) continue;

				// 建立 Mod資訊
				ModInfo modInfo = new ModInfo();
				modInfo.LoadMemo(modMetaData);
				modInfo.platformType = ModPlatformType.Local;
				modInfo.loadType = ModLoadType.Mod;
				if (modInfo.path == null) {
					modInfo.path = fileInfo.FullName;
				}

				archiveMods.Add(modInfo);
			}

			// Mod資料夾 ==
			
			// Mod安裝路徑中 每個子目錄
			DirectoryInfo[] dirsInMod = modFolder.GetDirectories();
			for (int idx_dir = 0; idx_dir < dirsInMod.Length; idx_dir++) {

				DirectoryInfo dirInfo = dirsInMod[idx_dir];
				
				// 找尋 Mod資訊檔
				string metaInfoPath = PathUtil.Combine(dirInfo.FullName, "meta.info");
				FileInfo metaFileInfo = new FileInfo(metaInfoPath);

				// 若 不存在 則 忽略
				if (metaFileInfo.Exists == false) continue;

				// 讀取 Mod資訊檔
				string txt = File.ReadAllText(metaFileInfo.FullName);
				DictSO modMetaData = DictSO.Json(txt);
				// 若格式有誤 則 忽略
				if (modMetaData == null) continue;

				// 建立 Mod資訊
				ModInfo modInfo = new ModInfo();
				modInfo.LoadMemo(modMetaData);
				modInfo.platformType = ModPlatformType.Local;
				modInfo.loadType = ModLoadType.Dir;
				if (modInfo.path == null) {
					modInfo.path = dirInfo.FullName;
				}
				// Debug.Log("ADD: "+modInfo.id+" / "+modInfo.path);
				dirMods.Add(modInfo);
			}

			// 依序加入 =============

			if (mainMod != null) {
				// 若 沒有加入過 則 加入
				if (addedMod.Contains(mainMod.id) == false) {
					modInfos.Add(mainMod);
					addedMod.Add(mainMod.id);
				}
			}

			foreach (ModInfo each in dirMods) {
				// 若 已經有加入過 則 忽略
				if (addedMod.Contains(each.id)) continue;
				// 加入
				modInfos.Add(each);
				addedMod.Add(each.id);
			}

			foreach (ModInfo each in archiveMods) {
				// 若 已經有加入過 則 忽略
				if (addedMod.Contains(each.id)) continue;
				// 加入
				modInfos.Add(each);
				addedMod.Add(each.id);
			}

		}


		// Local 根目錄/封裝Mod檔 ============

		// 每個檔案
		FileInfo[] modFiles = modsFolder.GetFiles();
		for (int idx_file = 0; idx_file < modFiles.Length; idx_file++) {
			FileInfo file = modFiles[idx_file];

			// 檢查副檔名
			if (file.Extension != ".mod") continue;

			// 尋找 Mod資訊檔
			byte[] fileBytes = ModViewer.GetModFile(file.FullName, "meta.info");
			if (fileBytes == null) continue;

			// 讀取 Mod資訊檔
			string txt = ResUtil.text.Create(fileBytes);
			DictSO modMetaData = DictSO.Json(txt);
			// 若格式有誤 則 忽略
			if (modMetaData == null) continue;

			// 建立 Mod資訊
			ModInfo modInfo = new ModInfo();
			modInfo.LoadMemo(modMetaData);
			modInfo.platformType = ModPlatformType.Local;
			modInfo.loadType = ModLoadType.Mod;

			if (addedMod.Contains(modInfo.id)) continue;

			modInfos.Add(modInfo);
			addedMod.Add(modInfo.id);
		}

#if STEAM
		
		// SteamUGC =========================
		// SteamUGC Mod資料夾 =============
		// SteamUGC 資料夾/封裝檔 =========
		// SteamUGC 資料夾/Mod資料夾 ======

		if (SteamInst.Inst().isInitialized) {

			List<SteamUGCItemInfo> ugcItemInfos = SteamWorkshop.Inst().GetSubscribeds();

			for (int idx_ugc = 0; idx_ugc < ugcItemInfos.Count; idx_ugc++) {

				SteamUGCItemInfo ugcInfo = ugcItemInfos[idx_ugc];

				if (Directory.Exists(ugcInfo.path) == false) continue;

				// 取得路徑
				DirectoryInfo modFolder = new DirectoryInfo(ugcInfo.path);

				ModInfo mainMod = null;
				List<ModInfo> dirMods = new List<ModInfo>();
				List<ModInfo> archiveMods = new List<ModInfo>();

				// 取得 描述檔 =====
				
				// 主要Mod ==
				string mainMetaInfoPath = PathUtil.Combine(modFolder.FullName, "meta.info");
				FileInfo mainMetaFileInfo = new FileInfo(mainMetaInfoPath);
				if (mainMetaFileInfo.Exists) {
					// 讀取 Mod資訊檔
					string txt = File.ReadAllText(mainMetaFileInfo.FullName);
					DictSO modMetaData = DictSO.Json(txt);
					// 若格式有誤 則 忽略
					if (modMetaData == null) continue;

					// 建立 Mod資訊
					mainMod = new ModInfo();
					mainMod.LoadMemo(modMetaData);
					mainMod.platformType = ModPlatformType.Steam;
					mainMod.loadType = ModLoadType.Dir;
					if (mainMod.path == null) {
						mainMod.path = ugcInfo.path;
					}
				}


				// 封裝Mod檔 ==

				// Mod安裝路徑中 每個檔案
				FileInfo[] filesInMod = modFolder.GetFiles();
				for (int idx_file = 0; idx_file < filesInMod.Length; idx_file++) {

					FileInfo fileInfo = filesInMod[idx_file];
					// 若 為 .Mod 
					if (Array.IndexOf(ModUtil.MOD_EXTENSIONS, fileInfo.Extension) == -1) continue;
					// 尋找 Mod資訊檔
					byte[] fileBytes = ModViewer.GetModFile(fileInfo.FullName, "meta.info");
					if (fileBytes == null) continue;

					// 讀取 Mod資訊檔
					string txt = ResUtil.text.Create(fileBytes);
					DictSO modMetaData = DictSO.Json(txt);
					// 若格式有誤 則 忽略
					if (modMetaData == null) continue;

					// 建立 Mod資訊
					ModInfo modInfo = new ModInfo();
					modInfo.LoadMemo(modMetaData);
					modInfo.platformType = ModPlatformType.Steam;
					modInfo.loadType = ModLoadType.Mod;
					if (modInfo.path == null) {
						modInfo.path = fileInfo.FullName;
					}

					archiveMods.Add(modInfo);
				}

				// Mod資料夾 ==
				
				// Mod安裝路徑中 每個子目錄
				DirectoryInfo[] dirsInMod = modFolder.GetDirectories();
				for (int idx_dir = 0; idx_dir < dirsInMod.Length; idx_dir++) {

					DirectoryInfo dirInfo = dirsInMod[idx_dir];
					

					// 找尋 Mod資訊檔
					string metaInfoPath = PathUtil.Combine(dirInfo.FullName, "meta.info");
					FileInfo metaFileInfo = new FileInfo(metaInfoPath);

					// 若 不存在 則 忽略
					if (metaFileInfo.Exists == false) continue;

					// 讀取 Mod資訊檔
					string txt = File.ReadAllText(metaFileInfo.FullName);
					DictSO modMetaData = DictSO.Json(txt);
					// 若格式有誤 則 忽略
					if (modMetaData == null) continue;

					// 建立 Mod資訊
					ModInfo modInfo = new ModInfo();
					modInfo.LoadMemo(modMetaData);
					modInfo.platformType = ModPlatformType.Steam;
					modInfo.loadType = ModLoadType.Dir;
					if (modInfo.path == null) {
						modInfo.path = dirInfo.FullName;
					}
					// Debug.Log("ADD: "+modInfo.id+" / "+modInfo.path);
					dirMods.Add(modInfo);
				}

				// 依序加入 =============

				if (mainMod != null) {
					// 若 沒有加入過 則 加入
					if (addedMod.Contains(mainMod.id) == false) {
						modInfos.Add(mainMod);
						addedMod.Add(mainMod.id);
					}
				}

				foreach (ModInfo each in dirMods) {
					// 若 已經有加入過 則 忽略
					if (addedMod.Contains(each.id)) continue;
					// 加入
					modInfos.Add(each);
					addedMod.Add(each.id);
				}

				foreach (ModInfo each in archiveMods) {
					// 若 已經有加入過 則 忽略
					if (addedMod.Contains(each.id)) continue;
					// 加入
					modInfos.Add(each);
					addedMod.Add(each.id);
				}
			}

		}

#endif

		// 加入 所有Mod資訊
		foreach (ModInfo modInfo in modInfos) {
			ModInfo.AddMod(modInfo);
		}

	}

	/** 加入 所有模組資訊 */
	public static void AddMod (ModInfo mod) {
		if (ModInfo.id2ModInfo.ContainsKey(mod.id)) {
			ModInfo exist = ModInfo.id2ModInfo[mod.id];
			exist.isObsoleted = true;
			ModInfo.id2ModInfo[mod.id] = mod;
		} else {
			ModInfo.id2ModInfo.Add(mod.id, mod);
		}
	}

	/** 取得 模組資訊 */
	public static ModInfo GetMod (string id) {
		if (id == null) return null;
		if (ModInfo.id2ModInfo.ContainsKey(id) == false) {
			Debug.Log("[ModInfo] mod:"+id+" not exist.");
			return null;
		}
		return ModInfo.id2ModInfo[id];
	}

	/** 取得 所有模組資訊 */
	public static List<ModInfo> GetMods () {
		ModInfo[] modInfos_arr = new ModInfo[ModInfo.id2ModInfo.Values.Count];
		ModInfo.id2ModInfo.Values.CopyTo(modInfos_arr, 0);
		List<ModInfo> modInfos = new List<ModInfo>(modInfos_arr);
		return modInfos;
	}

	/*=========================================Members===========================================*/

	/** 名稱 */
	public string id = null;

	/** 版本 */
	public string version = "";

	/** 路徑 */
	public string path {
		set { this._path = value.Replace("\\", "/"); }
		protected get { return this._path; }
	}
	protected string _path = null;

	/* 標籤 */
	protected List<string> tags = new List<string>();

	/** 平台類型 */
	public ModPlatformType platformType = ModPlatformType.Err;

	/** 讀取類型 */
	public ModLoadType loadType = ModLoadType.Err;

	/** 是否 廢棄 */
	public bool isObsoleted = false;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	public void LoadMemo (DictSO data) {
		data.TryGetString("id", (res)=>{
			this.id = res;
		});

		data.TryGetString("version", (res)=>{
			this.version = res;
		});

		data.TryGetString("path", (res)=>{
			this.path = res;
		});

		data.TryGetList<string>("tags", (res)=>{
			this.tags = res;
		});
	}
	
	/** 取得標籤 */
	public List<string> GetTags () {
		return this.tags;
	}

	/** 加入標籤 */
	public void AddTag (string tag) {
		if (this.tags.Contains(tag)) return;
		this.tags.Add(tag);
	}

	
	/** 取得模組路徑 (遊戲資料目錄/Module/模組名稱modName)  */
	public string GetModPath () {

		// 若 有指定其他 則 直接返回
		if (this.path != null) return this.path;

		// 檢查並區分 模組的種類
		string fullPath = PathUtil.Combine(ModUtil.MOD_ROOT_PATH, this.id);

		switch (this.loadType) {
			case ModLoadType.Mod:
				fullPath += ".mod";
				break;
			case ModLoadType.Dir:
				break;
			case ModLoadType.Err:
				Debug.Log("[ModInfo] Err: "+fullPath);
				return null;
		}

		fullPath = fullPath.Replace("\\", "/");

		return fullPath;
	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}



}