using UnityEngine;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Uzil.Lua;
using Uzil.Res;
using Uzil.Values;
using Uzil.BuiltinUtil;

namespace Uzil.Mod {

public class ModRes {
	public ModRes (string mod, string path) {
		this.mod = mod;
		this.path = path.Replace("\\", "/");
	}
	public string mod;
	public string path;
}

public class ModMgr {
	public static bool isDebug = false;
	private static void log (object msg) {
		if (ModMgr.isDebug) {
			Debug.Log(msg);
		}
	}

	/*======================================Constructor==========================================*/

	public ModMgr () {
		this.Init();
	}

	/*=====================================Static Members========================================*/

	/** 單例 */
	public static ModMgr _instance;

	/*=====================================Static Funciton=======================================*/
	
	/** 單例 */
	public static ModMgr Inst () {
		if (ModMgr._instance == null) ModMgr._instance = new ModMgr();
		return ModMgr._instance;
	}


	/** 取得模組後的檔案(byte[]) */
	public static async Task<byte[]> GetResAsync (string path, ModResType type) {

		// 準備 讀取 任務
		Task<byte[]> task = Task.Run<byte[]>(()=>{
			return ModMgr.GetRes(path, type);
		});

		// 是否已經取消
		bool isCancel = false;

		// 註冊 當 離開應用程式 設為取消
		Event evt = ApplicationEvent.onApplicationQuit;
		EventListener listener = evt.Add(()=>{
			isCancel = true;
		});

		// 執行並等待 讀取任務 完成
		byte[] res = await task;
		
		// 移除 當離開應用程式 事件
		evt.RemoveListener(listener);

		// 若已經取消 則 返回 空
		if (isCancel) return null;

		return res;
	}

	/** 取得模組後的檔案(byte[]) */
	public static byte[] GetRes (string path, ModResType type) {
		string[] format = new string[]{""};
		switch (type){
			case ModResType.Text:
				format = ResUtil.textFormat;
				break;
			case ModResType.Texture:
				format = ResUtil.textureFormat;
				break;
			case ModResType.Audio:
				format = ResUtil.audioFormat;
				break;
			case ModResType.Lua:
				format = ResUtil.luaFormat;
				break;
		}
		return ModMgr.GetRes(path, format);
	}

	/** 取得模組後的檔案(byte[]) */
	public static byte[] GetRes (string path, string[] ext) {
		if (path == null || path == "") {
			if (ModMgr.isDebug) log("[ModMgr]: path is null or \"\"");
			return null;
		}

		ModMgr modMgr = ModMgr.Inst();

		path = path.Replace("\\", "/");
		
		// 嘗試 以不同副檔名 取出 資源
		ModRes res = null;
		for (int i = 0; i < ext.Length; i++) {
			res = modMgr.GetModRes(path+ext[i]);			
			if (res != null) break;
		}

		// 若 都不存在 則 返回 空
		if (res == null) {
			if (ModMgr.isDebug) log("[ModMgr]: GetModRes Fail : "+path);
			return null;
		}

		if (ModMgr.isDebug) log("[ModMgr]: GetModRes Success : "+path);

		// 取得 該資源的Mod資訊
		ModInfo modInfo = ModInfo.GetMod(res.mod);
		if (modInfo == null) return null;

		// 以 Viewer 從 Mod中 取出檔案
		return ModViewer.GetModFile(modInfo.GetModPath(), res.path);
	}

	/** 將模組檔案加入索引 */
	public static void AddRes (ModRes modRes) {
		ModMgr.Inst().AddModRes(modRes);
	}

	/*=========================================Members===========================================*/

	/** 路徑(索引) 對應的 模塊 */
	private Dictionary<string, Vals> resPath2ModRes = new Dictionary<string, Vals>();

	/** 是否已讀取內建 */
	protected bool isBaseLoaderAdded = false;

	/** 暫存 所有模組列表 */
	protected List<string> totalModList = new List<string>();

	/** 目前已經讀取的 */
	protected List<string> loadedMods = new List<string>(); 

	/** 是否已經讀取過 系統模組 */
	protected bool isSystemModLoaded = false;

	/*========================================Components=========================================*/
	
	/** 讀取模塊 */
	public List<ModLoader> loaderList = new List<ModLoader>();

	/*==========================================Event============================================*/

	/** 當 所有讀取器 已經讀取 */
	public Event onAllLoaded = new Event();

	/*======================================Unity Function=======================================*/
	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	// #### ##    ## #### ######## 
	//  ##  ###   ##  ##     ##    
	//  ##  ####  ##  ##     ##    
	//  ##  ## ## ##  ##     ##    
	//  ##  ##  ####  ##     ##    
	//  ##  ##   ###  ##     ##    
	// #### ##    ## ####    ##    

	/** 初始化 */
	public void Init () {
		ModInfo.Init();
	}

	// ##        #######     ###    ########  ######## ########  
	// ##       ##     ##   ## ##   ##     ## ##       ##     ## 
	// ##       ##     ##  ##   ##  ##     ## ##       ##     ## 
	// ##       ##     ## ##     ## ##     ## ######   ########  
	// ##       ##     ## ######### ##     ## ##       ##   ##   
	// ##       ##     ## ##     ## ##     ## ##       ##    ##  
	// ########  #######  ##     ## ########  ######## ##     ## 

	/** 加入基本Loader */
	public void AddBaseLoader () {
		if (this.isBaseLoaderAdded == false){
			this.AddLoader(new ModLoader_LuaOnUnloaded().Sort(-99));
			this.AddLoader(new ModLoader_File().Sort(1));
			this.AddLoader(new ModLoader_Strings().Sort(2));
			this.AddLoader(new ModLoader_LuaScript().Sort(3));
			this.AddLoader(new ModLoader_Texture().Sort(4));
			this.AddLoader(new ModLoader_Audio().Sort(5));
			this.AddLoader(new ModLoader_Data().Sort(6));
			this.AddLoader(new ModLoader_LuaOnLoaded().Sort(99));
			this.isBaseLoaderAdded = true;
		}
	}

	/** 加入Loader */
	public void AddLoader (ModLoader loader) {
		this.loaderList.Add(loader);
	}
	
	/** 加入Loader */
	public void AddLoader (ModLoader loader, string tag = null) {
		if (tag != null) {
			loader.AddTag(tag);
		}
		this.AddLoader(loader);
	}

	/** 加入Loader */
	public void AddLoader (ModLoader loader, List<string> tags) {
		if (tags != null) {
			foreach (string eachTag in tags){
				loader.AddTag(eachTag);
			}
		}

		this.AddLoader(loader);
	}

	/** 排序Loader */
	public void SortLoader () {
		this.loaderList.Sort((a, b)=>{
			return a.priority - b.priority;
		});
	}

	// ##        #######     ###    ########  
	// ##       ##     ##   ## ##   ##     ## 
	// ##       ##     ##  ##   ##  ##     ## 
	// ##       ##     ## ##     ## ##     ## 
	// ##       ##     ## ######### ##     ## 
	// ##       ##     ## ##     ## ##     ## 
	// ########  #######  ##     ## ########  

	/** 重新讀取 */
	public void ReloadAll () {

		this.unloadMods(this.loadedMods, this.loaderList);

		this.loadSystemMods();
		
		LuaUtil.Reload();

		this.updateTotalModList();
		this.loadMods(this.totalModList, this.loaderList);

		this.onAllLoaded.Call();
	}

	// public void UnloadAll () {
	// 	this.unloadMods(this.loadedMods, this.loaderList);
	// }

	/** 
	 * 重新讀取 特定讀取器
	 * 需用在 沒有Unload行為 的Loader上
	 */
	// public void ReloadPart (string toSearchTag) {
	// 	List<string> list = new List<string>();
	// 	list.Add(toSearchTag);
	// 	this.ReloadPart(list);
	// }

	/** 
	 * 重新讀取 特定讀取器
	 * 需用在 沒有Unload行為 的Loader上
	 */
	// public void ReloadPart (List<string> toSearchTags) {
		
	// 	// 找尋符合tag的Loader============
	// 	List<ModLoader> matchLoaders = new List<ModLoader>();
	// 	foreach (ModLoader eachLoader in this.loaderList) {
	// 		if (matchLoaders.Contains(eachLoader)) continue;

	// 		// 每個 該Loader的tag
	// 		foreach (string tag in eachLoader.GetTags()) {

	// 			// 有符合則加入搜尋結果，並繼續檢查下一個Loader
	// 			if (toSearchTags.Contains(tag)){
	// 				matchLoaders.Add(eachLoader);
	// 				continue;
	// 			}
	// 		}
			
	// 	}

	// 	//===================
	// 	this.loadSystemMods();
	// 	this.updateTotalModList();
	// 	this.loadMods(this.totalModList, matchLoaders);
	// }

	/** 讀取模組 */
	public void LoadMod (ModInfo modInfo, List<ModLoader> loaders = null) {
		if (this.loadedMods.Contains(modInfo.id)) return;

		string path = modInfo.GetModPath();

		if (ModMgr.isDebug) log("[ModMgr]: start load : "+path); 

		// 若 沒指定 讀取器 則 指定 預設
		if (loaders == null) loaders = this.loaderList;

		// 讓每個 讀取模塊 讀取模組包===================
		foreach (ModLoader each in loaders) {
			each.Load(modInfo);
		}
		
		if (ModMgr.isDebug) log("[ModMgr]: loaded : "+path);

		this.loadedMods.Add(modInfo.id);
	}

	/** 取得 已讀取 中的 排序 */
	public int GetIndexOfLoaded (string id) {
		return this.loadedMods.IndexOf(id);
	}

	/** 取得 已讀取 的 數量 */
	public int GetLoadedCount () {
		return this.loadedMods.Count;
	}

	// ########  ########  ######  
	// ##     ## ##       ##    ## 
	// ##     ## ##       ##       
	// ########  ######    ######  
	// ##   ##   ##             ## 
	// ##    ##  ##       ##    ## 
	// ##     ## ########  ######  

	/** 以路徑索引 取得 實際模組檔案資訊 */
	public ModRes GetModRes (string path) {
		path = path.Replace("\\", "/");

		if (ModMgr.isDebug) {
			foreach (KeyValuePair<string, Vals> each in this.resPath2ModRes){
				log("[ModMgr]: each idx : "+each.Key);
			}
		}

		// 若 不存在 則 回傳 空
		if (!this.resPath2ModRes.ContainsKey(path)) {
			return null;
		}
		// 取得
		return (ModRes) this.resPath2ModRes[path].GetCurrent();
	}

	/** 加入 模組資源索引 */
	public void AddModRes (ModRes modRes, ModInfo modInfo = null) {
		if (ModMgr.isDebug) log("[ModMgr]: AddModRes : "+modRes.path);

		// 若 沒有指定 模組資訊 則 從 資源中找尋
		if (modInfo == null) {
			modInfo = ModInfo.GetMod(modRes.mod);
		}

		string idxPath = modRes.path;

		// 取得 路徑 對應的 多重數值
		Vals modResVals;
		if (this.resPath2ModRes.ContainsKey(idxPath)) {
			modResVals = this.resPath2ModRes[idxPath];
		} else {
			modResVals = new Vals(null);
			this.resPath2ModRes.Add(idxPath, modResVals);
		}

		// 以 列表中的順序決定 覆蓋優先權 (列表中越後，覆蓋權越大)
		int priority = this.totalModList.Count - this.totalModList.IndexOf(modInfo.id);

		// 設置 多重數值
		modResVals.Set(modInfo.id, priority, modRes);
	}

	/** 移除 模組資源索引 */
	public void RemoveModRes (ModInfo modInfo) {
		List<string> toRm = new List<string>();

		// 每一個 路徑 對應 模組資源(多重數值)
		foreach (KeyValuePair<string, Vals> pair in this.resPath2ModRes) {
			
			// 從 多重數值中 移除 該user(模組ID)
			Vals vals = pair.Value;
			vals.Remove(modInfo.id);

			// 若 已經沒有User 則 預計移除
			if (vals.GetCount() == 0) {
				toRm.Add(pair.Key);
			}
		}

		// 移除用不到的多重數值
		foreach (string each in toRm) {
			this.resPath2ModRes.Remove(each);
		}
	}

	
	/** 回傳找到路徑的所有副檔名 */
	public List<string> FindModResExt (string path) {
		List<string> allExt = new List<string>();
		if (path == null || path == "") return allExt;
		
		ModRes res = null;

		Action<string[]> doAct = (extArray) => {
			foreach (string eachExt in extArray){

				if (eachExt != "" && path.EndsWith(eachExt)) {
					res = this.GetModRes(path);
				} else {
					res = this.GetModRes(path + eachExt);
				}

				if (res != null) allExt.Add(eachExt);
			}
		};

		doAct(ResUtil.audioFormat);
		doAct(ResUtil.luaFormat);
		doAct(ResUtil.textureFormat);
		doAct(ResUtil.textFormat);
		
		return allExt;
	}

	/** 列出所有記錄的路徑與對應的模組名稱 */
	public void LogRes () {
		Debug.Log("[ModMgr]: =====LogRes Begin=====");
		foreach (KeyValuePair<string, Vals> eachPair in this.resPath2ModRes){
			object valCur = eachPair.Value.GetCurrent();
			Debug.Log("[ModMgr]: res["+eachPair.Key+"] in ["+(valCur as ModRes).mod+"]");
		}
		Debug.Log("[ModMgr]: =====LogRes End=====");
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

	/** 讀取 模組 */
	protected void loadMods (List<string> modList, List<ModLoader> loaderList) {
		
		for (int idx = 0; idx < modList.Count; idx++) {
			string eachMod = modList[idx];
			ModInfo modInfo = ModInfo.GetMod(eachMod);

			if (modInfo == null) continue;
			
			// 呼叫讀取模組 (限定讀取器)
			this.LoadMod(modInfo, loaderList);
		}
	}

	/** 讀取 模組 */
	protected void unloadMod (ModInfo modInfo, List<ModLoader> loaders = null) {
		if (this.loadedMods.Contains(modInfo.id) == false) return;

		// 若為系統Mod 則 忽略
		if (Array.IndexOf(ModUtil.SYSTEM_MODLIST, modInfo.id) != -1) return;

		// 若 沒指定 讀取器 則 指定 預設
		if (loaders == null) loaders = this.loaderList;

		for (int idx = loaders.Count-1; idx >= 0; idx--) {
			ModLoader each = loaders[idx];
			each.Unload(modInfo);
		}

		// 內建卸載
		this.RemoveModRes(modInfo);

		this.loadedMods.Remove(modInfo.id);
	}
	protected void unloadMods (List<string> modList, List<ModLoader> loaderList) {
		
		// 暫存 系統Mod 列表
		string[] systemModList = ModUtil.SYSTEM_MODLIST;

		// 每一個Mod
		for (int idx = modList.Count-1; idx >= 0; idx--) {
			string eachMod = modList[idx];

			// 若為系統Mod 則 忽略
			if (Array.IndexOf(systemModList, eachMod) != -1) continue;

			ModInfo modInfo = ModInfo.GetMod(eachMod);
			
			// 呼叫卸載模組 (限定讀取器)
			this.unloadMod(modInfo, loaderList);
		}
	}

	/** 讀取系統模組 */
	// 系統模組不能涉及讀取行為，僅能提供檔案
	protected void loadSystemMods () {
		this.isSystemModLoaded = true;

		// 讀取 系統模組
		string[] systemModList = ModUtil.SYSTEM_MODLIST;
		for (int idx = 0; idx < systemModList.Length; idx++) {
			string eachMod = systemModList[idx];
			ModInfo modInfo = ModInfo.GetMod(eachMod);
			this.LoadMod(modInfo);
		}
	}

	/**
	 * 更新 所有需要讀取的模組
	 * 加入順序為 設定檔中的優先加入，其次是可取得到的所有Mod，再來才是必備Mod。若前者已經有設置則不重複設置。
	 * 加入後的排序為 必備Mod、從設定檔加入的、其他Mod
	 */
	protected void updateTotalModList () {

		List<string> modList = new List<string>();

		List<ModInfo> modInfos = ModInfo.GetMods();
		List<ModCfgData> modCfgs = ModUtil.LoadModsConfig();

		Dictionary<string, ModCfgData> id2ModCfg = new Dictionary<string, ModCfgData>();

		// 每個 設定檔
		foreach (ModCfgData modCfg in modCfgs) {
			// 加入索引
			if (id2ModCfg.ContainsKey(modCfg.id)) {
				id2ModCfg[modCfg.id] = modCfg;
			} else {
				id2ModCfg.Add(modCfg.id, modCfg);
			}

			// 若 已存在 則 忽略
			if (modList.Contains(modCfg.id)) continue;

			// 取得 模組資訊
			ModInfo modInfo = ModInfo.GetMod(modCfg.id);
			if (modInfo == null) continue;

			// 試著加入列表
			this.tryAddModList(modList, modInfo, modCfg);
		}


		// 每一個模組
		for (int idx = 0; idx < modInfos.Count; idx++) {
			ModInfo modInfo = modInfos[idx];

			// 若 已存在 則 忽略
			if (modList.Contains(modInfo.id)) continue;

			ModCfgData modCfg = null;
			if (id2ModCfg.ContainsKey(modInfo.id)) {
				modCfg = id2ModCfg[modInfo.id];
			}

			// 試著加入列表
			this.tryAddModList(modList, modInfo, modCfg);
		}

		// 檢查 並 處理 必需模組
		string[] requiredModList = ModUtil.REQUIRED_MODLIST;
		for (int idx = requiredModList.Length-1; idx >= 0; idx--) {
			
			string each = requiredModList[idx];
			
			if (modList.Contains(each) == false) {
				modList.Insert(0, each);
			}
		}

		this.totalModList = modList;
	}

	protected void tryAddModList (List<string> list, ModInfo modInfo, ModCfgData modCfg) {
		
		// 依照 平台類別 ==========
		switch (modInfo.platformType) {

			// 本地Mod			
			case ModPlatformType.Local:
			// Steam工作坊
			case ModPlatformType.Steam:
				// 若 設定檔 存在 且 關閉 則 不加入
				if (modCfg != null && modCfg.isActive == false) {
					return;
				}
				break;

			case ModPlatformType.Err:
				return;
		}

		list.Add(modInfo.id);
	}


}



}