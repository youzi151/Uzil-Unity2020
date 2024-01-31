#if STEAM

using System;
using System.IO;
using System.Collections.Generic;

using Steamworks;

namespace Uzil.ThirdParty.Steam {


public class SteamUGCUploadInfo {

	
	public class UpdateInfo {

		internal string _uploadInfoPath = "";
		
		// Main =========================
		
		/** 標籤 */
		public List<string> tags {
			get; 
			internal set;
		} = null;
		
		/** 預覽圖片 */
		protected string previewImgPath = null;
		
		/** 內容路徑 */
		protected string contentPath = null;
		
		/** 內容更新註記 */
		public string contentUpdateNote {
			get; 
			internal set;
		} = null;
		
		// Text =========================

		/** 指定語言 */
		public string language {
			get; 
			protected set;
		} = "";
		
		/** 項目標題 */
		public string title {
			get; 
			protected set;
		} = "";
		
		/** 項目敘述 */
		public string description {
			get; 
			protected set;
		} = "";

		/** 取得預覽圖片路徑 */
		public string GetPreviewImagePath () {
			if (this.previewImgPath == null) return null;

			string path = this.previewImgPath.Replace("\\", "/");
			
			if (path.StartsWith("./")) {
				DirectoryInfo rootDirInfo = new DirectoryInfo(this._uploadInfoPath).Parent;
				path = rootDirInfo.FullName+"/"+path.Substring(2);
			}
			
			return path.Replace("\\", "/");;
		}

		/** 取得檔案路徑 */
		public string GetContentPath () {
			if (this.contentPath == null) return null;

			string path = this.contentPath.Replace("\\", "/");
			
			if (path.StartsWith("./")) {
				DirectoryInfo rootDirInfo = new DirectoryInfo(this._uploadInfoPath).Parent;
				path = rootDirInfo.FullName+"/"+path.Substring(2);
			}
			
			return path.Replace("\\", "/");;
		}

		/** 讀取 */
		public void LoadMemo (DictSO data) {
			
			data.TryGetList<string>("tags", (res)=>{
				this.tags = res;
			});

			data.TryGetString("previewImagePath", (res)=>{
				this.previewImgPath = res;
			});

			data.TryGetString("contentPath", (res)=>{
				this.contentPath = res;
			});
			
			data.TryGetString("contentUpdateNote", (res)=>{
				this.contentUpdateNote = res;
			});


			data.TryGetString("language", (res)=>{
				this.language = res;
			});
			data.TryGetString("title", (res)=>{
				this.title = res;
			});
			data.TryGetString("desc", (res)=>{
				this.description = res;
			});
			data.TryGetString("description", (res)=>{
				this.description = res;
			});
		}

	}

	/** 從檔案讀取 */
	public static SteamUGCUploadInfo LoadFile (string filePath) {
		// UnityEngine.Debug.Log("["+filePath+"] isExist? "+File.Exists(filePath));
		if (File.Exists(filePath) == false) return null;

		SteamUGCUploadInfo newOne = new SteamUGCUploadInfo();
		newOne._rawFilePath = filePath;
		newOne._raw = File.ReadAllText(filePath);
		// UnityEngine.Debug.Log(newOne._raw);

		DictSO data = DictSO.Json(newOne._raw, true);
		// UnityEngine.Debug.Log(data != null);
		if (data == null) return null;

		newOne.LoadMemo(data);

		return newOne;
	}

	/** 原始檔案路徑 */
	protected string _rawFilePath = "";

	/** 原始資料 */
	protected string _raw = "";

	/** 項目發布ID */
	public PublishedFileId_t publishedID {
		get; 
		protected set;
	} = default(PublishedFileId_t);

	/** 是否為新建立 */
	public bool isCreateNew = false;

	/** 主要更新 */
	public UpdateInfo update_main {
		get;
		protected set;
	} = null;

	/** 文字更新 */
	public List<UpdateInfo> update_texts {
		get; 
		protected set;
	} = null;

	/** 取得 所有更新 */
	public List<UpdateInfo> GetUpdates () {
		List<UpdateInfo> res = new List<UpdateInfo>();
		if (this.update_main != null) res.Add(this.update_main);
		if (this.update_texts != null) res.AddRange(this.update_texts);
		return res;
	}

	/** 讀取 */
	public void LoadMemo (DictSO data) {
		// 從 資料 讀取 既存 項目ID

		object publishedIDObj = null;
		if (data.ContainsKey("publishedID")) {
			publishedIDObj = data.Get("publishedID");

			if (DictSO.IsNumeric(publishedIDObj) == false) {
				
				string str = publishedIDObj.ToString();

				if (str == "<CREATE>") {

					this.isCreateNew = true;

				} else {

					Int64 id = DictSO.Int64(publishedIDObj);
					if (id != default(Int64)) {
						this.publishedID = new PublishedFileId_t((ulong) id);
					}
				}

			} else {
				Int64 id = DictSO.Int64(publishedIDObj);
				this.publishedID = new PublishedFileId_t((ulong) id);		
			}	
		}

		// 主要更新 ===========

		DictSO mainData = new DictSO();
		
		data.TryGetList<string>("tags", (res)=>{
			mainData.Ad("tags", res);
		});

		data.TryGetString("previewImagePath", (res)=>{
			mainData.Ad("previewImagePath", res);
		});

		data.TryGetString("contentPath", (res)=>{
			mainData.Ad("contentPath", res);
		});
		
		data.TryGetString("contentUpdateNote", (res)=>{
			mainData.Ad("contentUpdateNote", res);
		});

		if (mainData.Count > 0) {
			this.update_main = new UpdateInfo();
			this.update_main._uploadInfoPath = this._rawFilePath;
			this.update_main.LoadMemo(mainData);
		}

		// 文字更新 ===========

		data.TryGetList<DictSO>("texts", (res)=>{
			
			if (this.update_texts == null) {
				this.update_texts = new List<UpdateInfo>();
			} else {
				this.update_texts.Clear();
			}

			foreach (DictSO each in res) {
				UpdateInfo textInfo = new UpdateInfo();
				textInfo._uploadInfoPath = this._rawFilePath;
				textInfo.LoadMemo(each);
				this.update_texts.Add(textInfo);
			}
		});
	}

	/** 改寫 檔案 */
	public void Rewrite (Func<string, string> rewrite, out SteamUGCUploadInfo replace) {
		SteamUGCUploadInfo newOne = new SteamUGCUploadInfo();
		
		string newRaw = rewrite(this._raw);

		DictSO newData = DictSO.Json(newRaw);
		newOne._rawFilePath = this._rawFilePath;
		newOne._raw = newRaw;
		newOne.LoadMemo(newData);

		File.WriteAllText(this._rawFilePath, newRaw);

		replace = newOne;
	}

	/** 是否 發布ID已經存在 */
	public bool IsPublishedIDExist () {
		return this.publishedID != default(PublishedFileId_t);
	}
}

}

#endif