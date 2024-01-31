using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

using UnityEngine;

using UzCoroutine = Uzil.Misc.Coroutine;

namespace Uzil.i18n {

public class SubReplacer_Action : SubReplacer {
	protected class KeyAndArgs {
		public string key;
		public DictSO args;
	}

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*
	 * 字詞的關鍵字標示
	 * \<\!(?<action>(?:(?<!\!\>).(?!\<\!))+)\!\> 
	 */
	public const string IDENTIFY = @"<!"; 
	public const string PATTERN = PATTERN_BEGIN + "(?<" + PATTERN_GROUP + ">(?:(?<!" + PATTERN_END + ").(?!" + PATTERN_BEGIN + "))+)" + PATTERN_END;
	public const string PATTERN_BEGIN = @"\<\!";
	public const string PATTERN_END = @"\!\>";
	public const string PATTERN_GROUP = @"action";

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/* 關鍵字 動作 */
	// Key : 動作(參數args, 回覆resCB)
	public Dictionary<string, Action<DictSO, Action<string>>> key2Action = new Dictionary<string, Action<DictSO, Action<string>>>();

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
		if (process.content.Contains(SubReplacer_Action.IDENTIFY) == false) {
			process.NextSubReader();
			return;
		}

		this.replaceTotal(process);
	}

	// public void RenderAsync(){

	// }

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/
	private void replaceTotal (ReadProcess process) {

		Action<Action> replaceAllAct = (nextTotal) => {

			List<Action<Action<string>>> replaceActions = new List<Action<Action<string>>>();

			// 所有關鍵字代換為檔案中的字詞
			MatchCollection matches = Regex.Matches(process.content, SubReplacer_Action.PATTERN);
			foreach (Match match in matches) {
				string matchVal = match.Value.Substring(2, match.Value.Length - 2/*去頭*/- 2/*去尾*/);
				KeyAndArgs ka = this.parseKeyAndArgs(matchVal);
				string key = ka.key;
				if (!this.key2Action.ContainsKey(key)) continue;

				
				Action<DictSO, Action<string>> action = this.key2Action[key];
				replaceActions.Add((next)=>{
					action(ka.args, (res) => {
						if (res == null) res = this.getUnReplace(key);
						process.content = process.content.Replace(match.Value, res);
						next(null);
					});
				});
			}

			
			Async.Parallel<string>(
				replaceActions, 
				(idx, res_null)=>{
					process.EachWord();
				}, 
				(res_null)=>{
					nextTotal();
				}
			);

		};
		

		
		// 預備要執行的 所有 全替換
		List<Action<Action>> replaceAllActs = new List<Action<Action>>();

		int recursiveTime = 3; // 若讀取到的還是關鍵字，的遞回次數
		for (int idx = 0; idx < recursiveTime; idx++) {
			replaceAllActs.Add(replaceAllAct);
		}

		Async.EachSeries<Action<Action>>(
			replaceAllActs, 
			(each, next)=>{
				// 全替換
				each(
					// 完成後
					()=>{

						// 若 已經沒有 則 停止
						if (!Regex.IsMatch(process.content, SubReplacer_Action.PATTERN)) {
							next(false);
						}
						
						next(true);
					}
				);
			},
			()=>{
				process.NextSubReader();
			}
		);
		

	}


	private KeyAndArgs parseKeyAndArgs (string str) {
		KeyAndArgs ka = new KeyAndArgs();

		string[] funcNameAndArgs1 = str.Split('{');

		ka.key = funcNameAndArgs1[0];

		if (funcNameAndArgs1.Length > 1) {

			string[] argsAndEnd = funcNameAndArgs1[1].Split('}');

			if (argsAndEnd.Length > 1) {

				StringBuilder argsJson_str = new StringBuilder();
				argsJson_str.Append("{");

				//串接/復原 除了最後一個}以外的字串
				for (int i = 0; i < argsAndEnd.Length - 1; i++) {
					argsJson_str.Append(argsAndEnd[i]).Append("}");
				}

				ka.args = DictSO.Json(argsJson_str.ToString());

			}

		}

		if (ka.args == null) {
			ka.args = new DictSO();
		}

		return ka;
	}
}


}