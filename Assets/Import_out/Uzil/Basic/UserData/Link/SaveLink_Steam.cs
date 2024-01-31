#if STEAM

using System.Collections.Generic;
using UnityEngine;

using Uzil.ThirdParty.Steam;


namespace Uzil.UserData {

public class SaveLink_Steam : SaveLink {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*========================================Interface==========================================*/

	/** 同步 到 */
	public override void SyncTo () {
		if (SteamInst.Inst().isInitialized == false) return;
        // TODO:
		
	}

	/** 同步 自 */
	public override void SyncFrom () {
		if (SteamInst.Inst().isInitialized == false) return;
		// TODO:
	}

	/*=====================================Public Function=======================================*/
	
	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}

#endif