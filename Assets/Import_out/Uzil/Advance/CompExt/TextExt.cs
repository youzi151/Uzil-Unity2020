using System;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

using Uzil.i18n;

namespace Uzil.CompExt {

public class TextExt : MonoBehaviour, IMemoable {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/* 所有實例 */
	private static List<TextExt> instancePool = new List<TextExt>();

	/*=====================================Static Funciton=======================================*/

	/* 全域更新 */
	public static void UpdateRender () {
		for (int i = 0; i < TextExt.instancePool.Count; i++){
			TextExt.instancePool[i].Rerender();
		}
	}
	public static bool isUpdateListenerAdded = false;

	/*=========================================Members===========================================*/

	/* 代換內容 */
	[Header("To Be Replace")]
	[TextArea(3, 5)]
	public string content;

	/* 是否立即代換 */
	[Header("<$$> keyword only")]
	public bool isImmediate = true;

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
	
	private TextMeshProUGUI _textUI;
	public TextMeshProUGUI textUI {
		get {
			if (this._textUI == null) {
				this._textUI = this.GetComponent<TextMeshProUGUI>();
			}
			return this._textUI;
		}
		set {
			this._textUI = value;
		}
	}

	public string text {
		get {
			return this.content;
		}
		set {
			this.SetText(value);
		}
	}

	public string renderedText;

	public Color color {
		get {
			return this.textUI.color;
		}
		set {
			this.SetColor(color);
		}
	}

	public List<TextMeshProUGUI> extraTexts = new List<TextMeshProUGUI>();

	public List<TextSizer> textSizers = new List<TextSizer>();

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
		TextExt.instancePool.Add(this);

		// 若StrReplacer的當語言變更事件，還未包含"_TextExt"時，註冊 "語言變更時，會進行全域更新"
		if (!TextExt.isUpdateListenerAdded) {
			TextExt.isUpdateListenerAdded = true;
			EventListener listener = new EventListener(TextExt.UpdateRender).ID("_TextExt");
			StrReplacer.Inst().onUpdate.AddListener(listener);
		}

		this.Rerender();
	}

	public void OnEnable () {
		// 進行代換
		this.Rerender();
	}

	public void Update () {
		if (this.textUI == null) return;

		// 當改變內容時更新 -> 棄用：以SetText或text = "blabla"來取代，否則會重複呼叫
		// if (this.textUI.havePropertiesChanged){
			// this.Rerender();
		// }

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
		TextExt.instancePool.Remove(this);
		this.isDestroyed = true;
	}

	/*========================================Interface==========================================*/

	/** [IMemoable] 紀錄為Json格式 */
	public object ToMemo () {
		DictSO memo = DictSO.New();

		// 文字
		memo.Set("text", this.text);

		// 顏色
		memo.Set("color", DictSO.ColorToHex(this.color));

		// 是否立刻翻譯
		memo.Set("isImmediate", this.isImmediate);

		// 更新間隔
		memo.Set("updateInterval", this.updateInterval_sec);

		// 文字大小
		memo.Set("fontSize", this.textUI.fontSize);

		if (this.textUI.enableAutoSizing) {
			memo.Set("autoFontSize", new float[]{this.textUI.fontSizeMin, this.textUI.fontSizeMax});
		}
		
		return memo;
	}
	
    /** [IMemoable] 讀取Json格式 */
	public void LoadMemo (object memoJson) {
		DictSO memo = DictSO.Json(memoJson);
		if (memo == null) return;

		// 文字
		memo.TryGetString("content", (res)=>{
			this.SetText(res);
		});
		memo.TryGetString("text", (res)=>{
			this.SetText(res);
		});

		// 文字大小
		memo.TryGetFloat("fontSize", (res)=>{
			this.SetSize(res);
		});
		// min/max
		memo.TryGetVector2("autoFontSize", (res)=>{
			this.SetAutoSize(res.x, res.y);
		});

		// 文字靠齊
		memo.TryGetEnum<TextAlignmentOptions>("alignment", (res)=>{
			this.textUI.alignment = res;
		});

		// 顏色
		memo.TryGetColor("color", (res)=>{
			this.SetColor(res);
		});

		// 是否包住
		memo.TryGetBool("isWrapping", (res)=>{
			this.textUI.enableWordWrapping = res;
		});

		// 是否立刻翻譯
		memo.TryGetBool("isImmediate", (res)=>{
			this.isImmediate = res;
		});

		// 更新間隔
		memo.TryGetFloat("updateInterval", (res)=>{
			this.updateInterval_sec = res;
		});

	}

	/*=====================================Public Function=======================================*/

	/** 設置文字 */
	public void SetText (string str, bool isRender = true) {
		this.content = str;
		if (isRender) {
			this.Rerender();
		}else{
			this.setRenderedText(str);
		}
	}

	/** 強制設置文字(不翻譯) */
	public void SetTextForce (string str) {
		this.setRenderedText(str);
	}

	/** 設置大小 */
	public void SetSize (float fontSize) {
		this.textUI.enableAutoSizing = false;
		this.textUI.fontSize = fontSize;
		foreach (TextMeshProUGUI each in this.extraTexts) {
			each.fontSize = fontSize;
		}
	}

	/** 設置大小 */
	public void SetAutoSize (float min, float max) {
		this.textUI.enableAutoSizing = true;
		this.textUI.fontSizeMin = min;
		this.textUI.fontSizeMax = max;
	}

	/** 設置顏色 */
	public void SetColor (Color color) {
		this.textUI.color = color;
		foreach (TextMeshProUGUI each in this.extraTexts) {
			each.color = color;
		}
	}

	/** 重新翻譯 */
	public void Rerender () {
		if (Application.isPlaying == false && Application.isEditor) return;

		if (this.isImmediate) {
			this.renderImmediate();
		}

		this.isCallRerender = true;
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

	/*重新代換*/
	private void renderImmediate () {
		string toReplace = this.content;
		
		foreach (Func<string, string> each in this.beforePipe) {
			toReplace = each(toReplace);
		} 

		toReplace = StrReplacer.RenderNow(toReplace);

		foreach (Func<string, string> each in this.afterPipe) {
			toReplace = each(toReplace);
		} 

		this.setRenderedText(toReplace);
		this.renderedText = toReplace;
	}
	private void render () {
		if (this.textUI == null) return;
		// Debug.Log("[TextExt]: Call Render: " + this.gameObject.name);
		// Debug.Log("[TextExt]: toReplace: " + this.content);

		// 取得 自己的翻譯順序
		int renderIdx = this.callbackIdx;
		this.callbackIdx++;

		// 舊文字內容
		string oldContent = this.content;
		string oldStr = this.content;

		// 前置處理
		oldStr = this.do_beforePipe(oldStr);

		StrReplacer.Render(oldStr, (res)=>{

			// Debug.Log("[TextExt]: Render done: " + oldStr + " <to> "+res);
			string toReplace = res;

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
					if (this.textUI == null) return;
					// 若來源已經被改為別的內容
					if (this.content != oldContent) return;

					//若重複
					if (this.textUI.text == toReplace) return;

					// Debug.Log("[TextExt]: " + oldStr + " <to> " + toReplace);

					// 後期處理
					toReplace = this.do_afterPipe(toReplace);

					// 設置
					this.setRenderedText(toReplace);
					
					this.renderedText = toReplace;
				}, 
				invokeIdx
			);
		});
	}

	private void setRenderedText (string txt) {
		this.textUI.text = txt;
		foreach (TextMeshProUGUI each in this.extraTexts) {
			each.text = txt;
		}
		foreach (TextSizer each in this.textSizers) {
			each.Rebuild();
		}
	}

	private string do_beforePipe (string str) {
		foreach (Func<string, string> each in this.beforePipe) {
			str = each(str);
		}
		return str;
	}

	private string do_afterPipe (string str) {
		foreach (Func<string, string> each in this.afterPipe) {
			str = each(str);
		}
		return str;
	}

}



}