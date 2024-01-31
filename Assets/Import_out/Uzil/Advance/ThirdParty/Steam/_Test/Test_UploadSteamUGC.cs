using System.IO;
using System.Collections.Generic;

using UnityEngine;

using Uzil.Util;

using SFB;

#if STEAM
using Steamworks;
using Uzil.ThirdParty.Steam;
#endif

public class Test_UploadSteamUGC : MonoBehaviour {

	
	[SerializeField]
	[TextArea(3, 10)]
	public string statusLog = "";



#if STEAM
	// Start is called before the first frame update
	void Start() {
		SteamInst inst = SteamInst.Inst();
		if (inst.isInitialized == false) return;
		
		SteamWorkshop ugcInst = SteamWorkshop.Inst();
		ugcInst.Init(inst.GetAppID());
		ugcInst.onUploadProcessing = (msg, processed)=>{
			this.statusLog = "["+msg+"]  "+(processed*100)+" / 100%";
		};

	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.F4)) {
			
			SteamInst inst = SteamInst.Inst();
			if (inst.isInitialized == false) return;

			string[] filesPath = StandaloneFileBrowser.OpenFilePanel("select upload.info file...", PathUtil.GetRootPath(), "", false);
			if (filesPath.Length == 0) return;

			SteamUGCUploadInfo uploadInfo = SteamUGCUploadInfo.LoadFile(filesPath[0]);
			
			Debug.Log(filesPath[0]);
			Debug.Log(uploadInfo != null);

			if (uploadInfo == null) return;

			
			SteamWorkshop ugcInst = SteamWorkshop.Inst();
			ugcInst.UploadItem(uploadInfo, (result)=>{
				
				Debug.Log("upload: "+result.isSuccess);

				if (result.isUserNeedsToAcceptWorkshopLegalAgreement) {
					Debug.Log("NeedsToAcceptWorkshopLegalAgreement");
				}
				
				// 導向頁面
				SteamFriends.ActivateGameOverlayToWebPage("steam://url/CommunityFilePage/"+result.publishedID.ToString());
			});
		}
		
	}
#endif
}