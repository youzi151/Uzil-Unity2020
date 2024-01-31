using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Uzil;

namespace Uzil.BlockPage {

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class BlockRowObj : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 面板區塊 */
	private List<BlockObj> blocks = new List<BlockObj>();

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*== 基本 ===========*/

	/** 更新頁面資料 */
	public void UpdateData (DictSO pageData) {
		foreach (BlockObj each in this.blocks) {
			each.OnPageUpdate(pageData);
		}
	}

	public float GetHeight () {
		return (this.transform as RectTransform).rect.height;
	}

	public float GetWidth () {
		return (this.transform as RectTransform).rect.width;
	}


	/*== 區塊 ===========*/

	/** 取得所有區塊 */
	public List<BlockObj> GetBlocks () {
		return this.blocks;
	}

	/** 新增區塊 */
	public void AddBlock (BlockObj block) {
		if (this.blocks.Contains(block)) return;

		block.transform.SetParent(this.transform, false);
		block.row = this;

		this.blocks.Add(block);
	}

	/** 移除區塊 */
	public void RemoveBlock (BlockObj block) {
		if (this.blocks.Contains(block) == false) return;
		this.blocks.Remove(block);
	}

	/** 重建 */
	public void Rebuild () {
		LayoutGroup layout = this.GetComponent<LayoutGroup>();
		layout.CalculateLayoutInputHorizontal();
		layout.CalculateLayoutInputVertical();
		layout.SetLayoutHorizontal();
		layout.SetLayoutVertical();
	}


	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}
