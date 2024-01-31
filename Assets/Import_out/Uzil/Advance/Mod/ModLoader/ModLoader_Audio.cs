using System;
using System.IO;
using System.Collections.Generic;

using UnityEngine;

using Uzil.Res;
using Uzil.Util;
using Uzil.Audio;

namespace Uzil.Mod {

public class ModLoader_Audio : ModLoader {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	public static string[] audioSetFileName = { "audioSet.json", "set.json", "audioSet" };

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 讀取 */
	// 1. 加入 <Mod>/Audio 中 的 所有聲檔 的 模組代表與路徑
	// 2. 尋找 AudioSet 音效組合設定檔
	public override void Load (ModInfo modInfo) {

		string modPath = modInfo.GetModPath();

		bool isFoundAudioSet = false;

		//=======取得檔案名============

		List<string> array = ModViewer.GetModFilesName(modPath);

		foreach (string filePath in array) {
			// 不在資料夾內 則 略過
			if (!filePath.StartsWith(ModUtil.AUDIO_FOLDERNAME)) continue;

			try {
				FileInfo fileInfo = new FileInfo(filePath);

				// 若不為 合法檔名
				if (Array.IndexOf(ResUtil.audioFormat, fileInfo.Extension) == -1) {

					// 若為 設定檔
					if (isFoundAudioSet == false && Array.IndexOf(ModLoader_Audio.audioSetFileName, fileInfo.Name) != -1) {

						// Debug.Log("[ModLoader_Audio]: read audioSet: " + filePath);

						byte[] audioSetData = ModViewer.GetModFile(modPath, filePath);

						// 解析為json、DictSO
						string json = ResUtil.text.Create(audioSetData);
						// 讀入
						this.readAudioSet(json);

						isFoundAudioSet = true;
					}

					continue;
				}

				// 若 為 正常音效檔名

				// 加入路徑清單
				ModMgr.AddRes(new ModRes(modInfo.id, filePath));

			}
			catch (Exception e) {
				Debug.Log(e);
			}
		}

	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

	private void readAudioSet (string json) {

		if (json == null) return;
		DictSO audioSet = DictSO.Json(json);
		if (audioSet == null) return;

		foreach (KeyValuePair<string, object> pair in audioSet) {
			AudioSet.Set(pair.Key, (pair.Value.ToString()));
		}

	}

}



}