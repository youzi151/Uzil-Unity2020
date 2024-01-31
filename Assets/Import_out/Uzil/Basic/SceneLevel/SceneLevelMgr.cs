using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Uzil.SceneLevel {

public class SceneLevelMgr : MonoBehaviour {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	private static SceneLevelMgr _instance;

	/*=====================================Static Funciton=======================================*/

	public static SceneLevelMgr Inst () {
		if (SceneLevelMgr._instance == null){
			GameObject rootGObj = RootUtil.GetMember("SceneLevelMgr");
            SceneLevelMgr._instance = rootGObj.AddComponent<SceneLevelMgr>();
		}
		return SceneLevelMgr._instance;
	}


	/* 讀取場景 */
	public static void Load (string levelName, bool isWait = false, EventListener onLoaded = null) {
		SceneLevelMgr instance = SceneLevelMgr.Inst();

        // 預讀 關卡
		instance.PrepareLoadLevel(levelName);

        // 設 是否等候
		instance.isWait = isWait;

        // 若 當讀取完畢 事件 存在
		if (onLoaded != null) {
			instance.onLoaded += onLoaded.Once(); // 以參數傳入的話，事件為一次性
		}

        // 讀取 讀取畫面
		SceneManager.LoadScene(Const_SceneLevel.SCENE_LOADING);

        // 讀取關卡
		instance.LoadLevel();
	}

	/* 當以isWait狀態讀取場景時，呼叫喚場 */
	public static void Turn () {
		SceneLevelMgr instance = SceneLevelMgr.Inst();
		instance.TurnLevel();
	}

	/* 重讀當前場景 */
	public static void Reload (EventListener onLoaded = null) {
		SceneLevelMgr.Reload(false, onLoaded);
	}
	public static void Reload (bool isWait, EventListener onLoaded = null) {
		SceneLevelMgr instance = SceneLevelMgr.Inst();

		if (onLoaded != null){
			instance.onLoaded += onLoaded.Once();//以參數傳入的話，事件為一次性
		}

		instance.isWait = isWait;

		if (instance.prepareLoadLevelName == "_null") {
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		} else {
			SceneManager.LoadScene(Const_SceneLevel.SCENE_LOADING);
			instance.LoadLevel();
		}
	}

	/* 返回先前讀取的關卡 */
	public static void Back (bool isWait = false, EventListener onLoaded = null) {
		SceneLevelMgr instance = SceneLevelMgr.Inst();

		int historyCount = instance.loadedHistory.Count;
		if (historyCount <= 1) return;

		string previousLevelName = instance.loadedHistory[historyCount-2];

		SceneLevelMgr.Load(previousLevelName, isWait, onLoaded);
	}

	/*=========================================Members===========================================*/

	/* 關卡腳本 */
	public string levelScript = "_null";

	/* 要讀取的關卡名稱 */
	public string prepareLoadLevelName = "_null";

	/* 讀取完後是否等待，不立即換場 */
	public bool isWait = false;

	/* 是否已經呼叫過 onLoaded */
	private bool isCalledLoaded = false;

	/* 異步讀取狀態 */
	private AsyncOperation async;

	/* 已讀取的關卡紀錄 */
	public List<string> loadedHistory = new List<string>();

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/* 當新關卡已讀取完畢 */
	public Event onLoaded = new Event();

	/* 當已經切換到新關卡 */
	public Event onTurned = new Event();

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		// 若 沒有讀取狀態
		if (this.async == null) {
			return;
		}

		// 若 讀取進度已經超過80%(因為讀取完畢時的數值不會為100%)，且尚未呼叫已讀取
		else if (this.async.progress >= 0.8f && this.isCalledLoaded == false) {
			this.onLoaded.Call();
			this.isCalledLoaded = true;
		}

		// 若 已轉場到新關卡
		else if (this.async.progress >= 1f) {

			this.onTurned.Call();
			this.reset();
		}
	
	}
	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/
	
	/* 準備讀取關卡 */
	public void PrepareLoadLevel (string name, string levelScript = "_null") {
		this.prepareLoadLevelName = name;
		if (levelScript != "_null") this.levelScript = levelScript;

		int loadedCount = this.loadedHistory.Count;
		if (loadedCount > 0) {
			
			// 若 記錄中有重複的，則移除自該關卡以後的紀錄
			bool isFound = false;
			List<string> toRemove = new List<string>();
			foreach (string eachLoaded in this.loadedHistory) {
				if (eachLoaded == name) isFound = true;
				if (isFound) {
					toRemove.Add(eachLoaded);
				}
			}
			foreach (string eachToRemove in toRemove) {
				this.loadedHistory.Remove(eachToRemove);
			}
		}
		
	}

	/* 讀取關卡 */
	public void LoadLevel () {
		this.StartCoroutine(load());
	}

	/* 轉場 */
	public void TurnLevel () {
		this.async.allowSceneActivation = true;
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

	/* 異步讀取 */
	private IEnumerator load () {
		yield return new WaitForSeconds(1f);
		async = SceneManager.LoadSceneAsync(this.prepareLoadLevelName);
		async.allowSceneActivation = !this.isWait;

		yield return async;

		// 若 無重複 則 加入讀取紀錄
		if (this.loadedHistory.Contains(this.prepareLoadLevelName) == false) {
			this.loadedHistory.Add(this.prepareLoadLevelName);
		}

		this.onLoaded.Call();
	}

	/* 讀取完關卡後，重置設定、狀態 */
	private void reset () {
		this.isWait = false;
		this.isCalledLoaded = false;
		this.async = null;
	}

}


}