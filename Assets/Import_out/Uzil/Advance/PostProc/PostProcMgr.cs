using System;
using System.Collections.Generic;

using Uzil;
using Uzil.Values;

namespace Uzil.PostProc {
public class PostProcMgr {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/** 預設Key */
	public const string defaultKey = "_default";

	/** key:實體 */
	private static Dictionary<string, PostProcMgr> key2Instance = new Dictionary<string, PostProcMgr>();
	
	/*=====================================Static Funciton=======================================*/

	/** 取得實體 */
	public static PostProcMgr Inst (string key = null) {
		if (key == null) key = PostProcMgr.defaultKey;
		
		PostProcMgr instance = null;

		// 若 實體存在 則 取用
		if (PostProcMgr.key2Instance.ContainsKey(key)) {
			instance = PostProcMgr.key2Instance[key];
		}
		// 否則 建立
		else {
			instance = new PostProcMgr();
			instance.id = key;
			PostProcMgr.key2Instance.Add(key, instance);
		}

		return instance;
	}

	/** 銷毀 */
	public static void DestroyInst (string key = null) {
		if (key == null) key = PostProcMgr.defaultKey;

		if (PostProcMgr.key2Instance.ContainsKey(key) == false) return;
		PostProcMgr.key2Instance.Remove(key);
	}

	/*=========================================Members===========================================*/

	public string id { get; protected set; }

	public Dictionary<string, Dictionary<string, Vals>> effect2Params = new Dictionary<string, Dictionary<string, Vals>>();

	public Dictionary<string, float> user2Priority = new Dictionary<string, float>();

	/*========================================Components=========================================*/

	public List<PostProcHandler> postProcHandlers = new List<PostProcHandler>();

	/*==========================================Event============================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 設置 優先順序 */
	public void SetPriority (string userName, float priority) {
		
		if (this.user2Priority.ContainsKey(userName)) {
			if (priority != 0) {
				this.user2Priority[userName] = priority;	
			} else {
				this.user2Priority.Remove(userName);
			}
		} else {
			if (priority != 0) {
				this.user2Priority.Add(userName, priority);
			}
		}

		foreach (KeyValuePair<string, Dictionary<string, Vals>> pair in this.effect2Params) {
			foreach (KeyValuePair<string, Vals> pair2 in pair.Value) {
				Vals vals = pair2.Value;
				vals.SetPriority(userName, priority);
			}
		}
	}

	/** 設置 效果參數 */
	public void SetEffect (string userName, string effect, DictSO data) {
		if (userName == null || effect == null) return;

		Dictionary<string, Vals> effectParams;
		if (this.effect2Params.ContainsKey(effect)) {
			effectParams = this.effect2Params[effect];
		} else {
			effectParams = new Dictionary<string, Vals>();
			this.effect2Params.Add(effect, effectParams);
		}

		float priority = 0f;
		if (this.user2Priority.ContainsKey(userName)) {
			priority = this.user2Priority[userName];
		}

		foreach (KeyValuePair<string, object> pair in data) {
			string paramName = pair.Key;
			Vals values;
			if (effectParams.ContainsKey(paramName)) {
				values = effectParams[paramName];
			} else {
				values = new Vals(null);
				effectParams.Add(paramName, values);
			}

			values.Set(userName, priority, pair.Value);
		}

	}

	/** 設置 效果參數 */
	public void RemoveEffects (string userName) {
		if (userName == null) return;
		
		List<string> toRmEffect = new List<string>();
		foreach (KeyValuePair<string, Dictionary<string, Vals>> pair_effectParams in this.effect2Params) {

			string effect = pair_effectParams.Key;
			Dictionary<string, Vals> effectParams = pair_effectParams.Value;

			List<string> toRmParam = new List<string>();
			foreach (KeyValuePair<string, Vals> pair in effectParams) {
				Vals values = pair.Value;
				values.Remove(userName);
				if (values.GetCount() == 0) {
					toRmParam.Add(pair.Key);
				}
			}

			foreach (string each in toRmParam) {
				effectParams.Remove(each);
			}

			if (effectParams.Count == 0) {
				toRmEffect.Add(effect);
			}
		}

		foreach (string each in toRmEffect) {
			this.effect2Params.Remove(each);
		}

	}

	/** 更新 效果 */
	public void UpdateEffect () {
		// UnityEngine.Debug.Log(DictSO.ToJson(this.effect2Params));
		for (int idx = 0; idx < this.postProcHandlers.Count; idx++) {
			PostProcHandler each = this.postProcHandlers[idx];
			each.HandleEffect(this.effect2Params);
		}
	}


	

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/
}


}