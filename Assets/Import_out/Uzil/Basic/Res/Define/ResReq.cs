using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Uzil.Res {

/** 資源索引資訊 */
public class ResReq {

	/*======================================Constructor==========================================*/

	public ResReq () {}

	public ResReq (string path, params string[] tags) {
		this.path = path;
		this.tags = new List<string>(tags);
	}

	public ResReq (string path, object user, params string[] tags) {
		this.path = path;
		this.user = user;
		this.tags = new List<string>(tags);
	}

	public ResReq (System.Type type, string path, object user, params string[] tags) {
		this.type = type;
		this.path = path;
		this.user = user;
		this.tags = new List<string>(tags);
	}

	public ResReq (ResReq req) {
		this.type = req.type;
		this.path = req.path;
		this.tags = new List<string>(req.tags);
		foreach (KeyValuePair<string, object> pair in req.args) {
			this.args.Add(pair.Key, pair.Value);
		}
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
	public static ResReq Parse (string str) {
		ResReq req = new ResReq();

		if (str == null) return req;

		string[] pathAndTags = str.Split(new char[]{':'}, 2);
		
		string path = pathAndTags[0];
		req.path = path;

		if (pathAndTags.Length > 1) {
			string tags_str = pathAndTags[1];
			
			// parse tags
			// 是否有符合的字串
			if (Regex.IsMatch(tags_str, ResReq.PATTERN)) {

				// 所有關鍵字代換為字典中的字詞
				MatchCollection Matches = Regex.Matches(tags_str, ResReq.PATTERN);
				foreach (Match match in Matches){
					string tag = match.Value;
					req.tags.Add(tag);
				}
			}
		}

		return req;
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

	/** 使用者 */
	public object user;

	/** 是否重新讀取 */
	public bool isForceReload = false;

	/** 資料 */
	public DictSO args = new DictSO();

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/* 設置 路徑 */
	public ResReq Path (string path) {
		this.path = path;
		return this;
	}

	/* 設置 標籤 */
	public ResReq Tag (params string[] tags) {
		for (int idx = 0; idx < tags.Length; idx++) {
			string tag = tags[idx];
			if (this.tags.Contains(tag) == false) {
				this.tags.Add(tag);
			}
		}
		return this;
	}

	/* 設置 使用者 */
	public ResReq User (object user) {
		this.user = user;
		return this;
	}

	/* 設置 資料 */
	public ResReq Arg (string key, object value) {
		this.args.Set(key, value);
		return this;
	}

	/* 是否 相等 */
	public bool IsEqual (ResReq target) {
		if (this.path == target.path && this.type == target.type) return true;
		return false;
	}

	/* 是否 合法 */
	public bool IsValid () {
		if (this.IsValidPath() == false) return false;
		if (this.user == null) return false;
		return true;
	}
	/* 是否 合法 */
	public bool IsValidPath () {
		if (this.path == null || this.path == "") return false;
		return true;
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

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

}



}