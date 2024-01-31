using UnityEngine;

namespace Uzil.Util {

public class ColorUtil {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/* 以名稱或hex取得顏色 */
	public static Color Get (string hexOrName) {

		switch (hexOrName) {
			case "white":
				return Color.white;
			case "black":
				return Color.black;
			case "blue":
				return Color.blue;
			case "red":
				return Color.red;
			case "green":
				return Color.green;
			case "gray":
				return Color.gray;
			case "yellow":
				return Color.yellow;
		}

		Color color = new Color();

		bool isSuccess = ColorUtility.TryParseHtmlString(hexOrName, out color);

		if (!isSuccess) return new Color(-1, -1, -1);

		return color;
	}

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/
	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

}



}