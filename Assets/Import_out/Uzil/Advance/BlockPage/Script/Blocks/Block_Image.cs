using UnityEngine;
using UnityEngine.UI;

using Uzil;
using Uzil.ObjInfo;

namespace Uzil.BlockPage {

[ExecuteInEditMode]
public class Block_Image : BlockObj {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/** 圖片 */
	public Image contentImg;

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 設置資料 */
	public override void _SetData (DictSO data) {
		base._SetData(data);

		// 內文
		if (data.ContainsKey("image")) {
			ImageInfo imgInfo = new ImageInfo(data.Get("image"));
			
			imgInfo.ApplyOn(this.contentImg);

			if (imgInfo.size != null) {
				RectTransform rect = (this.contentImg.transform as RectTransform);
				rect.sizeDelta = (Vector2) ((Vector3) imgInfo.size);
			}
		}
	}


	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/



}

}