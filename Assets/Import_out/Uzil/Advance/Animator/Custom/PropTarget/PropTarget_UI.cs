using UnityEngine;
using UnityEngine.UI;

using Uzil.ObjInfo;

namespace Uzil.Anim {

public class PropTarget_UI : PropTarget_RectTransform {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 圖像 */
	public Image image;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 設置 */
	public override void SetTo (string propName, object value, bool isAddtive) {
		
		// switch (propName) {

		// }

		base.SetTo(propName, value, isAddtive);
	}

	/** 應用 */
	protected override void applyTo (Animator animator, string propName, object value) {
		
		switch (propName) {

			case "sprite":
			case "texture":
				TextureInfo texInfo = (TextureInfo) value;
				ImageInfo imgInfo = new ImageInfo(texInfo.GetRaw());
				this.applySprite(imgInfo.HoldSprite(this.image.gameObject));
				return;

			case "fillAmount":
				this.applyFillAmount((float) value);
				return;
			
			case "color":
				this.applyColor((float?[]) value);
				return;

		}

		base.applyTo(animator, propName, value);
	}

	/*===================================Protected Function======================================*/

	/**== 設置到 ==================*/

	

	/**== 應用到 ==================*/

	/** 應用貼圖 */
	protected void applySprite (Sprite sprite) {
		if (this.image == null) return;
		this.image.sprite = sprite;
	}

	
	/** 應用填滿 */
	protected void applyFillAmount (float fillAmount) {
		if (this.image == null) return;
		this.image.fillAmount = fillAmount;
	}
	
	/** 應用顏色 */
	protected void applyColor (float?[] rgba) {
		if (this.image == null) return;
		Color color = this.image.color;
		 
		if (rgba[0] != null) color.r = (float) rgba[0];
		if (rgba[1] != null) color.g = (float) rgba[1];
		if (rgba[2] != null) color.b = (float) rgba[2];
		if (rgba[3] != null) color.a = (float) rgba[3];

		this.image.color = color;
	}

	/*====================================Private Function=======================================*/


}

}