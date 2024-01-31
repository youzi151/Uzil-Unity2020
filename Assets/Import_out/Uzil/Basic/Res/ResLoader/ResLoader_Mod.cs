using System;
using System.Collections.Generic;

using UnityEngine;

using Uzil.Mod;
using Uzil.Util;

namespace Uzil.Res {

/* 
 * 需要搭配 Uzil.Mod 使用
 * 
 */

public class ResLoader_Mod : ResLoader {

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

	/* 卸載資源 */
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

		/*== 音效片段 ==========*/
		if (type == typeof(AudioClip)) {
			
			AudioClip clip = (info.result as ResResult<AudioClip>).value;
			info.result = null;

			clip.UnloadAudioData();
			AudioClip.DestroyImmediate(clip);
			return;
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
		
		ModMgr modMgr = ModMgr.Inst();
		ResResult result = null;

		/*== 字串 ==========*/
		if (type == typeof(string)) {
			// 讀取資源
			byte[] byteData = ModMgr.GetRes(req.path, ModResType.Text);
			string text = ResUtil.text.Create(byteData);

			ResResult<string> result_type = new ResResult<string>(text);			
			result = result_type;
		}

		/*== 貼圖 ==========*/
		else if (type == typeof(Texture)) {
			
			// 讀取資源
			byte[] byteData = ModMgr.GetRes(req.path, ModResType.Texture);
			Texture tex = ResUtil.texture.Create2D(byteData);

			ResResult<Texture> result_type = new ResResult<Texture>(tex);
			result = result_type;
		}

		/*== 音效片段 ==========*/
		else if (type == typeof(AudioClip)) {

			AudioClip clip = null;

			// 讀取資源
			byte[] byteData = ModMgr.GetRes(req.path, ModResType.Audio);

			DictSO args = req.args;
			if (args.ContainsKey("name") == false) {
				args.Add("name", req.path);
			}

			string ext = PathUtil.GetExtension(req.path);
			if (ext == null || ext == "") {
				List<string> extList = modMgr.FindModResExt(req.path);
				foreach (string eachExt in extList) {
					clip = ResUtil.audio.Create(byteData, eachExt, args);
					if (clip != null) {
						break;
					}
				}
			} else {
				clip = ResUtil.audio.Create(byteData, ext, args);
			}
			
			ResResult<AudioClip> result_type = new ResResult<AudioClip>(clip);
			result = result_type;
		}

		/*== 遊戲物件 ==========*/
		else if (type == typeof(GameObject)) {
			// 不支援
		}

		if (result == null) {
			throw new ResNotFoundException(req);
		}

		if (this.path2Result.ContainsKey(req.path) == false && result.IsValueExist() ) {
			this.path2Result.Add(req.path, result);
		}

		return result;
	}

	/* 取得資源 非同步 */
	public override async void GetAsync (ResReq req, Action<ResResult> cb) {

		// 若 非強制重載 且 已存在 該資源路徑 的 註冊 則 返回
		if (!req.isForceReload && this.path2Result.ContainsKey(req.path)) {
			cb(this.path2Result[req.path]);
			return;
		}

		System.Type type = req.type;
		if (ResUtil.IsSupportType(type) == false) {
			throw new ResTypeNotSupportException(type);
		}

		ModMgr modMgr = ModMgr.Inst();

		Action<ResResult> onFoundRes = (result)=>{
			
			// 若已經取得過 (可能在非同步時另外有同步讀取完成)
			if (this.path2Result.ContainsKey(req.path)) {
				result = this.path2Result[req.path];
			} else {
				this.path2Result.Add(req.path, result);
			}

			cb(result);
		};

		/*== 音效片段 ==========*/
		if (type == typeof(AudioClip)) {
			
			AudioClip clip = null;

			// 讀取資源
			byte[] byteData = await ModMgr.GetResAsync(req.path, ModResType.Audio);

			List<string> extList;
			string ext = PathUtil.GetExtension(req.path);
			if (ext == null || ext == "") {
				extList = modMgr.FindModResExt(req.path);
			} else {
				extList = new List<string>(){ext};
			}

			DictSO arg = new DictSO().Set("name", req.path);

			Async.EachSeries<string>(extList, (eachExt, next)=>{
				
				ResUtil.audio.CreateAsync(byteData, eachExt, arg, (_clip)=>{
					
					if (_clip != null) {
						clip = _clip;
						next(false);
						return;
					}

					next(true);
				});

			}, ()=>{

				ResResult<AudioClip> result_type = null;
				if (clip != null) {
					result_type = new ResResult<AudioClip>(clip);
				}

				onFoundRes(result_type);
			});

			
		}


		// 沒有非同步方式 則 以同步方式取得
		else {
			base.GetAsync(req, cb);
		}
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

}


}