using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Uzil.Mod {

public class ModLoader {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/* 優先度 (越小越先) */
	public int priority = 50; // 建議 0~100

	/* 功能標籤 */
	protected List<string> tags = new List<string>();

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** 取得標籤 */
	public virtual List<string> GetTags () {
		return this.tags;
	}

	/** 加入標籤 */
	public virtual void AddTag (string tag) {
		if (this.tags.Contains(tag)) return;
		this.tags.Add(tag);
	}

	/** 讀取 */
	public virtual void Load (ModInfo modInfo) {

	}

	/** 卸載 */
	public virtual void Unload (ModInfo modInfo) {

	}

	/** 優先度 (越小越先) */
	public ModLoader Sort (int priority) {
		this.priority = priority;
		return this;
	}

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}



}