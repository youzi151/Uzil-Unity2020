using UnityEngine;
using UnityEngine.UI;

using Uzil;
using Uzil.CompExt;


namespace Uzil.BlockPage {

[ExecuteInEditMode]
public class Block_Text : BlockObj {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	
	/** 文字 */
	public TextExt text;

	/** 尺寸適應 */
	public ContentSizeFitter textSizer;

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 設置資料 */
	public override void _SetData (DictSO data) {
		
		this.text.LoadMemo(data);

		base._SetData(data);
	}

	public override void Update () {
		base.Update();
	}

	/** 重建 */
	public override void Rebuild () {
		
		RectTransform rectTrans = this.contentRectTrans as RectTransform;
		Vector2 size = rectTrans.rect.size;

		base.Rebuild();

		// this.text.textUI.ForceMeshUpdate(true);
		Vector2 preferredSize = this.text.textUI.GetPreferredValues();
		// preferredSize = this.text.textUI.GetPreferredValues();
		// LayoutRebuilder.ForceRebuildLayoutImmediate(this.text.textUI.rectTransform);

		if (this.isFitWidth) {
			size.x = preferredSize.x;
			rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
		}
		if (this.isFitHeight) {
			size.y = preferredSize.y;
			rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
		}
	}

	/** 當刷新頁面 */
	public override void OnPageUpdate (DictSO pageData) {
		this.text.Rerender();
	}

	/*===================================Protected Function======================================*/


	/*====================================Private Function=======================================*/

	


}

}