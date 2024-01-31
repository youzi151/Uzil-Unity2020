using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;

using Uzil.Res;
using Uzil.Lua;
using Uzil.Anim;
using Uzil.Misc;
using Uzil.CompExt;
using Uzil.BuiltinUtil;

namespace Uzil.UI {
public class UIMgr {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/** 預設Key */
	public const string defaultKey = "_default";

	/** key:實體 */
	private static Dictionary<string, UIMgr> key2Instance = new Dictionary<string, UIMgr>();
	
	/*=====================================Static Funciton=======================================*/


	/** 取得 Canvas */
	public static CanvasObj GetCanvas (string key = null) {
		if (key == null) key = UIMgr.defaultKey;
		return CanvasUtil.Inst("_UIMgr_"+key);
	}

	/** 取得實體 */
	public static UIMgr Inst (string key = null) {
		if (key == null) key = UIMgr.defaultKey;
		
		UIMgr instance = null;

		// 若 實體存在 則 取用
		if (UIMgr.key2Instance.ContainsKey(key)) {
			instance = UIMgr.key2Instance[key];
		}
		// 否則 建立
		else {
			instance = new UIMgr();
			
			instance.canvas = UIMgr.GetCanvas(key);
			// GameObject.DontDestroyOnLoad(instance.canvas.gameObject);

			UIMgr.AddInst(key, instance);
			
		}


		return instance;
	}

	/** 新增 */
	public static void AddInst (string key, UIMgr inst) {
		if (UIMgr.key2Instance.ContainsKey(key)) {
			UIMgr.DestroyInst(key);
		}

		inst.id = key;

		UIMgr.key2Instance.Add(key, inst);

		InvokerUpdate.Inst("_UIMgr").Add(()=>{
			inst.Update();
		}).Tag(key);
	}

	/** 銷毀 */
	public static void DestroyInst (string key = null) {
		if (key == null) key = UIMgr.defaultKey;

		if (UIMgr.key2Instance.ContainsKey(key) == false) return;
		UIMgr.key2Instance.Remove(key);

		// 註銷 更新
		InvokerUpdate.Inst("_UIMgr").Cancel(key);
		// 銷毀
		CanvasUtil.Destroy("_UIMgr_"+key);
	}

	/** 排序 */
	public static void SortInst (string key, float sort) {
		if (key == null) key = UIMgr.defaultKey;

		if (UIMgr.key2Instance.ContainsKey(key) == false) return;
		UIMgr inst = UIMgr.key2Instance[key];

		inst.sort = sort;
		inst.canvas.canvas.sortingOrder = (int) sort;

		List<UIMgr> list = new List<UIMgr>();
		foreach (KeyValuePair<string, UIMgr> pair in UIMgr.key2Instance) {
			list.Add(pair.Value);
		}
		list.Sort((a, b)=>{
			return (int) (a.sort - b.sort);
		});

		for (int idx = 0; idx < list.Count; idx++) {
			list[idx].canvas.transform.SetSiblingIndex(idx);
		}
	}

	/*=========================================Members===========================================*/

	public string id { get; protected set; }

	/** Canvas */
	public CanvasObj canvas;

	/** 渲染方式 */
	protected RenderMode renderMode = RenderMode.ScreenSpaceOverlay;

	/** 容器 */
	public RectTransform container = null;

	/** ID : UI資訊 */
	protected Dictionary<string, UIInfo> id2UI = new Dictionary<string, UIInfo>();

	protected List<UIInfo> uiList = new List<UIInfo>();

	/** 排序 */
	protected float sort = 0f;


	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	public void Update () {

		if (this.renderMode == RenderMode.ScreenSpaceCamera) {
			Camera main = CameraUtil.GetMain();
			if (this.canvas.canvas.worldCamera != main) {
				this.canvas.canvas.worldCamera = main;
			}
		}

		foreach (UIInfo each in this.uiList) {
			if (each.animator != null) {
				each.animator.Update(Time.deltaTime);
			}
		}
	}

	/** 取得 */
	public UIInfo GetUI (string id) {
		if (this.id2UI.ContainsKey(id) == false) return null;
		return this.id2UI[id];
	}

	/** 設置 渲染模式 */
	public void SetRenderMode (RenderMode renderMode) {
		this.renderMode = renderMode;
		
		if (this.canvas == null) return;

		this.canvas.canvas.renderMode = renderMode;
		if (renderMode == RenderMode.ScreenSpaceCamera) {
			this.canvas.canvas.worldCamera = CameraUtil.GetMain();
		} else {
			this.canvas.canvas.worldCamera = null;
		}
	}

	/** 建立 */
	public UIInfo Create (string id, DictSO data) {
		// 若 沒有指定 ID 則 
		if (id == null) {
			// 試著從data中取得
			data.TryGetString("id", (res)=>{
				id = res;
			});
		}

		bool isIDExist = id != null;

		if (isIDExist && this.id2UI.ContainsKey(id)) {
			this.Remove(id);
		}
		// 建立UI
		GameObject gObj = isIDExist ? new GameObject(id) : new GameObject();
		RectTransform trans = gObj.AddComponent<RectTransform>();
		DestroyListener destroyListener = gObj.AddComponent<DestroyListener>();

		if (this.container != null) {
			trans.SetParent(this.container, false);
		} else {
			this.canvas.AddUIObj(gObj);
		}

		// 建立UI資訊
		UIInfo uiInfo = new UIInfo();
		uiInfo.destroyListener = destroyListener;
		uiInfo.trans = trans;
		uiInfo.propTarget = new PropTarget_UI();
		uiInfo.id = id;

		// 註冊 當銷毀
		destroyListener.onDestroy.Add(()=>{
			// 移除此UI
			this.Remove(uiInfo);
		});

		// 加入 註冊
		if (isIDExist) {
			this.id2UI.Add(id, uiInfo);
		}
		// 加入列表
		this.uiList.Add(uiInfo);

		// 設置資料
		this.SetData(uiInfo, data);

		this.updateSort();

		return uiInfo;
	}

	/** 移除 */
	public void Remove (string id) {
		if (this.id2UI.ContainsKey(id) == false) return;
		UIInfo ui = this.GetUI(id);
		this.id2UI.Remove(id);
		
		this.Remove(ui);
	}

	/** 移除 */
	public void Remove (UIInfo ui) {
		if (this.uiList.Contains(ui) == false) return;
		this.uiList.Remove(ui);

		this.destroy(ui);

		List<UIInfo> toRm = new List<UIInfo>();
		foreach (UIInfo each in this.uiList) {
			if (each.parent == ui) toRm.Add(each);
		}
		foreach (UIInfo each in toRm) {
			this.Remove(each);
		}

	}

	/** 清空 */
	public void Clear () {
		this.id2UI.Clear();
		List<UIInfo> toDestroy = new List<UIInfo>();
		foreach (UIInfo info in this.uiList) {
			toDestroy.Add(info);
		}
		this.uiList.Clear();
		foreach (UIInfo info in toDestroy) {
			this.destroy(info);
		}
	}

	/** 強制更新 */
	public void ForceUpdate (string id) {
		UIInfo ui = this.GetUI(id);
		if (ui == null) return;

		LayoutRebuilder.ForceRebuildLayoutImmediate(ui.trans);
	}

	/** 排序 */
	public void Sort (string id, float sort) {
		UIInfo ui = this.GetUI(id);
		if (ui == null) return;

		ui.sort = sort;
		this.updateSort(ui.parent);
	}

	/** 設置資料 */
	public void SetData (string id, DictSO data) {
		UIInfo ui = this.GetUI(id);
		if (ui == null) return;
		this.SetData(ui, data);
	}
	public void SetData (UIInfo ui, DictSO data) {

		data.TryGetString("prefab", (res)=>{

			DictSO prefabData = DictSO.Json(ResMgr.Get<string>(new ResReq(res)));
			if (prefabData == null) return;

			data = prefabData;
		});

		UIInfo parentUI = null;
		if (data.ContainsKey("_parent")) {
			parentUI = (UIInfo) data.Get("_parent");
		}
		if (parentUI == null) {
			if (data.ContainsKey("parent")){
				string parent = data.GetString("parent");
				parentUI = this.GetUI(parent);
			}
		}

		if (parentUI != null) {
			ui.trans.SetParent(parentUI.trans, false);
		}
		ui.parent = parentUI;
		

		// 基本 
		SetDataUtil.SetRectTransform(ui.trans, data);


		// 類型
		if (data.ContainsKey("image")) {
			ui.ClearComps();

			Image image = ui.RequestComp<Image>();
			// 指定到動畫屬性目標
			ui.propTarget.image = image;
			// 設置
			SetDataUtil.SetImage(image, data.GetDictSO("image"));

			// 遮罩
			if (data.ContainsKey("mask")) {
				Mask mask = ui.RequestComp<Mask>();
				SetDataUtil.SetMask(mask, data.GetDictSO("mask"));
				mask.showMaskGraphic = false;
			}
			
		}
		// OR
		else if (data.ContainsKey("text")) {
			ui.ClearComps();

			TextMeshProUGUI text = ui.RequestComp<TextMeshProUGUI>();
			TextExt textExt = ui.RequestComp<TextExt>();
			textExt.textUI = text;
			
			ContentSizeFitter sizeFitter = ui.RequestComp<ContentSizeFitter>();
			sizeFitter.enabled = false;

			DictSO textData = data.GetDictSO("text");
			if (textData == null) {
				string str = data.GetString("text");
				if (str != null) {
					textData = new DictSO().Set("text", str);
				}
			}

			// 設置
			textExt.LoadMemo(textData);

			textData.TryGetEnum<UnityEngine.UI.ContentSizeFitter.FitMode>("horizontalFit", (res)=>{
				sizeFitter.enabled = true;
				sizeFitter.horizontalFit = res;
			});

			textData.TryGetEnum<UnityEngine.UI.ContentSizeFitter.FitMode>("verticalFit", (res)=>{
				sizeFitter.enabled = true;
				sizeFitter.verticalFit = res;
			});


		}

		data.TryGetList("childs", (childDatas)=>{
			for (int idx = 0; idx < childDatas.Count; idx++) {
				
				object dataOrName = childDatas[idx];
				DictSO childData = DictSO.Json(dataOrName);

				if (childData == null) {
					childData = DictSO.Json(ResMgr.Get<string>(new ResReq(dataOrName.ToString())));
				}

				if (childData == null) continue;

				childData.Set("_parent", ui);
				
				UIInfo childInfo = this.Create(childData.GetString("id"), childData);
				RectTransform childTrans = childInfo.trans;
			}
		});

		// 動畫
		if (data.ContainsKey("animator")) {

			DictSO animatorData = data.GetDictSO("animator");

			// 若 動畫資料為空 (不是DictSO形式)
			if (animatorData == null) {

				// 試以 字串 取得
				string animatorName = data.GetString("animator");
				// 尋找/讀取json
				string json = ResMgr.Get<string>(new ResReq(animatorName));

				// 若 json 存在 則 解析為資料
				if (json != null) {
					animatorData = DictSO.Json(json);
				}

			}

			// 若 資料存在
			if (animatorData != null) {
				// 建立
				Animator_Custom animator = new Animator_Custom();
				animator.Load(animatorData);
				ui.animator = animator;

				animator.getTarget = this.getTarget;

				animator.Init();

			}

		}

		// 事件
		if (data.ContainsKey("events")) {
			DictSO events = data.GetDictSO("events");
			if (events != null) {
				
				// 請求組件
				EventTrigger eventTrigger = ui.RequestComp<EventTrigger>();

				DictSO evtTriggerData = new DictSO();
				
				// 轉 script 成 Action
				foreach (KeyValuePair<string, object> name2Script in events) {
					Action<DictSO> cb = (data)=>{
						LuaUtil.DoString(name2Script.Value.ToString());
					};
					evtTriggerData.Add(name2Script.Key, cb);
				}

				SetDataUtil.SetEventTrigger(eventTrigger, evtTriggerData);
			}
		}

		// 排序
		data.TryGetFloat("sort", (res)=>{
			ui.sort = res;
			this.updateSort(ui.parent);
		});

	}

	/** 取得資料 */
	public DictSO GetData (string id, List<string> requests) {
		if (requests == null || requests.Count == 0) return null;
		UIInfo ui = this.GetUI(id);
		if (ui == null) return null;

		DictSO data = SetDataUtil.GetRectTransform(ui.trans, requests);
		
		return data;
	}

	/** 設置屬性 */
	public void SetProp (string id, string key, object value) {
		if (key == null || key == "") return;
		UIInfo ui = this.GetUI(id);
		if (ui == null) return;
		this.SetData(ui, new DictSO().Set(key, value));
	}

	
	/** 設置 圖像資料 */
	public void SetImageData (UIInfo ui, DictSO data) {
		
		SetDataUtil.SetRectTransform(ui.trans, data);

		Image image = ui.trans.GetComponent<Image>();
		if (image == null) return;

		SetDataUtil.SetImage(image, data);
	}
	/** 設置 圖像資料 */
	public void SetImageData (string id, DictSO data) {
		UIInfo ui = this.GetUI(id);
		if (ui == null) return;
		this.SetImageData(ui, data);
	}
	/** 設置 圖像屬性 */
	public void SetImageProp (string id, string key, object value) {
		if (key == null || key == "") return;
		this.SetImageData(id, new DictSO().Set(key, value));
	}

	/** 設置 文字資料 */
	public void SetTextData (UIInfo ui, DictSO data) {
		TextExt textExt = ui.trans.GetComponent<TextExt>();
		if (textExt == null) return;

		textExt.LoadMemo(data);
	}
	/** 設置 文字資料 */
	public void SetTextData (string id, DictSO data) {
		UIInfo ui = this.GetUI(id);
		if (ui == null) return;
		this.SetTextData(ui, data);
	}
	/** 設置 文字屬性 */
	public void SetTextProp (string id, string key, object value) {
		if (key == null || key == "") return;
		this.SetTextData(id, new DictSO().Set(key, value));
	}

	/** 設置 動畫參數 */
	public void SetAnimParam (string id, string paramName, object value) {
		if (paramName == null || paramName == "") return;
		
		UIInfo ui = this.GetUI(id);
		if (ui == null) return;
		if (ui.animator == null) return;

		ui.animator.Param(paramName, value);
	}

	/*===================================Protected Function======================================*/

	/** 銷毀 */
	protected void destroy (UIInfo ui) {
		RectTransform trans = ui.trans;
		if (trans == null) return;
		if (ui.destroyListener.isDestroyed) return;
		GameObject.DestroyImmediate(trans.gameObject);
	}

	protected PropTarget_UI getTarget (string target) {
		UIInfo ui = this.GetUI(target);
		if (ui == null) return null;
		return ui.propTarget;
	}

	protected void updateSort (UIInfo targetParent = null) {
	
		List<UIInfo> rootChilds = new List<UIInfo>();
		Dictionary<UIInfo, List<UIInfo>> parent2Childs = new Dictionary<UIInfo, List<UIInfo>>();

		// 每個 ui
		for (int idx = 0; idx < this.uiList.Count; idx++) {
			
			UIInfo each = this.uiList[idx];

			// 存在 目標父物件 且 該ui的父物件不是目標的話 則 忽略
			if (targetParent != null && each.parent != targetParent) continue;

			// 該ui不存在 父物件 則 加入至根子物件
			if (each.parent == null) {
				rootChilds.Add(each);
				continue;
			}

			List<UIInfo> childs;
			if (parent2Childs.ContainsKey(each.parent) == false) {
				childs = new List<UIInfo>();
				parent2Childs.Add(each.parent, childs);
			} else {
				childs = parent2Childs[each.parent];
			}

			childs.Add(each);
		}

		this.sortList(rootChilds);
		foreach (KeyValuePair<UIInfo, List<UIInfo>> pair in parent2Childs) {
			this.sortList(pair.Value);
		}
	}

	protected void sortList (List<UIInfo> list) {
		list.Sort((a, b)=>{
			return (int) (a.sort - b.sort);
		});
		for (int idx = 0; idx < list.Count; idx++) {
			list[idx].trans.SetSiblingIndex(idx);
		}
	}

	/*====================================Private Function=======================================*/
}


}