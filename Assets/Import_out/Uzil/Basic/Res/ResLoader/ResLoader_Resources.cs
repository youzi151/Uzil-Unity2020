using System;
using System.Collections.Generic;

using UnityEngine;

namespace Uzil.Res {


public class ResLoader_Resources : ResLoader {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/* 路徑對應資源結果 */
	public Dictionary<string, ResResult> path2Result = new Dictionary<string, ResResult>();

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 預載資源 */
	// 回傳 bool 是否已經處理, ResResult 預載的資源資訊
	public override void Preload (ResReq req, Action<bool, ResResult> cb) {
		// 路徑
		string path = req.path;

		// 若 已存在 該資源路徑 的 註冊 則 返回 已經處理
		if (!req.isForceReload && this.path2Result.ContainsKey(path)) {
			cb(true, this.path2Result[path]);
			return;
		}
		
		// 取得資源 (會在取得時連帶註冊)
		ResResult result = this.Get(req);
		if (result == null) {
			cb(false, null);
			return;
		}

		// 回傳 已經處理
		cb(true, result);
	}

	/* 卸載資源 (回傳是否已處理) */
	public override void Unload (ResInfo info) {
		// 路徑
		string path = info.path;
		// 若 尚未存在 該資源路徑 的 註冊 則 返回 沒有處理
		if (this.path2Result.ContainsKey(path) == false) return;
		
		// 若 結果為空 則 返回 沒有處理
		if (this.path2Result[path] == null) return;

		// 移除該紀錄
		this.path2Result.Remove(path);

		if (info.result == null) return;

		if (info.result.IsValueExist() == false) {
			return;
		}

		System.Type type = info.type;
		if (ResUtil.IsSupportType(type) == false) {
			throw new ResTypeNotSupportException(type);
		}

		UnityEngine.Object asset = null;

		/*== 音效片段 ==========*/
		if (type == typeof(AudioClip)) {
			asset = (info.result as ResResult<AudioClip>).value;
		}

		//============================

		if (asset == null) {
			Resources.UnloadAsset(asset);
		}
	}

	/* 取得資源 */
	public override ResResult Get (ResReq req) {
		
		// 若 非強制重載 且 已存在 該資源路徑 的 註冊 則 返回
		if (!req.isForceReload && this.path2Result.ContainsKey(req.path)) {
			return this.path2Result[req.path];
		}
		
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
			// 讀取資源 (使用內建方法)
			ResResult<Texture> result_type = new ResResult<Texture>(this.loadTexture(req));
			result = result_type;
		}

		/*== 音效片段 ==========*/
		else if (type == typeof(AudioClip)) {
			// 讀取資源
			ResResult<AudioClip> result_type = new ResResult<AudioClip>(this.loadAudio(req));
			result = result_type;
		}

		/*== 遊戲物件 ==========*/
		else if (type == typeof(GameObject)) {
			// 讀取資源
			ResResult<GameObject> result_type = new ResResult<GameObject>(this.loadGameObject(req));
			result = result_type;
		}

		if (result == null) {
			throw new ResNotFoundException(req);
		}

		if (this.path2Result.ContainsKey(req.path) == false) {
			this.path2Result.Add(req.path, result);
		}

		return result;
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

	/* 讀取文字檔 */
	private string loadText (ResReq req) {
		TextAsset text = Resources.Load<TextAsset>(req.path);
		if (text == null) return null;
		return text.text;
	}

	/* 讀取貼圖 */
	private Texture loadTexture (ResReq req) {
		Texture tex = Resources.Load<Texture>(req.path);
		return tex;
	}

	/* 讀取音效 */
	private AudioClip loadAudio (ResReq req) {
		AudioClip audioClip = Resources.Load<AudioClip>(req.path);
		return audioClip;
	}

	private GameObject loadGameObject (ResReq req) {
		GameObject gameObject = Resources.Load<GameObject>(req.path);
		return gameObject;
	}

}


}