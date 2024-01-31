using UnityEngine;

using System;
using System.IO;

namespace Uzil.Res {

public class ResUtil_Text {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/
	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	public string FindExist (string fullPath) {
		for (int i = 0;; i++) {

			if (i >= ResUtil.textFormat.Length) return null;

			string withExt = fullPath + ResUtil.textFormat[i];
			if (File.Exists(withExt)) {
				return withExt;
			}

		}
	}

	/* 以byte[]建立字串 */
	public string Create (byte[] byteData) {
		if (byteData == null) return null;
		StreamReader stream = new StreamReader(new MemoryStream(byteData));
		return stream.ReadToEnd();
	}

	/* 底層讀取(從完整路徑) */
	public string Read (string fullPath) {
		string result = null;
		try {
			fullPath = this.FindExist(fullPath);

			if (fullPath == null) return null;

			result = File.ReadAllText(fullPath);

		} catch (Exception e) {
			Debug.Log(e);
		}
		return result;
	}

	/* 底層寫入 */
	public void Write (string fullPath, string content) {
		// print(ResUtil.GetDataPath() + "/" + fullPath);
		// print(content);
		FileInfo file = new FileInfo(fullPath);
		file.Directory.Create();
		File.WriteAllText(file.FullName, content,  System.Text.Encoding.UTF8);
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/


	/* 是否為空值 */
	private bool isNull (string str) {
		return str == null || str == "";
	}

}



}