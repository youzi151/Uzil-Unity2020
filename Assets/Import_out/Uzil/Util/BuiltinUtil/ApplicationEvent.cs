using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using UzEvent = Uzil.Event;

namespace Uzil.BuiltinUtil {

/**
 * 提供給 非MonoBehaviour 使用 OnApplicationPause, OnApplicationResume...等
 */

public class ApplicationEvent : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/** 是否 已經離開 */
	protected static bool IsAppQuit = false;

	/** 實例 */
	protected static bool _isInstInit = false;
	protected static ApplicationEvent _inst = null;
	public static ApplicationEvent Inst () {
		if (ApplicationEvent.IsAppQuit) return null;
		if (ApplicationEvent._inst == null) {
			ApplicationEvent._isInstInit = true;
			ApplicationEvent._inst = RootUtil.GetChild("ApplicationEvent", RootUtil.GetMember("BuiltinUtil")).AddComponent<ApplicationEvent>();
			ApplicationEvent._inst.Init();
			ApplicationEvent._isInstInit = false;
		}
		return ApplicationEvent._inst;
	}

	public static UzEvent onApplicationPause {
		get {
			if (ApplicationEvent.IsAppQuit) return null;
			return ApplicationEvent.Inst().onAppPause;
		}
	}

	public static UzEvent onApplicationResume {
		get {
			if (ApplicationEvent.IsAppQuit) return null;
			return ApplicationEvent.Inst().onAppResume;
		}
	}

	public static UzEvent onApplicationQuit {
		get {
			if (ApplicationEvent.IsAppQuit) return null;
			return ApplicationEvent.Inst().onAppQuit;
		}
	}

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	public UzEvent onAppPause = new UzEvent();
	public UzEvent onAppResume = new UzEvent();
	public UzEvent onAppQuit = new UzEvent();

	/*======================================Unity Function=======================================*/

	void Awake () {
		// 禁用 非 Inst() 建立
		if (ApplicationEvent._isInstInit == false) {
			UnityEngine.Object.Destroy(this);
			return;
		}
	}

	void OnApplicationPause (bool pauseStatus) {
		if (pauseStatus) this.onAppPause.Call();
		else this.onAppResume.Call();
	}

	void OnApplicationQuit() {
		ApplicationEvent.IsAppQuit = true;
		this.onAppQuit.Call();
	}
	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 初始化 */
	public void Init () {
		
#if UNITY_EDITOR
		EditorApplication.pauseStateChanged += (state)=>{
			if (state == PauseState.Paused) {
				this.OnApplicationPause(true);
			} else if (state == PauseState.Unpaused) {
				this.OnApplicationPause(false);
			}
		};
#endif
		
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}
