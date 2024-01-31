using UnityEngine;
using Uzil.Res;

using Uzil.Util;

using System.Collections.Generic;

/*
	[使用方式]：
	void   UserData.Save(string 完整路徑, string 寫入內容)
	object UserData.Load(string 完整路徑)

	[完整路徑格式]：
	"<#"+檔案路徑(含檔名)+":"+欄位+"#>"

	
	用於讀取檔案與key，例如:
	{
		"key1":123,
		"key2":321
	}

	不支援巢狀資料檔案，例如:
	{
		"key1":{
			"key2":123
		}
	}
	因為寫入時會很麻煩

 */

namespace Uzil.UserData {

public class Save {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/
	
	protected static Dictionary<string, Save> _id2inst = new Dictionary<string, Save>();
	public static Save Inst (string key = "_Save") {
		return Save.Inst<Save>(key);
	}
	public static T Inst<T> (string key) where T:Save, new() {
		if (Save._id2inst.ContainsKey(key)) return (T) Save._id2inst[key];
		Save inst = new T();
		Save._id2inst.Add(key, inst);
		return (T) inst;
	}

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/
	
	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 是否存在 */
	public bool IsExist (string path, string key) {
		PathKey pk = this.handlePathKey(path, key);
		if (!pk.isValid) return false;
		return this.IsExist(pk);
	}
	public bool IsExist (PathKey pk) {
		string path = PathUtil.Combine(PathUtil.GetDataPath(), pk.path);
		
		bool fileExist = ResUtil.text.FindExist(path) != null;
		if (fileExist == false) return false;

		if (pk.key == null) return true;

		PathKey fileOnlyPK = new PathKey(pk);
		fileOnlyPK.key = null;

		DictSO fileObj = DictSO.Json(this.get(fileOnlyPK));
		if (fileObj == null) return false;
		
		return fileObj.ContainsKey(pk.key);
	}

	
	/** 讀取字串 */
	public string GetStr (string path, string key) {
		return this.GetStr(this.handlePathKey(path, key));
	}
	public string GetStr (PathKey pk) {
		object obj = this.get(pk);
		return obj == null? null : DictSO.ToJson(obj);
	}

	/** 讀取數字 */
	public double? GetFloat (string path, string key) {
		return this.GetFloat(this.handlePathKey(path, key));
	}
	public double? GetFloat (PathKey pk) {
		object obj = this.get(pk);
		return obj == null? -1 : DictSO.Double(obj);
	}

	public int? GetInt (string path, string key) {
		return this.GetInt(this.handlePathKey(path, key));
	}
	public int? GetInt (PathKey pk) {
		object obj = this.get(pk);
		return obj == null? -1 : DictSO.Int(obj);
	}

	/** 讀取布林 */
	public bool? GetBool (string path, string key) {
		return this.GetBool(this.handlePathKey(path, key));
	}
	public bool? GetBool (PathKey pk) {
		object obj = this.get(pk);
		return obj == null? false : DictSO.Bool(obj);
	}

	/** 讀取物件 */
	public DictSO GetObj (string path, string key) {
		return this.GetObj(this.handlePathKey(path, key));
	}
	public DictSO GetObj (PathKey pk) {
		object obj = this.get(pk);
		return obj == null? null : DictSO.Json(obj);
	}

	/** 讀取陣列 */
	public List<T> GetList<T> (string path, string key) {
		return this.GetList<T>(this.handlePathKey(path, key));
	}
	public List<T> GetList<T> (PathKey pk) {
		object obj = this.get(pk);
		return obj == null? null : DictSO.List<T>(obj);
	}
	
	/** 設置字串 */
	public bool SetStr (string path, string key, string content) {
		return this.SetStr(this.handlePathKey(path, key), content);
	}
	public bool SetStr (PathKey pk, string content) {
		return this.set(pk, content);
	}

	/** 設置數字 */
	public bool SetNum (string path, string key, float content) {
		return this.SetNum(this.handlePathKey(path, key), content);
	}
	public bool SetNum (PathKey pk, float content) {
		return this.set(pk, content);
	}
	public bool SetNum (string path, string key, int content) {
		return this.SetNum(this.handlePathKey(path, key), content);
	}
	public bool SetNum (PathKey pk, int content) {
		return this.set(pk, content);
	}

	/** 設置布林 */
	public bool SetBool (string path, string key, bool content) {
		return this.SetBool(this.handlePathKey(path, key), content);
	}
	public bool SetBool (PathKey pk, bool content) {
		return this.set(pk, content);
	}

	/** 設置物件 */
	public bool SetObj (string path, string key, DictSO content) {
		return this.SetObj(this.handlePathKey(path, key), content);
	}
	public bool SetObj (PathKey pk, DictSO content) {
		return this.set(pk, content);
	}

	/** 設置陣列 */
	public bool SetList<T> (string path, string key, List<T> content) {
		return this.SetList(this.handlePathKey(path, key), content);
	}
	public bool SetList<T> (PathKey pk, List<T> content) {
		return this.set(pk, content);
	}

	/** 設置其他 */
	public bool Set (string path, string key, object content) {
		return this.Set(this.handlePathKey(path, key), content);
	}
	public bool Set (PathKey pk, object content) {
		return this.set(pk, content);
	}

	/** 同步 到 */
	public void SyncTo () {
		this.syncTo();
	}

	/** 同步 從 */
	public void SyncFrom () {
		this.syncFrom();
	}

	/** 移除 內容 */
	public bool Remove (string path, string key) {
		return this.Remove(this.handlePathKey(path, key));
	}
	public bool Remove (PathKey pk) {
		if (!pk.isValid) return false;

		if (this.IsExist(pk) == false) return false;

		// 讀取
		string fileTxt = this.read(pk.path);

		// 防呆
		if (fileTxt == "" || fileTxt == null) {
			return false;
		}
		
		// 若 無key 則 忽略
		if (pk.key == "" || pk.key == null) {
			return false;
		}
		
		DictSO dict = DictSO.Json(fileTxt);

		if (dict == null) return false;

		dict.Remove(pk.key);

		fileTxt = dict.ToJson();

		//寫入
		this.write(pk.path, fileTxt);

		return true;
	}

	/** 刪除 檔案 */
	public bool Delete (string path) {
		PathKey pk = this.handlePathKey(path, null);
		if (pk.isValid == false) return false;
		
		if (this.IsExist(pk) == false) return false;
		
		return this.delete(path);
	}

	/*===================================Protected Function======================================*/

	/*== protected ===============*/

	/** 設置 內容 */
	protected virtual bool set (PathKey pk, object content) {
		if (!pk.isValid) {
			Debug.LogError("[Save] set fail, invalid path: "+pk.path);
            return false;
		}

		bool isExist = this.IsExist(pk);

		// 試著讀取現有檔案，複/加寫資料，若無則建立
		string fileTxt = this.read(pk.path);
		
		DictSO data = null;

		// 若 有讀取到文字
		if (fileTxt != null && fileTxt != "") {
			// 解析
			data = DictSO.Json(fileTxt);
			
			// 若 解析結果 為 空  但  檔案存在
			if (data == null && isExist) {
				Debug.LogError("[Save] set fail, file corruption or format error");
				return false;
			}
		}

		if (data == null) data = new DictSO();

		if (pk.key == null) {
			fileTxt = content.ToString();
		} else {
			// 加入DictSO
			data.Set(pk.key, content);
			fileTxt = data.ToJson();
		}

		//寫入
		this.write(pk.path, fileTxt);
        return true;

	}

	/** 讀取 內容 */
	protected virtual object get (PathKey pk) {
		// Debug.Log("[Save] try get: "+pk.orinPath);
		if (!pk.isValid) return null;

		// 讀取
		string fileTxt = this.read(pk.path);
		
		// Debug.Log("[Save] try get res: "+fileTxt);

		// 防呆
		if (fileTxt == "" || fileTxt == null) {
			return null;
		}
		// 若 無key則回傳檔案本身
		if (pk.key == "" || pk.key == null) {
			return fileTxt;
		}
		
		DictSO dict = DictSO.Json(fileTxt);

		if (dict == null) return null;

		return dict.Get(pk.key);
	}

	/*== File =================*/

	/** 寫入 檔案 */
	protected virtual void write (string path, string content) {
		// Debug.Log("[Save] write file["+path+"] = "+content);
		ResUtil.text.Write(PathUtil.Combine(PathUtil.GetDataPath(), path), content);
	}

	/** 讀取 檔案 */
	protected virtual string read (string path) {
		// 從 外部檔案 取得
		string str = ResUtil.text.Read(PathUtil.Combine(PathUtil.GetDataPath(), path));
		
		// if (str == null) Debug.LogError("[Save] Fail to read file ["+PathUtil.Combine(PathUtil.GetDataPath(), path)+"]");
		// else Debug.Log("[Save] get file :"+path);

		// 若無法取得 則 從 資源中 取得
		if (str == null) {
			try {
				str = ResMgr.Get<string>(new ResReq().Path(path));
				// Debug.Log("[Save] get res:"+path);
			} catch (System.Exception) {
				// Debug.LogError("[Save] Fail to read res ["+path+"]");
			}
		}

		return str;
	}

	/** 移除檔案 */
	protected virtual bool delete (string path) {
		bool res = ResUtil.Delete(PathUtil.Combine(PathUtil.GetDataPath(), path));
		return res;
	}

	/** 同步 到 */
	protected virtual void syncTo () {

	}

	/** 同步 從 */
	protected virtual void syncFrom () {

	}

	/** 處理 路徑與鍵值 */
	protected virtual PathKey handlePathKey (string path, string key) {
		PathKey pk = new PathKey();
		pk.path = path;
		pk.key = key;
		return pk;
	}

	/*====================================Private Function=======================================*/

}


}