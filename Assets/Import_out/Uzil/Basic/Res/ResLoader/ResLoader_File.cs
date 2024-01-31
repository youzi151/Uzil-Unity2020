using System;
using System.Collections.Generic;

using UnityEngine;

using Uzil.Util;

namespace Uzil.Res {


public class ResLoader_File : ResLoader {

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

		// 若 非強制重載 且 已存在 該資源路徑 的 註冊 則 返回 已經處理
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
		
		// 移除該紀錄
		this.path2Result.Remove(path);
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
			ResResult<string> result_type = new ResResult<string>(this.loadText(req));
			result = result_type;
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
			// 不支援
		}

		if (result == null) {
			throw new ResNotFoundException(req);
		}

		if (this.path2Result.ContainsKey(req.path) == false && result.IsValueExist()) {
			this.path2Result.Add(req.path, result);
		}

		return result;
	}

	/* 取得資源 非同步 */
	public override void GetAsync (ResReq req, Action<ResResult> cb) {
		
		// 若 非強制重載 且 已存在 該資源路徑 的 註冊 則 返回
		if (!req.isForceReload && this.path2Result.ContainsKey(req.path)) {
			cb(this.path2Result[req.path]);
			return;
		}

		System.Type type = req.type;
		if (ResUtil.IsSupportType(type) == false) {
			throw new ResTypeNotSupportException(type);
		}

		Action<ResResult> onFoundRes = (result)=>{
			
			// 若已經取得過 (可能在非同步時另外有同步讀取完成)
			if (this.path2Result.ContainsKey(req.path)) {
				result = this.path2Result[req.path];
			} else {
				this.path2Result.Add(req.path, result);
			}

			cb(result);
		};

		ResResult result = null;

		/*== 音效片段 ==========*/
		if (type == typeof(AudioClip)) {

			// 讀取資源
			this.loadAudioAsync(req, (res)=>{
				ResResult<AudioClip> result_type = new ResResult<AudioClip>(res);
				result = result_type;
				onFoundRes(result);
			});

		}

		// 沒有非同步方式 則 以同步方式取得
		else {
			base.GetAsync(req, cb);
		}
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

	/* 讀取文字檔 */
	private string loadText (ResReq req) {
		string filePath = PathUtil.Combine(PathUtil.GetDataPath(), req.path);
		return ResUtil.text.Read(filePath);
	}

	/* 讀取貼圖 */
	private Texture loadTexture (ResReq req) {
		string filePath = PathUtil.Combine(PathUtil.GetDataPath(), req.path);
		return ResUtil.texture.Read2D(filePath);
	}

	/* 讀取音效 */
	private AudioClip loadAudio (ResReq req) {
		string filePath = PathUtil.Combine(PathUtil.GetDataPath(), req.path);
		return ResUtil.audio.Read(filePath);
	}
	private void loadAudioAsync (ResReq req, Action<AudioClip> cb) {
		string filePath = PathUtil.Combine(PathUtil.GetDataPath(), req.path);
		ResUtil.audio.ReadAsync(filePath, (res)=>{
			cb(res);
		});
	}

}


}