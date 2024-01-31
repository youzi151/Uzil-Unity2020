#if STEAM

using UnityEngine;
using Steamworks;

namespace Uzil.ThirdParty.Steam {

public class RichPresenceKey_Steam {
	public const string Status = "status";
	public const string Connect = "connect";
	public const string Display = "steam_display";
	public const string Group = "steam_player_group";
	public const string GroupSize = "steam_player_group_size";

	public const string Display_Content = "#steam_display_content";
	public const string Display_CustomAny = "display_any";
}

public class SteamInst : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/** 實例 */
	protected static bool _isInstInit = false;
	protected static SteamInst _inst;
	public static SteamInst Inst () {
		if (SteamInst._inst == null) {
			SteamInst._isInstInit = true;
			SteamInst._inst = RootUtil.GetChild("Steam", RootUtil.GetMember("ThirdParty")).AddComponent<SteamInst>();
			SteamInst._isInstInit = false;
		}

		return SteamInst._inst;
	}

	/*=====================================Static Funciton=======================================*/

	[AOT.MonoPInvokeCallback(typeof(SteamAPIWarningMessageHook_t))]
	protected static void SteamAPIDebugTextHook(int nSeverity, System.Text.StringBuilder pchDebugText) {
		Debug.LogWarning(pchDebugText);
	}

	/*=========================================Members===========================================*/

	/** 是否已初始化 */
	public bool isInitialized {get; protected set;} = false;

	/** APPID */
	protected AppId_t appID = default(AppId_t);
	
	/** 訊息偵聽 */
	protected SteamAPIWarningMessageHook_t _steamAPIWarningMessageHook;

	/*========================================Components=========================================*/

	/** 統計與成就 */
	protected SteamStats _stats = new SteamStats();
	/** 取得 統計與成就 */
	public SteamStats Stats () {
		return this._stats;
	}

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Awake () {
		// 禁用 非 Inst() 建立
		if (SteamInst._isInstInit == false) {
			UnityEngine.Object.Destroy(this);
			return;
		}
	}

	// Update is called once per frame
	void Update () {
		if (this.isInitialized == false) return;

		SteamAPI.RunCallbacks();
	}

	void OnDestroy() {
		if (this.isInitialized == false) return;

		SteamAPI.Shutdown();
	}

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	public void Init (uint appID) {
		if (this.isInitialized) return;
		if (appID == 0) return;

		// 檢查 ==============

		if (!Packsize.Test()) {
			Debug.LogError("[Steamworks.NET] Packsize Test returned false, the wrong version of Steamworks.NET is being run in this platform.", this);
		}

		if (!DllCheck.Test()) {
			Debug.LogError("[Steamworks.NET] DllCheck Test returned false, One or more of the Steamworks binaries seems to be the wrong version.", this);
		}
		
		// try {
		// 	// If Steam is not running or the game wasn't started through Steam, SteamAPI_RestartAppIfNecessary starts the
		// 	// Steam client and also launches this game again if the User owns it. This can act as a rudimentary form of DRM.

		// 	// Once you get a Steam AppID assigned by Valve, you need to replace AppId_t.Invalid with it and
		// 	// remove steam_appid.txt from the game depot. eg: "(AppId_t)480" or "new AppId_t(480)".
		// 	// See the Valve documentation for more information: https://partner.steamgames.com/doc/sdk/api#initialization_and_shutdown
		// 	if (SteamAPI.RestartAppIfNecessary(new AppId_t((uint) appID))) {
		// 		Application.Quit();
		// 		return;
		// 	}
		// }
		// catch (System.DllNotFoundException e) { // We catch this exception here, as it will be the first occurrence of it.
		// 	Debug.LogError("[Steamworks.NET] Could not load [lib]steam_api.dll/so/dylib. It's likely not in the correct location. Refer to the README for more details.\n" + e, this);

		// 	Application.Quit();
		// 	return;
		// }

		// 初始化 ============

		// Initializes the Steamworks API.
		// If this returns false then this indicates one of the following conditions:
		// [*] The Steam client isn't running. A running Steam client is required to provide implementations of the various Steamworks interfaces.
		// [*] The Steam client couldn't determine the App ID of game. If you're running your application from the executable or debugger directly then you must have a [code-inline]steam_appid.txt[/code-inline] in your game directory next to the executable, with your app ID in it and nothing else. Steam will look for this file in the current working directory. If you are running your executable from a different directory you may need to relocate the [code-inline]steam_appid.txt[/code-inline] file.
		// [*] Your application is not running under the same OS user context as the Steam client, such as a different user or administration access level.
		// [*] Ensure that you own a license for the App ID on the currently active Steam account. Your game must show up in your Steam library.
		// [*] Your App ID is not completely set up, i.e. in Release State: Unavailable, or it's missing default packages.
		// Valve's documentation for this is located here:
		// https://partner.steamgames.com/doc/sdk/api#initialization_and_shutdown
		this.isInitialized = SteamAPI.Init();
		if (!this.isInitialized) {
			Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", this);
			return;
		}

		// Set up our callback to receive warning messages from Steam.
		// You must launch with "-debug_steamapi" in the launch args to receive warnings.
		this._steamAPIWarningMessageHook = new SteamAPIWarningMessageHook_t(SteamAPIDebugTextHook);
		SteamClient.SetWarningMessageHook(this._steamAPIWarningMessageHook);

		this.appID = new AppId_t(appID);

		Debug.Log("[Steamworks.NET] Initialized");
	}

	public AppId_t GetAppID () {
		return this.appID;
	}

	/** 取得玩家ID */
	public string GetAccountID () {
		if (this.isInitialized == false) return null;
		return SteamUser.GetSteamID().GetAccountID().ToString();
	}

	/** 取得 語言 */
	public string GetLanguage () {
		if (this.isInitialized == false) return null;
		return SteamApps.GetCurrentGameLanguage();
	}

	/** 設置 遊玩狀態 */
	// 特殊Key : 當地化token
	// 當地化token : 當地化文字(含 %變數%)
	// %變數% : 由SetRichPresence所設置的內容
	public bool SetRichPresence (string str) {
		if (this.isInitialized == false) return false;

		bool res_steam = false;
		
		res_steam = SteamFriends.SetRichPresence(RichPresenceKey_Steam.Display_CustomAny, (str == null ? "" : str));
		if (!res_steam) return res_steam;

		if (str == null || str == "") {
			res_steam = SteamFriends.SetRichPresence(RichPresenceKey_Steam.Display, null);
		} else {
			res_steam = SteamFriends.SetRichPresence(RichPresenceKey_Steam.Display, RichPresenceKey_Steam.Display_Content);
		}

		// Debug.Log("SteamFriends.SetRichPresence("+RichPresenceKey_Steam.Display+", \""+str+"\") = "+res_steam);

		return res_steam;
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}

#endif