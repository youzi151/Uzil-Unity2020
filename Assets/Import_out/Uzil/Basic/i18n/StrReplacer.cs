using System;
using System.Collections.Generic;

using UnityEngine;

using Uzil.Options;
using Uzil.UserData;

namespace Uzil.i18n {

/*
 * 代換字串
 * XXX: 目前用來交替執行 字典<->讀檔 的方法有點亂，應該要用個別做成一個filter。
 * 但因為頂多就是字典跟讀檔兩個交替，沒有必要拆更細
 */

public class StrReplacer {

	/*======================================Constructor==========================================*/

	public StrReplacer (string _language = null) {
		// 子翻譯器
		this.subReaders.Add(this.words_subreader as SubReplacer);
		this.subReaders.Add(this.action_subreader as SubReplacer);
		this.subReaders.Add(this.customVar_subreader as SubReplacer);
		this.subReaders.Add(this.file_subreader as SubReplacer);
		this.subReaders.Add(this.script_subreader as SubReplacer);
	}

	/*=====================================Static Members========================================*/

	/** 單例 */
	public static StrReplacer _instance;

	/*=====================================Static Funciton=======================================*/

	/** 單例 */
	public static StrReplacer Inst () {
		if (StrReplacer._instance == null) {
			StrReplacer instnace = new StrReplacer();
			instnace.Init();
			StrReplacer._instance = instnace;
		}
		return StrReplacer._instance;
	}

	/** 從 設定檔 讀取 語言*/
	public static string LoadLanguageInConfig () {
		return ProfileSave.main.GetStr(Option.FileName_Game, "language");
	}

	/** 代換 */
	public static void Render (string content, Action<string> response) {
		StrReplacer.Render(content, null, null, response);
	}

	public static void Render (string content, Action<string> eachWord, Action<string> onNextSubReader, Action<string> response) {
		StrReplacer.Inst().RenderNormal(content, eachWord, onNextSubReader, response);
	}

	/** 立即代換(只支持字典) */
	public static string RenderNow (string content) {
		return StrReplacer.Inst().RenderImmediate(content);
	}


	/*=========================================Members===========================================*/

	/** 語言 */
	public string language{get; private set;} = null;

	/** 語言資料 */
	public List<LanguageInfo> langInfos = new List<LanguageInfo>();

	/*========================================Components=========================================*/
	
	public List<SubReplacer> subReaders = new List<SubReplacer>();

	public SubReplacer_Words words_subreader = new SubReplacer_Words();
	
	public SubReplacer_Action action_subreader = new SubReplacer_Action();

	public SubReplacer_CustomVar customVar_subreader = new SubReplacer_CustomVar();
	
	public SubReplacer_File file_subreader = new SubReplacer_File();

	public SubReplacer_Script script_subreader = new SubReplacer_Script();

	/*==========================================Event============================================*/

	/** 當 更新 */
	//1.於 讀取模組(含本體)的字典中，註冊語言改變時，重新載入
	//2.於 各Text 中，註冊語言改變時，重新呼叫Render
	public Event onUpdate = new Event();

	/** 當 語言 變更 */
	public Event onLanguageChanged = new Event();

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 初始化 */
	// XXX: 不能涉及任何 Inst 行為，否則會loop
	public void Init (string _language = null) {
		string language = _language;

		// 若為 預設 則 讀取設定檔
		if (language == null) {
			
			language = StrReplacer.LoadLanguageInConfig();
			// 若無設定檔則預設空
			if (language == null || language == "") {
				language = null;
			}

		}

		// 設置語言 	
		this.SetLanguage(language);
	}

	/** 更新 */
	public void Update () {
		this.onUpdate.Call();
	}
	
	/** 加入 */
	public void AddSubReader (SubReplacer subReader) {
		if (this.subReaders.Contains(subReader)) return;
		this.subReaders.Add(subReader);
	}
	
	// ##          ###    ##    ##  ######   ##     ##    ###     ######   ######## 
	// ##         ## ##   ###   ## ##    ##  ##     ##   ## ##   ##    ##  ##       
	// ##        ##   ##  ####  ## ##        ##     ##  ##   ##  ##        ##       
	// ##       ##     ## ## ## ## ##   #### ##     ## ##     ## ##   #### ######   
	// ##       ######### ##  #### ##    ##  ##     ## ######### ##    ##  ##       
	// ##       ##     ## ##   ### ##    ##  ##     ## ##     ## ##    ##  ##       
	// ######## ##     ## ##    ##  ######    #######  ##     ##  ######   ######## 

	/** 取得語言資訊列表 */
	public List<LanguageInfo> GetLanguageInfos () {
		return this.langInfos;
	}

	/** 尋找 語言代碼 */
	public string FindLanguageCode (string targetLang) {
		return this.FindLanguageCode(new List<string>{targetLang});
	}
	public string FindLanguageCode (List<string> targetLangs) {
		
		// 各語言 代碼 / 別名 的小寫
		string[] codes_low = new string[this.langInfos.Count];
		string[][] aliases_low = new string[this.langInfos.Count][];

		// 每種語言
		for (int idx = 0; idx < this.langInfos.Count; idx++) {
			LanguageInfo each = this.langInfos[idx];
			
			// 暫存 代碼 小寫
			codes_low[idx] = each.code.ToLower();

			// 暫存 別名 小寫
			string[] aliases = new string[each.alias.Count];
			for (int idx2 = 0; idx2 < each.alias.Count; idx2++) {
				aliases[idx2] = each.alias[idx2].ToLower();
			}
			aliases_low[idx] = aliases;
		}

		// 依序 目標語言
		for (int idx = 0; idx < targetLangs.Count; idx++) {
			string targetLang = targetLangs[idx];
			if (targetLang == null) continue;
			targetLang = targetLang.ToLower();

			// 每種語言 代碼(小寫) 若 為 該目標語言(小寫) 則 返回 該語言 代碼
			for (int langIdx = 0; langIdx < codes_low.Length; langIdx++) {
				if (targetLang == codes_low[langIdx]) return this.langInfos[langIdx].code;
			}

			// 每種語言 別名(小寫) 若 有包含 該目標語言(小寫) 則 返回 該語言 代碼
			for (int langIdx = 0; langIdx < aliases_low.Length; langIdx++) {
				string[] alias_low = aliases_low[langIdx];
				for (int aliasIdx = 0; aliasIdx < alias_low.Length; aliasIdx++) {
					if (alias_low[aliasIdx] == targetLang) return this.langInfos[langIdx].code;
				}
			}

		}

		return null;
	}

	/** 取得語言 */
	public string GetLanguage () {
		return this.language;
	}

	/** 重設語言 */
	public void SetLanguage (string language) {
		string langCode = this.FindLanguageCode(language);
		if (langCode != null) {
			this.SetLanguageCode(language);
		}
	}
	public void SetLanguageCode (string langCode) {
		this.language = langCode;
		this.SetKeyword("_lang", this.language);
		this.onLanguageChanged.Call();
		this.onUpdate.Call();
	}

	/** 新增 語言資料 */
	public void AddLanguage (LanguageInfo info) {
		for (int idx = 0; idx < this.langInfos.Count; idx++) {
			LanguageInfo each = this.langInfos[idx];
			if (each.code == info.code) {
				this.langInfos[idx] = info;
				return;
			}
		}
		this.langInfos.Add(info);
	}


	// ##    ## ######## ##    ## ##      ##  #######  ########  ########  
	// ##   ##  ##        ##  ##  ##  ##  ## ##     ## ##     ## ##     ## 
	// ##  ##   ##         ####   ##  ##  ## ##     ## ##     ## ##     ## 
	// #####    ######      ##    ##  ##  ## ##     ## ########  ##     ## 
	// ##  ##   ##          ##    ##  ##  ## ##     ## ##   ##   ##     ## 
	// ##   ##  ##          ##    ##  ##  ## ##     ## ##    ##  ##     ## 
	// ##    ## ########    ##     ###  ###   #######  ##     ## ########  

	/** 設置關鍵字 字典與動作 */
	// 不包含讀檔，讀檔為即時讀取不預載
	public void SetKeyword (string key, string _value) {
		if (this.words_subreader.words.ContainsKey(key)) {
			this.words_subreader.words[key] = _value;
		}else{
			this.words_subreader.words.Add(key, _value);
		}
	}
	public void SetKeyword (string key, Action<DictSO, Action<string>> action) {
		if (this.action_subreader.key2Action.ContainsKey(key)) {
			this.action_subreader.key2Action[key] = action;
		}else{
			this.action_subreader.key2Action.Add(key, action);	
		}
	}

	/** 取得原始內容 (該key對應到的rawContent) */
	public string GetRaw (string key) {
		if (this.words_subreader.words.ContainsKey(key)) {
			return this.words_subreader.words[key];
		} else {
			return null;
		}
	}

	/** 移除關鍵字 */
	public void RemoveKeyword (string key) {
		if (this.words_subreader.words.ContainsKey(key)) {
			this.words_subreader.words.Remove(key);
		}
		if (this.action_subreader.key2Action.ContainsKey(key)) {
			this.action_subreader.key2Action.Remove(key);
		}
	}

	// ########  ######## ##    ## ########  ######## ########  
	// ##     ## ##       ###   ## ##     ## ##       ##     ## 
	// ##     ## ##       ####  ## ##     ## ##       ##     ## 
	// ########  ######   ## ## ## ##     ## ######   ########  
	// ##   ##   ##       ##  #### ##     ## ##       ##   ##   
	// ##    ##  ##       ##   ### ##     ## ##       ##    ##  
	// ##     ## ######## ##    ## ########  ######## ##     ## 

	/** 一般代換 */
	public void RenderNormal (string content, Action<string> eachWord, Action<string> onNextSubReader, Action<string> response) {
		
		ReadProcess rp = new ReadProcess(content);

		rp.onEachWord = eachWord;
		rp.onNextSubReader = onNextSubReader;
		rp.onResponse = response;

		if (content == "" || content == null){
			rp.Finish();
			return;
		}

		this.RenderBySubReader(rp);
	}

	/** 即時代換 (僅支援部分Subreader) */
	public string RenderImmediate (string content) {
		string str = content;
		for (int i = 0; i < 3; i++) {
			str = this.customVar_subreader.RenderImmediate(str);
			str = this.words_subreader.RenderImmediate(str);
		}
		return str;
	}


	/** 代換字詞(cb: string為代換結果, int為剩餘嘗試次數) */
	public void RenderBySubReader (ReadProcess process) {
		this.callSubReader(0, process);
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

	private void callSubReader (int idx, ReadProcess process) {
		int nextIdx = (int)Mathf.Repeat(idx+1, this.subReaders.Count);
		process.next = ()=>{
			this.callSubReader(nextIdx, process);
		};
		this.subReaders[idx].Render(process);
	}

}


}