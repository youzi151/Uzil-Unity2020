using System;
using System.Collections.Generic;


namespace Uzil.Util {

public class UniqID {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/* 預設 */
	public static UniqID main {
		get {
			return UniqID.Inst();
		}
	}

	/* key:實體 */
	public static Dictionary<string, UniqID> key2IDs = new Dictionary<string, UniqID>();

	/*=====================================Static Funciton=======================================*/

	/* 取得實體 */
	public static UniqID Inst (string key = null) {
		if (key == null) key = "_default";
		
		UniqID uniqID = null;
		if (UniqID.key2IDs.ContainsKey(key)) {
			uniqID = UniqID.key2IDs[key];
		} else {
			uniqID = new UniqID();
			UniqID.key2IDs.Add(key, uniqID);
		}
		return uniqID;
	}


	/* 若有重複的ID則修正 */
	public static string Fix (string id, Func<string, bool> passCheck) {
		string fix = "";
		int i = 0;
		while (!passCheck(id + fix)) {
			i++;
			fix = "_" + i;
		}

		return id + fix;
	}

	/*=========================================Members===========================================*/

	/* 前綴 */
	public string prefixStr = "";

	public List<int> usedInt = new List<int>();

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/
	
	/* 請求 */
	public int Request () {
		int id = 0;
		while (this.usedInt.Contains(id)) {
			id++;
		}
		this.usedInt.Add(id);
		return id;
	}

	/* 請求 */
	public string RequestStr () {
		int id = 0;
		while (this.usedInt.Contains(id)) {
			id++;
		}
		this.usedInt.Add(id);
		return this.prefixStr + id.ToString();
	}

	/* 釋放 */
	public void Release (int id) {
		this.usedInt.Remove(id);
	}

	/* 釋放 */
	public void Release (string _str) {
		if (_str == null || _str == "") return;

		int id = -1;

		string str = _str;
		if (this.prefixStr != "" && this.prefixStr != null) {
			str = str.Substring(this.prefixStr.Length, str.Length - this.prefixStr.Length);
			
		}

		if (int.TryParse(str, out id) == false) return;

		this.usedInt.Remove(id);
	}

	public void Clear () {
		this.usedInt.Clear();
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

}



}