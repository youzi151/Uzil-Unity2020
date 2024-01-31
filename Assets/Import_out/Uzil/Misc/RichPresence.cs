#if STEAM
using Uzil.ThirdParty.Steam;
#endif

#if DISCORD
using Uzil.ThirdParty.Discord;
#endif

namespace Uzil.Misc.RichPresence {

public class RichPresence {

	public const string DefaultIcon = "default";

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/** 設置 一般狀態 */
	public static void SetGeneral (string str) {
		

#if STEAM
		SteamInst.Inst().SetRichPresence(str);
#endif


#if DISCORD
		DiscordInst.Inst().RequestActivity((activity)=>{
			activity.State = str;
			return activity;
		}
		// , (res)=>{
		// 	UnityEngine.Debug.Log("Discord.SetActivity(\""+str+"\") = "+res);
		// }
		);
#endif

	}

	/** 設置 大圖標 */
	public static void SetLargeImage (string iconID) {
#if DISCORD
		DiscordInst.Inst().RequestActivity((activity)=>{
			activity.Assets.LargeImage = iconID;
			return activity;
		});
#endif
	}

	/** 設置 小圖標 */
	public static void SetSmallImage (string iconID) {
#if DISCORD
		DiscordInst.Inst().RequestActivity((activity)=>{
			activity.Assets.SmallImage = iconID;
			return activity;
		});
#endif
	}

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

}



}