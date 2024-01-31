using System.Text.RegularExpressions;

#if STEAM
using Uzil.ThirdParty.Steam;
#endif

namespace Uzil.UserData {

public class UserDataUtil {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/
	
	/** 模板名稱 */
	public static readonly string templateName = "_template";
	
	public static readonly string defaultName = "default";

	/** 用戶名稱 */
	protected static string _userName = null;
	
	/** 取得 用戶名稱 */
	public static string GetUserName () {
		if (UserDataUtil._userName != null && UserDataUtil._userName != "") return UserDataUtil._userName;
#if STEAM
		if (SteamInst.Inst().isInitialized) {
			string steamID3 = SteamInst.Inst().GetAccountID();
			if (steamID3 != null && steamID3 != "") {
				return "steam_"+steamID3;
			}
		}
#endif
		return "unknown";
	}


	/** 當前個案名稱 */
	protected static string _currentProfileName = UserDataUtil.defaultName;

	/** 當個案切換 */
	public static Event onProfileChanged = new Event();

	/** 取得 當前個案名稱 */
	public static string GetProfileName () {
		if (UserDataUtil._currentProfileName == null) return UserDataUtil.defaultName;
		return UserDataUtil._currentProfileName;
	}

	/** 設置 當前個案名稱 */
	public static bool SetProfile (string profileName) {
		if (Regex.IsMatch(profileName, "[\\/?%*:|\"<>]+") == true) return false;
	
		UserDataUtil._currentProfileName = profileName;

		UserDataUtil.onProfileChanged.Call();

		return true;
	}

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/



}

}