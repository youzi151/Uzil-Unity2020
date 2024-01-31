using Uzil;
using Uzil.i18n;

namespace UZAPI {

public class I18n {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/
		
	/** 取得語言 */
	public static string GetLanguage () {
		return StrReplacer.Inst().GetLanguage();
	}

	/** 設置語言 */
	public static void SetLanguage (string lang) {
		StrReplacer.Inst().SetLanguage(lang);
	}

	/** 新增語言 */
	public static void AddLanguage (string langInfo) {
		LanguageInfo info = new LanguageInfo();
		info.LoadMemo(langInfo);
		StrReplacer.Inst().AddLanguage(info);
	}

	/** 更新 */
	public static void Update () {
		StrReplacer.Inst().Update();
	}
	
	/** 設置 關鍵字 與 原始內容 */
	public static void Keyword (string key, string raw) {
		StrReplacer.Inst().SetKeyword(key, raw);
	}

	/** 取得 該關鍵字的原始內容 */
	public static string Raw (string key) {
		return StrReplacer.Inst().GetRaw(key);
	}
	
	/** 移除關鍵字 */
	public static void Remove (string key) {
		StrReplacer.Inst().RemoveKeyword(key);
	}

	/** 立刻代換 */
	public static string Render (string raw) {
		return StrReplacer.Inst().RenderImmediate(raw);
	}


	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}

}