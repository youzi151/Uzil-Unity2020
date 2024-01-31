#if STEAM

using System;
using System.Collections.Generic;

using Steamworks;

using UnityEngine;

namespace Uzil.ThirdParty.Steam {


public class SteamWorkshopUploadResult {
	public bool isUserNeedsToAcceptWorkshopLegalAgreement = false;
	public EResult steamResult = EResult.k_EResultNone;
	public PublishedFileId_t publishedID = default(PublishedFileId_t);
	public bool isSuccess {
		get {
			return this.steamResult == EResult.k_EResultOK || this.steamResult == EResult.k_EResultNone;
		}
	}
}

public class SteamWorkshop {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	protected static SteamWorkshop _inst = null;

	public static SteamWorkshop Inst () {
		if (SteamWorkshop._inst != null) return SteamWorkshop._inst;

		SteamWorkshop inst = new SteamWorkshop();

		SteamWorkshop._inst = inst;
		return SteamWorkshop._inst;
	}

	/*=========================================Members===========================================*/

	/** 目標AppID */
	public AppId_t appID = default(AppId_t);

	protected bool isInited = false;
	protected bool isUploading = false;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	public Action<string, float> onUploadProcessing = null;

	protected Action<CreateItemResult_t, bool> onCreateItemResult = null;
	protected CallResult<CreateItemResult_t> steamCB_onCreateItemResult;
	
	protected Action<SubmitItemUpdateResult_t, bool> onSubmitItemUpdateResult = null;
	protected CallResult<SubmitItemUpdateResult_t> steamCB_onSubmitItemUpdateResult;

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/
	
	public void Init (AppId_t appID) {
		if (this.isInited) return;
		
		this.appID = appID;

		this.isInited = true;
		
	}

	// ########  ##          ###    ##    ## 
	// ##     ## ##         ## ##    ##  ##  
	// ##     ## ##        ##   ##    ####   
	// ########  ##       ##     ##    ##    
	// ##        ##       #########    ##    
	// ##        ##       ##     ##    ##    
	// ##        ######## ##     ##    ##    

	/** 設置 成就 */
	public List<SteamUGCItemInfo> GetSubscribeds () {
		List<SteamUGCItemInfo> result = new List<SteamUGCItemInfo>();

		// 訂閱 的 Mod 數量
		uint steamItemCount = SteamUGC.GetNumSubscribedItems();
		
		if (steamItemCount <= 0) return result;

		// 取得 所有訂閱Mod ID
		PublishedFileId_t[] ids = new PublishedFileId_t[steamItemCount];
		SteamUGC.GetSubscribedItems(ids, steamItemCount);

		// 每一個 訂閱Mod ID
		for (int idx = 0; idx < ids.Length; idx++) {
			PublishedFileId_t itemID = ids[idx];
			
			// 檢查 Mod項目 狀態
			EItemState state = (EItemState) SteamUGC.GetItemState(itemID);
			if ((state & EItemState.k_EItemStateInstalled) != EItemState.k_EItemStateInstalled) continue;

			// 要讀取到的欄位 / 所用參數
			ulong sizeOnDisk;
			string absPath;
			uint folderSize = 1024;
			uint updateTime;

			// 讀取 Mod項目 安裝資訊
			bool isSucess = SteamUGC.GetItemInstallInfo(itemID, out sizeOnDisk, out absPath, folderSize, out updateTime);
			if (!isSucess) continue;

			// 建立 自用資料格式
			SteamUGCItemInfo itemInfo = new SteamUGCItemInfo();
			itemInfo.id = ((ulong)itemID);
			itemInfo.path = absPath;

			result.Add(itemInfo);
		}

		return result;
	}

	// ##     ## ########  ##        #######     ###    ########  
	// ##     ## ##     ## ##       ##     ##   ## ##   ##     ## 
	// ##     ## ##     ## ##       ##     ##  ##   ##  ##     ## 
	// ##     ## ########  ##       ##     ## ##     ## ##     ## 
	// ##     ## ##        ##       ##     ## ######### ##     ## 
	// ##     ## ##        ##       ##     ## ##     ## ##     ## 
	//  #######  ##        ########  #######  ##     ## ########  

	/**
	 * 上傳
	 * 回傳 是否成功"開始"上傳
	 */
	public bool UploadItem (SteamUGCUploadInfo uploadInfo, Action<SteamWorkshopUploadResult> onDone) {
		if (uploadInfo == null) return false;

		// 尚未初始化 / 正在上傳中 / 沒設置APPID 則 返回
		if (this.isInited == false) return false;
		if (this.isUploading) return false;
		if (this.appID == default(AppId_t)) return false;

		if (uploadInfo.IsPublishedIDExist() == false && uploadInfo.isCreateNew == false) {
			Debug.LogError("[SteamWorkshop]: PublishedID field error");
			return false;
		}

		// 設為 上傳中
		this.isUploading = true;

		// 是否 新建立
		bool isNewCreate = uploadInfo.isCreateNew;
		// 是否 首次更新
		bool isFirstUpdate = isNewCreate;
		// 是否 需要同意協議
		bool isUserNeedsToAcceptWorkshopLegalAgreement = false;

		// 結果
		EResult steamResult = EResult.k_EResultNone;

		// 項目ID
		PublishedFileId_t publishedID = default(PublishedFileId_t);

		Async.Waterfall(
			new List<Action<Action<bool>>>{

				// 建立 項目 ===============================
				(next)=>{

					// 若 已有 既存項目ID
					if (!isNewCreate) {
						
						// 設置 項目ID
						publishedID = uploadInfo.publishedID;

						// 下一階段
						next(true);
						return;
					}
					
					Debug.Log("[SteamWorkshop]: SteamUGC.CreateItem");
					
					// 設置 當建立完成後
					this.onCreateItemResult = (res, isFail)=>{
						steamResult = res.m_eResult;

						// 若 有誤
						if (isFail || res.m_eResult != EResult.k_EResultOK) {
							Debug.LogError("fail on create:"+res.m_eResult+" / ioFailure:"+isFail+" / isNeedsAcceptLi...."+res.m_bUserNeedsToAcceptWorkshopLegalAgreement);
							next(false); return;							
						}

						// 若 需要同意協議 則 紀錄
						if (res.m_bUserNeedsToAcceptWorkshopLegalAgreement) {
							isUserNeedsToAcceptWorkshopLegalAgreement = true;
						}

						// 設置 項目ID
						publishedID = res.m_nPublishedFileId;

						// 寫入 至 UploadInfo檔案
						uploadInfo.Rewrite((raw)=>{
							return raw.Replace("<CREATE>", publishedID.ToString());
						}, out uploadInfo);

						// 下一階段
						next(true);
					};

					this.steamCB_onCreateItemResult = CallResult<CreateItemResult_t>.Create((result, isFail)=>{
						this.onCreateItemResult(result, isFail);
					});

					// 呼叫 Steam 建立項目
					SteamAPICall_t apiCall = SteamUGC.CreateItem(this.appID, EWorkshopFileType.k_EWorkshopFileTypeCommunity);
					this.steamCB_onCreateItemResult.Set(apiCall);
				},

				// 更新/設置 項目 ==========================
				(next)=>{

					// 所有 更新
					List<SteamUGCUploadInfo.UpdateInfo> updates = uploadInfo.GetUpdates();

					// 當前更新Handle
					UGCUpdateHandle_t currentHandle = UGCUpdateHandle_t.Invalid;

					// 更新進度 相關
					int currentUpdateIdx = 0;
					int updatesCount = updates.Count;
					ulong byteProcessed;
					ulong byteTotal;

					// 註冊 每幀檢查 更新狀況
					InvokerUpdate updateInst = InvokerUpdate.Inst("_steamwork");
					// updateInst.isDebug = true;
					updateInst.Add(()=>{
						if (this.onUploadProcessing == null) return;

						// 進度 計算
						EItemUpdateStatus status = SteamUGC.GetItemUpdateProgress(currentHandle, out byteProcessed, out byteTotal);
						float byteProcess = byteTotal == 0 ? 0 : (byteProcessed / byteTotal);
						float statusProcess = (((uint)status) + byteProcess) / ((uint)EItemUpdateStatus.k_EItemUpdateStatusCommittingChanges);
						float totalProcess = (currentUpdateIdx + statusProcess) / updatesCount;

						this.onUploadProcessing(status.ToString(), totalProcess);

					}).Tag("_SteamWorkshop.UploadItem.UpdateProcessed");


					// 每個 更新 (非同步依序)
					Async.EachSeries<SteamUGCUploadInfo.UpdateInfo>(
						
						updates,

						(eachUpdate, nextContent)=>{

							// 開始 項目更新
							currentHandle = SteamUGC.StartItemUpdate(this.appID, publishedID);

							// Debug.Log("[SteamWorkshop]: SteamUGC.StartItemUpdate: "+publishedID+" contentIdx: "+currentUpdateIdx);


							// 主要更新 =================
							
							if (eachUpdate.tags != null) {
								SteamUGC.SetItemTags(currentHandle, eachUpdate.tags);
							}

							string previewImgPath = eachUpdate.GetPreviewImagePath();
							if (previewImgPath != null) {
								SteamUGC.SetItemPreview(currentHandle, previewImgPath);
							}

							string contentPath = eachUpdate.GetContentPath();
							if (contentPath != null) {
								SteamUGC.SetItemContent(currentHandle, contentPath);
							}
							
							// 文字更新 =================

							if (eachUpdate.language != null) {
								SteamUGC.SetItemUpdateLanguage(currentHandle, eachUpdate.language);
							}

							if (eachUpdate.title != null) {
								SteamUGC.SetItemTitle(currentHandle, eachUpdate.title);
							}

							if (eachUpdate.description != null) {
								SteamUGC.SetItemDescription(currentHandle, eachUpdate.description);
							}
							
							// 其他更新 =================

							// 若是 首個 更新
							if (isFirstUpdate) {
								isFirstUpdate = false;

								// 若是 建立後初次更新 則 預設 private
								if (isNewCreate) {
									SteamUGC.SetItemVisibility(currentHandle, ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPrivate);
								}
							}

							// ===============================

							// 設置 當完成送出項目更新後
							this.onSubmitItemUpdateResult = (res, isFail)=>{
								if (steamResult == EResult.k_EResultNone) {
									steamResult = res.m_eResult;
								}

								// 若 有誤
								if (isFail || res.m_eResult != EResult.k_EResultOK) {
									Debug.LogError("fail on update["+currentUpdateIdx+"]: "+res.m_eResult);

									steamResult = res.m_eResult;
									
									nextContent(true); return;
								}

								// 若 需要同意協議 則 紀錄
								if (res.m_bUserNeedsToAcceptWorkshopLegalAgreement) {
									isUserNeedsToAcceptWorkshopLegalAgreement = true;
								}

								currentHandle = UGCUpdateHandle_t.Invalid;

								Debug.Log("[SteamWorkshop]: onSubmitItemUpdateResult: "+publishedID+" contentIdx: "+currentUpdateIdx);

								currentUpdateIdx++;

								nextContent(true);
							};

							this.steamCB_onSubmitItemUpdateResult = CallResult<SubmitItemUpdateResult_t>.Create((result, isFail)=>{
								this.onSubmitItemUpdateResult(result, isFail);
							});

							// 送出 上傳
							// Debug.Log("[SteamWorkshop]: SteamUGC.SubmitItemUpdate");

							string contentUpdateNote = isFirstUpdate ? uploadInfo.update_main.contentUpdateNote : null;
							SteamAPICall_t apiCall = SteamUGC.SubmitItemUpdate(currentHandle, contentUpdateNote);
							this.steamCB_onSubmitItemUpdateResult.Set(apiCall);

						}, 
						
						()=>{
							
							Debug.Log("[SteamWorkshop]: Content all Uploaded");
							next(true);
						}
					);

				}

			},

			// 皆完成後
			()=>{
				// 移除 每幀檢查 更新狀況
				InvokerUpdate.Inst("_steamwork").Cancel("_SteamWorkshop.UploadItem.UpdateProcessed");
				
				// 關閉 正在上傳狀態
				this.isUploading = false;

				if (isUserNeedsToAcceptWorkshopLegalAgreement) {
					Debug.Log("[SteamWorkshop]: NeedsToAcceptWorkshopLegalAgreement");
				}

				// 回呼
				onDone(new SteamWorkshopUploadResult() {
					steamResult = steamResult,
					isUserNeedsToAcceptWorkshopLegalAgreement = isUserNeedsToAcceptWorkshopLegalAgreement,
					publishedID = publishedID
				});
			}

		);
		
		return true;
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}

#endif