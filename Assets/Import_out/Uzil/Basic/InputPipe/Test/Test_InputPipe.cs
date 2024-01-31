using UnityEngine;

using Uzil.Util;


namespace Uzil.InputPipe {
public class Test_InputPipe : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	InputMgr inputMgr;

	public TextAsset textAsset;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*========================================Interface==========================================*/

	/*======================================Unity Function=======================================*/

	void Awake () {

		InputUtil.LogAllKeyCode();

		LoadInputSetting.LoadSetting();

		// 取得 實例
		this.inputMgr = InputMgr.Inst("_test");
		
		// 新增 轉換器 T or G -> 887
		// this.inputMgr.AddHandler("_testLayer", new InputHandler_Convert(887, new List<int>{
		// 	(int) KeyCode.T, (int) KeyCode.G
		// }));

		// C#
		this.inputMgr.AddOnInput("_testLayer", 87, new EventListener((data)=>{
			InputSignal signal = (InputSignal) data.Get("signal");
			Debug.Log("OnInput:"+signal.virtualKeyCode+" / "+signal.GetValueNum());
		}));

		this.inputMgr.AddOnInput("_testLayer", 88, new EventListener((data)=>{
			InputSignal signal = (InputSignal) data.Get("signal");
			Debug.Log("OnInput:"+signal.virtualKeyCode+" / "+signal.GetValueNum());
		}));
		

		// Lua
		// LuaUtil.DoString(this.textAsset.text);
		
	}

	void Update () {

		if (Input.GetKeyDown((KeyCode)283)) Debug.Log(283);

		// InputSignal signal = this.inputMgr.GetInput("1", 887);
		// if (signal == null) return;
		// Debug.Log("Update:"+signal.value);

		//test
		
	}

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}
