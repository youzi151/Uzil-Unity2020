using UnityEngine;


namespace Uzil.Res {

public class ResUtil_Sprite {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/
	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/* 以byte[]建立Sprite */
	public Sprite Create (Texture2D tex, DictSO args = null) {

		float pixelPerUnit;
		if (args != null && args.ContainsKey("pixelPerUnit")) {
			pixelPerUnit = args.GetFloat("pixelPerUnit");
		} else {
			pixelPerUnit = 100f;
		}

		Vector4 border;
		if (args != null && args.ContainsKey("border")) {
			border = (Vector4) args.GetVector4("border");
		} else {
			border = new Vector4();
		}

		uint extrude = 0;
		SpriteMeshType meshType = SpriteMeshType.FullRect;

		return Sprite.Create(
			tex, 
			new Rect(0, 0, tex.width, tex.height),
			new Vector2(0.5f, 0.5f),
			pixelPerUnit,
			extrude,
			meshType,
			border
		);
	
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/


}



}