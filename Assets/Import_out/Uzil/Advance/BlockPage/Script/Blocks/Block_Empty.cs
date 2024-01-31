using System.Collections.Generic;

using UnityEngine;

using Uzil;


namespace Uzil.BlockPage {

[ExecuteInEditMode]
public class Block_Empty : BlockObj {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 填充 高度 */
	public float fillHeight = -1f;
	/** 填充 寬度 */
	public float fillWidth = -1f;
	public bool isFillWidthAuto = false;

	/** 是否反序更新 */
	public override bool isReverseUpdateSort {
		get {
			return this.isFillWidthAuto;
		}
	}

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 設置資料 */
	public override void _SetData (DictSO data) {
		base._SetData(data);

		// 寬度
		if (data.ContainsKey("fillWidth")) {
			object _width = data.Get("fillWidth");

			this.isFillWidthAuto = (_width.ToString() == "fill");

			if (!this.isFillWidthAuto) {
				this.fillWidth = data.GetFloat("fillWidth");
			}
		}

		// 高度
		if (data.ContainsKey("fillHeight")) {
			this.fillHeight = data.GetFloat("fillHeight");
		}
	}

	/** 重建 */
	public override void Rebuild () {
		
		base.Rebuild();

		if (this.fillWidth != -1f || this.isFillWidthAuto) {
			this.updateFillWidth();
		}

		if (this.fillHeight != -1f) {
			this.updateFillHeight();
		}

	}

	/** 當刷新頁面 */
	public override void OnPageUpdate (DictSO data) {

	}

	/*===================================Protected Function======================================*/

	protected void updateFillWidth () {

		// 蒐集之前的block
		List<BlockObj> list;
		if (this.row != null) {
			list = this.row.GetBlocks();
		} else if (this.transform.parent != null) {
			list = new List<BlockObj>();
			for (int idx = 0; idx < this.transform.parent.childCount; idx++) {
				BlockObj each = this.transform.parent.GetChild(idx).GetComponent<BlockObj>();
				if (each == null) continue;
				list.Add(each);
			}
		} else {
			return;
		}

		float selfWidth;

		if (isFillWidthAuto) {

			if (this.row != null) {
				selfWidth = this.row.GetWidth();
			} else {
				selfWidth = this.page.container.GetComponent<RectTransform>().sizeDelta.x;
			}

			for (int idx = 0; idx < list.Count; idx++) {
				BlockObj each = list[idx];
				if (each.isReverseUpdateSort) continue;
				
				selfWidth -= each.GetWidth();
				
				// 若已無剩下 則 跳出
				if (selfWidth < 0) {
					break;
				}
			}

			for (int idx = list.Count-1; idx >= 0; idx--) {
				BlockObj each = list[idx];
				if (!each.isReverseUpdateSort) continue;
				if (each == this) break;
				
				selfWidth -= each.GetWidth();
				
				// 若已無剩下 則 跳出
				if (selfWidth < 0) {
					break;
				}
			}

		} else {

			selfWidth = this.fillWidth;

			for (int idx = 0; idx < list.Count; idx++) {
				BlockObj each = list[idx];

				// 若 已達自身 則 跳出
				if (each == this) break;

				// 剩餘寬度 減去每個Block寬度
				selfWidth -= each.GetWidth();
				
				// 若已無剩下 則 跳出
				if (selfWidth < 0) {
					break;
				}
			}

		}

		// 若 有剩於寬度 則 設為 此Block寬度
		if (selfWidth >= 0) {
			this.layout.minWidth = selfWidth;
		} else {
			this.layout.minWidth = 0;
		}
	}	

	protected void updateFillHeight () {

		float until = this.fillHeight;

		BlockRowObj selfRow;
		if (this.row != null) {
			selfRow = this.row;
		} else {
			selfRow = this.transform.parent.GetComponent<BlockRowObj>();
		}

		// 蒐集之前的row
		List<BlockRowObj> list;
		if (this.page != null) {
			list = this.page.GetRows();
		} else {
			list = new List<BlockRowObj>();
			
			Transform pageTrans = selfRow.transform.parent;
			
			for (int idx = 0; idx < pageTrans.childCount; idx++) {
				BlockRowObj each = pageTrans.GetChild(idx).GetComponent<BlockRowObj>();
				if (each == null) continue;
				list.Add(each);
			}
		}
		
		for (int idx = 0; idx < list.Count; idx++) {
			BlockRowObj each = list[idx];

			// 若 已達自身 則 跳出
			if (each == selfRow) break;

			// 剩餘高度 減去每個Block高度
			until -= each.GetHeight();
			
			// 若已無剩下 則 跳出
			if (until < 0) {
				break;
			}
		}

		

		// 若 有剩於高度 則 設為 此Block高度
		if (until >= 0) {
			this.layout.minHeight = until;
		} else {
			this.layout.minHeight = 0;
		}
	}

	/*====================================Private Function=======================================*/


}

}