using UnityEngine;

using Uzil.Values;
using Uzil.BuiltinUtil;

namespace Uzil {


public class TimeInstance : MonoBehaviour, IMemoable {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	public string key {get; private set;}

	/** 是否運轉中 */
	public bool isTiming = false;

	/** 是否計時中 數值請求表 */
	private Vals isTiming_values = new Vals(false);

	/** 時間比例 */
	public float timeScale {
		get {
			if (this.isTiming == false) return 0;
			return (float) this.timeScale_values.GetCurrent();
		}
	}

	/** 時間比例 數值請求表 */
	private Vals timeScale_values = new Vals(1f);

	/** 當前時間 */
	public float time = 0f;

	/** 起始時間 */
	private float _startTime = 0f;

	/** 當幀時間差 */
	public float deltaTime = 0f;

	/** 前一幀時間 */
	private float _lastTime = 0f;

	/** 前一幀時間到暫停之間的差距 */
	private float _deltaTimeToPause = 0f;

	/** 是否為應用程式要求的暫停 */
	private bool isPauseByApplication = false;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Awake () {
		this._startTime = Time.realtimeSinceStartup;
		this._lastTime = this._startTime;

		this.isTiming_values.Set("_default", 0, false);
		this.isTiming_values.Set("_application", 1, false);
		
		ApplicationEvent.onApplicationPause.Add(()=>{
			this.onAppPause(true);
		});

		ApplicationEvent.onApplicationResume.Add(()=>{
			this.onAppPause(false);
		});
	}
	
	// Update is called once per frame
	void Update () {
		float realTime = Time.realtimeSinceStartup;

		// 當幀時間差
		float realDeltaTime = realTime - this._lastTime;
		this.deltaTime = realDeltaTime * this.timeScale;

		this._lastTime = realTime;

		// 若 計時中 則 推進時間
		if (this.isTiming) {
			this.time += this.deltaTime;
		}

	}

	protected void onAppPause (bool isPause) {
		if (this.key == "_realTime") return;
		if (isPause) this.pause(true);
		else this.resume(true);
	}
	
	/*========================================Interface==========================================*/

	/** [IMemoable] 紀錄為Json格式 */
	public object ToMemo () {
		DictSO memo = DictSO.New();

		/* 是否運轉中 */
		memo.Set("isTiming_values", this.isTiming_values.ToMemo());

		/* 時間比例 數值請求表 */
		memo.Set("timeScale_values", this.timeScale_values.ToMemo());

		/* 當前時間 */
		memo.Set("time", time);
		
		/* 起始時間 */
		memo.Set("startTime", this._startTime);

		return memo;
	}
	
	/** [IMemoable] 讀取Json格式 */
	public void LoadMemo (object memoJson) {
		DictSO data = DictSO.Json(memoJson);

		/* 當前時間 */
		if (data.ContainsKey("time")) {
			this.time = data.GetFloat("time");
		}

		/* 起始時間 */
		if (data.ContainsKey("startTime")) {
			this._startTime = data.GetFloat("startTime");
		}

		/* 時間比例 數值請求表 */
		if (data.ContainsKey("timeScale_AskValueList")) {
			DictSO timeScale_data = data.GetDictSO("timeScale_AskValueList");
			this.timeScale_values.LoadMemo(timeScale_data);
		}

		/* 是否運轉中 */
		if (data.ContainsKey("isTiming_values")) {
			DictSO isTiming_data = data.GetDictSO("isTiming_values");
			this.isTiming_values.LoadMemo(isTiming_data);
			this.updatedIsTiming();
		}

		this.updatedTimeScale();
	}

	void OnDestroy () {
		TimeUtil.Del(this.key);
	}

	/*=====================================Public Function=======================================*/

	public void Init (string key) {
		this.key = key;
		this.Resume();
		this.updatedTimeScale();
	}

	/** 復原計時 */
	public void Resume () {
		this.resume(false);
	}
	protected void resume (bool isPauseByApplication = false) {

		string valuesName = isPauseByApplication ? "_application" : "_default";
		
		// 設為 計時中
		this.isTiming_values.SetValue(valuesName, true);
		this.updatedIsTiming();

		// 若 非計時中 則 返回
		if (!this.isTiming) return;

		// 設 上一幀 為 當前時間往前倒推 暫停前上一幀到暫停時的時間差距
		this._lastTime = Time.realtimeSinceStartup - this._deltaTimeToPause;
		// 歸零 暫停前的時間差距
		this._deltaTimeToPause = 0f;
		// 更新 時間比例
		this.updatedTimeScale();
	}

	/** 暫停計時 */
	public void Pause () {
		this.pause(false);
	}
	protected void pause (bool isPauseByApplication = false) {

		string valuesName = isPauseByApplication ? "_application" : "_default";

		// 設為 非計時中
		this.isTiming_values.SetValue(valuesName, false);
		this.updatedIsTiming();

		// 若 仍為計時中 則 返回
		if (this.isTiming) return;

		// 紀錄 上一幀 到 暫停當下 的 時間差距
		this._deltaTimeToPause = Time.realtimeSinceStartup - this._lastTime;
		// 更新 時間比例
		this.updatedTimeScale();
	}

	/** 設置時間比例 */
	public void SetTimeScale (float timeScale) {
		this.timeScale_values.defaultValue = timeScale;
	}

	/** 設置時間比例 */
	public void SetTimeScale (string userName, float timeScale) {
		Vals_User user = this.timeScale_values.Get(userName);
		if (user == null) {
			user = new Vals_User(userName, timeScale);
			this.timeScale_values.Add(user);
		} else {
			this.timeScale_values.SetValue(userName, timeScale);
		}

		this.updatedTimeScale();
	}

	/** 設置時間比例 */
	public void SetTimeScale (string userName, float priority, float timeScale) {
		Vals_User user = this.timeScale_values.Get(userName);
		if (user == null) {
			user = new Vals_User(userName, priority, timeScale);
			this.timeScale_values.Add(user);
		} else {
			this.timeScale_values.SetPriority(userName, priority);
			this.timeScale_values.SetValue(userName, timeScale);
		}

		this.updatedTimeScale();
	}

	/** 移除時間比例 */
	public void RemoveTimeScale (string userName) {
		
		this.timeScale_values.Remove(userName);

		this.updatedTimeScale();
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

	private void updatedTimeScale () {
		TimeUtil.UpdatedTimeScale(this.key);
	}

	private void updatedIsTiming () {
		this.isTiming = (bool) this.isTiming_values.GetCurrent();
	}

}


}