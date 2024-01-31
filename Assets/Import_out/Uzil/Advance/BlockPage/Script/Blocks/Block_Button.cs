using UnityEngine;
using UnityEngine.UI;

using Uzil.ObjInfo;
using Uzil.CompExt;

namespace Uzil.BlockPage {

[ExecuteInEditMode]
public class Block_Button : BlockObj {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/** 文字 */
	public TextExt contentText;

	/** 圖片 */
	public Image image;

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** 設置資料 */
	public override void _SetData (DictSO data) {
		base._SetData(data);
		
		// 文字
		data.TryGetString("text", (res)=>{
			this.contentText.SetText(res);
		});

		// 文字偏移
		data.TryGetVector2("textOffset", (res)=>{
			RectTransform textRectTrans = this.contentText.transform as RectTransform;
			textRectTrans.anchoredPosition = res;
		});
		
		// 圖片
		data.TryGetDictSO("image", (res)=>{
			ImageInfo imgInfo = new ImageInfo(data.Get("image"));
			imgInfo.ApplyOn(this.image);
		});

	}

	/** 當 刷新頁面*/
	public override void OnPageUpdate (DictSO pageData) {
		this.contentText.Rerender();
	}

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/



}

}