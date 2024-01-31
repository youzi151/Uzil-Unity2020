using UnityEngine;

using UzEvent = Uzil.Event;

namespace Uzil.BuiltinUtil {

/**
 * 提供給 非MonoBehaviour 使用 OnApplicationPause, OnApplicationResume...等
 */
public class ResolutionChangeEvent : MonoBehaviour {
	

	/** 實例 */
	protected static bool _isInstInit = false;
	protected static ResolutionChangeEvent _inst = null;
	public static ResolutionChangeEvent Inst () {
		if (ResolutionChangeEvent._inst == null) {

			ResolutionChangeEvent._isInstInit = true;
			ResolutionChangeEvent._inst = RootUtil.GetChild("ResolutionChangeEventInit", RootUtil.GetMember("BuiltinUtil")).AddComponent<ResolutionChangeEvent>();
			ResolutionChangeEvent._inst.Init();
			ResolutionChangeEvent._isInstInit = false;
			
		}
		return ResolutionChangeEvent._inst;
	}
	
	/** 之前的 寬 */
	protected int lastScreenWidth = 0;

	/** 之前的 寬 */
	protected int lastScreenHeight = 0;

	/** 當解析度變更 事件 */
	public UzEvent onResolutionChanged = new UzEvent();

	
	void Awake () {
		// 禁用 非 Inst() 建立
		if (ResolutionChangeEvent._isInstInit == false) {
			UnityEngine.Object.Destroy(this);
			return;
		}
	}
	
	void Update () {
		// 若 任一屬性變化
		if (this.lastScreenWidth != Screen.width
		 || this.lastScreenHeight != Screen.height) {

			// 更新 所有屬性
			this.lastScreenWidth = Screen.width;
			this.lastScreenHeight = Screen.height;
			
			// 呼叫事件
			this.onResolutionChanged.Call();
			// 發送事件
			EventBus.Inst().Post("onResolutionChanged", new DictSO().Set("width", Screen.width).Set("height", Screen.height));
		}
	}

	public void Init () {
		this.lastScreenWidth = Screen.width;
		this.lastScreenHeight = Screen.height;
	}

}

}
