using System.IO;

using UnityEngine;

using Uzil.Util;

namespace Uzil.Res {

public enum LoadType {
	ASSETBUNDLE, FILE, RESOURCES, ALL
}

public class ResUtil {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====Config=====*/
	public static bool isDebug = false;
	public static string[] textFormat = {"", ".sav", ".json", ".cfg", ".lv", ".txt"};
	public static string[] textureFormat = {"", ".png", ".jpg"};
	public static string[] luaFormat = {"", ".lua"};
	public static string[] audioFormat = {"", ".wav", ".mp3"};

	public static string ASSETBUNDLE_PREFIX = "ab:";

	/*====Instance====*/
	private static ResUtil instance;

	/*===Component====*/
	public static ResUtil_GameObject gameObject = new ResUtil_GameObject();
	public static ResUtil_Text text = new ResUtil_Text();
	public static ResUtil_Sprite sprite = new ResUtil_Sprite();
	public static ResUtil_PhysicMaterial physicMaterial = new ResUtil_PhysicMaterial();
	public static ResUtil_Texture texture = new ResUtil_Texture();
	public static ResUtil_Audio audio = new ResUtil_Audio();


	/*=====================================Static Funciton=======================================*/

	/* 是否為支援的類型 */
	public static bool IsSupportType (System.Type type) {
		if (
			type == typeof(string) ||
		    type == typeof(Texture) ||
			type == typeof(GameObject) ||
			type == typeof(AudioClip) ||
			type == typeof(PhysicsMaterial2D)
		) return true;
		
		Debug.Log("[ResMgr]: not support type["+type.ToString()+"]");
		return false;
	}

	/* 刪除 */
	public static bool Delete (string _path) {
		DirectoryInfo info = new DirectoryInfo(_path);
		
		string path = info.FullName;
		path = path.Replace("\\", "/");

		Debug.Log("[ResUtil] : delete["+path+"]");

		// 防止 惡意刪除
		if (path.Contains("..")) return false;

		// 防呆
		if (path.StartsWith(PathUtil.GetRootPath()) == false) return false;

		File.Delete(path);

		return true;
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