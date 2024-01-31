using System;
using System.Collections.Generic;

using UnityEngine;


namespace Uzil.Misc {

public class DragItemSort_Vertical {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/
	
	/**== 要註冊的行為 =========*/

	/** 取得 所有項目 */
	public Func<List<Transform>> getTransformList = ()=> {return null;};

	/** 設置 拖曳中 */
	public Action<Transform, bool> setDragging = (trans, isDragging)=>{};

	/** 設置 排序 */
	public Action<List<Transform>> setSort = (list)=>{};

	/** 當 完成排序 */
	public Action onSorted = ()=>{};

	/** 當 項目點擊 */
	public Action<Transform> onClick = (item)=>{};

	/**== 屬性 ================*/

	/** 是否啟用拖曳 */
	public bool isDragEnable = true;

	/** 是否按壓 */
	private bool isPressed = false;

	/** 是否拖曳項目 */
	private bool isDragItem = false;
	
	/** 已按壓時間 */
	private float pressedTime_sec = 0f;
	
	/** 按壓中的項目 */
	private Transform pressedItem = null;
	
	/** 按壓時的操作位置 */
	private float pressedInputPosY_percent = 0f;
	
	/** 按壓時項目的 */
	private float pressedItemPosY = 0f;
	
	/** 排序是否變化 */
	private bool isSortChanged = false;
	

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 當 項目 被 按下 (受呼叫) */
	public void OnItemDown (Transform item) {
		// 設為 按壓中
		this.isPressed = true;
		// 所選項目
		this.pressedItem = item;

		// 紀錄 按下時的資訊

		// 輸入位置
		float inputPosY_percent = Input.mousePosition.y / Screen.height;
		this.pressedInputPosY_percent = inputPosY_percent;
		// 所選項目位置
		this.pressedItemPosY = this.pressedItem.transform.localPosition.y;
	}

	/** 當 項目 被 彈起 (受呼叫) */
	public void OnItemUp (Transform item) {

		// 若 彈起時的項目 為 按下的項目 且 不在拖曳中
		if (item == this.pressedItem && !this.isDragItem) {
			// 視為 點擊
			this.onClick(item);
		}

		// 若 有變更排序 呼叫事件
		if (this.isSortChanged) {
			this.onSorted();
		}

		// 歸零 按壓時間
		this.pressedTime_sec = 0;
		// 設為 非按壓中
		this.isPressed = false;
		// 設為 非拖曳中
		this.isDragItem = false;
		// 設為 尚未變更排序
		this.isSortChanged = false;

		// 若 所選項目存在
		if (this.pressedItem != null) {
			// 關閉 拖曳狀態
			this.setDragging(this.pressedItem, false);
			// 清空
			this.pressedItem = null;
		}
	}

	/** 每幀更新 (受呼叫) */
	public void Update () {

		// 若 按下中
		if (this.isPressed) {

			// 若 項目尚未拖曳 則
			if (!this.isDragItem) {
				
				// 推進 按壓時間
				this.pressedTime_sec += Time.deltaTime;

				// 若 按壓時間 超過 指定時間
				if (this.pressedTime_sec > 0.5f) {
					// 進入拖曳
					this.setDragging(this.pressedItem, true);
					this.isDragItem = true;
				}

			} 

			if (this.isDragEnable == false) return;

			// 輸入位置
			float inputPosY_percent = Input.mousePosition.y / Screen.height;
			float canvasHeight = 1080f;

			// 輸入位置 與 按下時輸入位置 差距
			float delta = (inputPosY_percent - this.pressedInputPosY_percent) * canvasHeight;
			// 若 差距過小 則 歸零
			if (Mathf.Abs(delta) < 0.1f) delta = 0f;

			// 設 預期項目位置 為 按下時項目位置 加上 輸入位置與按下時輸入位置的差距
			float exceptedItemPosY = this.pressedItemPosY + delta;

			// 所有 項目UI
			List<Transform> itemList = this.getTransformList();
			// 要求排序 的 晶片資料
			List<Transform> requestSort = new List<Transform>();

			// 上個位置
			int lastIdx = itemList.IndexOf(this.pressedItem);
			// 預計插入位置
			int insertIdx = 0;

			// 每個項目
			for (int idx = 0; idx < itemList.Count; idx++) {
				Transform each = itemList[idx];

				// 要拿來判斷的位置
				float eachPos = each.transform.localPosition.y;
				eachPos += (each as RectTransform).sizeDelta.y / 2f;
				
				// 若 該項目位置 高於 預期項目位置 則 
				if (eachPos > exceptedItemPosY) {
					// 設置 預計插入位置
					insertIdx = idx;
				}
				// 加入 要求排序的晶片資料
				requestSort.Add(each);
			}

			// 若 位置有改變
			if (lastIdx != insertIdx) {

				// 變更 項目 在 要求排序的晶片資料中 的 位置
				requestSort.Remove(this.pressedItem);
				requestSort.Insert(insertIdx, this.pressedItem);

				// 重新排序 列表UI 以 要求排序的晶片資料 為 準
				this.setSort(requestSort);

				// 設 排序已發生變更
				this.isSortChanged = true;

				// 若 項目尚未拖曳 則 進入拖曳
				if (!this.isDragItem) {
					this.setDragging(this.pressedItem, true);	
					this.isDragItem = true;
				}
			}
		}
	}
	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}

}