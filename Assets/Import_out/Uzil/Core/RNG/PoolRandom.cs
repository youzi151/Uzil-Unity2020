using System;
using System.Collections.Generic;

using Uzil;

namespace Uzil.RNG {

/**
 * 池隨機
 * 設置後會將隨機結果存於池中，每次取得後移除。
 * 用於不重複結果的偽取隨機。
 * 
 * TODO:應有更好的演算寫法
 */


public class PoolRandom : IMemoable {
	public bool isDebug = false;

	/*======================================Constructor==========================================*/

	public PoolRandom (int min = 0, int max = 1) {
		this.min = min;
		this.max = max;

		this.ResetPool();
	}

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/* ID */
	public string id;

	/* 設定是否有發生改變 */
	private bool isChanged = true;

	/* 最小值 */
	private int min = 0;
	
	/* 最大值 */
	private int max = 1;

	/* 種子值 */
	private string seed = null;

	/* 每一個數的比例 */
	private Dictionary<int, int> rateDict = new Dictionary<int, int>();

	/* 產生出的值 */
	private Queue<float> queue = new Queue<float>();

	public bool isPreventRepeat = true;

	/* 最後一個取得的值 */
	//因為有可能第二輪的第一個，與第一輪的最後一個重複，所以要額外檢查
	private int last;
	private bool isLastExist = false;

	private System.Random digitRng = new System.Random();

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/
	
	public object ToMemo () {
		DictSO memo = DictSO.New();

		memo.Set("isChanged", this.isChanged);
		memo.Set("min", this.min);
		memo.Set("max", this.max);
		
		List<DictSO> rate = new List<DictSO>();
		foreach (KeyValuePair<int, int> pair in this.rateDict) {
			rate.Add(new DictSO().Set("num", pair.Key).Set("rate", pair.Value));
		}
		memo.Set("rate", rate);

		memo.Set("queue", this.queue.ToArray());
		memo.Set("isPreventRepeat", this.isPreventRepeat);

		if (this.isLastExist) {
			memo.Set("last", this.last);
		}

		return memo;
	}

	public void LoadMemo (object memo) {
		DictSO data = DictSO.Json(memo);

		if (data.ContainsKey("isChanged")) {
			this.isChanged = data.GetBool("isChanged");
		}
		if (data.ContainsKey("min")) {
			this.min = data.GetInt("min");
		}
		if (data.ContainsKey("max")) {
			this.max = data.GetInt("max");
		}
		if (data.ContainsKey("rate")) {
			this.rateDict.Clear();

			List<DictSO> rates = data.GetList<DictSO>("rate");
			foreach (DictSO each in rates) {
				this.rateDict.Add(each.GetInt("num"), each.GetInt("rate"));
			}
		}
		if (data.ContainsKey("queue")) {
			this.queue = new Queue<float>(data.GetList<float>("queue"));
		}
		if (data.ContainsKey("isPreventRepeat")) {
			this.isPreventRepeat = data.GetBool("isPreventRepeat");
		}

		if (data.ContainsKey("last")) {
			this.last = data.GetInt("last");
			this.isLastExist = true;
		} else {
			this.isLastExist = false;
		}

	}



	/*=====================================Public Function=======================================*/

	/* 重新整理 */
	public void Refresh () {
		if (this.isChanged) {
			this.ResetPool();
			this.isChanged = false;
		}
	}

	/* 重置隨機池 */
	public void ResetPool () {
		this.last = 0;
		this.isLastExist = false;
		this.resetQueue(this.queue);
	}

	/* 下一輪 */
	public void NextRound () {
		this.resetQueue(this.queue);
	}


	/* 取出隨機值 */
	public int GetInt () {
		return (int) Math.Floor(this.GetFloat());
	}

	/* 取出隨機值(float) */
	public float GetFloat () {
		this.Refresh();

		//取得
		float geted = this.getFloat(this.queue);

		//若queue為空，重新填充queue
		if (this.queue.Count <= 0) {
			this.NextRound();
		}

		return geted;
	}

	/* 不改變隨機池的情況下，模擬取出 */
	// 因為getInt有處理一些狀況，故不直接使用Queue.Peek()
	public float PeekFloat () {
		return this.getFloat(new Queue<float>(this.queue.ToArray()));
	}
	/* 調整參數 (需重新一輪NextRound/RefreshPool後才有效)======================*/

	/* 設置範圍 */
	public void SetRange (int newMin, int newMax) {
		if (this.min == newMin && this.max == newMax) return;
		// UnityEngine.Debug.Log("Replace Range("+this.min+", "+this.max+") to ("+newMin+", "+newMax+")");
		this.min = newMin;
		this.max = newMax;

		//標記為已改變
		this.isChanged = true;
	}

	/* 設置比例 */
	public void SetRate (int val, int rate) {
		if (val > this.max || val < this.min) return;

		// 防呆 理應不會發生
		if (this.rateDict.ContainsKey(val) == false){
			this.addVal(val);
		}

		// 檢查是否無改變
		int oldRate = this.rateDict[val];
		if (oldRate == rate) return;

		this.rateDict[val] = rate;

		// 標記為已改變
		this.isChanged = true;
	}

	/* 設置種子 */
	public void SetSeed (string str) {
		string toSet;
		if (str == null) {
			toSet = null;
		} else {
			toSet = this.id + str;
		}

		if (this.seed == toSet) return;
		this.seed = toSet;

		this.resetQueue(this.queue);
	}

	public void ResetRate () {
		this.rateDict.Clear();

		// 標記為已改變
		this.isChanged = true;
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

	private void addVal (int val) {
		// 取得或建立rate
		int rateCount;
		if (!this.rateDict.ContainsKey(val)) {
			this.rateDict.Add(val, 1);
			rateCount = 1;
		} else {
			rateCount = this.rateDict[val];
		}

		//依照rate數量加入
		for (int eachRate = 0; eachRate < rateCount; eachRate++){
			float rng = 10000+eachRate;
			float detail = (this.digitRng.Next((int)rng) / rng);
			this.queue.Enqueue(val+detail);
		}	

	}

	private void addToQueueWithShuffle (List<float> vals) {
		float[] array = vals.ToArray();
		this.shuffle(array);
		foreach (float each in array){
			this.queue.Enqueue(each);
		}
	}

	private float getFloat (Queue<float> _queue) {
		float result = this.min;

		if (_queue.Count <= 0) return result;

		// 從 queue取出
		result = _queue.Dequeue();

		// 若 盡量避免重複
		if (this.isPreventRepeat && this.isLastExist) {

			int maxTry = _queue.Count;
			int result_int = (int) Math.Floor(result);

			// 若 與前一個輸出數字重複
			while (result_int == this.last && maxTry-- > 0) {

				// 則放回最末端
				_queue.Enqueue(result);
				// 取出下一個
				result = _queue.Dequeue();
				result_int = (int) Math.Floor(result);
			}
		}
		
		this.last = (int) Math.Floor(result);
		this.isLastExist = true;
		
		return result;
	}

	private void resetQueue (Queue<float> _queue) {
		if (_queue == null) return;

		if (this.seed != null) {
			this.digitRng = new System.Random(this.seed.GetHashCode());
		} else {
			this.digitRng = new System.Random();
		}

		// 先清空
		_queue.Clear();

		// 範圍內的每一個值 依照自己的比例 加入佇列中
		for (int val = this.min; val <= this.max; val++){
			this.addVal(val);
		}

		// 取出來擾亂
		float[] array = this.shuffle(_queue.ToArray());

		// 清空原本的
		_queue.Clear();

		// 存入
		foreach (float each in array){
			_queue.Enqueue(each);
		}
		
	}

	private float[] shuffle (float[] array)  {  
		System.Random rng;
		if (this.seed != null) {
			rng = new System.Random(this.seed.GetHashCode());
		} else {
			rng = new System.Random();
		}
		
	    int n = array.Length;
	    while (n > 1) {
	        n--;  
	        int k = rng.Next(n + 1);  
	        float val = array[k];
	        array[k] = array[n];  
	        array[n] = val;  
	    }


	    return array;
	}

}



}