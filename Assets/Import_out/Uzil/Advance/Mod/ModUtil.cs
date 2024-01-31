using System.Collections.Generic;

using UnityEngine;

using Uzil.UserData;

using Uzil.Util;

namespace Uzil.Mod {


/** 模組種類：錯誤、資料夾、zip封裝 */
public enum ModLoadType {
	Err, Dir, Mod
}

/** 模組種類：錯誤、資料夾、zip封裝 */
public enum ModPlatformType {
	Err, Local, Steam
}


/** 資源類別 */
public enum ModResType {
	Text, Texture, Audio, Lua, None
}

public class ModCfgData {
	public string id = null;
	public bool? isActive = null;

	public ModCfgData (DictSO data) {
		if (data == null) return;
		
		data.TryGetString("id", (res)=>{
			this.id = res;
		});
		data.TryGetBool("active", (res)=>{
			this.isActive = res;
		});
	}
}

public class ModUtil {

	/** 解壓縮密碼 */
	public static string ZIP_PASSWORD = "Example";
	protected static bool isZippw_generated = false;

	/** 取得 壓縮密碼 */
	public static string GetZipPassword () {
		return ModUtil.ZIP_PASSWORD
	}

	
	/** 模組路徑 */
	public static string MOD_ROOT_PATH {
		get {
			return PathUtil.Combine(PathUtil.GetDataPath(), "Mods");
		}
	}

	/** 模組列表 */
	public static readonly string[] MOD_EXTENSIONS = { ".mod" };

	/** 是否為模組副檔名 */
	public static bool IsModExtension (string path) {
		for (int idx = 0; idx < ModUtil.MOD_EXTENSIONS.Length; idx++) {
			string ext = ModUtil.MOD_EXTENSIONS[idx];
			if (path.EndsWith(ext)) return true;
		}
		return false;
	}

	/** 模組列表 */
	public const string MODLIST_CONFIG_NAME = "mods.cfg";

	/** 在模組包中 音效檔案 的 路徑 */
	public const string AUDIO_FOLDERNAME = "Audio";

	/** 在模組包中 資料檔案 的 路徑 */
	public const string DATA_FOLDERNAME = "Data";

	/** 在模組包中 貼圖檔案 的 路徑 */
	public const string TEXTURE_FOLDERNAME = "Texture";

	/** 在模組包中 字串檔案 的 路徑 */
	public const string STRINGS_FOLDERNAME = "Strings";

	/** 在模組包中 Lua檔案 的 路徑 */
	public const string LUASCRIPT_FOLDERNAME = "LuaScript";

	/** 系統 模組包 */
	public static readonly string[] SYSTEM_MODLIST = {"_lua", "_data"};

	/** 必備 模組包 */
	public static readonly string[] REQUIRED_MODLIST = {"_lang", "_main", "_achv", "_audio"};

	/** 讀取 模組設定檔 */
	public static List<ModCfgData> LoadModsConfig () {
		List<ModCfgData> result = new List<ModCfgData>();
		
		List<DictSO> modList = ProfileSave.main.GetList<DictSO>(ModUtil.MODLIST_CONFIG_NAME, null);
		if (modList == null) return result;

		for (int idx = 0; idx < modList.Count; idx++) {
			DictSO each = modList[idx];
			if (each == null) continue;
			ModCfgData cfgData = new ModCfgData(each);
			result.Add(cfgData);
		}

		return result;
	}

}

}