using System.Collections.Generic;
using System.Text.RegularExpressions;

using UzEvent = Uzil.Event;

namespace Uzil.Res {

/** 資源索引資訊 */
public class ResInfo {

	/*======================================Constructor==========================================*/

	public ResInfo () {}

	public ResInfo (string path, params string[] tags) {
		this.path = path;
		this.tags = new List<string>(tags);
	}

	public ResInfo (System.Type type, string path, params string[] tags) {
		this.type = type;
		this.path = path;
		this.tags = new List<string>(tags);
	}

	public ResInfo (ResInfo info) {
		this.type = info.type;
		this.path = info.path;
		this.tags = new List<string>(info.tags);
	}

	public ResInfo (ResReq req) {
		this.type = req.type;
		this.path = req.path;
		this.tags = new List<string>(req.tags);
	}

	/*=====================================Static Members========================================*/

	/**
	 * 模式
	 * 不為 '[' ']' ',' '空格' 的任意長度字元，且 後面沒有任一 '['
	 */
	public static string PATTERN = "[^\\[\\]\\,\\s]+(?!.*\\[)";

	/*=====================================Static Funciton=======================================*/

	/** 
	 * 解讀
	 * e.g. "folder/file.ext:[tag1, tag2]"
	 * path = "folder/file.ext"
	 * tags = ["tag1", "tag2"]
	 */
	public static ResInfo Parse (string str) {
		ResInfo info = new ResInfo();

		if (str == null) return info;

		string[] pathAndTags = str.Split(new char[]{':'}, 2);
		
		string path = pathAndTags[0];
		info.path = path;

		if (pathAndTags.Length > 1) {
			string tags_str = pathAndTags[1];
			
			// parse tags
			// 是否有符合的字串
			if (Regex.IsMatch(tags_str, ResInfo.PATTERN)) {

				// 所有關鍵字代換為字典中的字詞
				MatchCollection Matches = Regex.Matches(tags_str, ResInfo.PATTERN);
				foreach (Match match in Matches){
					string tag = match.Value;
					info.tags.Add(tag);
				}
			}
		}

		return info;
	}

	/*=========================================Members===========================================*/

	/*== 辨識用 ======*/

	/** 所屬標籤 */
	public List<string> tags = new List<string>();

	/** 類型 */
	public System.Type type = typeof(object);

	/*== 內容 ========*/

	/** 路徑 */
	public string path;

	/** 結果 */
	public ResResult result = null;

	/** 讀取來源 */
	public ResLoader loader = null;

	/** 使用者 */
	public List<object> users = new List<object>();

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	public UzEvent onUnload = new UzEvent();

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/* 設置 路徑 */
	public ResInfo Path (string path) {
		this.path = path;
		return this;
	}

	/* 設置 標籤 */
	public ResInfo Tag (params string[] tags) {
		for (int idx = 0; idx < tags.Length; idx++) {
			string tag = tags[idx];
			if (this.tags.Contains(tag) == false) {
				this.tags.Add(tag);
			}
		}
		return this;
	}

	/* 是否 符合請求 */
	public bool IsMatchReq (ResReq target) {
		if (this.path == target.path && this.type == target.type) return true;
		return false;
	}
	public bool IsMatchInfo (ResInfo target) {
		if (this.path == target.path && this.type == target.type) return true;
		return false;
	}

	/* 是否 路徑合法 */
	public bool IsPathValid () {
		if (this.path == null || this.path == "") return false;
		
		return true;
	}

	/* 取得結果 */
	public T GetResult<T> () where T:class {
		if (this.result == null) return null;
		return ((ResResult<T>) this.result).value;
	}

	/* 取得該資源中有包含的AssetBundle名稱 */
	public string GetBundleName () {
		foreach (string tag in this.tags) {
			if (tag.StartsWith(ResUtil.ASSETBUNDLE_PREFIX)) {
				return tag.Substring(ResUtil.ASSETBUNDLE_PREFIX.Length, tag.Length - ResUtil.ASSETBUNDLE_PREFIX.Length);
			}
		}
		return null;
	}

	/* 更改使用者 */
	public void ReplaceUser (object last, object newOne) {
		if (this.users.Contains(last) == false || this.users.Contains(newOne)) return;
		this.users.Remove(last);
		this.users.Add(newOne);
	}

	/* 卸載 */
	public void Unload (bool isSkipUserCountCheck = false) {
		if (!isSkipUserCountCheck) {
			if (this.users.Count > 0) return;
		}

		if (this.loader != null) {
			this.loader.Unload(this);
		}

		this.onUnload.Call();
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

}



}