using System;
using System.Collections.Generic;
using UnityEngine;

namespace Uzil.Values {

public class Vals : IMemoable {

	
	/*======================================Constructor==========================================*/

	public Vals (object defaultValue) {
		this.defaultValue = defaultValue;
	}

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/* 預設值 */
	public object defaultValue = null;

	/* 使用者請求列表 */
	protected List<Vals_User> users = new List<Vals_User>();

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/
	
	/*========================================Interface==========================================*/

	/* [IMemoable] 讀取Json格式 */
	public void LoadMemo (object memo) {
		DictSO data = DictSO.Json(memo);

		/* 預設值 */
		if (data.ContainsKey("default")) {
			this.defaultValue = data.Get("default");
		}

		/* 使用者列表 */
		this.users.Clear();
		if (data.ContainsKey("users")) {

			List<DictSO> users = data.GetList<DictSO>("users");
			foreach (DictSO eachUserData in users) {

				Vals_User eachUser = new Vals_User();
				
				eachUser.LoadMemo(eachUserData);
				
				this.users.Add(eachUser);
			}
		}
	}

	/* [IMemoable] 紀錄為Json格式 */
	public object ToMemo () {
		DictSO memo = DictSO.New();

		/* 預設值 */
		if (DictSO.IsJsonable(this.defaultValue)) {
			memo.Set("default", this.defaultValue);
		} else {
			Debug.Log("[AskValueList]: can't memo type:"+this.defaultValue.GetType());
		}

		/* 使用者列表 */
		List<object> users = new List<object>();
		foreach (Vals_User each in this.users) {
			users.Add(each.ToMemo());
		}
		memo.Set("users", users);

		return memo;
	}

	/*=====================================Public Function=======================================*/
	
	/** 取得數量 */
	public int GetCount () {
		return this.users.Count;
	}

	/** 取得當前數值 */
	public object GetCurrent () {
		if (this.users.Count <= 0) return this.defaultValue;
		return this.users[0].value;
	}

	/** 取得當前 使用者請求 */
	public Vals_User GetCurrentUser () {
		if (this.users.Count <= 0) return null;
		return this.users[0];
	}
	
	/** 取得 使用者請求 */
	public Vals_User Get (string name) {
		foreach (Vals_User each in this.users) {
			if (each.name == name) return each;
		}
		return null;
	}

	/** 設置 使用者請求 (不存在則建立) */
	public void Set (string name, float priority, object value) {
		Vals_User user = this.Get(name);
		if (user == null) {
			user = new Vals_User(name, priority, value);
			this.Add(user); 
		} else {
			user.priority = priority;
			user.value = value;
			this.Sort();
		}
	}


	/** 設置 使用者請求 優先度 */
	public void SetPriority (string name, float priority) {
		Vals_User user = this.Get(name);
		if (user == null) return;
		user.priority = priority;
		this.users.Sort();
	}
	
	/** 設置 使用者請求 值 */
	public void SetValue (string name, object value) {
		Vals_User user = this.Get(name);
		if (user == null) return;
		user.value = value;
	}


	/** 增加 使用者請求 */
	public void Add (Vals_User user) {
		if (this.users.Contains(user)) return;
		this.users.Add(user);
		this.users.Sort();
	}

	/** 移除 使用者請求 */
	public void Remove (string name) {
		List<Vals_User> toRm = new List<Vals_User>();
		foreach (Vals_User each in this.users) {
			if (each.name == name) {
				toRm.Add(each);
			}
		}
		foreach (Vals_User each in toRm) {
			this.users.Remove(each);
		}
	}

	/** 移除 使用者請求 */
	public void Remove (Vals_User user) {
		if (this.users.Contains(user)) return;
		this.users.Add(user);
		this.users.Sort();
	}

	/** 清空 */
	public void Clear () {
		this.users.Clear();
	}

	/** 排序 */
	public void Sort () {
		this.users.Sort();
	}


	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}
