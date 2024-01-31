using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Uzil.Res;
using Uzil.i18n;

namespace Uzil.CompExt {

public class ImageExt : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/* 所有實例 */
	private static List<ImageExt> instancePool = new List<ImageExt>();

	/*=====================================Static Funciton=======================================*/

	/** 全域更新 */
	public static void UpdateRender () {
		for (int i = 0; i < ImageExt.instancePool.Count; i++){
			ImageExt.instancePool[i].Rerender();
		}
	}

	/*=========================================Members===========================================*/

	/** 圖片路徑 */
	[Header("Image Src")]
	public string srcPath = "XXX/<$_lang$>/XXX.png";
	protected string lastSrcPath = "XXX/<$_lang$>/XXX.png";

	
	/** 是否立即代換 */
	[Header("<$$> keyword only")]
	public bool isImmediateRenderPath = true;

	[Header("Setting")]
	/* 自動更新間隔 */
	public float updateInterval_sec = -1f;
	
	/* 自動更新倒數 */
	private float counter = 0f;

	/* 是否呼叫代換 */
	private bool isCallRerender = false;

	/* 代換回傳序號 */
	private int callbackIdx = 0;

	private InvokerSerial renderInvokerSerial = new InvokerSerial();

	/* 是否已經銷毀 */
	private bool isDestroyed = false;

	/*========================================Components=========================================*/

	/** 目標 */
	public Image target {
		get {
			if (this._target != null) return this._target;
			this._target = this.GetComponent<Image>();
			return this._target;
		}
	}
	protected Image _target = null;

	protected ResInfo _resInfo = null;

	public List<Image> extraTargets = new List<Image>();

	/* 前置處理 列表< 處理器<傳入文字, 回呼<回傳文字>> > */
	public List<Func<string, string>> beforePipe = new List<Func<string, string>>();

	/* 後期處理 列表< 處理器<傳入文字, 回呼<回傳文字>> > */
	public List<Func<string, string>> afterPipe = new List<Func<string, string>>();

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	public void Awake () {

		// 設定倒數設定
		this.counter = updateInterval_sec > 0? updateInterval_sec : 0;

		// 加入實例
		ImageExt.instancePool.Add(this);

		// 若StrReplacer的當語言變更事件，還未包含"_ImageExt"時，註冊 "語言變更時，會進行全域更新"
		string listenerName = "_ImageExt";
		StrReplacer reader = StrReplacer.Inst();
		if (!reader.onUpdate.Contains(listenerName)) {
			EventListener listener = new EventListener(ImageExt.UpdateRender).ID(listenerName);
			reader.onUpdate += listener;
		}

		this.Rerender();
	}

	public void OnEnable () {
		// 進行代換
		this.Rerender();
	}

	public void Update () {
		if (this.target == null) return;

		// 定期更新
		if (this.updateInterval_sec >= 0) {
			this.counter -= Time.deltaTime;
			if (this.counter <= 0){
				this.Rerender();
				this.counter = this.updateInterval_sec;
			}
		}


		if (this.isCallRerender) {
			this.isCallRerender = false;
			this.render();
		}
	}

	public void LateUpdate() {
		if (this.renderInvokerSerial.taskList.Count == 0) return;
		this.renderInvokerSerial.DoAll();
		this.callbackIdx = 0;
		this.renderInvokerSerial.Clear();
	}

	public void OnDestroy () {
		ImageExt.instancePool.Remove(this);
		this.isDestroyed = true;
		if (this._resInfo != null) {
			ResMgr.Unhold(this._resInfo, this);
		}
	}
	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 重新翻譯 */
	public void Rerender () {
		if (this.isImmediateRenderPath) {
			this.renderImmediate();
		}

		this.isCallRerender = true;
	}

	/*===================================Protected Function======================================*/
	/** 重新代換 */
	protected void renderImmediate () {
		this._renderToAll((src, cb)=>{
			cb(StrReplacer.RenderNow(src));
		});
	}

	protected void render () {
		this._renderToAll((src, cb)=>{
			StrReplacer.Render(src, (res)=>{
				cb(res);
			});
		});
	}

	protected void _renderToAll (Action<string, Action<string>> renderFn) {
		
		// normal
		string src = this.srcPath;
		this._render(
			src,
			renderFn,
			/* check */
			(toReplace)=>{
				// 若來源已經被改為別的內容
				if (this.srcPath != src) return false;
				// 若重複
				if (this.lastSrcPath == toReplace) return false;
				return true;
			},
			/* doWithDst */
			(result)=>{
				this.unhold();
				this._resInfo = ResMgr.Hold<Sprite>(new ResReq().Path(result).User(this));
				Sprite sp = this._resInfo == null? null : this._resInfo.GetResult<Sprite>();
				this._setToTarget((image)=>{
					image.sprite = sp;
				});
			}
		);

	}

	protected void _render (string src, Action<string, Action<string>> renderFunc, Func<string, bool> check,Action<string> doWithDst) {
	
		// 取得 自己的翻譯順序
		int renderIdx = this.callbackIdx;
		this.callbackIdx++;

		// 舊路徑
		string oldSrc = src;

		// 前置處理
		oldSrc = this.do_beforePipe(oldSrc);

		renderFunc(oldSrc, (toReplace)=>{

			// 領 號碼牌
			int invokeIdx = renderIdx;
			// 若 上一輪翻譯 已經結束，則視為新的一輪
			if (this.callbackIdx == 0){
				invokeIdx = 0;
			}

			// 以 號碼牌 委託任務 
			// 因為是非同步 所以有可能在首次呼叫render還沒得到回應時，就被再次呼叫render
			this.renderInvokerSerial.Set(
				()=>{
					// 避免回傳結果時，此UI已不見/銷毀
					if (this.isDestroyed) return;

					if (check(toReplace) == false) return;

					// 後期處理
					toReplace = this.do_afterPipe(toReplace);

					doWithDst(toReplace);
				}, 
				invokeIdx
			);
		});
	}

	protected void _setToTarget (Action<Image> eachFn) {

		if (this.target != null) {
			eachFn(this.target);
		}

		foreach (Image each in this.extraTargets) {
			eachFn(each);
		}
	}

	protected void unhold () {
		if (this._resInfo == null) return;
		ResMgr.Unhold(this._resInfo, this);
	}

	
	protected string do_beforePipe (string str) {
		foreach (Func<string, string> each in this.beforePipe) {
			str = each(str);
		}
		return str;
	}

	protected string do_afterPipe (string str) {
		foreach (Func<string, string> each in this.afterPipe) {
			str = each(str);
		}
		return str;
	}

	/*====================================Private Function=======================================*/
}


}
