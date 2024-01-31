using UnityEngine;

using Uzil;
using Uzil.BlockPage;

namespace UZAPI {

public class BlockPage {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** 更新排版 */
	public static void UpdateLayout (string pageID) {
		BlockPageObj page = BlockPageUtil.GetPage(pageID);
		// if (page == null) return;
		page.UpdateLayout();
	}

	/** 新增 區塊 */
	public static void AddBlock (string pageID, string blockID, string blockType, string blockJson) {
		BlockPageObj page = BlockPageUtil.GetPage(pageID);
		if (page == null) return;

		DictSO data = DictSO.Json(blockJson);

		BlockObj block = BlockPageUtil.CreateBlock(blockType);
		page.AddBlock(blockID, block);

		block.SetData(data);
	}

	/** 新增 區塊 */
	public static void InsertBlock (string pageID, int rowIdx, string blockID, string blockType, string blockJson) {
		BlockPageObj page = BlockPageUtil.GetPage(pageID);
		if (page == null) return;

		DictSO data = DictSO.Json(blockJson);

		BlockObj block = BlockPageUtil.CreateBlock(blockType);
		page.InsertBlock(rowIdx, blockID, block);

		block.SetData(data);
	}

	/** 設置 區塊 */
	
	public static void SetBlock (string pageID, string blockID, string blockJson) {
		BlockPageObj page = BlockPageUtil.GetPage(pageID);
		if (page == null) return;

		BlockObj block = page.GetBlock(blockID);
		if (block == null) return;

		DictSO data = DictSO.Json(blockJson);
		block.SetData(data);
	}

	/** 移除 區塊 */
	public static void RemoveBlock (string pageID, string blockID) {
		BlockPageObj page = BlockPageUtil.GetPage(pageID);
		if (page == null) return;

		page.RemoveBlock(blockID);
	}

	/** 清空 區塊 */
	public static void ClearBlocks (string pageID) {
		BlockPageObj page = BlockPageUtil.GetPage(pageID);
		if (page == null) return;

		page.ClearBlocks();	
	}

	/** 移除 行 */
	public static void RemoveRow (string pageID, int rowIdx) {
		BlockPageObj page = BlockPageUtil.GetPage(pageID);
		if (page == null) return;

		page.RemoveRow(rowIdx);
	}

	/** 移除 所有 */
	public static void Clear (string pageID) {
		BlockPageObj page = BlockPageUtil.GetPage(pageID);
		if (page == null) return;

		page.Clear();
	}



	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}

}