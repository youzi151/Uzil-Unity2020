using System.Text.RegularExpressions;

namespace Uzil.i18n {


public class SubReplacer_CustomVar : SubReplacer {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*
	 * 字詞的關鍵字標示
	 * \<\%(?<var>(?:(?<!\%\>).(?!\<\%))+)\%\> 
	 */
	public const string IDENTIFY = @"<%"; 
	public const string PATTERN = PATTERN_BEGIN + "(?<" + PATTERN_GROUP + ">(?:(?<!" + PATTERN_END + ").(?!" + PATTERN_BEGIN + "))+)" + PATTERN_END;
	public const string PATTERN_BEGIN = @"\<\%";
	public const string PATTERN_END = @"\%\>";
	public const string PATTERN_GROUP = @"var";

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
		if (process.content.Contains(SubReplacer_CustomVar.IDENTIFY) == false) {
			process.NextSubReader();
			return;
		}

		CustomVar defaultCVarInst = CustomVarUtil.Inst();

		int recursiveTime = 3; // 若 讀取到的還是關鍵字 的 遞回次數

		Regex regex = new Regex(SubReplacer_CustomVar.PATTERN);

		MatchCollection matches = regex.Matches(process.content);

		while (matches.Count != 0) {
			if (recursiveTime-- <= 0) break;

			// 所有關鍵字 代換
			foreach (Match match in matches) {
				CustomVar varInst = defaultCVarInst;
				string keyword;

				string instKey2Keyword = match.Groups[SubReplacer_CustomVar.PATTERN_GROUP].Value;
				string[] split = instKey2Keyword.Split(new char[]{':'}, 2);
				if (split.Length > 1) {
					string instKey = split[0];
					varInst = CustomVarUtil.Inst(instKey);
					keyword = split[1];
				} else {

					keyword = split[0];
					
					CustomVar tryFind = CustomVarUtil.Find(keyword);
					if (tryFind != null) {
						varInst = tryFind;
					}
				}

				// 檢查 類別
				bool isString = varInst.ContainsStr(keyword);
				bool isFloat = varInst.ContainsFloat(keyword);
				// 都不存在 則 返回
				if (!isString && !isFloat) continue;

				// String優先
				string toReplace = this.getUnReplace(match.Value);
				if (isString) {
					toReplace = varInst.GetStr(keyword);
				}
				else if (isFloat) {
					toReplace = varInst.GetFloat(keyword).ToString();
				}

				//覆蓋	
				process.content = process.content.Replace(match.Value, toReplace);
				process.EachWord();
			}

			matches = regex.Matches(process.content);
		}

		process.NextSubReader();
	}

	// public void RenderAsync(){

	// }

	/* 立即代換 */
	public string RenderImmediate (string oldStr) {
		
		string result = oldStr;
		if (result == null || result == "") return result;

		// 若 沒有任何關鍵字 則 跳過
		if (oldStr.Contains(SubReplacer_CustomVar.IDENTIFY) == false) {
			return oldStr;
		}

		CustomVar customVar = CustomVarUtil.Inst();

		int recursiveTime = 3; //若讀取到的還是關鍵字，的遞回次數

		Regex regex = new Regex(SubReplacer_CustomVar.PATTERN);

		MatchCollection matches = regex.Matches(result);

		while (matches.Count != 0) {
			if (recursiveTime-- <= 0) break;

			// 所有關鍵字 代換
			foreach (Match match in matches) {
				CustomVar inst = customVar;
				string keyword;

				string instKey2Keyword = match.Groups[SubReplacer_CustomVar.PATTERN_GROUP].Value;
				string[] split = instKey2Keyword.Split(new char[]{':'}, 2);
				if (split.Length > 1) {
					string instKey = split[0];
					inst = CustomVarUtil.Inst(instKey);
					keyword = split[1];
				} else {
					keyword = split[0];
				}

				// 檢查 類別
				bool isString = inst.ContainsStr(keyword);
				bool isFloat = inst.ContainsFloat(keyword);
				// 若 都不存在 則 返回
				if (!isString && !isFloat) continue;

				// String優先
				string toReplace = match.Value;
				if (isString) {
					toReplace = inst.GetStr(keyword);
				}
				else if (isFloat) {
					toReplace = inst.GetFloat(keyword).ToString();
				}

				// 覆蓋	
				result = result.Replace(match.Value, toReplace);
			}

			matches = regex.Matches(result);
		}

		return result;
	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}


}