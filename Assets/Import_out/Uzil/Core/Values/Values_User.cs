using System;

namespace Uzil.Values {

/* 使用者請求 */

public class Vals_User : IComparable<Vals_User>, IMemoable {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	public Vals_User () {}

	public Vals_User (string name, object value) {
		this.name = name;
		this.value = value;
	}

	public Vals_User (string name, float priority, object value) {
		this.name = name;
		this.priority = priority;
		this.value = value;
	}

	/*=========================================Members===========================================*/

	/* 名稱 */
	public string name;

	/* 優先度 (越小越先) */
	public float priority = 0f;

	/* 值 */
	public object value = null;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	int IComparable<Vals_User>.CompareTo(Vals_User b) {
		return this.priority.CompareTo(b.priority);
	}

	/* [IMemoable] 讀取Json格式 */
	public void LoadMemo (object memo) {
		DictSO data = DictSO.Json(memo);
		
		/* 名稱 */
		this.name = data.GetString("name");
		
		/* 優先度 */
		this.priority = data.GetFloat("priority");

		/* 值 */
		if (data.ContainsKey("value")) {
			try {
				this.value = data.Get("value");
			} catch (Exception e) {
				UnityEngine.Debug.Log(e);
			}
		}

	}

	/* [IMemoable] 紀錄為Json格式 */
	public object ToMemo () {
		DictSO memo = DictSO.New();
		
		/* 名稱 */
		memo.Set("name", this.name);
		
		/* 優先度 */
		memo.Set("priority", this.priority);
		
		/* 值 */
		if (DictSO.IsJsonable(this.value)) {
			memo.Set("value", this.value);
		} else {
			UnityEngine.Debug.Log("[AskValueList_User]: can't memo type:"+this.value.GetType());
		}

		return memo;
	}

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}
