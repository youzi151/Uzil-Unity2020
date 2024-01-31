
using Uzil;
using UzEventBus = Uzil.EventBus;

namespace UZAPI {

public class EventBus {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** 清空 */
	public static void Clear (string evtBusKey) {
		UzEventBus evtBus = UzEventBus.Inst(evtBusKey);
		if (evtBus == null) return;

		evtBus.Clear();
	}

	/** 發送事件 */
	public static void Post (string evtBusKey, string tag) {
		UZAPI.EventBus.Post(evtBusKey, tag, "");
	}
	public static void Post (string evtBusKey, string tag, string data_json) {		
		UzEventBus evtBus = UzEventBus.Inst(evtBusKey);
		if (evtBus == null) return;

		DictSO data = null;
		if (data_json != null && data_json != "") {
			data = DictSO.Json(data_json);
		}

		evtBus.Post(tag, data);
	}

	/** 註冊事件 (for C#) */
	public static void On (string evtBusKey, string tag, EventListener listener) {
		UzEventBus evtBus = UzEventBus.Inst(evtBusKey);
		if (evtBus == null) return;

		evtBus.On("", tag, listener);
	}
	public static void On (string evtBusKey, string id, string tag, EventListener listener) {
		UzEventBus evtBus = UzEventBus.Inst(evtBusKey);
		if (evtBus == null) return;

		evtBus.On(id, tag, listener);
	}

	/** 註冊事件 (for Lua) */
	public static void On (string evtBusKey, string tag, int luaCallbackID) {
		EventBus.On(evtBusKey, "", tag, luaCallbackID);
	}
	public static void On (string evtBusKey, string id, string tag, int luaCallbackID) {		
		UzEventBus evtBus = UzEventBus.Inst(evtBusKey);
		if (evtBus == null) return;

		EventListener listener = new EventListener((data) => {
			UZAPI.Callback.CallLua_cs(luaCallbackID, data);
		});

		evtBus.On(id, tag, listener);
	}

	/** 註冊任何事件 (for Lua) */
	public static void OnAny (string evtBusKey, int luaCallbackID) {
		EventBus.OnAny(evtBusKey, "", luaCallbackID);
	}
	public static void OnAny (string evtBusKey, string id, int luaCallbackID) {
		UzEventBus evtBus = UzEventBus.Inst(evtBusKey);
		if (evtBus == null) return;

		EventListener listener = new EventListener((data) => {
			UZAPI.Callback.CallLua_cs(luaCallbackID, data);
		});

		evtBus.OnAny(id, listener);
	}


	/** 排序事件 */
	public static void Sort (string evtBusKey, string id, string tag, float sort) {
		UzEventBus evtBus = UzEventBus.Inst(evtBusKey);
		if (evtBus == null) return;
		evtBus.Sort(id, tag, sort);
	}

	/** 排序任意事件 */
	public static void SortAny (string evtBusKey, string id, float sort) {
		UzEventBus evtBus = UzEventBus.Inst(evtBusKey);
		if (evtBus == null) return;
		evtBus.SortAny(id, sort);
	}


	/** 註銷事件 */
	public static void Off (string evtBusKey, string id) {
		EventBus.OffByID(evtBusKey, id);
	}
	public static void OffByID (string evtBusKey, string id) {
		UzEventBus evtBus = UzEventBus.Inst(evtBusKey);
		if (evtBus == null) return;

		evtBus.OffID(id);
	}
	public static void OffByTag (string evtBusKey, string tag) {
		UzEventBus evtBus = UzEventBus.Inst(evtBusKey);
		if (evtBus == null) return;

		evtBus.OffTag(tag);
	}

	/** 取得預設Key */
	public static string GetDefaultKey () {
		return UzEventBus.defaultKey;
	}

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}

}