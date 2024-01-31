using System;
using System.Runtime.InteropServices;

using UnityEngine;

using UzEvent = Uzil.Event;
using Uzil.OtherLib.WindowsIconTools;

namespace Uzil.Misc {

/** 
 * 各個階段
 * 因為 每種設置視窗行為 (UnityEngine.Screen 或 user32) 都需要間隔1frame來讓Unity處理
 * 否則 實際畫面解析度 會有誤差問題
 */
public enum WindowStyleStep {
	None, Prepare, Set, Setting, Restyle, ResetPos, Done
}

public class WindowStyle : MonoBehaviour {

	struct WinRect {
		public int l, t, r, b;
	}

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/
	
	/** 實例 */
	protected static bool _isInstInit = false;
	protected static WindowStyle _inst = null;
	public static WindowStyle Inst () {
		if (WindowStyle._inst == null) {
			
			WindowStyle._isInstInit = true;
			WindowStyle._inst = RootUtil.GetChild("WindowStyle", RootUtil.GetMember("Misc")).AddComponent<WindowStyle>();
			WindowStyle._inst.Init();
			WindowStyle._isInstInit = false;

		}
		return WindowStyle._inst;
	}


	/** 要設定的項目 */
	public const int GWL_STYLE = -16;

	/** 視窗樣式 */
	public const long WS_WINDOWED = 
	//  0x14CA0000
		0x00080000 | // WS_SYSMENU (maybe?)
		0x00020000 | // WS_MINIMIZEBOX (maybe?)
		0x00C00000 | // WS_CAPTION
		0x04000000 | // WS_CLIPSIBLINGS
		0x10000000   // WS_VISIBLE
	;

	public const uint FLAGS_RESET = 
	0x0001 | // SWP_NOSIZE
	0x0002 | // SWP_NOMOVE
	0x0004 | // SWP_NOZORDER
	0x0020   // SWP_FRAMECHANGED
	;

	public const uint FLAGS_SIZEFIX = 
	0x0002 | // SWP_NOMOVE
	0x0004 | // SWP_NOZORDER
	0x0020   // SWP_FRAMECHANGED
	;



	/** 視窗 */
	protected static HandleRef _mainWindow;
	private static HandleRef mainWindow {
        get {
			if (_mainWindow.Handle == IntPtr.Zero) {
#if (UNITY_EDITOR || UNITY_STANDALONE)
				_mainWindow = new HandleRef(null, GetActiveWindow());
#else
				_mainWindow = new HandleRef(null, System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle);
#endif
			}
            return _mainWindow;
        }
	}

	/** 視窗 Icon */
	public static Texture2D icon = null;
	protected static Texture2D _lastIcon = null;
	protected static Texture2D _convertedIcon = null;
	protected static Texture2D GetIcon () {
		if (WindowStyle.icon == null) {
			WindowStyle.icon = Resources.Load<Texture2D>("Uzil/Texture/icon");
		}
		
		if (WindowStyle.icon != WindowStyle._lastIcon) {
			WindowStyle._convertedIcon = null;
			WindowStyle._lastIcon = WindowStyle.icon;
		}

		if (WindowStyle.icon == null) return null;

		if (WindowStyle._convertedIcon == null) {
			if (WindowStyle.icon.format == TextureFormat.BGRA32) {
				WindowStyle._convertedIcon = WindowStyle.icon;
			} else {
				WindowStyle._convertedIcon = new Texture2D(WindowStyle.icon.width, WindowStyle.icon.height, TextureFormat.BGRA32, false);
				WindowStyle._convertedIcon.SetPixels32(WindowStyle.icon.GetPixels32());
			}
		}

		return WindowStyle._convertedIcon;
	}

	/*=====================================Static Funciton=======================================*/

	/** 取得 視窗  */
	// https://docs.microsoft.com/en-us/windows/desktop/api/winuser/nf-winuser-getactivewindow
	[DllImport("user32.dll")]
	private static extern IntPtr GetActiveWindow();

	/** 取得 視窗矩形 */
	[DllImport("user32.dll")]
	private static extern bool GetWindowRect(HandleRef hWnd, out WinRect rect);

	/** 取得 內容矩形 */
	[DllImport("user32.dll")]
	private static extern bool GetClientRect(HandleRef hWnd, out WinRect rect);


	/** 設置 視窗相關 */
	private static IntPtr SetWindowLongPtr(HandleRef hWnd, int nIndex, long dwNewLong){
		if (IntPtr.Size == 8) return SetWindowLongPtr64(hWnd, nIndex, (IntPtr) dwNewLong);
		return new IntPtr(SetWindowLong32(hWnd, nIndex, (Int32) dwNewLong));
	}
	[DllImport("user32.dll", EntryPoint = "SetWindowLong")]
	private static extern int SetWindowLong32(HandleRef hWnd, int nIndex, int dwNewLong);
	[DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
	private static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, IntPtr dwNewLong);

	/** 取得 視窗相關 */
	[DllImport("user32.dll")]
	private static extern uint GetWindowLong(HandleRef hWnd, int nIndex);

	/** 設置 視窗位置 */
    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(HandleRef hWnd, IntPtr insertAfter, int x, int y, int w, int h, uint flags);

	/*=========================================Members===========================================*/

	/** 設置 階段 */
	protected WindowStyleStep _step {
		get {
			return this.__step;
		}
		set {
			// Debug.LogError(DictSO.EnumTo<WindowStyleStep>(value));
			this.__step = value;
		}
	}
	protected WindowStyleStep __step = WindowStyleStep.None;

	/** 目標 解析度 */
	protected Resolution? _targetResolution = null;
	protected bool _isTargetResolutionExist = false;

	/** 目標 全螢幕模式 */
	protected FullScreenMode? _targetFullScreenMode = null;

	/** 當 設置完畢 */
	public UzEvent onSetDone = new UzEvent();

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	public void Update () {

		if (this._step == WindowStyleStep.None) return;
		
		// 若 階段 為 準備中
		// 準備中階段的用意 為 避免在背景執行停止的狀況下，呼叫Set後只執行一次Update而導致設置過程中斷
		if (this._step == WindowStyleStep.Prepare) {
			this._step = WindowStyleStep.Set;
			return; // nextFrame
		}
		
		// 若 階段 為 設置
		if (this._step == WindowStyleStep.Set) {

			Resolution resolution;
			FullScreenMode fullScreenMode;
			
			// 取得 當前解析度
			if (this._isTargetResolutionExist) {
				resolution = (Resolution) this._targetResolution;
			} else {
				// 從 螢幕 取得 解析度
				resolution = Screen.currentResolution;
				// 以 視窗 解析度 覆蓋 尺寸
				resolution.width = Screen.width;
				resolution.height = Screen.height;
			}

			// 取得 當前全螢幕模式 
			if (this._targetFullScreenMode != null) {
				fullScreenMode = (FullScreenMode) this._targetFullScreenMode;
			} else {
				fullScreenMode = Screen.fullScreenMode;
			}
			
			if (this._targetFullScreenMode != null || this._isTargetResolutionExist) {
				// 實際改變
				Screen.SetResolution(resolution.width, resolution.height, fullScreenMode);
				// 清除 Unity多餘設置
				DeleteUnityGarbage.Inst().ClearWindowsRegistry();
			}

			this._step = WindowStyleStep.Setting;

			return; // nextFrame
		}

		// 若 階段 為 設置中 (Unity設置Screen相關後，會有不知名延遲)
		if (this._step == WindowStyleStep.Setting) {

			// 檢查 當前解析度 是否已經設為 目標解析度
			if (this._targetResolution != null && this._isTargetResolutionExist) {
				Resolution targetRes = (Resolution) this._targetResolution;
				// 若 尚未 則 返回
				if (Screen.width != targetRes.width || Screen.height != targetRes.height) {
					// Debug.LogError("Screen["+Screen.width+","+Screen.height+"] != target["+targetRes.width+","+targetRes.height+"]");
					return; // nextFrame
				} 
				// 若 成功 則 清空 目標
				this._isTargetResolutionExist = false;
			}

			// 檢查 當前全螢幕模式 是否已經設為 目標全螢幕模式
			if (this._targetFullScreenMode != null) {
				// 若 尚未 則 返回
				if (Screen.fullScreenMode != this._targetFullScreenMode) {
					// Debug.LogError("Screen.fullScreenMode["+DictSO.EnumTo<FullScreenMode>(Screen.fullScreenMode)+"] != targetFullScreenMode["+DictSO.EnumTo<FullScreenMode>((FullScreenMode)this._targetFullScreenMode)+"]");
					return; // nextFrame
				}
				// 若 成功 則 清空 目標
				this._targetFullScreenMode = null;
				// 設置 目標解析度 供修正視窗大小用
				this._targetResolution = new Resolution{width = Screen.width, height = Screen.height};
			}


			this._step = WindowStyleStep.Restyle;
			// return; // nextFrame
		}
		
		// 若 階段 為 重設樣式
		if (this._step == WindowStyleStep.Restyle) {

			if (Screen.fullScreenMode == FullScreenMode.Windowed) {

				// 重設視窗樣式
				SetWindowLongPtr(WindowStyle.mainWindow, WindowStyle.GWL_STYLE, WindowStyle.WS_WINDOWED);
				
				// 重設Icon
				Texture2D icon = WindowStyle.GetIcon();
				if (icon != null) {
					WindowIconTools.SetIcon(icon, WindowIconKind.Small);
					WindowIconTools.SetIcon(icon, WindowIconKind.Big);
				}
				// WindowIconTools.SetOverlayIcon(icon);

				// 刷新視窗
				Resolution targetResolution = (Resolution) this._targetResolution;
				SetWindowPos(WindowStyle.mainWindow, (IntPtr)(-2), 0, 0, targetResolution.width, targetResolution.height, WindowStyle.FLAGS_RESET);

				this._step = WindowStyleStep.ResetPos;
				return; // nextFrame

			} else {

				this._step = WindowStyleStep.Done;
				// return;
			}

		}

		// 若 階段 為 校正視窗
		if (this._step == WindowStyleStep.ResetPos) {

			if (Screen.fullScreenMode == FullScreenMode.Windowed) {
				
				// 取得 框架寬高 (視窗與內容差)
				WinRect winRect, cliRect;
				GetWindowRect(WindowStyle.mainWindow, out winRect);
				GetClientRect(WindowStyle.mainWindow, out cliRect);
				int frameWidth  = (winRect.r - winRect.l) - cliRect.r;
				int frameHeight = (winRect.b - winRect.t) - cliRect.b;

				// if (frameWidth == 0 && frameHeight == 0) {
				// 	this._step = WindowStyleStep.Restyle;
				// 	return; // nextFrame
				// }

				// 修正 視窗大小
				Resolution targetResolution = (Resolution) this._targetResolution;
				this._targetResolution = null;
				Debug.LogError("targetRes:"+targetResolution.width+"x"+targetResolution.height);
				SetWindowPos(WindowStyle.mainWindow, (IntPtr)(-2), 0, 0, targetResolution.width + frameWidth, targetResolution.height + frameHeight, WindowStyle.FLAGS_SIZEFIX);
				
				this._step = WindowStyleStep.Done;
				return; // nextFrame

			} else {

				this._step = WindowStyleStep.Done;
				// return;
			}

		}

		// 若 階段 為 完成
		if (this._step == WindowStyleStep.Done) {
				
			// 清除 Unity多餘設置
			DeleteUnityGarbage.Inst().ClearWindowsRegistry();

			this._step = WindowStyleStep.None;

			this.onSetDone.Call();
			return;
		}
	}
	
	public void OnApplicationQuit () {

	}

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 初始化 */
	public void Init () {
		
	}
	
	public void SetWindowStyle (long style) {
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
		SetWindowLongPtr(WindowStyle.mainWindow, WindowStyle.GWL_STYLE, style);
#endif
	}

	/** 取得 */
	public long? GetWindowStyle () {
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
		return GetWindowLong(WindowStyle.mainWindow, WindowStyle.GWL_STYLE);
#else
		return null;
#endif
	}

	public bool SetResolution (int w, int h) {
		
		// 設置 目標
		Resolution res = new Resolution();
		res.width = w;
		res.height = h;
		res.refreshRate = 0;

		// 檢查是否為支援的解析度
		bool isValid = false;
		foreach (Resolution each in Screen.resolutions) {
			if (each.width == res.width && each.height == res.height) {
				isValid = true;
				break;
			}
		}

		if (!isValid) return false;

		this._targetResolution = res;
		this._isTargetResolutionExist = true;

		this._step = WindowStyleStep.Prepare;
		return true;
	}

	public void SetFullScreenMode (FullScreenMode fullScreenMode) {

		// 設置 目標
		this._targetFullScreenMode = fullScreenMode;

		this._step = WindowStyleStep.Prepare;
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

}

}