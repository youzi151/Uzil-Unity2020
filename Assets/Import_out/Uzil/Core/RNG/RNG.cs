using System.Collections.Generic;

using UnityEngine;

using Uzil;

namespace Uzil.RNG {

/**
 * 可記錄的隨機實例
 * 目前僅持有 池隨機
 * TODO:應有更好的演算寫法
 */


public class RNG : MonoBehaviour, IMemoable {
	public bool isDebug = false;

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	public string key {get; private set;}


	/* ID 對應 隨機池 */
	public Dictionary<string, PoolRandom> id2PoolRandom = new Dictionary<string, PoolRandom>();

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	
    /* 紀錄為Json格式 */
	public object ToMemo () {
		DictSO data = new DictSO();

        DictSO poolRandomData = new DictSO();
		foreach (KeyValuePair<string, PoolRandom> pair in this.id2PoolRandom) {
			poolRandomData.Set(pair.Key, pair.Value.ToMemo());
		}
		data.Set("poolRandom", poolRandomData);
		
		return data;
	}
	
	/* 讀取Json格式 */
	public void LoadMemo (object memoJson) {
		DictSO data = DictSO.Json(memoJson);

		this.id2PoolRandom.Clear();
		if (data.ContainsKey("poolRandom")) {
			DictSO poolRandomData = data.GetDictSO("poolRandom");
			foreach (KeyValuePair<string, object> pair in poolRandomData) {
				PoolRandom poolRandom = new PoolRandom();
				poolRandom.LoadMemo(pair.Value);
				this.id2PoolRandom.Add(pair.Key, poolRandom);
			}
		}
	}
	void OnDestroy() {
		RNGUtil.Del(this.key);	
	}

	/*=====================================Public Function=======================================*/

	/** 初始化 */
	public void Init (string key) {
		this.key = key;
	}

	/** 取得 池隨機 若 不存在 則 建立 */
	public PoolRandom GetPoolRandom (string id) {
		if (this.id2PoolRandom.ContainsKey(id)) {
			return this.id2PoolRandom[id];
		}
		
		PoolRandom pool = new PoolRandom();
		pool.id = id;
		this.id2PoolRandom.Add(id, pool);
		return pool;
	}

	/** 移除 池隨機 */
    public void RemovePoolRandom (string id) {
        if (this.id2PoolRandom.ContainsKey(id) == false) return;
		this.id2PoolRandom.Remove(id);
    }

	public void Clear () {
		this.id2PoolRandom.Clear();
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

}


}