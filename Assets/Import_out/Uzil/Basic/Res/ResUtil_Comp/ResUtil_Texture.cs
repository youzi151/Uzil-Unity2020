using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;


namespace Uzil.Res {

public class ResUtil_Texture {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/
	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/* 以byte[]建立貼圖 */
	public Texture2D Create2D (byte[] byteData) {
		if (byteData == null) return null;
		Texture2D tex = null;
		tex = new Texture2D(2, 2);
		tex.LoadImage(byteData); //..this will auto-resize the texture dimensions.
		return tex;
	}

	/* 讀取檔案 */
	public Texture2D Read2D (string filePath) {
		
		string targetPath = null;
		byte[] fileData = null;
		
		for (int i = 0;; i++){
			if (i >= ResUtil.textureFormat.Length) return null;

			string withExt = filePath + ResUtil.textureFormat[i];

			if (File.Exists(withExt)){
				targetPath = withExt;
				break;
			}
		}

		if (targetPath != null) {
			fileData = File.ReadAllBytes(targetPath);
		}

		if (fileData == null) return null;
		return ResUtil.texture.Create2D(fileData);
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/


}



}