using System.Text.RegularExpressions;

using Uzil.Lua;

namespace Uzil.i18n {

public class SubReplacer_Script : SubReplacer {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*
	 * 字詞的關鍵字標示
	 * \<\@(?<script>(?:(?<!\@\>).(?!\<\@))+)\@\> 
	 */
	public const string IDENTIFY = @"<@"; 
	public const string PATTERN = PATTERN_BEGIN + "(?<" + PATTERN_GROUP + ">(?:(?<!" + PATTERN_END + ").(?!" + PATTERN_BEGIN + "))+)" + PATTERN_END;
	public const string PATTERN_BEGIN = @"\<\@";
	public const string PATTERN_END = @"\@\>";
	public const string PATTERN_GROUP = @"script";

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

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
		if (process.content.Contains(SubReplacer_Script.IDENTIFY) == false) {
			process.NextSubReader();
			return;
		}

		this.eachMatchScript(process);
	}

	// public void RenderAsync(){

	// }

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/
	private void eachMatchScript (ReadProcess process) {

		int recursiveTime = 3; //若讀取到的還是關鍵字，的遞回次數

		Regex regex = new Regex(SubReplacer_Script.PATTERN);

		MatchCollection matches = regex.Matches(process.content);

		while (matches.Count != 0) {
			if (recursiveTime-- <= 0) break;

			// 所有關鍵字 代換
			foreach (Match match in matches) {

				string script = match.Groups[SubReplacer_Script.PATTERN_GROUP].Value;

				string res = this.getUnReplace(script);
				res = LuaUtil.DoString<string>(script);

				process.content = process.content.Replace(match.Value, res);

				process.EachWord();
			}

			matches = regex.Matches(process.content);
		}

		process.NextSubReader();
	}

}


}