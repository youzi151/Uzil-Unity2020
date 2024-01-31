#if DISCORD

using System;
using System.Collections.Generic;

using UnityEngine;
using Discord;

using Discd = Discord;
using DscrdInst = Discord.Discord;

namespace Uzil.ThirdParty.Discord {

public class DiscordInst : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/** 實例 */
	protected static bool _isInstInit = false;
	protected static DiscordInst _inst;
	public static DiscordInst Inst () {
		if (DiscordInst._inst == null) {
			DiscordInst._isInstInit = true;
			DiscordInst._inst = RootUtil.GetChild("Discord", RootUtil.GetMember("ThirdParty")).AddComponent<DiscordInst>();
			DiscordInst._isInstInit = false;
		}

		return DiscordInst._inst;
	}

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 是否已初始化 */
	public bool isInitialized {get; protected set;} = false;

	/** DiscordSDK主體 */
	protected DscrdInst _discord = null;

	/** 遊玩狀態資料 (struct) */
	protected Activity _activity;

	/** 預設 遊玩狀態資料 */
	public Activity defaultActivity;

	/** 是否 遊玩狀態資料改變 */
	protected bool _isActivityChanged = false;

	/** 遊玩狀態資料改變 回呼 */
	protected Queue<Action<bool>> _activityUpdateCBList = new Queue<Action<bool>>();

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Awake () {
		// 禁用 非 Inst() 建立
		if (DiscordInst._isInstInit == false) {
			UnityEngine.Object.Destroy(this);
			return;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (this.isInitialized == false) return;
		if (this._discord == null) return;
		
		this._discord.RunCallbacks();
	}

	void LateUpdate() {
		if (this.isInitialized == false) return;
		if (this._discord == null) return;

		this.checkActivityChanged();
	}

	void OnDestroy() {
		if (this.isInitialized == false) return;
		if (this._discord == null) return;

		

		this._discord.Dispose();
	}
	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 初始化 */
	public void Init (long appID) {
		if (this.isInitialized) return;
		if (appID == -1) return;

		// 建立 主體 
		this._discord = new DscrdInst(appID, (System.UInt64) Discd.CreateFlags.NoRequireDiscord);

		// 狀態管理
		ActivityManager activityMgr = this._discord.GetActivityManager();

		// 初始狀態
		this.defaultActivity = new Activity {
			Type = ActivityType.Playing
		};
		this._activity = this.defaultActivity;
		
		// 更新初始狀態
		activityMgr.UpdateActivity(this._activity, (res)=>{

			// 需要 清除才能去掉 詳細狀態 (以及 狀態文字右邊 的 詳細狀態icon)
			activityMgr.ClearActivity((res)=>{});
		});
		
		
		// 已經 完成初始化
		this.isInitialized = true;
		Debug.Log("[DiscordSDK] Initialized");
	}

	/**
	 * 請求變更 遊玩狀態
	 * @param {Func<Activity, Activity>} setActivity 對現有的狀態資料進行修改並回傳
	 * @param {Action<bool>} callback 回傳是否成功設置
	 */
	public void RequestActivity (Func<Activity, Activity> setActivity, Action<bool> callback = null) {
		if (this.isInitialized == false) return;
		
		// 修改 遊玩狀態
		this._activity = setActivity(this._activity);

		this._isActivityChanged = true;

		if (callback != null) this._activityUpdateCBList.Enqueue(callback);
	}

	public bool IsActivityEmpty (Activity activity) {
		if (this.isInitialized == false) return true;

		return activity.Equals(new Activity());
	}

	/*===================================Protected Function======================================*/

	protected void checkActivityChanged () {
		if (this.isInitialized == false) return;

		if (this._isActivityChanged == false) return;
		this._isActivityChanged = false;

		Action<Discd.Result> final = (res)=>{
			bool isSuccess = res == Discd.Result.Ok;
			while (this._activityUpdateCBList.Count > 0) {
				this._activityUpdateCBList.Dequeue()(isSuccess);
			}
		};
		
		// 更新狀態
		ActivityManager activityMgr = this._discord.GetActivityManager();
		activityMgr.UpdateActivity(this._activity, (res)=>{

			// 若 狀態 不為 空
			if (this.IsActivityEmpty(this._activity) == false) {
				final(res);
				return;
			}

			// 否則 清空狀態

			// 需要 清除才能去掉 詳細狀態 (以及 狀態文字右邊 的 詳細狀態icon)
			activityMgr.ClearActivity((res)=>{
				final(res);
			});

		});
	}

	/*====================================Private Function=======================================*/
}


}

#endif