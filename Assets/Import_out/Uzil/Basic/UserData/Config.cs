using System.Text;
using Uzil.Util;

namespace Uzil.UserData {

public class Config : Save {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/
	
	/** 實例 */
	public static new Config Inst (string key = "_ConfigSave") {
		return Save.Inst<Config>(key);
	}
	
	/** 主要 */
	public static Config main {
		get {
			if (Config._main == null) {
				Config._main = Config.Inst();
			}
			return Config._main;
		}
	}
	private static Config _main = null;

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

	/** 讀取 內容 */
	protected override object get (PathKey pk) {
		// 要 建立的
		PathKey toCreatePK = null;

		// 若 不存在
		if (this.IsExist(pk) == false) {
			// 準備 要建立的檔案路徑
			toCreatePK = pk;
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
		StringBuilder sb = new StringBuilder();

		// 開頭 + 目錄
		sb.Append("<#Config/");

		// 防止 上一層
		path = path.Replace("..", ".");

		// 目標路徑
		sb.Append(path);

		// 鍵值
		if (valKey != null && valKey != "") {
			sb.Append(":").Append(valKey);
		}

		// 結尾
		sb.Append("#>");

		return new PathKey(sb.ToString());
	}

}

}