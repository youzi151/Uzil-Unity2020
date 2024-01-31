using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Uzil.BuiltinUtil;

using XInputDotNetPure;

namespace Uzil.Util {

public enum InputKeyState {
	None, Down, Pressed, Up
}

public enum InputType {
	None, Key, Axis
}

public class InputUtil {

	/*=========================================Members===========================================*/

	/* 是否呼叫按下 */
	public static bool isCallDown = false;

	/** 所有按鍵 */
	public readonly static Array keycodes = Enum.GetValues(typeof(KeyCode));

	/** 搖桿數量 */
	protected readonly static int gamePadCount = 4;
	/** 搖桿按鍵數量 */
	protected readonly static int gamePadBtnCount = 29;

	/** 搖桿狀態 當前 */
	public static GamePadState[] gamePadStates = new GamePadState[gamePadCount];
	/** 搖桿狀態 前一次 */
	protected static GamePadState[] gamePadStates_last = new GamePadState[gamePadCount];
	/** 搖桿 按鍵狀態 比較 */
	protected static ButtonState[,,] gamePadBtnStateCompares = new ButtonState[gamePadCount, gamePadBtnCount, 2];

	/** 是否已經刷新過 (避免多次刷新 洗掉狀態變換) */
	protected static bool isUpdated = false;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 刷新 */
	public static void UpdateOnce () {
		if (InputUtil.isUpdated) return;
		InputUtil.isUpdated = true;

		InputUtil.updateXInput();
	}

	/** 準備好下次刷新 */
	public static void ReadyNextUpdate () {
		InputUtil.isUpdated = false;
	}

	/** 取得輸入類型 */
	public static InputType GetInputType (int keyCode) {

		if (keyCode < 1000) {

		} else if (keyCode < ((InputUtil.gamePadCount+1) * 1000)) {

			// 搖桿序號
			int gamePadIdx = (keyCode / 1000)-1;
			// 在搖桿中的keycodeIdx
			int keyIdx = keyCode % 1000;

			switch (keyIdx) {
				case 17: // XInput.LS.X
				case 18: // XInput.LS.Y
				case 23: // XInput.RS.X
				case 24: // XInput.RS.Y
					return InputType.Axis;
			}
		}

		return InputType.Key;
	}


	// ########   #######  #### ##    ## ######## ######## ########  
	// ##     ## ##     ##  ##  ###   ##    ##    ##       ##     ## 
	// ##     ## ##     ##  ##  ####  ##    ##    ##       ##     ## 
	// ########  ##     ##  ##  ## ## ##    ##    ######   ########  
	// ##        ##     ##  ##  ##  ####    ##    ##       ##   ##   
	// ##        ##     ##  ##  ##   ###    ##    ##       ##    ##  
	// ##         #######  #### ##    ##    ##    ######## ##     ## 

	/* 檢測是否[操作]被按下 */
	public static bool IsInputDown () {
		bool isDown = false;

		switch (SystemInfo.deviceType) {
			case DeviceType.Handheld:

				if (Input.touchCount > 0) {
					isDown = true;
					InputUtil.isCallDown = true;
				}

				break;

			case DeviceType.Desktop:

				if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) {
					isDown = true;
					InputUtil.isCallDown = true;
				}

				break;
			case DeviceType.Unknown:

				break;


		}
		return isDown;

	}


	/* 檢測是否[操作]彈起 */
	public static bool IsInputUp () {
		if (InputUtil.isCallDown && InputUtil.IsInputDown() == false) {
			InputUtil.isCallDown = false;
			return true;
		}
		return false;
	}

	/* 取得[操作]按下數量 */
	public static int GetInputCount () {

		switch (SystemInfo.deviceType) {
			case DeviceType.Handheld:

				return Input.touchCount;

			case DeviceType.Desktop:

				if (Input.GetMouseButton(1)) return 2;

				return 1;

		}
		return 0;
	}

	/* 是否[操作]位於UI上 */
	public static bool IsPointerOverUI () {
		Canvas canvas = CanvasUtil.GetMain().canvas;
		if (canvas == null) return false;
		Vector2 screenPosition = InputUtil.GetInputPos();


		// 通过画布上的 GraphicRaycaster 组件发射射线
		// 实例化点击事件
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		// 将点击位置的屏幕坐标赋值给点击事件
		eventDataCurrentPosition.position = screenPosition;
		// 获取画布上的 GraphicRaycaster 组件
		GraphicRaycaster uiRaycaster = canvas.gameObject.GetComponent<GraphicRaycaster>();

		List<RaycastResult> results = new List<RaycastResult>();
		// GraphicRaycaster 发射射线
		uiRaycaster.Raycast(eventDataCurrentPosition, results);

		if (results.Count == 0) return false;

		return true;
	}

	/**
	 * 取得輸入位置----------------------------------------------------------
	 * 
	 * 觸控操作時，如果在手指離開後才取得輸入位置的話會沒辦法拿到。
	 * 故使用lastPos作為最後的位置，無法取得輸入時代替輸入位置。
	 */

	static Vector3 lastPos = Vector3.zero;

	/* 取得輸入位置 */
	public static Vector3 GetInputPos () {
		// 初始化輸入位置
		Vector3 inputPos = Vector3.zero;

		// 依據操作是否為觸碰，return 游標位置
		/* XXX:不要回傳預設的Vector3.zero，可以賦值就要賦值，必要時以lastPos代替 */


		switch (SystemInfo.deviceType) {
			case DeviceType.Handheld:

				if (Input.touchCount != 0) { inputPos = (Vector3) Input.GetTouch(0).position; }
				else return lastPos;

				break;
			case DeviceType.Desktop:

				if (Input.GetMouseButton(0) || Input.GetMouseButton(1)) { inputPos = (Vector3) Input.mousePosition; }
				else return lastPos;

				break;
		}


		lastPos = inputPos;
		return inputPos;
	}


	/**
	 * 取得碰撞物件----------------------------------------------------------
	 */
	public static GameObject GetHitObj () {
		if (CameraUtil.GetMain() == null) return null;

		if (EventSystem.current.IsPointerOverGameObject()) {
			if (EventSystem.current.currentSelectedGameObject != null) {
				return EventSystem.current.currentSelectedGameObject;
			}
		}

		// 以游標位置從攝影機射出Ray
		Ray screenRay = CameraUtil.GetMain().ScreenPointToRay(InputUtil.GetInputPos());
		RaycastHit hitInfo;

		// 檢測是否碰撞物件，並依 碰撞資訊 return被碰撞物
		// 若沒碰撞到物件則 return null
		if (Physics.Raycast(screenRay, out hitInfo)) {
			GameObject hitObj = hitInfo.collider.gameObject;
			//Debug.Log ("hit "+ hitObj.name);
			return hitObj;
		}
		else {
			return null;
		}
	}

	/* 取得位置差距 */
	public static Vector2 GetDeltaPostion () {
		switch (SystemInfo.deviceType) {

			case DeviceType.Handheld:

				if (Input.touchCount != 0) {
					return Input.GetTouch(0).deltaPosition;
				}

				break;

			case DeviceType.Desktop:

				// if (Input.GetMouseButton(0)||Input.GetMouseButton(1)){
				// 	return Input.mousePosition;
				// }

				break;
		}

		return Vector2.zero;
	}

	// ##    ## ######## ##    ## 
	// ##   ##  ##        ##  ##  
	// ##  ##   ##         ####   
	// #####    ######      ##    
	// ##  ##   ##          ##    
	// ##   ##  ##          ##    
	// ##    ## ########    ##    

	public static void LogAllKeyCode () {
		foreach (KeyCode each in InputUtil.keycodes) {
			Debug.Log(each.ToString()+":"+(int)each);
		}
	}

	public static bool GetKey (int keycode) {
		return InputUtil.getKey(keycode, InputKeyState.Pressed);
	}
	
	public static bool GetKeyUp (int keycode) {
		return InputUtil.getKey(keycode, InputKeyState.Up);
	}

	public static bool GetKeyDown (int keycode) {
		return InputUtil.getKey(keycode, InputKeyState.Down);
	}

	protected static bool getKey (int keycode, InputKeyState getKeyState) {

		if (keycode < 1000) {

			KeyCode unityKeyCode = (KeyCode) keycode;

			switch (getKeyState) {
				case InputKeyState.Pressed:
					if (Input.GetKey(unityKeyCode)) { return true; }
					break;

				case InputKeyState.Up:
					if (Input.GetKeyUp(unityKeyCode)) { return true; }
					break;

				case InputKeyState.Down:
					if (Input.GetKeyDown(unityKeyCode)) { return true; }
					break;
			}
			
		} else if (keycode < ((InputUtil.gamePadCount+1) * 1000)) {

			// 搖桿序號
			int gamePadIdx = (keycode / 1000)-1;
			// 在搖桿中的keycodeIdx
			int btnIdx = keycode % 1000;

			if (btnIdx < InputUtil.gamePadBtnCount) {

				ButtonState last = InputUtil.gamePadBtnStateCompares[gamePadIdx, btnIdx, 0];
				ButtonState curr = InputUtil.gamePadBtnStateCompares[gamePadIdx, btnIdx, 1];
				
				switch (getKeyState) {
					case InputKeyState.Pressed:
						if (curr == ButtonState.Pressed) { return true; }
						break;
						
					case InputKeyState.Up:
						if (InputUtil.isXInputBtnUp(last, curr)) { return true; }
						break;

					case InputKeyState.Down:
						if (InputUtil.isXInputBtnDown(last, curr)) { return true; }
						break;
				}
			}

			
		}

		return false;
	}

	/** 取得 所有按壓中按鍵 */
	public static List<int> GetKeys () {
		List<int> list = new List<int>();
		InputUtil.eachXInputBtn((last, curr) => curr == ButtonState.Pressed, list);
		InputUtil.eachKeyCode(each => Input.GetKey(each) ? (KeyCode?) each : null, list);
		return list;
	}

	/** 取得 所有按下按鍵 */
	public static List<int> GetKeysDown () {
		List<int> list = new List<int>();
		InputUtil.eachXInputBtn(InputUtil.isXInputBtnDown, list);
		if (Input.anyKeyDown) {
			InputUtil.eachKeyCode(each => Input.GetKeyDown(each) ? (KeyCode?) each : null, list);
		}
		return list;
	}

	/** 取得 所有彈起按鍵 */
	public static List<int> GetKeysUp () {
		List<int> list = new List<int>();
		InputUtil.eachXInputBtn(InputUtil.isXInputBtnUp, list);
		InputUtil.eachKeyCode(each => Input.GetKeyUp(each) ? (KeyCode?) each : null, list);
		return list;
	}

	//    ###    ##     ## ####  ######  
	//   ## ##    ##   ##   ##  ##    ## 
	//  ##   ##    ## ##    ##  ##       
	// ##     ##    ###     ##   ######  
	// #########   ## ##    ##        ## 
	// ##     ##  ##   ##   ##  ##    ## 
	// ##     ## ##     ## ####  ######  

	public static float GetAxis (int keycode) {

		if (keycode < 1000) {

		} else if (keycode < ((InputUtil.gamePadCount+1) * 1000)) {

			// 搖桿序號
			int gamePadIdx = (keycode / 1000)-1;
			// 在搖桿中的keycodeIdx
			int axisIdx = keycode % 1000;

			GamePadState state = InputUtil.gamePadStates[gamePadIdx];

			switch (axisIdx) {
				case 17:
					return state.ThumbSticks.Left.X;
				case 18:
					return state.ThumbSticks.Left.Y;
				case 23:
					return state.ThumbSticks.Right.X;
				case 24:
					return state.ThumbSticks.Right.Y;
			}
			
		}

		return 0;
	}


	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

	private static void updateXInput () {

		// GamePad
		for (int padIdx = 0; padIdx < InputUtil.gamePadCount; padIdx++) {
			
			GamePadState last = InputUtil.gamePadStates[padIdx];
			GamePadState curr = GamePad.GetState((PlayerIndex) padIdx);
			InputUtil.gamePadStates_last[padIdx] = last; 
			InputUtil.gamePadStates[padIdx] = curr;

			setGamePadBtnStateCompare(padIdx, 0, last.Buttons.Start, curr.Buttons.Start);
			setGamePadBtnStateCompare(padIdx, 1, last.Buttons.Back, curr.Buttons.Back);
			setGamePadBtnStateCompare(padIdx, 2, last.Buttons.Guide, curr.Buttons.Guide);
			setGamePadBtnStateCompare(padIdx, 3, last.Buttons.LeftStick, curr.Buttons.LeftStick);
			setGamePadBtnStateCompare(padIdx, 4, last.Buttons.RightStick, curr.Buttons.RightStick);
			setGamePadBtnStateCompare(padIdx, 5, last.Buttons.LeftShoulder, curr.Buttons.LeftShoulder);
			setGamePadBtnStateCompare(padIdx, 6, last.Buttons.RightShoulder, curr.Buttons.RightShoulder);
			setGamePadBtnStateCompare(padIdx, 7, last.Buttons.A, curr.Buttons.A);
			setGamePadBtnStateCompare(padIdx, 8, last.Buttons.B, curr.Buttons.B);
			setGamePadBtnStateCompare(padIdx, 9, last.Buttons.X, curr.Buttons.X);
			setGamePadBtnStateCompare(padIdx, 10, last.Buttons.Y, curr.Buttons.Y);
			setGamePadBtnStateCompare(padIdx, 11, last.DPad.Up, curr.DPad.Up);
			setGamePadBtnStateCompare(padIdx, 12, last.DPad.Down, curr.DPad.Down);
			setGamePadBtnStateCompare(padIdx, 13, last.DPad.Left, curr.DPad.Left);
			setGamePadBtnStateCompare(padIdx, 14, last.DPad.Right, curr.DPad.Right);
			setGamePadBtnStateCompare(padIdx, 15, InputUtil.axisToBtn(last.Triggers.Left, true), InputUtil.axisToBtn(curr.Triggers.Left, true));
			setGamePadBtnStateCompare(padIdx, 16, InputUtil.axisToBtn(last.Triggers.Right, true), InputUtil.axisToBtn(curr.Triggers.Right, true));
			// 17 : Axis ThumbSticks.Left.X
			// 18 : Axis ThumbSticks.Left.Y
			setGamePadBtnStateCompare(padIdx, 19, InputUtil.axisToBtn(last.ThumbSticks.Left.Y, true), InputUtil.axisToBtn(curr.ThumbSticks.Left.Y, true));
			setGamePadBtnStateCompare(padIdx, 20, InputUtil.axisToBtn(last.ThumbSticks.Left.Y, false), InputUtil.axisToBtn(curr.ThumbSticks.Left.Y, false));
			setGamePadBtnStateCompare(padIdx, 21, InputUtil.axisToBtn(last.ThumbSticks.Left.X, false), InputUtil.axisToBtn(curr.ThumbSticks.Left.X, false));
			setGamePadBtnStateCompare(padIdx, 22, InputUtil.axisToBtn(last.ThumbSticks.Left.X, true), InputUtil.axisToBtn(curr.ThumbSticks.Left.X, true));
			// 23 : Axis ThumbSticks.Right.X
			// 24 : Axis ThumbSticks.Right.Y
			setGamePadBtnStateCompare(padIdx, 25, InputUtil.axisToBtn(last.ThumbSticks.Right.Y, true), InputUtil.axisToBtn(curr.ThumbSticks.Right.Y, true));
			setGamePadBtnStateCompare(padIdx, 26, InputUtil.axisToBtn(last.ThumbSticks.Right.Y, false), InputUtil.axisToBtn(curr.ThumbSticks.Right.Y, false));
			setGamePadBtnStateCompare(padIdx, 27, InputUtil.axisToBtn(last.ThumbSticks.Right.X, false), InputUtil.axisToBtn(curr.ThumbSticks.Right.X, false));
			setGamePadBtnStateCompare(padIdx, 28, InputUtil.axisToBtn(last.ThumbSticks.Right.X, true), InputUtil.axisToBtn(curr.ThumbSticks.Right.X, true));
		}
	}

	private static void eachKeyCode (Func<KeyCode, KeyCode?> filtter, List<int> res) {
		foreach (KeyCode each in InputUtil.keycodes) {
			KeyCode? toAdd = filtter(each);
			if (toAdd != null) res.Add((int) toAdd);
		}
	}

	private static void eachXInputBtn (Func<ButtonState, ButtonState, bool> filtter, List<int> res) {

		// GamePad
		for (int padIdx = 0; padIdx < InputUtil.gamePadCount; padIdx++) {

			int keyCode_base = (padIdx+1) * 1000;
			
			for (int btnIdx = 0; btnIdx < InputUtil.gamePadBtnCount; btnIdx++) {
				
				ButtonState last = InputUtil.gamePadBtnStateCompares[padIdx, btnIdx, 0];
				ButtonState curr = InputUtil.gamePadBtnStateCompares[padIdx, btnIdx, 1];

				if (filtter(last, curr)) {
					res.Add(keyCode_base+btnIdx);
				}
			}
		}

	}

	private static bool isXInputBtnDown (ButtonState last, ButtonState curr) {
		return last == ButtonState.Released && curr == ButtonState.Pressed;
	}

	private static bool isXInputBtnUp (ButtonState last, ButtonState curr) {
		return last == ButtonState.Pressed && curr == ButtonState.Released;
	}

	private static ButtonState axisToBtn (float axisVal, bool isPostive) {
		if (isPostive) {
			if (axisVal > 0.1f) return ButtonState.Pressed;
		} else {
			if (axisVal < -0.1f) return ButtonState.Pressed;
		}
		return ButtonState.Released;
	}

	private static void setGamePadBtnStateCompare (int padIdx, int keyCodeInGamePad, ButtonState last, ButtonState curr) {
		InputUtil.gamePadBtnStateCompares[padIdx, keyCodeInGamePad, 0] = last;
		InputUtil.gamePadBtnStateCompares[padIdx, keyCodeInGamePad, 1] = curr;
	}

}


}