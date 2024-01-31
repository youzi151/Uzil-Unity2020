using System;
using System.IO;
using System.Collections.Generic;

using Uzil.Res;
using Uzil.i18n;
using Uzil.Values;

namespace Uzil.Mod {

/**
 * 因為Strings的行為比較單純，僅有 關鍵字與字串 的取用，所以不用Vals來解決各Mod間的 衝突處理。
 * 改以
 */

public class ModLoader_Strings : ModLoader {


	/*======================================Constructor==========================================*/

	public ModLoader_Strings () {
		// 註冊事件 當語言切換
		if (this.onLanguageChanged == null) {
			this.onLanguageChanged = new EventListener(() => {

				// 卸載 所有目前已使用的Mod與關鍵字列表
				string[] mods = new string[this.mod2UsedKeywords.Count];
				this.mod2UsedKeywords.Keys.CopyTo(mods, 0);

				// 倒序 卸載 目前已使用的Mods
				for (int idx = mods.Length-1; idx >= 0; idx--) {
					ModInfo modInfo = ModInfo.GetMod(mods[idx]);
					if (modInfo == null) continue;
		
					this.Unload(modInfo);
				}
				
				// 讀取 目前使用的Mods
				List<ModInfo> modInfos = ModInfo.GetMods();
				for (int idx = 0; idx < modInfos.Count; idx++) {
					ModInfo modInfo = modInfos[idx];
					if (modInfo == null) continue;
					this.Load(modInfo);
				}

			}).ID("_ModLoader_Strings").Sort(-1);
		}
		StrReplacer.Inst().onLanguageChanged.AddListener(this.onLanguageChanged); // 內有自動檢查 是否已經含有該Listener
	}

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/* 語言標籤 */
	const string languageTag = "language";

	/* 當語言切換 的 事件偵聽者 */
	private EventListener onLanguageChanged;

	private Dictionary<string, List<string>> mod2UsedKeywords = new Dictionary<string, List<string>>();

	private Dictionary<string, Vals> key2WordVals = new Dictionary<string, Vals>();

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/* 取得標籤 */
	public override List<string> GetTags () {
		if (this.tags.Contains(ModLoader_Strings.languageTag) == false) {
			this.tags.Add(ModLoader_Strings.languageTag);
		}
		return base.GetTags();
	}

	/** 讀取 */
	public override void Load (ModInfo modInfo) {

		string modPath = modInfo.GetModPath();
		string langCode = StrReplacer.Inst().GetLanguage();
		if (langCode == null) return;

		string startsPath = ModUtil.STRINGS_FOLDERNAME + "/" + langCode;

		//======= 取得檔案名與內容 ============
		Dictionary<string, byte[]> files = ModViewer.GetModFiles(modPath, (filePath) => {
			if (!filePath.StartsWith(startsPath)) return false;

			FileInfo info = new FileInfo(filePath);

			//若是資料夾則不解壓縮
			if (filePath.EndsWith("/")) return false;
			//若不是正確副檔名則不解壓縮
			if (Array.IndexOf(ResUtil.textFormat, info.Extension) == -1) return false;

			return true;
		});

		// 讀為字串
		Dictionary<string, string> keywordsFiles = new Dictionary<string, string>();
		foreach (KeyValuePair<string, byte[]> pair in files) {
			byte[] eachArray = pair.Value;
			StreamReader stream = new StreamReader(new MemoryStream(eachArray));
			keywordsFiles.Add(pair.Key, stream.ReadToEnd());
		}

		//======= 處理檔案名與內容 ============
		
		// 前次使用的關鍵字
		string[] lastKeywords = new string[this.key2WordVals.Keys.Count];
		this.key2WordVals.Keys.CopyTo(lastKeywords, 0);

		// 優先度
		ModMgr modMgr = ModMgr.Inst();
		float priority = modMgr.GetIndexOfLoaded(modInfo.id);
		if (priority == -1) priority = -(modMgr.GetLoadedCount()+1);

		// 每個文件
		foreach (KeyValuePair<string, string> filePair in keywordsFiles) {

			DictSO key2words = DictSO.Json(filePair.Value);
			if (key2words == null) continue;
			
			// 每個單字
			foreach (KeyValuePair<string, object> keywordPair in key2words) {
				string key = keywordPair.Key;
				string word = keywordPair.Value.ToString();

				Vals vals;
				if (this.key2WordVals.ContainsKey(key)) {
					vals = this.key2WordVals[key];
				} else {
					vals = new Vals(null);
					this.key2WordVals.Add(key, vals);
				}

				// 設置 該單字的內文Vals User
				vals.Set(modInfo.id, priority, word);
			}

			// 所有 關鍵字
			string[] keywords = new string[key2words.Count];
			key2words.Keys.CopyTo(keywords, 0);
			
			// 加入 該Mod使用的所有關鍵字 至 所有Mod對應所使用的所有關鍵字
			if (this.mod2UsedKeywords.ContainsKey(modInfo.id)) {
				this.mod2UsedKeywords[modInfo.id] = new List<string>(keywords);
			} else {
				this.mod2UsedKeywords.Add(modInfo.id, new List<string>(keywords));
			}

		}

		// 更新 單字 至 StrReplacer
		this.updateKeywords2StrReplacer(lastKeywords);
	}

	/** 卸載 */
	public override void Unload (ModInfo modInfo) {
		// 若 該Mod 沒有被紀律 (沒使用過此Loader) 則 忽略
		if (this.mod2UsedKeywords.ContainsKey(modInfo.id) == false) return;

		List<string> keywords = this.mod2UsedKeywords[modInfo.id];
		foreach (string key in keywords) {
			if (this.key2WordVals.ContainsKey(key) == false) continue;

			Vals vals = this.key2WordVals[key];

			vals.Remove(modInfo.id);

			if (vals.GetCount() == 0) {
				this.key2WordVals.Remove(key);
			}
		}

		this.mod2UsedKeywords.Remove(modInfo.id);
	}


	/*===================================Protected Function======================================*/

	protected void updateKeywords2StrReplacer (string[] lastKeywords) {
		StrReplacer strReplacer = StrReplacer.Inst();
		
		foreach (string eachLast in lastKeywords) {
			if (this.key2WordVals.ContainsKey(eachLast)) continue;
			strReplacer.RemoveKeyword(eachLast);
		}

		foreach (KeyValuePair<string, Vals> pair in this.key2WordVals) {
			string val = (string) pair.Value.GetCurrent();
			strReplacer.SetKeyword(pair.Key, val);
		}
	}

	/*====================================Private Function=======================================*/

}



}