using System.Collections.Generic;

using UnityEngine;

namespace Uzil {

public class CustomVar : MonoBehaviour {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	public string key {get; private set;}
	

	/* 變數庫 */
	public Dictionary<string, int> dict_int = new Dictionary<string, int>();
	public Dictionary<string, double> dict_float = new Dictionary<string, double>();
	public Dictionary<string, string> dict_str = new Dictionary<string, string>();
	public Dictionary<string, bool> dict_bool = new Dictionary<string, bool>();

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	
	/* 紀錄為Json格式 */
	public object ToMemo () {
		DictSO data = new DictSO();

		data.Set("int", this.toDictSO<int>(this.dict_int));
		data.Set("float", this.toDictSO<double>(this.dict_float));
		data.Set("str", this.toDictSO<string>(this.dict_str));
		data.Set("bool", this.toDictSO<bool>(this.dict_bool));

		return data;
	}

	/* 讀取Json格式 */
	public void LoadMemo (object memoJson) {
		this.Clear();


		DictSO data = DictSO.Json(memoJson);
		if (data == null) return;

		/* int */
		if (data.ContainsKey("int")) {
			DictSO numDict = data.GetDictSO("int");
			foreach (KeyValuePair<string, object> pair in numDict) {
				if (pair.Value == null) continue;
				int val = numDict.GetInt(pair.Key);
				this.dict_float.Add(pair.Key, val);
			}
		}

		/* float */
		if (data.ContainsKey("float")) {
			DictSO numDict = data.GetDictSO("float");
			foreach (KeyValuePair<string, object> pair in numDict) {
				if (pair.Value == null) continue;
				float val = numDict.GetFloat(pair.Key);
				this.dict_float.Add(pair.Key, val);
			}
		}

		/* string */
		if (data.ContainsKey("str")) {
			DictSO strDict = data.GetDictSO("str");
			foreach (KeyValuePair<string, object> pair in strDict) {
				if (pair.Value == null) continue;
				string val = strDict.GetString(pair.Key);
				this.dict_str.Add(pair.Key, val);
			}
		}

		/* bool */
		if (data.ContainsKey("bool")) {
			DictSO boolDict = data.GetDictSO("bool");
			foreach (KeyValuePair<string, object> pair in boolDict) {
				if (pair.Value == null) continue;
				bool val = boolDict.GetBool(pair.Key);
				this.dict_bool.Add(pair.Key, val);
			}
		}

	}

	void OnDestroy() {
		CustomVarUtil.Del(this.key);	
	}

	/*=====================================Public Function=======================================*/

	/* 初始化 */
	public void Init (string key) {
		this.key = key;
		this.Clear();
	}

	/* 詢問變數 */
	public bool Contains (string key) {
		return this.ContainsBool(key) || this.ContainsInt(key) || this.ContainsFloat(key) || this.ContainsStr(key);
	}
	public bool ContainsInt (string key) {
		return this.dict_int.ContainsKey(key);
	}
	public bool ContainsFloat (string key) {
		return this.dict_float.ContainsKey(key);
	}
	public bool ContainsStr (string key) {
		return this.dict_str.ContainsKey(key);
	}
	public bool ContainsBool (string key) {
		return this.dict_bool.ContainsKey(key);
	}
	

	/* 設置變數 */
	public void SetInt (string key, int val) {
		if (this.dict_int.ContainsKey(key)) {
			this.dict_int[key] = val;
		}
		else {
			this.dict_int.Add(key, val);
		}
	}
	public void SetFloat (string key, double val) {
		if (this.dict_float.ContainsKey(key)) {
			this.dict_float[key] = val;
		}
		else {
			this.dict_float.Add(key, val);
		}
	}
	public void SetStr (string key, string val) {
		if (this.dict_str.ContainsKey(key)) {
			this.dict_str[key] = val;
		}
		else {
			this.dict_str.Add(key, val);
		}
	}
	public void SetBool (string key, bool val) {
		if (this.dict_bool.ContainsKey(key)) {
			this.dict_bool[key] = val;
		}
		else {
			this.dict_bool.Add(key, val);
		}
	}

	/* 取得變數 */
	public int GetInt (string key) {
		if (this.dict_int.ContainsKey(key)) {
			return this.dict_int[key];
		}
		else {
			return 0;
		}
	}
	public double GetFloat (string key) {
		if (this.dict_float.ContainsKey(key)) {
			return this.dict_float[key];
		}
		else {
			return 0f;
		}
	}
	public string GetStr (string key) {
		if (this.dict_str.ContainsKey(key)) {
			return this.dict_str[key];
		}
		else {
			return null;
		}
	}
	public bool GetBool (string key) {
		if (this.dict_bool.ContainsKey(key)) {
			return this.dict_bool[key];
		}
		else {
			return false;
		}
	}

	/* 移除變數 */
	public void DelInt (string key) {
		if (this.dict_int.ContainsKey(key)) {
			this.dict_int.Remove(key);
		}
	}
	public void DelFloat (string key) {
		if (this.dict_float.ContainsKey(key)) {
			this.dict_float.Remove(key);
		}
	}
	public void DelStr (string key) {
		if (this.dict_str.ContainsKey(key)) {
			this.dict_str.Remove(key);
		}
	}
	public void DelBool (string key) {
		if (this.dict_bool.ContainsKey(key)) {
			this.dict_bool.Remove(key);
		}
	}

	public void Clear () {
		this.dict_int.Clear();
		this.dict_float.Clear();
		this.dict_str.Clear();
		this.dict_bool.Clear();
	}


	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

	private DictSO toDictSO<T> (Dictionary<string, T> from) {
		DictSO data = DictSO.New();
		foreach (KeyValuePair<string, T> pair in from) {
			data.Set(pair.Key, pair.Value);
		}
		return data;
	}

}

}