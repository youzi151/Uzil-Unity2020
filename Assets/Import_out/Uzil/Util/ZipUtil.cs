using System;
using System.IO;
using System.Collections.Generic;

using Ionic.Zip;


namespace Uzil.Util {

public class ZipUtil {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/* 解壓縮到記憶體 */
	public static Dictionary<string, byte[]> UnZipToMemory (string path, string password, Predicate<string> isExtract = null) {
		Dictionary<string, byte[]> result = new Dictionary<string, byte[]>();

		using (ZipFile zip = ZipFile.Read(path)) {
			zip.Password = password;
			foreach (ZipEntry e in zip) {
				if (isExtract != null) {
					if (!isExtract(e.FileName)) { continue; }
				}

				MemoryStream data = new MemoryStream();
				e.Extract(data);
				result.Add(e.FileName, data.ToArray());
				// Debug.Log(e.FileName);
			}
		}
		return result;
	}



	/* 取得所有檔名(含路徑) */
	public static List<string> GetFilesName (string path) {
		List<string> result = new List<string>();
		using (ZipFile zip = ZipFile.Read(path)) {
			foreach (ZipEntry e in zip) {
				result.Add(e.FileName);
			}
		}
		return result;
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