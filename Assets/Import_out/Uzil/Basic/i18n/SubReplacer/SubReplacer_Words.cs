using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Uzil.i18n {

public class SubReplacer_Words : SubReplacer {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*
	 * 字詞的關鍵字標示
	 * \<\$(?<word>(?:(?<!\$\>).(?!\<\$))+)\$\> 
	 */
	public const string IDENTIFY = @"<$"; 
	public const string PATTERN = PATTERN_BEGIN + "(?<" + PATTERN_GROUP + ">(?:(?<!" + PATTERN_END + ").(?!" + PATTERN_BEGIN + "))+)" + PATTERN_END;
	public const string PATTERN_BEGIN = @"\<\$";
	public const string PATTERN_END = @"\$\>";
	public const string PATTERN_GROUP = @"word";

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
		if (process.content.Contains(SubReplacer_Words.IDENTIFY) == false) {
			process.NextSubReader();
			return;
		}

		int recursiveTime = 3; // 若 讀取到的還是關鍵字 的 遞回次數

		Regex regex = new Regex(SubReplacer_Words.PATTERN);

		MatchCollection matches = regex.Matches(process.content);

		while (matches.Count != 0) {
			if (recursiveTime-- <= 0) break;

			// 所有關鍵字 代換
			foreach (Match match in matches) {

				string key = match.Groups[SubReplacer_Words.PATTERN_GROUP].Value;
				string replace = this.getUnReplace(key);

				if (this.words.ContainsKey(key)) {
					replace = this.words[key];
					process.content = process.content.Replace(match.Value, replace);
					process.EachWord();
				}
				else {
					process.content = process.content.Replace(match.Value, replace);
				}
			}

			matches = regex.Matches(process.content);

			
		}

		process.NextSubReader();
	}

	/* 立即代換 */
	public string RenderImmediate (string oldStr) {

		string result = oldStr;
		if (result == null || result == "") return result;

		// 若 沒有任何關鍵字 則 跳過
		if (oldStr.Contains(SubReplacer_Words.IDENTIFY) == false) {
			return oldStr;
		}

		int recursiveTime = 3; // 若讀取到的還是關鍵字，的遞回次數

		Regex regex = new Regex(SubReplacer_Words.PATTERN);

		MatchCollection matches = regex.Matches(result);

		while (matches.Count != 0) {
			if (recursiveTime-- <= 0) break;

			// 所有關鍵字 代換
			foreach (Match match in matches) {

				string key = match.Groups[SubReplacer_Words.PATTERN_GROUP].Value;
				string replace = "key";

				if (this.words.ContainsKey(key)) {
					replace = this.words[key];
					result = result.Replace(match.Value, replace);
				}
			}

			matches = regex.Matches(result);
		}

		return result;
	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}


}