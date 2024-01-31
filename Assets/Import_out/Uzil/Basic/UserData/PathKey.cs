
namespace Uzil {

public class PathKey {

	/*======================================Constructor==========================================*/
	public PathKey () {}

	public PathKey (string pathAndKey) {
		this.orinPath = pathAndKey;

		if (PathKey.IsValid(pathAndKey) == false) return;

		// 以 開頭關鍵字 的 最後一個字
		string _prefixChar = PathKey.BEGIN_CHAR.Substring(PathKey.BEGIN_CHAR.Length-1, 1);
		char[] prefixChar = _prefixChar.ToCharArray();
        
        // 去掉開頭
		string[] prefixAndPath = pathAndKey.Split(prefixChar, 2);
		pathAndKey = prefixAndPath[1];
		
		// 去結尾
		pathAndKey = pathAndKey.Substring(0, pathAndKey.Length-(PathKey.END_CHAR.Length));

		// 拆分 路徑 與 鍵值
		string _splitChar = PathKey.KEY_CHAR.Substring(PathKey.KEY_CHAR.Length-1, 1);
		char[] splitChar = _splitChar.ToCharArray();
		string[] array = pathAndKey.Split(splitChar, 2);

		this.path = array[0];
		if (array.Length > 1){
			this.key = array[1];
		}
	}

	public PathKey (PathKey pk) {
		this.orinPath = pk.orinPath;
		this.path = pk.path;
		this.key = pk.key;
	}

	/*=====================================Static Members========================================*/

	public const string BEGIN_CHAR = "<#";
	public const string END_CHAR = "#>";
	public const string KEY_CHAR = ":";

	/*=====================================Static Funciton=======================================*/

    /* 是否合法 */
	public static bool IsValid (string pathkey) {
		if (pathkey == null || pathkey == "") return false;
		if (!pathkey.StartsWith(PathKey.BEGIN_CHAR)) return false;
		if (!pathkey.EndsWith(PathKey.END_CHAR)) return false;
		return true;
	}

	/*=========================================Members===========================================*/

    /* 原始路徑 */
	public string orinPath;

    /* 路徑 */
	public string path;

    /* 鍵值 */
	public string key;

    /* 是否合法 */
	public bool isValid{
		get{
			return PathKey.IsValid(this.orinPath);
		}
	}

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

    /* 取得完整路徑與鍵值 */
	public string GetFullPath () {
		string res = this.orinPath;
		if (res.StartsWith(PathKey.BEGIN_CHAR)){
			res = res.Substring(PathKey.BEGIN_CHAR.Length);
		}
		if (res.EndsWith(PathKey.END_CHAR)){
			res = res.Substring(0, res.Length-PathKey.END_CHAR.Length);
		}
		return res;
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

}



}