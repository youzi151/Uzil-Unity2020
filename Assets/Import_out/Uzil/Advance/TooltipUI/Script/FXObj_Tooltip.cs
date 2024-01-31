using UnityEngine;
using UnityEngine.UI;

using TMPro;

using Uzil.FX;
using Uzil.Res;
using Uzil.Misc;
using Uzil.CompExt;
using Uzil.BuiltinUtil;

namespace Uzil.TooltipUI {

public class FXObj_Tooltip : FXObj {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	public TooltipObj tooltip = null;
	protected TextExt textExt = null;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	public override void Update () {

		base.Update();

	}

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 初始化 */
	public override void Init (DictSO initData) {

		base.Init(initData);
		if (initData == null) return;

		// Canvas ==========
		string canvasKey = null;
		initData.TryGetString("canvas", (res)=>{
			canvasKey = res;
		});
		CanvasObj canvas = CanvasUtil.Inst(canvasKey);

		// 建立、取得 訊息視窗
		ResInfo res = ResMgr.Hold<GameObject>(new ResReq(TooltipObj.PREFAB_TOOLTIPOBJ).User(this));
		ResMgr.UnholdOnDestroy(res, this, this.gameObject);
		GameObject prefab = res.GetResult<GameObject>();
		GameObject gObj = GameObject.Instantiate(prefab);
		gObj.transform.SetParent(canvas.transform, false);
		gObj.transform.localPosition = Vector2.zero;

		this.tooltip = gObj.GetComponent<TooltipObj>();
		UIFollow follow = gObj.GetComponent<UIFollow>();
		
		follow.SetFollower((tooltip.transform as RectTransform));
		follow.SetTarget(this.transform);

		// 若 內容 為 文字
		if (initData.ContainsKey("text")) {

			// 取得文字資料
			DictSO textData = initData.GetDictSO("text");
			if (textData == null) {
				string str = initData.GetString("text");
				if (str != null) {
					textData = new DictSO().Set("text", str);
				}
			}

			// 若文字資料存在
			if (textData != null) {

				// 容器
				GameObject container = new GameObject();
				container.AddComponent<RectTransform>();
				
				HorizontalLayoutGroup layout = container.AddComponent<HorizontalLayoutGroup>();
				ContentSizeFitter sizeFitter = container.AddComponent<ContentSizeFitter>();
				sizeFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
				sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
				
				this.tooltip.SetContent((container.transform as RectTransform));
				
				// 文字
				GameObject textGObj = new GameObject();
				TextMeshProUGUI text = textGObj.AddComponent<TextMeshProUGUI>();
				this.textExt = textGObj.AddComponent<TextExt>();
				this.textExt.textUI = text;
				textGObj.transform.SetParent(container.transform, false);

				this.textExt.LoadMemo(textData);

			}
		}
			
	}

	/** 設置資料  */
	public override void SetData (DictSO data) {
		if (data == null) return;

		base.SetData(data);

		if (this.tooltip != null) {
			this.tooltip.LoadMemo(data);
		}

		// 若 文字 存在 且 有 文字資料
		if (this.textExt != null && data.ContainsKey("text")) {
			
			// 取得文字資料
			DictSO textData = data.GetDictSO("text");
			if (textData == null) {
				string str = data.GetString("text");
				if (str != null) {
					textData = new DictSO().Set("text", str);
				}
			}

			// 設置文字
			this.textExt.LoadMemo(textData);
		}
	}

	/** 播放 */
	// public override void Play (DictSO playData) {
	// 	base.Play(); // 已經Init過了 不需要傳參數data
	// }

	/** 停止 */
	public override void Stop () {

		if (this.tooltip != null) {

			GameObject toDestroy = this.tooltip.gameObject;
			this.tooltip.onHide.Add(()=>{
				GameObject.Destroy(toDestroy);
				this.done();
			});

			this.tooltip.Hide();

		}
		
		this.tooltip = null;
		this.textExt = null;
	}

	/** 終止 */
	public override void Terminate () {
		if (this.tooltip != null) {
			GameObject toDestroy = this.tooltip.gameObject;
			GameObject.Destroy(toDestroy);
		}
		
		this.tooltip = null;
		this.textExt = null;

		this.done();
	}

	/** 發送訊息 */
	// public override void Call (string msg, DictSO data = null) {
	// 	base.Call(msg, data);
	// }


	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/


}

}