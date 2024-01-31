using System.Collections.Generic;

using UnityEngine;

using Uzil.Res;
using Uzil.Util;

namespace Uzil.BlockPage {

public class BlockPageUtil {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/**==定義========================*/

	public const string PREFAB_PAGE = "BlockPage/Prefab/BlockPage";
	public const string PREFAB_ROW = "BlockPage/Prefab/BlockRow";

	public const string PREFAB_BLOCKDIR = "BlockPage/Prefab/Blocks/";

	public const string PREFAB_BLOCKNAME_PREFIX = "Block_";

	/**==單例========================*/

	/** ID:實例 */
	public static Dictionary<string, BlockPageObj> id2instance = new Dictionary<string, BlockPageObj>();

	/*=====================================Static Funciton=======================================*/
	
	/** 取得 頁面 */
	public static BlockPageObj GetPage (string id) {
		if (BlockPageUtil.id2instance.ContainsKey(id)) {
			return BlockPageUtil.id2instance[id];
		} else {
			return null;
		}
	}
	
	/** 新增 頁面 */
	public static void AddPage (BlockPageObj page) {
		if (BlockPageUtil.id2instance.ContainsKey(page.id)) {

			if (BlockPageUtil.id2instance[page.id] != page) {
				Debug.Log("Page["+page.id+"] is exist.");
			}
			
			return;
		}

		BlockPageUtil.id2instance.Add(page.id, page);
		
		page.onDestroy.Add(()=>{
			BlockPageUtil.DestroyPage(page.id);
		});
	}

	/** 建立頁面 */
	public static BlockPageObj CreatePage (string id = null) {

		GameObject prefab = Resources.Load<GameObject>(BlockPageUtil.PREFAB_PAGE);
		if (prefab == null) return null;

		// 建立頁面
		GameObject pageGObj = GameObject.Instantiate(prefab);
		BlockPageObj page = pageGObj.GetComponent<BlockPageObj>();
		if (page == null) return null;

		page.id = id;

		if (id != null) {
			BlockPageUtil.AddPage(page);
		}

		return page;
	}

	/** 銷毀 頁面 */
	public static void DestroyPage (string id) {
		if (BlockPageUtil.id2instance.ContainsKey(id) == false) return;
		BlockPageUtil.id2instance[id].Destroy();
		BlockPageUtil.id2instance.Remove(id);
	}

	/** 建立行 */
	public static BlockRowObj CreateRow () {
		GameObject prefab = ResMgr.Get<GameObject>(new ResReq(BlockPageUtil.PREFAB_ROW));
		if (prefab == null) return null;

		// 建立
		GameObject gObj = GameObject.Instantiate(prefab);
		BlockRowObj row = gObj.GetComponent<BlockRowObj>();
		if (row == null) return null;

		return row;
	}

	/** 建立頁面 */
	public static BlockObj CreateBlock (string blockName) {
		return BlockPageUtil.CreateBlock("", blockName);
	}
	public static BlockObj CreateBlock (string blockDir, string blockName) {
		
		string path = PathUtil.Combine(BlockPageUtil.PREFAB_BLOCKDIR, blockDir, BlockPageUtil.PREFAB_BLOCKNAME_PREFIX+blockName);

		// 建立區塊
		GameObject prefab = Resources.Load<GameObject>(path);

		// 若 找不到 則
		if (prefab == null) {
			// 退回 內建目錄
			path = PathUtil.Combine(BlockPageUtil.PREFAB_BLOCKDIR, BlockPageUtil.PREFAB_BLOCKNAME_PREFIX+blockName);
			prefab = Resources.Load<GameObject>(path);
		}

		if (prefab == null) return null;

		GameObject blockGObj = GameObject.Instantiate(prefab);

		BlockObj block = blockGObj.GetComponent<BlockObj>(); 

		return block;
	}

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/
	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}
