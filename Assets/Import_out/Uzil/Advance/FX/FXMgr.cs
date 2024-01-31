using System.Collections.Generic;

using UnityEngine;

using Uzil.Util;

namespace Uzil.FX {


public class FXMgr : IMemoable {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/* 單例 */
	public static FXMgr instance;
	public static Transform fxRoot;

	/*=====================================Static Funciton=======================================*/

	/* 單例 */
	public static FXMgr Inst () {
		if (FXMgr.instance == null) {
			FXMgr.instance = new FXMgr();
			FXMgr.fxRoot = RootUtil.GetMember("FXMgr").transform;
		}

		return FXMgr.instance;
	}

	/*=========================================Members===========================================*/

	/* 每一個Prefab的pool */
	private Dictionary<string, Queue<FXObj>> poolDict = new Dictionary<string, Queue<FXObj>>();

	/* ID對應Source名稱 */
	public Dictionary<string, string> id2SourcePathDict = new Dictionary<string, string>();

	/* ID對應Instance */
	public Dictionary<string, FXObj> id2InstanceDict = new Dictionary<string, FXObj>();

	/* SourcePath對應Prefab */
	public Dictionary<string, GameObject> source2PrefabDict = new Dictionary<string, GameObject>();

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** [IMemoable] 紀錄為Json格式 */
	public object ToMemo () {
		DictSO data = new DictSO();

		DictSO fxDict = new DictSO();
		// 所有 當前特效
		foreach (KeyValuePair<string, FXObj> pair in this.id2InstanceDict) {
			// 若 不可還原 則 不保存
			if (pair.Value.isRestoreable == false) continue;

			// 否則 將 特效紀錄為Memo
			DictSO fxData = (DictSO) (pair.Value as IMemoable).ToMemo();

			// 加入來源名稱
			if (this.id2SourcePathDict.ContainsKey(pair.Key)) {
				fxData.Set("_source", this.id2SourcePathDict[pair.Key]);
			}

			// 寫入
			fxDict.Set(pair.Key, fxData);
		}
		data.Set("fxDict", fxDict);

		return data;
	}

	/** [IMemoable] 讀取Json格式 */
	public void LoadMemo (object memoJson) {
		DictSO data = DictSO.Json(memoJson);

		// 清除原有
		// 改在MemoMod呼叫Clear事先清除，確保與其他模塊之間無衝突

		//還原
		if (data.ContainsKey("fxDict")) {
			DictSO fxDict = data.GetDictSO("fxDict");
			foreach (KeyValuePair<string, object> pair in fxDict) {
				DictSO fxData = DictSO.Json(pair.Value);
				if (fxData.ContainsKey("_source") == false) continue;

				string id = pair.Key;
				string sourcePath = fxData.GetString("_source");

				FXObj fx = this.ReuseFX(id, sourcePath);
				(fx as IMemoable).LoadMemo(fxData);

			}
		}

		return;
	}

	/*=====================================Public Function=======================================*/


	/** 取得特效實體 */
	public FXObj GetFX (string id) {
		if (id == null || id == "") return null;
		if (this.id2InstanceDict.ContainsKey(id)) {
			return this.id2InstanceDict[id];
		}
		else {
			return null;
		}
	}

	/** 取用特效物件 */
	public FXObj ReuseFX (string id, string sourcePath, DictSO initData = null) {


		// 若 該特效的pool不存在，則建立
		if (this.poolDict.ContainsKey(sourcePath) == false) {
			this.poolDict.Add(sourcePath, new Queue<FXObj>());
		}

		// 取得該特效的pool
		Queue<FXObj> pool = this.poolDict[sourcePath];


		id = UniqID.Fix(id, (newID) => {
			return this.id2InstanceDict.ContainsKey(newID) == false;
		});

		FXObj fx = null;

		bool isHasInPool = pool.Count > 0;

		// 若pool內還有該特效則取用
		if (isHasInPool) {
			fx = pool.Dequeue();
		}
		// 或沒有 或 取用失敗 則 建立特效
		if (!isHasInPool || fx == null) {
			// Create FX
			fx = this.CreateFX(id, sourcePath);
			if (fx == null) return null;
		}

		
		// 基本設置
		fx.id = id;
		fx.gameObject.name = id;

		// 死亡或銷毀時回收
		EventListener onDone = new EventListener(() => {
			this.Recovery(fx);
		}).Once().ID("FXMgr.onDone");
		fx.onDone += onDone;

		// 初始化
		fx.Init(initData);

		// 加入追蹤記錄
		this.id2SourcePathDict.Add(id, sourcePath);
		this.id2InstanceDict.Add(id, fx);

		return fx;
	}

	/** 回收特效 */
	public void Recovery (FXObj fx) {
		string id = fx.id;

		// 若id不在記錄中  (也避免event call無限循環)
		if (!this.id2SourcePathDict.ContainsKey(id)) return;

		string prefabName = this.id2SourcePathDict[id];
		// 從記錄中移除		
		this.id2SourcePathDict.Remove(id);
		this.id2InstanceDict.Remove(id);
		// 放入該特效的pool
		this.poolDict[prefabName].Enqueue(fx);

		if (fx.isPlaying) {
			fx.Terminate();
		}

		fx.id = "(unUse)";
		// fx.gameObject.name = "(unUse)";
	}


	/** 建立特效 */
	public FXObj CreateFX (string id, string sourcePath) {
		GameObject gObj;
		FXObj fx;

		// 若 還不存在原型 則 建立並放入prefab
		if (!this.source2PrefabDict.ContainsKey(sourcePath)) {
			gObj = (GameObject) Resources.Load(PathUtil.Combine(Const_FX.PREFAB_FX_ROOT, sourcePath));
			if (gObj == null) {
				gObj = (GameObject) Resources.Load(Const_FX.PREFAB_FX_ROOT_PREFIX + sourcePath);
			}
			if (gObj == null) {
				gObj = (GameObject) Resources.Load(sourcePath);
			}
			if (gObj == null) {
				return null;
			}

			this.source2PrefabDict.Add(sourcePath, gObj);
		}

		gObj = GameObject.Instantiate(this.source2PrefabDict[sourcePath]);
		gObj.transform.SetParent(FXMgr.fxRoot, false);
		fx = gObj.GetComponent<FXObj>();

		return fx;
	}

	/** 清除乾淨 */
	public void Clear () {
		// 回收全部
		List<FXObj> toRecovery = new List<FXObj>();
		foreach (KeyValuePair<string, FXObj> pair in this.id2InstanceDict) {
			toRecovery.Add(pair.Value);
		}
		foreach (FXObj each in toRecovery) {
			this.Recovery(each);
		}
	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}


}