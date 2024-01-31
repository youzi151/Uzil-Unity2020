﻿using UnityEngine;
using UnityEngine.UI;
using System;

namespace Uzil.Lua {

public class LuaConsoleUI : MonoBehaviour {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	public Action<string> onDoLua = null;

	/*========================================Components=========================================*/
	
	public Text luaText;

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	public void DoLua () {
		string lua = this.luaText.text;
		this.onDoLua(lua);
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

}



}