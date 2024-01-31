#undef ISDEBUG

using System;
using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;

using Uzil.BuiltinUtil;

/**
 *
 * Preload <-> Unload
 * Hold <-> Unhold
 */


namespace Uzil.Res {

public class ResMgr {

	protected class ToUnhold {
		public ResInfo info;
		public object user;
	}

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/** 是否初始化 */
	protected static bool isInited = false;

	/** 資源識別(path):資源資訊 */
	protected static List<ResInfo> infos = new List<ResInfo>();

	/** 預先載入的資源 */
	protected static List<ResInfo> preloaded = new List<ResInfo>();

	/** 準備要釋放的資源識別 */
	protected static List<ToUnhold> toUnhold = new List<ToUnhold>();
	protected static string invokerKey_unhold = "ResMgr.unholdOnFrame";

	/** 讀取器 */
	protected static List<ResLoader> loaders = new List<ResLoader>();

	/** 自動卸載 最大秒數 */
	protected static float unholdAutoMaxTime_sec = 0.05f;
	

	/*=====================================Static Funciton=======================================*/

	/** 初始化 */
	public static void Init () {
		if (ResMgr.isInited) return;

		// 添加 內建讀取器
		ResMgr.AddLoader(new ResLoader_File());
		ResMgr.AddLoader(new ResLoader_Mod());
		ResMgr.AddLoader(new ResLoader_AssetBundle());
		ResMgr.AddLoader(new ResLoader_Resources());

		ResMgr.isInited = true;
	}

	/*== 讀取器 ===========================*/

	/** 加入Loader */
	public static void AddLoader (ResLoader loader) {
		if (ResMgr.loaders.Contains(loader)) return;
		ResMgr.loaders.Add(loader);
	}

	/*== 資源管理 =========================*/

	/** 預載資源 */
	// 只有在 尚未存在在Info時 (沒有被使用過 或 已經卸載) 會實際對loader請求Preload
	public static void Perload (ResReq req, Action<ResInfo> cb) {
		
		// 檢查是否已存在相關資源資訊
		ResInfo exist = ResMgr.findExist(req);
		
		// 若 存在 且 沒有指定強制重載
		if (exist != null && !req.isForceReload) {
			
			// 若 尚未預載 則 加入
			if (ResMgr.preloaded.Contains(exist) == false) {
				ResMgr.preloaded.Add(exist);
			}

			return;
		}

		// 開始預載 ===================

		ResInfo resInfo = null;
		ResResult resResult = null;

		// 依序 讀取每個 讀取器
		Async.EachSeries<ResLoader>(ResMgr.loaders, (eachLoader, next)=>{
			// 預載資源
			eachLoader.Preload(req, (isPreload, preloadedResult)=>{

				// 若沒有預載 則 下一個Loader
				if (isPreload == false) {
					next(true);
					return;
				} 

				// 若有回傳 資源資訊
				if (preloadedResult != null) {
					resResult = preloadedResult;
				}

				// 不繼續
				next(false);
			});
		}, ()=>{

			// 再次檢查 是否已經存在資源資訊
			ResInfo secondCheckExist = ResMgr.findExist(req);

			// 若 沒有讀取到 且 再次檢查也沒有現有 則 回傳 空
			if (resResult == null && secondCheckExist == null) {
				cb(null);
				return;
			}

			// 若 再次檢查時 已經存在資源資訊
			if (secondCheckExist != null) {
				// 取代 本次預載的 為 已經存在的
				resInfo = secondCheckExist;
			}
			// 若不存在 則 把 讀取到的加入 資源資訊
			else {
				resInfo = new ResInfo(req);
				resInfo.result = resResult;
				ResMgr.infos.Add(resInfo);
			}

			// 若 預載列表中 沒有 本次預載的 則 加入 預載列表
			if (ResMgr.preloaded.Contains(resInfo) == false) {
				ResMgr.preloaded.Add(resInfo);
			}

			cb(resInfo);
		});
	}

	/** 卸載資源 */
	public static void Unload (ResInfo inputInfo) {

		// 檢查是否已存在相關資源資訊
		ResInfo exist = ResMgr.findExist(inputInfo);
		// 若 不存在 則 返回
		if (exist == null) return;

		// 若 不是預載 則 返回
		if (ResMgr.preloaded.Contains(exist) == false) return; 

		// 從 已經預載 中 移除
		ResMgr.preloaded.Remove(exist);

		// 更新 檢查資源是否存活
		ResMgr.UpdateResAlive(new List<ResInfo>(){exist});
	}

	/** 卸載資源 (自動處理未使用的tag) */
	public static void UpdateResAlive (List<ResInfo> checkRange = null) {
		List<ResInfo> toUnload = new List<ResInfo>();

		// 若沒有指定範圍 則 設為 全部現有資源資訊
		if (checkRange == null) checkRange = ResMgr.infos;

		// 每一個要檢查的資源資訊
		foreach (ResInfo eachInfo in checkRange) {

			// 若 在預載列表中 則 忽略
			if (ResMgr.preloaded.Contains(eachInfo)) continue;
			// 若 仍有使用者 則 忽略
			if (eachInfo.users.Count > 0) continue;

			// 否則 加入要卸載的列表
			toUnload.Add(eachInfo);
		}

		// 每個 要卸載的
		foreach (ResInfo each in toUnload) {
			ResMgr.unloadRes(each);
		}
	}

	protected static void unloadRes (ResInfo resInfo) {
		resInfo.Unload(/* isSkipUserCountCheck */true);
		ResMgr.infos.Remove(resInfo);
		// // 依序 每個 讀取器
		// for (int idx = 0; idx < ResMgr.loaders.Count; idx++) {
		// 	ResLoader eachLoader = ResMgr.loaders[idx];
		// 	eachLoader.Unload(resInfo);
		// }
	}

	/*== 資源取得 =========================*/

	/** 取得/持有資源 */
	// getRaw 取得原始資料 (若有多層以上原始資料時用到，例如 取得Sprite, Sprite還要取得Texture)
	public static ResInfo Hold (ResReq req, Func<ResReq, ResInfo> loadRes = null) {
		// 若 尚未初始化 則 初始化
		if (ResMgr.isInited == false) ResMgr.Init();

		if (loadRes == null) loadRes = ResMgr.loadRes;
		
		// 若 要求不合法 則 返回 空
		if (req.IsValid() == false) {
			if (req.user == null) throw new Exception("request is not valid, it require user");
			else throw new Exception("request is not valid");
		}

		// 從 當前 正要卸載的 移除
		ToUnhold toUnhold = ResMgr.toUnhold.Find((each)=>{
			return each.user == req.user && each.info.path == req.path;
		});
		if (toUnhold != null) ResMgr.toUnhold.Remove(toUnhold);

		// 目標的資源資訊
		ResInfo targetInfo = null;

		// 檢查是否已存在相關資源資訊
		ResInfo exist = ResMgr.findExist(req);

		Async.Waterfall(
			new List<Action<Action<bool>>>(){
				
				// 試著 取得 已存在
				(next)=>{

					// 若 存在 則 取用
					if (exist != null) {

						// 若 該資訊 已存有 相關資源結果 且 沒有要求強制重讀 則 取用
						if (exist.result != null && !req.isForceReload) {
							
							targetInfo = exist;

							// 不繼續
							next(false);
							return;
						}
						
					}

					next(true);
				},

				// 試著 重新讀取
				(next)=>{
					targetInfo = loadRes(req);
						
					// 若 查無結果 則 回傳 空
					if (targetInfo == null) {
						next(false);
						return;
					}

					// 若 目標的資源資訊 還沒存在 則 註冊該資訊
					if (exist == null) {
						ResMgr.infos.Add(targetInfo);
					}

					next(true);
				}
			},
			()=>{

				if (targetInfo == null) return;

				// 若 該使用者 尚未註冊 在 該資源 則 註冊
				if (targetInfo.users.Contains(req.user) == false) {
					targetInfo.users.Add(req.user);
				}

			}
		);

		// 返回結果
		return targetInfo;
	}

	public static void HoldAsync (ResReq req, Action<ResInfo> cb, Action<ResReq, Action<ResInfo>> loadResAsync = null) {
		if (loadResAsync == null) loadResAsync = ResMgr.loadResAsync;

		// 若 尚未初始化 則 初始化
		if (ResMgr.isInited == false) ResMgr.Init();

		// 若 要求不合法
		if (req.IsValid() == false) {
			if (req.user == null) throw new Exception("request is not valid, it require user");
			else throw new Exception("request is not valid");
		}

		// 目標的資源資訊
		ResInfo targetInfo = null;

		// 檢查是否已存在相關資源資訊
		ResInfo exist = ResMgr.findExist(req);

		Async.Waterfall(
			new List<Action<Action<bool>>>(){
				
				// 試著 取得 已存在
				(next)=>{
					// 若 存在 則 取用
					if (exist != null) {

						// 若 該資訊 已存有 相關資源結果 則 取用
						if (exist.result != null && !req.isForceReload) {
							
							targetInfo = exist;

							// 不繼續
							next(false);
							return;
						}
						
					}

					next(true);
				},

				// 試著 重新讀取
				(next)=>{

					loadResAsync(req, (result)=>{
						
						// 若 查無結果 則 不繼續
						if (result == null) {
							next(false);
							return;
						}

						targetInfo = result;

						// 再次檢查是否已存在相關資源資訊
						ResInfo secondCheckExist = ResMgr.findExist(req);

						// 若 還沒存在 則 註冊該資訊
						if (secondCheckExist == null) {
							ResMgr.infos.Add(targetInfo);
						}
						// 已經存在 (可能在讀取期間有另一讀取成功) 則 取代
						else {
							targetInfo.Unload();
							targetInfo = secondCheckExist;
						}

						next(true);
					});
				}
			},
			()=>{

				if (targetInfo == null) {
					// Debug.LogError("res not exist: "+req.path);
					cb(null);
					return;
				}

				// 若 該使用者 尚未註冊 在 該資源 則 註冊
				if (targetInfo.users.Contains(req.user) == false) {
					targetInfo.users.Add(req.user);
				}
				
				cb(targetInfo);
			}
		);
		
	}

	/** 尋找 已存在資源資訊 */
	private static ResInfo findExist (ResReq req) {
		foreach (ResInfo eachExist in ResMgr.infos) {
			if (eachExist.IsMatchReq(req)) {
				return eachExist;
			}
		}
		return null;
	}
	private static ResInfo findExist (ResInfo info) {
		foreach (ResInfo eachExist in ResMgr.infos) {
			if (eachExist == info) return eachExist;
			if (eachExist.IsMatchInfo(info)) {
				return eachExist;
			}
		}
		return null;
	}

	protected static ResInfo loadRes (ResReq req) {
		
		// 依序 讀取每個 讀取器
		for (int idx = 0; idx < ResMgr.loaders.Count; idx++) {
			ResLoader eachLoader = ResMgr.loaders[idx];
			ResResult res = null;

			try {
				res = eachLoader.Get(req);
			} catch (System.Exception) {
				continue;
			}

			if (res == null) continue;

			// 若已取得 則 設置結果 並 跳出
			if (res.IsValueExist()) {
				ResInfo info = new ResInfo(req);
				info.result = res;
				info.loader = eachLoader;
				return info;
			}
		}

		return null;
	}

	protected static void loadResAsync (ResReq req, Action<ResInfo> cb) {
		
		ResResult result = null;
		ResLoader loader = null;

		// 依序 讀取每個 讀取器
		Async.EachSeries<ResLoader>(
			ResMgr.loaders,
			// each
			(eachLoader, next)=>{	
				// 非同步 取得
				eachLoader.GetAsync(req, (res)=>{
					// 若有取得到結果
					if (res != null) {
						// 且 存在 值
						if (res.IsValueExist()) {
							// 設置
							result = res;
							loader = eachLoader;
							// 不繼續
							next(false);
							return;
						}
					}
					// 否則 繼續
					next(true);
				});
			},
			// onDone
			()=>{

				if (result == null) {
					cb(null);
					return;
				}

				ResInfo info = new ResInfo(req);
				info.result = result;
				info.loader = loader;
				cb(info);
			}
		);
		
	}

	/** 取消持有 */
	public static void Unhold (ResInfo inputInfo, object user) {
		// 檢查是否已存在相關資源資訊
		ResInfo exist = ResMgr.findExist(inputInfo);
		// 若 不存在 則 返回
		if (exist == null) return;

		// 若 該資源的 使用者 不包含 指定使用者 則 返回
		if (exist.users.Contains(user) == false) return;
		// 移除 資源 的 使用者
		exist.users.Remove(user);
		// 更新
		ResMgr.UpdateResAlive(new List<ResInfo>(){exist});
	}

	protected static void unholdAuto () {
		if (ResMgr.toUnhold.Count == 0) return;
		
		// 開始計時
		float start = Time.realtimeSinceStartup;

		// 若 還有要卸載的
		while (ResMgr.toUnhold.Count > 0) {
			ToUnhold each = ResMgr.toUnhold[0];
			ResMgr.toUnhold.RemoveAt(0);
			
			// 卸載
			ResMgr.Unhold(each.info, each.user);

			// 若 超時
			if ((Time.realtimeSinceStartup - start) > ResMgr.unholdAutoMaxTime_sec) {
				// 呼叫 下一幀 繼續
				Invoker.Inst().Once(ResMgr.unholdAuto);
				return;
			}
		}
	}

	/*== 快捷 ==============*/

	
	/** 取得 (不持有) */
	public static T Get<T> (ResReq req) where T : class {
		req.type = typeof(T);
		req.user = req;
		
		if (req.IsValid() == false) {
			throw new Exception("req not valid");
		}

		ResInfo info = ResMgr.Hold<T>(req);
		if (info == null) {
			return null;
		}

		T res = info.GetResult<T>();

		InvokerOnFrame.Once(ResMgr.invokerKey_unhold, ResMgr.unholdAuto);
		ResMgr.toUnhold.Add(new ToUnhold{info = info, user = req.user});

		return res;
	}

	/* 持有 */
	public static ResInfo Hold<T> (ResReq req) {
		
		if (typeof(T) == typeof(DictSO)) {
			return ResMgr.HoldDictSO(req);
		} else if (typeof(T) == typeof(Sprite)) {
			return ResMgr.HoldSprite(req);
		} else if (typeof(T) == typeof(Texture) || typeof(T) == typeof(Texture2D)) {
			return ResMgr.HoldTexture(req);
		} else if (typeof(T) == typeof(AudioClip)) {
			return ResMgr.HoldAudio(req);
		} else if (typeof(T) == typeof(PhysicsMaterial2D)) {
			return ResMgr.HoldPhysicsMaterial2D(req);
		}

		req.type = typeof(T);
		return ResMgr.Hold(req);
	}

	/** 取得DictSO */
	public static ResInfo HoldDictSO (ResReq req) {
		req.type = typeof(DictSO);

		ResInfo finalRes = ResMgr.Hold(req, (_req)=>{
			
			ResReq req_text = new ResReq(req);
			req_text.user = req;
			
			ResInfo info_text = ResMgr.Hold<string>(req_text);
			if (info_text == null) return null;
			string text = (info_text.result as ResResult<string>).value;
			if (text == null) return null;

			DictSO data = DictSO.Json(text);
			if (data == null) return null;

			ResInfo info = new ResInfo(req);
			info_text.ReplaceUser(req, info);
			
			info.onUnload.Add(()=>{
				ResMgr.Unhold(info_text, info);
			});

			info.result = new ResResult<DictSO>(data);
			
			return info;
		});

		if (finalRes == null) {
			// Debug.LogError("[ResMgr] sprite not found: "+req.path);
			return null;
		}
		
		return finalRes;
	}


	/** 取得貼圖 */
	public static ResInfo HoldTexture (ResReq req) {
		req.type = typeof(Texture);

		ResInfo res = ResMgr.Hold(req);
		if (res == null) {
			// Debug.LogError("[ResMgr] texture not found: "+req.path);
			return null;
		}

		Texture texture = (res.result as ResResult<Texture>).value;

		SetDataUtil.SetTexture(texture, req.args);

		return res;
	}
	
	/** 取得Sprite */
	public static ResInfo HoldSprite (ResReq req) {
		req.type = typeof(Sprite);

		ResInfo finalRes = ResMgr.Hold(req, (_req)=>{
			
			ResReq req_tex = new ResReq(req);
			req_tex.user = req;
			
			ResInfo info_tex = ResMgr.HoldTexture(req_tex);
			if (info_tex == null) return null;
			Texture texture = (info_tex.result as ResResult<Texture>).value;
			if (texture == null) return null;

			Sprite sprite = ResUtil.sprite.Create((Texture2D) texture, req.args);
			if (sprite == null) return null;

			ResInfo info = new ResInfo(req);
			info_tex.ReplaceUser(req, info);
			
			info.onUnload.Add(()=>{
				ResMgr.Unhold(info_tex, info);
			});

			info.result = new ResResult<Sprite>(sprite);
			
			return info;
		});

		if (finalRes == null) {
			// Debug.LogError("[ResMgr] sprite not found: "+req.path);
			return null;
		}
		
		return finalRes;
	}

	/** 取得音效 */
	public static ResInfo HoldAudio (ResReq req) {
		req.type = typeof(AudioClip);

		ResInfo info = ResMgr.Hold(req);
		if (info == null) {
			// Debug.LogError("[ResMgr] audio not found: "+req.path);
			return null;
		}
		return info;
	}
	public static void HoldAudioAsync (ResReq req, Action<ResInfo> cb) {
		req.type = typeof(AudioClip);
		ResMgr.HoldAsync(req, (info)=>{
			if (info == null) {
				// Debug.LogError("[ResMgr] audio not found: "+req.path);
				cb(null);
				return;
			}
			cb(info);
		});
	}

	/** 取得2D物理材質 */
	public static ResInfo HoldPhysicsMaterial2D (ResReq req) {

		req.type = typeof(PhysicsMaterial2D);
		ResInfo info = ResMgr.Hold(req, (_req)=>{

			ResReq req_str = new ResReq(req);
			string str = ResMgr.Get<string>(req_str);
			if (str == null) return null;

			DictSO data = DictSO.Json(str);
			if (data == null) return null;

			PhysicsMaterial2D mat = ResUtil.physicMaterial.Create2D(data.Merge(req.args));

			ResInfo info_mat = new ResInfo(req);
			info_mat.result = new ResResult<PhysicsMaterial2D>(mat);

			return info_mat;
		});

		if (info == null) {
			// Debug.LogError("[ResMgr] PhysicsMaterial2D not found: "+req.path);
			return null;
		}

		return info;
	}


	public static void UnholdOnDestroy (ResInfo resInfo, object user, GameObject gObj) {
		Uzil.Misc.DestroyListener destroyListener = gObj.GetComponent<Uzil.Misc.DestroyListener>();
		if (destroyListener == null) {
			destroyListener = gObj.AddComponent<Uzil.Misc.DestroyListener>();
		}
		destroyListener.onDestroy.Add(()=>{
			ResMgr.Unhold(resInfo, user);
		});
	}

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