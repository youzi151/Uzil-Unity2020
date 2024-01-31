using System.Collections.Generic;

namespace Uzil.i18n {

	public class LanguageInfo : IMemoable{

		public string name;
		public string code;
		public List<string> alias = new List<string>();

		/** [IMemoable] 紀錄為Json格式 */
		public object ToMemo () {
			DictSO memo = DictSO.New();

			memo.Set("code", this.code);
			memo.Set("name", this.name);
			memo.Set("alias", this.alias);
			
			return memo;
		}
		
		/** [IMemoable] 讀取Json格式 */
		public void LoadMemo (object memoJson) {
			DictSO data = DictSO.Json(memoJson);
			data.TryGetString("name", (res)=>{
				this.name = res;
			});
			data.TryGetString("code", (res)=>{
				this.code = res;
			});
			data.TryGetList<string>("alias", (res)=>{
				this.alias = res;
			});
		}
	}

}