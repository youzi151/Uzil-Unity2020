using System.Collections.Generic;
using System.Text.RegularExpressions;

using Uzil.Res;

namespace Uzil.i18n {

public class SubReplacer_File : SubReplacer {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*
	 * 字詞的關鍵字標示
	 * \<\#(?<file>(?:(?<!\#\>).(?!\<\#))+)\#\> 
	 */
	public const string IDENTIFY = @"<#"; 
	public const string PATTERN = PATTERN_BEGIN + "(?<" + PATTERN_GROUP + ">(?:(?<!" + PATTERN_END + ").(?!" + PATTERN_BEGIN + "))+)" + PATTERN_END;
	public const string PATTERN_BEGIN = @"\<\#";
	public const string PATTERN_END = @"\#\>";
	public const string PATTERN_GROUP = @"file";

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/* 關鍵字 字典 */
	public Dictionary<string, string> words = new Dictionary<string, string>();

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 代換字詞 */
	public override void Render (ReadProcess process) {
		// 若 最後一個Renderer為自己 則 回覆
		if (process.lastReader == this) {
			process.Finish();
			return;
		}
		process.lastReader = this;

		// 若 沒有任何關鍵字 則 跳過
		if (process.content.Contains(SubReplacer_File.IDENTIFY) == false) {
			process.NextSubReader();
			return;
		}

		int recursiveTime = 3; //若讀取到的還是關鍵字，的遞回次數

		Regex regex = new Regex(SubReplacer_File.PATTERN);

		MatchCollection matches = regex.Matches(process.content);

		while (matches.Count != 0) {
			if (recursiveTime-- <= 0) break;

			// 所有關鍵字 代換
			foreach (Match match in matches) {
				string replace = this.readFile(match.Value);
				if (replace == null) replace = this.getUnReplace(match.Value);
				
				process.content = process.content.Replace(match.Value, replace);
				process.EachWord();
			}

		}

		process.NextSubReader();
	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/
	private string readFile (string pathkey) {

		string replace = null;

		PathKey pk = new PathKey(pathkey);

		string file = ResUtil.text.Read(pk.path);
		if (file == "") return replace;

		DictSO data = DictSO.Json(file);
		if (data == null) return replace;

		if (data.ContainsKey(pk.key)) {
			replace = data.Get(pk.key).ToString();
		}

		return replace;
	}
}


}