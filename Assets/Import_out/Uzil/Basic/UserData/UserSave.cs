using System.Text;
using Uzil.Util;

namespace Uzil.UserData {

public class UserSave : Save {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/** 實例 */
	public new static UserSave Inst (string key = "_UserSave") {
		return Save.Inst<UserSave>(key);
	}

	/** 主要 */
	public static UserSave main {
		get {
			if (UserSave._main == null) {
				UserSave._main = UserSave.Inst();
			}
			return UserSave._main;
		}
	}
	private static UserSave _main = null;

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** 讀取 內容 */
	protected override object get (PathKey pk) {
		// 要 建立的
		PathKey toCreatePK = null;

		// 若 不存在
		if (this.IsExist(pk) == false) {
			
			// 準備 要建立的檔案路徑
			toCreatePK = pk;
			
			// 還原 指定檔案路徑 (去除 handlePathKey加工)
			string path = PathUtil.GetRelativePath(this.getUserPath(UserDataUtil.GetUserName()), pk.path);

			// 覆蓋 目標路徑 為 預設用戶
			// 預備 從模板中取得
			pk = this.handlePathKey(UserDataUtil.templateName, path, pk.key);
		}
		
		object res = base.get(pk);

		if (res == null) return null;

		// 若 要建立的目標路徑 存在
		if (toCreatePK != null) {
			// 設置 到 要建立的目標路徑
			this.set(toCreatePK, res);
		}

		return res;
	}

	protected override PathKey handlePathKey (string path, string valKey) {
		return this.handlePathKey(UserDataUtil.GetUserName(), path, valKey);
	}

	protected PathKey handlePathKey (string userName, string path, string valKey) {
		StringBuilder sb = new StringBuilder();
		
		// 開頭
		sb.Append("<#");
		
		string filePath = this.getUserPath(userName) + path;
		// 防止 上一層
		filePath = filePath.Replace("..", ".");

		// 路徑
		sb.Append(filePath);

		// 鍵值
		if (valKey != null && valKey != "") {
			sb.Append(":").Append(valKey);
		}

		// 結尾
		sb.Append("#>");

		return new PathKey(sb.ToString());
	}

	/** 取得 用戶路徑 */
	protected string getUserPath (string userName) {
		StringBuilder sb = new StringBuilder();
		sb.Append("Save/").Append(userName).Append("/comm/");
		return sb.ToString();
	}

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}

}