using System;
using System.IO;
using System.Collections.Generic;

using UnityEngine;

namespace Uzil.Res {


public class ResLoader_AssetBundle : ResLoader {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 預載資源 */
	// 回傳 bool 是否已經處理, ResResult 預載的資源資訊
	public override void Preload (ResReq req, Action<bool, ResResult> cb) {
		List<string> toLoad = this.getAssetBundles(req);
	
		foreach (string each in toLoad) {
			ResAsbMgr.Use(each);
		}
		
		cb(true, null);
	}

	/* 卸載資源 */
	public override void Unload (ResInfo info) {
		List<string> toUnload = this.getAssetBundles(info);
	
		foreach (string each in toUnload) {
			ResAsbMgr.Unuse(each);
		}
	}

	/* 取得資源 */
	public override ResResult Get (ResReq req) {
		System.Type type = req.type;
		if (ResUtil.IsSupportType(type) == false) {
			throw new ResTypeNotSupportException(type);
		}

		ResResult result = null;

		/*== 字串 ==========*/
		if (type == typeof(string)) {
			result = new ResResult<string>(this.loadText(req));
		}

		/*== 貼圖 ==========*/
		else if (type == typeof(Texture)) {
			result = new ResResult<Texture>(this.loadTexture(req));
		}

		/*== 音效片段 ==========*/
		else if (type == typeof(AudioClip)) {
			result =  new ResResult<AudioClip>(this.loadAudio(req));
		}

		/*== 遊戲物件 ==========*/
		else if (type == typeof(GameObject)) {
			result = new ResResult<GameObject>(this.loadGameObject(req));			
		}



		if (result == null) {
			throw new ResNotFoundException(req);
		}

		return result;
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

	/* 讀取文字檔(從AssetBundle) */
	private string loadText (ResReq req) {
		
		// Bundle名稱
		string bundleName = req.GetBundleName();
		if (bundleName == null) return null;

		// 資源檔案名稱
		string fileName = Path.GetFileName(req.path);

		// 呼叫 AssetBundle管理 取用該Bundle
		AssetBundle assetBundle = ResAsbMgr.Use(bundleName);
		if (assetBundle == null) return null;

		// 從AssetBundle中取得資源
		TextAsset text = assetBundle.LoadAsset<TextAsset>(fileName);

		if (text == null) return null;
		
		return text.text;
	}

	private Texture2D loadTexture (ResReq req) {
		
		// Bundle名稱
		string bundleName = req.GetBundleName();
		if (bundleName == null) return null;
		
		// 資源檔案名稱
		string fileName = Path.GetFileName(req.path);

		// 呼叫 AssetBundle管理 取用該Bundle
		AssetBundle assetBundle = ResAsbMgr.Use(bundleName);
		if (assetBundle == null) return null;

		// 從AssetBundle中取得資源
		Texture2D tex = assetBundle.LoadAsset<Texture2D>(fileName);
		
		if (tex == null) return null;
		else return tex;
	}

	private AudioClip loadAudio (ResReq req) {
		
		// Bundle名稱
		string bundleName = req.GetBundleName();
		if (bundleName == null) return null;

		// 資源檔案名稱
		string fileName = Path.GetFileName(req.path);

		// 呼叫 AssetBundle管理 取用該Bundle
		AssetBundle assetBundle = ResAsbMgr.Use(bundleName);
		if (assetBundle == null) return null;

		// 從AssetBundle中取得資源
		AudioClip audioClip = assetBundle.LoadAsset<AudioClip>(fileName);
		
		if (audioClip == null) return null;
		return audioClip;
	}

	private GameObject loadGameObject (ResReq req) {
		
		// Bundle名稱
		string bundleName = req.GetBundleName();
		if (bundleName == null) return null;

		// 資源檔案名稱
		string fileName = Path.GetFileName(req.path);

		// 呼叫 AssetBundle管理 取用該Bundle
		AssetBundle assetBundle = ResAsbMgr.Use(bundleName);
		if (assetBundle == null) return null;

		// 從AssetBundle中取得資源
		GameObject gameObject = assetBundle.LoadAsset<GameObject>(fileName);
		
		if (gameObject == null) return null;
		return gameObject;
	}
	
	/* 從多個Info的tags中 取出 所有AssetBundle的名稱 */
	private List<string> getAssetBundles (ResReq req) {
		return this.getAssetBundles(req.tags);
	}
	private List<string> getAssetBundles (ResInfo info) {
		return this.getAssetBundles(info.tags);
	}

	private List<string> getAssetBundles (List<string> tags) {
		List<string> bundleNames = new List<string>();

		foreach (string tag in tags) {
			if (bundleNames.Contains(tag) == false) {
				if (tag.StartsWith(ResUtil.ASSETBUNDLE_PREFIX)) {
					bundleNames.Add(tag);
				}
			}
		}

		int prefixLength = ResUtil.ASSETBUNDLE_PREFIX.Length;
		for (int idx = 0; idx < bundleNames.Count; idx++) {
			string each = bundleNames[idx];
			bundleNames[idx] = each.Substring(prefixLength, each.Length - prefixLength);
		}

		return bundleNames;
	}
}


}