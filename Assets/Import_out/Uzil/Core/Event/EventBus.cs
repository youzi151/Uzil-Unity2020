using UnityEngine;
using System.Collections.Generic;


namespace Uzil {

public class EventBus : MonoBehaviour {

	protected class RegInfo {
		public RegInfo (string id, string tag, EventListener listener) {
			this.id = id;
			this.tag = tag;
			this.listener = listener;
		}

		public string id;
		public string tag;
		public EventListener listener;
	}

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/** 預設Key */
	public const string defaultKey = "_default";

	/** 實體記錄 */
	public static Dictionary<string, EventBus> key2Instance = new Dictionary<string, EventBus>();

	/** 當前使用的 */
	public static EventBus main {
        get {
            return EventBus.Inst();
        }
    }

	/*=====================================Static Funciton=======================================*/

	/** 是否存在 */
	public static bool IsExist (string key) {
		return EventBus.key2Instance.ContainsKey(key);
	}

    /** 取得實體 */
	public static EventBus Inst (string key = EventBus.defaultKey) {
		EventBus instance = null;

		// 若 已存在 則 取用
		if (EventBus.key2Instance.ContainsKey(key)) {
			
			instance = EventBus.key2Instance[key];

		}
		// 否則 建立
		else {
			
			// 取得根物件
			GameObject rootGObj = RootUtil.GetMember("EventBus");
			
			// 建立
			GameObject instanceGObj = RootUtil.GetChild(key, rootGObj);
			if (instanceGObj == null) return null; // 避免 存取已銷毀物件的問題
			instance = instanceGObj.AddComponent<EventBus>();
			instance.instanceKey = key;

			// 設置 當 場景卸載 檢查所屬場景
			RootUtil.onSceneUnload.Add((data)=>{
				string sceneName = data.GetString("sceneName");

				// 若 無所屬場景 則 返回
				if (instance.belongSceneName == null) return;

				// 若 卸載場景 為 此實體所屬場景 則 移除此實體
				if (sceneName == instance.belongSceneName) {
					EventBus.Del(instance.instanceKey);
				}
			});
			
			// 加入註冊
			EventBus.key2Instance.Add(key, instance);

		}

		return instance;
	}

    /** 移除實體 */
	public static void Del (string key) {
		if (EventBus.key2Instance.ContainsKey(key) == false) return;

		// 取得實體
		EventBus instance = EventBus.key2Instance[key];

		// 銷毀物件
		GameObject.Destroy(instance.gameObject);

		// 移除註冊
		EventBus.key2Instance.Remove(key);
	}


	/*=========================================Members===========================================*/

	/** ID */
	public string id;
	
	/** Tag對應事件 */
	protected Dictionary<string, Event> _eventTag2Event = new Dictionary<string, Event>();

	protected Event _anyEvent = new Event();

	/** 使用者對應註冊資訊 */
	protected Dictionary<string, List<RegInfo>> _id2RegInfoDict = new Dictionary<string, List<RegInfo>>();

	/** 預設ID */
	protected int _dispatchIndex = 0;


    
	/** 
	 * 所屬場景
	 * 若該場景銷毀 則 連帶銷毀此Invoker
	 */
	protected string belongSceneName = null;

    /** 實體鍵值 */
    protected string instanceKey;

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

    void OnDestroy () {
        EventBus.Del(this.instanceKey);    
    }

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 清空 */
	public void Clear () {
		this._eventTag2Event.Clear();
		this._anyEvent.Clear();
		this._id2RegInfoDict.Clear();
		this._dispatchIndex = 0;
	}

	/** 發送事件 */
	public void Post (string eventTag, DictSO data = null) {
		// 呼叫事件
		if (this._eventTag2Event.ContainsKey(eventTag)) {
			this._eventTag2Event[eventTag].Call(data);	
		}
		
		// 準備呼叫任何事件

		// 準備副本，以及預先把data轉為json(避免各處自行費時轉換)
		DictSO data_copy = null;
		if (data != null) {
			data_copy = data.GetCopy();
			data_copy.Set("_json", data.ToJson());
		}

		DictSO anyData = DictSO.New().Set("tag", eventTag)
								     .Set("data", data_copy);
		//呼叫任何事件
		this._anyEvent.Call(anyData);
	}

	/** 註冊事件 */
	public void On (string eventTag, EventListener listener) {
		this.On(null, eventTag, listener);
	}

	/** 註冊事件 */
	public void On (string id, string eventTag, EventListener listener) {
		if (id == "" || id == null) id = listener.id;
		if (id == "" || id == null) id = this.dispatchID();

		// 移除舊的
		this.OffID(id);

		// 寫入識別名稱
		listener.id = id;
		
		// 加入到註冊資訊中
		this.regInfo(id, new RegInfo(id, eventTag, listener));

		// 註冊事件
		if (this._eventTag2Event.ContainsKey(eventTag)) {
			this._eventTag2Event[eventTag].AddListener(listener);
		} else {
			Event observer = new Event();
			observer.AddListener(listener);
			this._eventTag2Event.Add(eventTag, observer);
		}
		
	}

	/** 排序事件 */
	public void Sort (string id, string tag, float sort) {
		if (this._eventTag2Event.ContainsKey(tag) == false) return;
		Event evt = this._eventTag2Event[tag];
		
		EventListener listener = evt.GetListener(id);
		if (listener == null) return;

		listener.Sort(sort);
		evt.Sort();
	}

	/** 註冊任何事件 */
	public void OnAny (string id, EventListener listener) {
		//寫入識別名稱
		listener.id = id;

		//移除舊的
		this.OffID(id);
		
		//加入到註冊資訊中
		this.regInfo(id, new RegInfo(id, "_any", listener));
		
		//註冊
		this._anyEvent.AddListener(listener);
	}

	/** 排序事件 */
	public void SortAny (string id, float sort) {
		EventListener listener = this._anyEvent.GetListener(id);
		if (listener == null) return;

		listener.Sort(sort);
		this._anyEvent.Sort();
	}

	/** 註銷事件 */
	public void Off (string id) {
		this.OffID(id);
	}
	public void OffID (string id) {
		if (!this._id2RegInfoDict.ContainsKey(id)) {
			return;
		}

		foreach (RegInfo info in this._id2RegInfoDict[id]) {
			Event callback;
			if (info.tag == "_any") {
				callback = this._anyEvent;
			} else {
				callback = this._eventTag2Event[info.tag];
			}
			callback.RemoveListener(info.listener);
		}

		this._id2RegInfoDict.Remove(id);
	}
	public void OffTag (string eventTag) {
		if (!this._eventTag2Event.ContainsKey(eventTag)) {
			return;
		}

		Event tagEvent = this._eventTag2Event[eventTag];
		tagEvent.Clear();

		//在每一個註冊資訊中查詢，若tag符合者則移除
		foreach (KeyValuePair<string, List<RegInfo>> eachID in this._id2RegInfoDict) {
			
			List<RegInfo> infoList = eachID.Value;
			List<RegInfo> removeList = new List<RegInfo>();
			
			foreach (RegInfo eachInfo in infoList){	
				if (eachInfo.tag == eventTag) removeList.Add(eachInfo);	
			}

			foreach (RegInfo eachInfo in removeList){	
				infoList.Remove(eachInfo);
			}
			
		}

	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

	/** 取得預設ID */
	private string dispatchID () {
		string id = "_anonymous_"+this._dispatchIndex++;
		while (this._id2RegInfoDict.ContainsKey(id)) {
			id = "_anonymous_"+this._dispatchIndex++;
		}

		return id;
	}

	private void regInfo (string id, RegInfo info) {
		List<RegInfo> infoList;
		if (this._id2RegInfoDict.ContainsKey(id) == false) {
			infoList = new List<RegInfo>();
			this._id2RegInfoDict.Add(id, infoList);
		} else {
			infoList = this._id2RegInfoDict[id];
		}

		infoList.Add(info);
		infoList.Sort();
	}
}



}