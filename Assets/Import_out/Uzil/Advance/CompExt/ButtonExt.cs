using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Uzil.Res;
using Uzil.i18n;

namespace Uzil.CompExt {

public class ButtonExt : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/* 所有實例 */
	private static List<ButtonExt> instancePool = new List<ButtonExt>();

	/*=====================================Static Funciton=======================================*/

	/** 全域更新 */
	public static void UpdateRender () {
		for (int i = 0; i < ButtonExt.instancePool.Count; i++){
			ButtonExt.instancePool[i].Rerender();
		}
	}

	/*=========================================Members===========================================*/

	/** 一般狀態 圖片路徑 */
	public string normal_imgPath = "XXX/<$_lang$>/XXX.png";
	protected string normal_imgPath_last = null;
	protected ResInfo normal_resInfo = null;

	/** 高亮狀態 圖片路徑 */
	public string highlighted_imgPath = "XXX/<$_lang$>/XXX.png";
	protected string highlighted_imgPath_last = null;
	protected ResInfo highlighted_resInfo = null;

	/** 按壓狀態 圖片路徑 */
	public string pressed_imgPath = "XXX/<$_lang$>/XXX.png";
	protected string pressed_imgPath_last = null;
	protected ResInfo pressed_resInfo = null;

	/** 選取狀態 圖片路徑 */
	public string selected_imgPath = "XXX/<$_lang$>/XXX.png";
	protected string selected_imgPath_last = null;
	protected ResInfo selected_resInfo = null;


	/** 關閉狀態 圖片路徑 */
	public string disabled_imgPath = "XXX/<$_lang$>/XXX.png";
	protected string disabled_imgPath_last = null;
	protected ResInfo disabled_resInfo = null;

	
	/** 是否立即代換 */
	[Header("<$$> keyword only")]
	public bool isImmediate = true;

	[Header("Setting")]
	/** 自動更新間隔 */
	public float updateInterval_sec = -1f;
	
	/** 自動更新倒數 */
	protected float counter = 0f;

	/** 是否呼叫代換 */
	protected bool isCallRerender = false;

	/** 代換回傳序號 */
	protected int callbackIdx = 0;

	protected InvokerSerial renderInvokerSerial = new InvokerSerial();

	/** 是否已經銷毀 */
	protected bool isDestroyed = false;

	/*========================================Components=========================================*/
	
	/** 目標圖像 */
	public Button target {
		get {
			if (this._target != null) return this._target;
			this._target = this.GetComponent<Button>();
			return this._target;
		}
	}
	protected Button _target = null;

	/** 額外目標 */
	public List<Button> extraTargets = new List<Button>();

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
		ButtonExt.instancePool.Add(this);

		// 若StrReplacer的當語言變更事件，還未包含"_ButtonExt"時，註冊 "語言變更時，會進行全域更新"
		string listenerName = "_ButtonExt";
		StrReplacer reader = StrReplacer.Inst();
		if (!reader.onUpdate.Contains(listenerName)) {
			EventListener listener = new EventListener(ButtonExt.UpdateRender).ID(listenerName);
			reader.onUpdate += listener;
		}

		this.Rerender();
	}

	public void OnEnable () {
		// 進行代換
		this.Rerender();

		if (this.isCallRerender) {
			this.isCallRerender = false;
			this.render();
		}

		this.renderInvokerSerial.DoAll();
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
		ButtonExt.instancePool.Remove(this);
		this.isDestroyed = true;
		
		this.unhold_normal();
		this.unhold_highlighted();
		this.unhold_pressed();
		this.unhold_selected();
		this.unhold_disabled();
	}
	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 重新翻譯 */
	public void Rerender () {
		if (this.isImmediate) {
			this.renderImmediate();
		}

		this.isCallRerender = true;
	}

	/*===================================Protected Function======================================*/
	/** 重新代換 */
	protected void renderImmediate () {
		this._renderToAll((toRender, callback)=>{
			callback(StrReplacer.RenderNow(toRender));
		});
	}

	protected void render () {
		this._renderToAll((toRender, callback)=>{
			StrReplacer.Render(toRender, (res)=>{
				callback(res);
			});
		});
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

	protected void _renderToAll (Action<string, Action<string>> renderFn) {
		
		// normal
		string normal_src = this.normal_imgPath;
		this._render(
			normal_src,
			renderFn,
			/* check */
			(toReplace)=>{
				// 若來源已經被改為別的內容
				if (this.normal_imgPath != normal_src) return false;
				// 若重複
				if (this.normal_imgPath_last == toReplace) return false;
				return true;
			},
			/* doWithDst */
			(result)=>{
				this.unhold_normal();
				this.normal_resInfo = ResMgr.Hold<Sprite>(new ResReq().Path(result).User(this.GetInstanceID()+".normal"));
				Sprite sp = this.normal_resInfo == null? null : this.normal_resInfo.GetResult<Sprite>();
				this._setToTarget((btn)=>{
					btn.image.sprite = sp;
				});
			}
		);

		// highlighted
		string highlighted_src = this.highlighted_imgPath;
		this._render(
			highlighted_src,
			renderFn,
			/* check */
			(toReplace)=>{
				// 若來源已經被改為別的內容
				if (this.highlighted_imgPath != highlighted_src) return false;
				// 若重複
				if (this.highlighted_imgPath_last == toReplace) return false;
				return true;
			},
			/* doWithDst */
			(result)=>{
				this.unhold_highlighted();
				this.highlighted_resInfo = ResMgr.Hold<Sprite>(new ResReq().Path(result).User(this.GetInstanceID()+".highlighted"));
				Sprite sp = this.highlighted_resInfo == null? null : this.highlighted_resInfo.GetResult<Sprite>();
				this._setToTarget((btn)=>{
					SpriteState spSt = btn.spriteState;
					spSt.highlightedSprite = sp;
					btn.spriteState = spSt;
				});
			}
		);

		// pressed
		string pressed_src = this.pressed_imgPath;
		this._render(
			pressed_src,
			renderFn,
			/* check */
			(toReplace)=>{
				// 若來源已經被改為別的內容
				if (this.pressed_imgPath != pressed_src) return false;
				// 若重複
				if (this.pressed_imgPath_last == toReplace) return false;
				return true;
			},
			/* doWithDst */
			(result)=>{
				this.unhold_pressed();
				this.pressed_resInfo = ResMgr.Hold<Sprite>(new ResReq().Path(result).User(this.GetInstanceID()+".pressed"));
				Sprite sp = this.pressed_resInfo == null? null : this.pressed_resInfo.GetResult<Sprite>();
				this._setToTarget((btn)=>{
					SpriteState spSt = btn.spriteState;
					spSt.pressedSprite = sp;
					btn.spriteState = spSt;
				});
			}
		);

		// selected
		string selected_src = this.selected_imgPath;
		this._render(
			selected_src,
			renderFn,
			/* check */
			(toReplace)=>{
				// 若來源已經被改為別的內容
				if (this.selected_imgPath != selected_src) return false;
				// 若重複
				if (this.selected_imgPath_last == toReplace) return false;
				return true;
			},
			/* doWithDst */
			(result)=>{
				this.unhold_selected();
				this.selected_resInfo = ResMgr.Hold<Sprite>(new ResReq().Path(result).User(this.GetInstanceID()+".selected"));
				Sprite sp = this.selected_resInfo == null? null : this.selected_resInfo.GetResult<Sprite>();
				this._setToTarget((btn)=>{
					SpriteState spSt = btn.spriteState;
					spSt.selectedSprite = sp;
					btn.spriteState = spSt;
				});
			}
		);

		// disabled
		string disabled_src = this.disabled_imgPath;
		this._render(
			disabled_src,
			renderFn,
			/* check */
			(toReplace)=>{
				// 若來源已經被改為別的內容
				if (this.disabled_imgPath != disabled_src) return false;
				// 若重複
				if (this.disabled_imgPath_last == toReplace) return false;
				return true;
			},
			/* doWithDst */
			(result)=>{
				this.unhold_disabled();
				this.disabled_resInfo = ResMgr.Hold<Sprite>(new ResReq().Path(result).User(this.GetInstanceID()+".disabled"));
				Sprite sp = this.disabled_resInfo == null? null : this.disabled_resInfo.GetResult<Sprite>();
				this._setToTarget((btn)=>{
					SpriteState spSt = btn.spriteState;
					spSt.disabledSprite = sp;
					btn.spriteState = spSt;
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

	protected void _setToTarget (Action<Button> eachFn) {

		if (this.target != null) {
			eachFn(this.target);
		}

		foreach (Button each in this.extraTargets) {
			eachFn(each);
		}
	}

	protected void unhold_normal () {
		if (this.normal_resInfo != null) {
			ResMgr.Unhold(this.normal_resInfo, this.GetInstanceID()+".normal");
		}
	}

	protected void unhold_pressed () {
		if (this.pressed_resInfo != null) {
			ResMgr.Unhold(this.pressed_resInfo, this.GetInstanceID()+".pressed");
		}
	}

	protected void unhold_highlighted () {
		if (this.highlighted_resInfo != null) {
			ResMgr.Unhold(this.highlighted_resInfo, this.GetInstanceID()+".highlighted");
		}
	}

	protected void unhold_selected () {
		if (this.selected_resInfo != null) {
			ResMgr.Unhold(this.selected_resInfo, this.GetInstanceID()+".highlighted");
		}
	}

	protected void unhold_disabled () {
		if (this.disabled_resInfo != null) {
			ResMgr.Unhold(this.disabled_resInfo, this.GetInstanceID()+".disabled");
		}
	}

	/*====================================Private Function=======================================*/
}


}
