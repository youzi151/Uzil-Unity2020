using UnityEngine;

namespace Uzil.Lua {

public class LuaConsole : MonoBehaviour {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/* 是否顯示 */
	public bool isShowed = false;

	/* 當前視窗 */
	public GameObject currentWindow;

	/* 預製物件 */
	public GameObject consoleUIPrefab;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	void Update () {
		if (Input.GetKeyDown(KeyCode.F1)) {
			this.Toggle();
		}
	}

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/* 開啟/關閉 */
	public void Toggle () {
		if (this.isShowed) this.Close();
		else this.Show();
	}

	/* 顯示 */
	public void Show () {
		if (this.isShowed) return;

		if (this.currentWindow == null) {
			this.currentWindow = GameObject.Instantiate(this.consoleUIPrefab);
			this.currentWindow.GetComponent<LuaConsoleUI>().onDoLua = (lua)=>{
				this.DoLua(lua);
			};
		}
		this.currentWindow.SetActive(true);
		this.isShowed = true;
	}

	/* 關閉 */
	public void Close () {
		if (this.currentWindow != null) {
			this.currentWindow.SetActive(false);
			this.isShowed = false;
		}
	}

	/* 執行Lua */
	public void DoLua (string lua) {
		LuaUtil.DoString(lua);
		this.Close();
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

}



}