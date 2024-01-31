using UnityEngine;

using Uzil.BuiltinUtil;

namespace Uzil.FX {

public class FXObj : MonoBehaviour, IMemoable {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** ID */
	public string id;

	/** 生存時間 */
	public float lifeTime = -1f;
	protected float leftTime = 1f;

	/** 是否已經播放 */
	public bool isPlaying = false;

	/** 是否能以IMemoable方式 還原 */
	public bool isRestoreable = false;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/** 當銷毀 */
	// NOTE: FXMgr 會掛 回收Recovery 在這邊
	public Event onDone = new Event();

	/*======================================Unity Function=======================================*/

	public virtual void Update () {

		if (this.lifeTime == -1) return;

		this.leftTime -= Time.deltaTime;

		if (this.leftTime > 0) return;

		//停止
		this.Stop();

	}

	/*========================================Interface==========================================*/

	/** [IMemoable] 紀錄為Json格式 */
	public virtual object ToMemo () {
		DictSO data = new DictSO();

		// data.Set("isPlaying", this.isPlaying);

		return data;
	}

	/** [IMemoable] 讀取Json格式 */
	public virtual void LoadMemo (object memoJson) {
		// DictSO data = DictSO.Json(memoJson);
		// if (data == null) return;

		// bool isPlaying_beforeLoad = this.isPlaying;
		// this.isPlaying = data.GetBool("isPlaying");
		// if (isPlaying_beforeLoad == false && this.isPlaying){
		// 	this.Play();
		// }
	}

	/*=====================================Public Function=======================================*/

	/** 初始化 */
	public virtual void Init (DictSO initData) {

		if (initData == null) return;

		// 歸零
		this.transform.localPosition = Vector3.zero;
		this.transform.localRotation = Quaternion.identity;
		this.transform.localScale = Vector3.one;

		// 寫入
		this.setData_base(initData);

		if (initData.ContainsKey("lifeTime")) {
			this.lifeTime = initData.GetFloat("lifeTime");
		}

		if (initData.ContainsKey("onDone")) {

			int callbackID = initData.GetInt("onDone");

			this.onDone.Clear();
			this.onDone.AddListener(new EventListener((data) => {
				UZAPI.Callback.CallLua_cs(callbackID);
			}));
		}

	}

	/** 播放 */
	public virtual void Play (DictSO playData = null) {
		this.leftTime = this.lifeTime;
		this.isPlaying = true;
	}

	/** 停止 */
	public virtual void Stop () {
		if (!this.isPlaying) return;
		this.isPlaying = false;

		this.done();
	}

	/** 終止 */
	public virtual void Terminate () {
		this.done();
	}

	/** 設置資料 */
	public virtual void SetData (DictSO data) {
		this.setData_base(data);
	}

	/** 發送訊息 */
	public virtual void Call (string msg, DictSO args = null) {
		
		string func = msg.ToLower();

		switch (func) {
			case "stop":
				this.Stop();
				break;
		}
	}

	/*===================================Protected Function======================================*/

	protected void done () {
		this.onDone.Call();
		this.onDone.Clear();
	}

	protected void setData_base (DictSO data) {
		SetDataUtil.SetTransform(this.transform, data);
	}

	/*====================================Private Function=======================================*/

}


}
