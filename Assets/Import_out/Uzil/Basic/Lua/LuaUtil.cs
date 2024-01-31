using System;
using System.Text;
using System.Collections;

using UnityEngine;

using LuaInterface;

using Uzil.Res;

namespace Uzil.Lua {

public class LuaUtil {

	/*======================================Constructor==========================================*/

	public LuaUtil () {

		this.state = new LuaState();

		LuaBinder.Bind(this.state);

#if UNITY_EDITOR

#else
		new LuaResLoader();
#endif
		this.state.Start();

		Application.wantsToQuit += ()=>{
			if (this.state != null) this.state.Dispose();
			return true;
		};
	}

	~LuaUtil () {

		if (this.state != null) this.state.Dispose();

#if UNITY_EDITOR
		Debug.Log("dispose state");
#endif
	}

	/*=====================================Static Members========================================*/

	/* 單例 */
	public static LuaUtil instance;


	/*=====================================Static Funciton=======================================*/

	/* 單例 */
	public static LuaUtil Inst () {
		if (LuaUtil.instance == null) {
			LuaUtil.Reload();
		}
		return LuaUtil.instance;
	}

	/* 重新讀取 */
	public static void Reload () {
		LuaUtil.instance = new LuaUtil();
		LuaUtil.instance._AddSearchPath(LuaConst.luaDir);
		LuaUtil.instance.openCJson();
		LuaUtil.instance.doMain();
	}

	/* 執行Lua文字 */
	public static T DoString<T> (string script) {
		return LuaUtil.Inst()._DoString<T>(script);
	}
	/* 執行Lua文字 */
	public static void DoString (string script) {
		LuaUtil.Inst()._DoString(script);
	}

	/* 讀取(執行)Lua檔案 */
	public static T DoFile<T> (string fileName) {
		return LuaUtil.Inst()._DoFile<T>(fileName);
	}
	/* 讀取(執行)Lua檔案 */
	public static void DoFile (string fileName) {
		LuaUtil.Inst()._DoFile(fileName);
	}

	/* 讀取Lua檔案(若已讀取過則不重複) */
	public static void Require (string fileName) {
		LuaUtil.Inst()._Require(fileName);
	}

	/* 呼叫Lua function */
	public static T CallFunction<T> (string funcName, params object[] args) {
		return LuaUtil.Inst()._CallFunction<T>(funcName, args);
	}
	/* 呼叫Lua function */
	public static void CallFunction (string funcName, params object[] args) {
		LuaUtil.Inst()._CallFunction(funcName, args);
	}

	/* 取得Lua 變數 */
	public static object GetItem (string itemPath) {
		return LuaUtil.Inst()._GetItem(itemPath);
	}

	/* 增加Lua路徑目錄 */
	public static void AddSearchPath (string fullPath) {
		LuaUtil.Inst()._AddSearchPath(fullPath);
	}

	/* 從LuaTable轉換 */
	public static DictSO TableToDict (LuaTable table) {
		DictSO dict = DictSO.New();
		foreach (DictionaryEntry entry in table.ToDictTable()) {
			dict.Set(entry.Key.ToString(), entry.Value);
		}
		return dict;
	}

	/*=========================================Members===========================================*/

	/* State */
	public LuaState state;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/* 執行Lua文字 */
	public T _DoString<T> (string script) {
		return this.state.DoString<T>(script);
		// return null;
	}
	/* 執行Lua文字 */
	public void _DoString (string script) {
		this.state.DoString(script);
		// return null;
	}

	/* 讀取(執行)Lua檔案 */
	public T _DoFile<T> (string fileName) {
		return this.state.DoFile<T>(fileName);
		// return null;
	}
	/* 讀取(執行)Lua檔案 */
	public void _DoFile (string fileName) {
		this.state.DoFile(fileName);
		// return null;
	}

	/* 讀取Lua檔案(若已讀取過則不重複) */
	public void _Require (string fileName) {
		this.state.Require(fileName);
	}

	/* 呼叫Lua function */
	public void _CallFunction (string funcName, params object[] args) {
		LuaFunction func = this.state.GetFunction(funcName);
		if (func == null) return;
		try {
			switch (args.Length) {
				case 0:
					func.Call();
					break;
				case 1:
					func.Call<object>(
						args[0]
					);
					break;
				case 2:
					func.Call<object, object>(
						args[0], args[1]
					);
					break;
				case 3:
					func.Call<object, object, object>(
						args[0], args[1], args[2]
					);
					break;
				case 4:
					func.Call<object, object, object, object>(
						args[0], args[1], args[2], args[3]
					);
					break;
				case 5:
					func.Call<object, object, object, object, object>(
						args[0], args[1], args[2], args[3], args[4]
					);
					break;
				case 6:
					func.Call<object, object, object, object, object, object>(
						args[0], args[1], args[2], args[3], args[4], args[5]
					);
					break;
				case 7:
					func.Call<object, object, object, object, object, object, object>(
						args[0], args[1], args[2], args[3], args[4], args[5], args[6]
					);
					break;
				case 8:
					func.Call<object, object, object, object, object, object, object, object>(
						args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]
					);
					break;
				case 9:
					func.Call<object, object, object, object, object, object, object, object, object>(
						args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]
					);
					break;
				default:
					Debug.Log("[LuaUtil]: max arg 9!");
					break;
			}
		}
		catch (LuaException e) {
			StringBuilder sb = new StringBuilder();
			sb.Append("[Lua] : call function fail: " + funcName + "(");
			for (int i = 0; i < args.Length; i++) {
				if (args[i] != null) {
					sb.Append(args[i].ToString());
				}
				else {
					sb.Append("null");
				}
				sb.Append(", ");
			}
			sb.Append(")");
			Debug.Log(sb.ToString());
			Debug.Log(e);
			Debug.Log(e.GetBaseException());
			throw e;
		}
	}

	/* 呼叫Lua function */
	public T _CallFunction<T> (string funcName, params object[] args) {
		T res = default(T);
		LuaFunction func = this.state.GetFunction(funcName);
		if (func == null) return res;
		try {
			switch (args.Length) {
				case 0:
					res = func.Invoke<T>();
					break;
				case 1:
					res = func.Invoke<object, T>(
						args[0]
					);
					break;
				case 2:
					res = func.Invoke<object, object, T>(
						args[0], args[1]
					);
					break;
				case 3:
					res = func.Invoke<object, object, object, T>(
						args[0], args[1], args[2]
					);
					break;
				case 4:
					res = func.Invoke<object, object, object, object, T>(
						args[0], args[1], args[2], args[3]
					);
					break;
				case 5:
					res = func.Invoke<object, object, object, object, object, T>(
						args[0], args[1], args[2], args[3], args[4]
					);
					break;
				case 6:
					res = func.Invoke<object, object, object, object, object, object, T>(
						args[0], args[1], args[2], args[3], args[4], args[5]
					);
					break;
				case 7:
					res = func.Invoke<object, object, object, object, object, object, object, T>(
						args[0], args[1], args[2], args[3], args[4], args[5], args[6]
					);
					break;
				case 8:
					res = func.Invoke<object, object, object, object, object, object, object, object, T>(
						args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7]
					);
					break;
				case 9:
					res = func.Invoke<object, object, object, object, object, object, object, object, object, T>(
						args[0], args[1], args[2], args[3], args[4], args[5], args[6], args[7], args[8]
					);
					break;
				default:
					Debug.Log("[LuaUtil]: max arg 9!");
					break;
			}
		}
		catch (Exception e) {
			StringBuilder sb = new StringBuilder();
			sb.Append("[Lua] : call function fail: " + funcName + "(");
			for (int i = 0; i < args.Length; i++) {
				if (args[i] != null) {
					sb.Append(args[i].ToString());
				}
				else {
					sb.Append("null");
				}
				sb.Append(", ");
			}
			sb.Append(")");
			Debug.Log(sb.ToString());
			Debug.Log(e);
		}
		return res;
		// return null;
	}

	/* 取得Lua 變數 */
	public object _GetItem (string itemPath) {
		return this.state[itemPath];
		// return null;
	}

	/* 增加Lua路徑目錄 */
	public void _AddSearchPath (string fullPath) {
		this.state.AddSearchPath(fullPath);
	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/


	/* 載入Json庫(備用) */
	private void openCJson () {
		this.state.LuaGetField(LuaIndexes.LUA_REGISTRYINDEX, "_LOADED");
		this.state.OpenLibs(LuaDLL.luaopen_cjson);
		this.state.LuaSetField(-2, "cjson");

		this.state.OpenLibs(LuaDLL.luaopen_cjson_safe);
		this.state.LuaSetField(-2, "cjson.safe");
	}

	private void doMain () {
		string mainFileStr = ResMgr.Get<string>(new ResReq(LuaConst.mainFilePath));
		this._DoString(mainFileStr);
	}

}



}