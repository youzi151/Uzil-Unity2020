using Uzil.FX;
using Uzil;

namespace UZAPI {

public class FX {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*========發射/取消特效========*/

	/** 發射特效 */
	public static string Emit (string id, string sourcePath, string data_json, int luaCallbackID = -1) {
		string preloadID = FX.Preload(id, sourcePath, data_json);

		FXObj fx = FXMgr.Inst().GetFX(preloadID);
		if (fx == null) return null;

		DictSO data = null;
		if (data_json != null) {
			data = DictSO.Json(data_json);
		}
		else {
			data = DictSO.New();
		}

		fx.SetData(data);

		if (luaCallbackID != -1/*null*/) {
			fx.onDone.AddListener(new EventListener(() => {
				UZAPI.Callback.CallLua(luaCallbackID, null);
			}));
		}

		fx.Play(data);
		
		return fx.id;
	}

	/** 預載 */
	public static string Preload (string id, string sourcePath, string initData_json) {
		FXObj fx = FXMgr.Inst().GetFX(id);
		if (fx != null) return fx.id;

		DictSO initData = new DictSO();
		if (initData_json != null)
			initData = DictSO.Json(initData_json);

		if (id != null) {
			fx = FXMgr.Inst().ReuseFX(id, sourcePath, initData);
		}
		else {
			fx = FXMgr.Inst().ReuseFX("_anonymous_", sourcePath, initData);
		}

		if (fx == null) return null;

		return fx.id;
	}
	
	/** 設置資料 */
	public static void SetData (string id, string data_json) {
		FXObj fx = FXMgr.Inst().GetFX(id);
		if (fx == null) return;
		fx.SetData(DictSO.Json(data_json));
	}

	/** 播放特效 */
	public static void Play (string id, string playData_json = null) {
		FXObj fx = FXMgr.Inst().GetFX(id);
		if (fx == null) return;

		DictSO playData = null;
		if (playData_json != null) {
			playData = DictSO.Json(playData_json);
		}
		if (playData == null) playData = new DictSO();
		
		fx.Play(playData);
	}

	/** 停止特效 */
	public static void Stop (string id) {
		FXObj fx = FXMgr.Inst().GetFX(id);
		if (fx == null) return;
		fx.Stop();
	}

	/** 停止特效 */
	public static void Destroy (string id) {
		FXObj fx = FXMgr.Inst().GetFX(id);
		if (fx == null) return;
		fx.Terminate();
	}


	/** 呼叫 */
	public static void Call (string id, string msg, string callData_json) {
		FXObj fx = FXMgr.Inst().GetFX(id);
		if (fx == null) return;

		DictSO callData = null;
		if (callData_json != null) {
			callData = DictSO.Json(callData_json);
		}
		if (callData == null) callData = new DictSO();

		fx.Call(msg, callData);
	}


	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}

}