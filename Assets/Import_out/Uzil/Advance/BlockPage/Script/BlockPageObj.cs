using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using UzEvent = Uzil.Event;

namespace Uzil.BlockPage {

public class BlockRegInfo {
	public BlockRowObj row;
	public BlockObj block;
}

[ExecuteInEditMode]
[RequireComponent(typeof(VerticalLayoutGroup))]
public class BlockPageObj : UIBehaviour {

	
	/*======================================Constructor==========================================*/

	public BlockPageObj (string json) {
		this.SetData(json);
	}
	public BlockPageObj () {

	}

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** ID */
	public string id = null;

	/** 面板內容資料 */
	public DictSO contentData = new DictSO();

	/** 面板區塊 */
	public Dictionary<string, BlockRegInfo> id2BlockInfos = new Dictionary<string, BlockRegInfo>();

	/** 行 */
	private List<BlockRowObj> _rows = new List<BlockRowObj>();

	/** 是否已經銷毀 */
	public bool isDestroyed { get { return this._isDestroyed; } }
	private bool _isDestroyed = false;

	/*========================================Components=========================================*/

	public Transform container {
		get {
			return this.transform;
		}
	}

	public CanvasGroup canvasGroup = null;

	/*==========================================Event============================================*/

	/** 當 銷毀 */
	public UzEvent onDestroy = new UzEvent();

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	protected override void Awake () {
		base.Awake();

		if (this.id != null && this.id != "") {
			BlockPageUtil.AddPage(this);
		}
	}
	
	// Update is called once per frame
	void Update () {
		// if (this.isCallUpdate) {
		// 	this.isCallUpdate = false;
		// } else {
		// 	this.canvasGroup.alpha = Mathf.MoveTowards(this.canvasGroup.alpha, 1f, 5f*Time.deltaTime);
		// }
	}

	protected override void OnDestroy() {
		base.OnDestroy();
		this._isDestroyed = true;
		this.onDestroy.Call();
	}

	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*== 基本 ===========*/

	/** 設置啟用 */
	public void SetActive (bool isActive) {
		this.gameObject.SetActive(isActive);
	}

	/** 初始化 */
	public void SetData (string data_json) {
		DictSO data = DictSO.Json(data_json);
		this.SetData(data);
	}
	public void SetData (DictSO data) {
		if (data == null) return;
		this.contentData = data;
	}

	/** 更新頁面資料 */
	public void UpdateData () {
		foreach (BlockRowObj row in this._rows) {
			row.UpdateData(this.contentData);
		}
	}

	/** 更新排版 */
	public void UpdateLayout () {
		// 重建 所有行 (區塊 容器)
		foreach (BlockRowObj row in this._rows) {
			
			Stack<BlockObj> reverseUpdate = new Stack<BlockObj>();

			List<BlockObj> blocks = row.GetBlocks();
			foreach (BlockObj block in blocks) {
				if (block.isReverseUpdateSort) {
					reverseUpdate.Push(block);
				} else {
					block.Rebuild();
				}
			}

			while (reverseUpdate.Count > 0) {
				BlockObj block = reverseUpdate.Pop();
				block.Rebuild();
			}

			row.Rebuild();
		}
		
		// 重建 自身 (行 容器)
		LayoutGroup layout = this.GetComponent<LayoutGroup>();
		layout.CalculateLayoutInputHorizontal();
		layout.CalculateLayoutInputVertical();
		layout.SetLayoutHorizontal();
		layout.SetLayoutVertical();

		// 標示 更新
		this.SetDirty();
	}

	/** 銷毀 */
	public void Destroy () {
		if (this.isDestroyed) return;
		GameObject.Destroy(this.gameObject);
	}


	/*== 行/區塊 ===========*/

	
	/** 取得所有 行 */
	public List<BlockRowObj> GetRows () {
		return this._rows;
	}

	/** 取得 行  */
	public BlockRowObj GetRow (int rowIdx) {
		while (this._rows.Count <= rowIdx) {
			this.AddRow();
		}
		return this._rows[rowIdx];
	}

	/** 新增 行 */
	public BlockRowObj AddRow () {
		BlockRowObj row = BlockPageUtil.CreateRow();
		row.transform.SetParent(this.container, false);
		this._rows.Add(row);
		return row;
	}

	/** 移除 行 */
	public void RemoveRow (int rowIdx) {
		if (rowIdx > this._rows.Count-1) return;
		BlockRowObj row = this._rows[rowIdx];

		List<BlockObj> blocks = row.GetBlocks();
		
		for (int idx = blocks.Count-1; idx >= 0; idx--) {
			BlockObj block = blocks[idx];
			row.RemoveBlock(block);
		}

		this._rows.RemoveAt(rowIdx);

		GameObject.Destroy(row.gameObject);
	}

	/** 取得 區塊 */
	public BlockObj GetBlock (string blockID) {
		if (this.id2BlockInfos.ContainsKey(blockID) == false) return null;
		return this.id2BlockInfos[blockID].block;
	}

	/** 取得 區塊資訊 */
	public BlockRegInfo GetBlockInfo (string blockID) {
		if (this.id2BlockInfos.ContainsKey(blockID) == false) return null;
		return this.id2BlockInfos[blockID];
	}

	/** 新增 區塊 */
	public void AddBlock (string blockID, BlockObj block) {
		this.InsertBlock(this._rows.Count, blockID, block);
	}
	public void InsertBlock (int rowIdx, string blockID, BlockObj block) {
		if (this.id2BlockInfos.ContainsKey(blockID)) return;

		block.page = this;

		BlockRowObj row = this.GetRow(rowIdx);

		row.AddBlock(block);

		BlockRegInfo regInfo = new BlockRegInfo(){
			row = row,
			block = block
		};

		block.id = blockID;

		this.id2BlockInfos.Add(blockID, regInfo);
	}

	/** 移除 區塊 */
	public void RemoveBlock (string blockID) {
		BlockRegInfo blockInfo = this.GetBlockInfo(blockID);
		if (blockInfo == null) return;
		
		this.id2BlockInfos.Remove(blockID);

		BlockRowObj row = blockInfo.row;
		BlockObj block = blockInfo.block;
		
		if (row != null) {
			row.RemoveBlock(block);
		}

		GameObject.Destroy(block.gameObject);
	}

	/** 清除所有 區塊 */
	public void ClearBlocks () {
		List<string> toRm = new List<string>();
		
		foreach (KeyValuePair<string, BlockRegInfo> pair in this.id2BlockInfos) {
			toRm.Add(pair.Key);
		}
		foreach (string each in toRm){
			this.RemoveBlock(each);
		}
	}

	/** 清除所有 */
	public void Clear () {
		this.ClearBlocks();
		for (int idx = this._rows.Count; idx > 0; idx--) {
			this.RemoveRow(idx-1);
		}
	}

	
	
	/*==事件=====================================*/

	/** 當被從面板中移除 */
	public void OnRemove () {
		
	}


	/*===================================Protected Function======================================*/
	
	/// <summary>
	/// Mark the LayoutGroup as dirty.
	/// </summary>
	protected void SetDirty()
	{
		if (!IsActive())
			return;
		if (!CanvasUpdateRegistry.IsRebuildingLayout()) {
			LayoutRebuilder.MarkLayoutForRebuild(this.transform as RectTransform);
		} else {
		}
			StartCoroutine(DelayedSetDirty(this.transform as RectTransform));
	}

	IEnumerator DelayedSetDirty(RectTransform rectTransform) {
		yield return null;
		LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
	}

#if UNITY_EDITOR
	protected override void OnValidate() {
		SetDirty();
	}

#endif

	/*====================================Private Function=======================================*/
}


}
