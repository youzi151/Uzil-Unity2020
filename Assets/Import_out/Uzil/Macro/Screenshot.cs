using UnityEngine;
using System.Collections;
using System;
using System.IO;

namespace Uzil.Macro {


/** 擷圖存檔類型 */
public enum ScreenshotType {
	PNG, JPG
}


/** 截圖存檔工具 */

public class Screenshot : MonoBehaviour {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	private static Screenshot instance; 
	public static Screenshot Inst () {
		if (Screenshot.instance == null){
			Screenshot.instance = new GameObject("Screenshot").AddComponent<Screenshot>();
		}
		return Screenshot.instance;
	}

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	public void OnDestroy () {
		if (Screenshot.instance == this) {
			Screenshot.instance = null;
		}
	}

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 匯出 攝影機畫面 */
	public void ExportScreen (Camera camera, string savePath, ScreenshotType screenshotType, Action onDone = null) {
		this.StartCoroutine(this.screenshot(camera, savePath, screenshotType, onDone));
	}
	IEnumerator screenshot (Camera camera, string savePath, ScreenshotType screenshotType, Action onDone) {
		//在擷取畫面之前請等到所有的Camera都Render完
		yield return new WaitForEndOfFrame();

		if (camera.targetTexture != null)
			RenderTexture.active = camera.targetTexture;

		Texture2D texture = new Texture2D((int)camera.pixelWidth, (int)camera.pixelHeight);
		//擷取全部畫面的資訊
		texture.ReadPixels(new Rect(0, 0, (int)camera.pixelWidth, (int)camera.pixelHeight),0,0, false);
		texture.Apply();

		//自己處理畫面資料的方法
		this.SaveTextureToFile(texture, savePath, screenshotType);

		if (onDone != null) onDone();
	}

	/** 匯出畫面中的某個RectTransform區塊 */
	public void ExportInScreen (Camera camera, RectTransform rectTrans, string savePath, ScreenshotType screenshotType, Action onDone = null) {
		Rect source = this.rectTransformToScreenSpace(rectTrans);
		source = new Rect(
			Mathf.RoundToInt(source.x),
			Mathf.RoundToInt(source.y),
			Mathf.RoundToInt(source.width),
			Mathf.RoundToInt(source.height)
		);
		this.ExportInScreen(camera, source, savePath, screenshotType, onDone);
	}
	public void ExportInScreen(Camera camera, Rect rect, string savePath, ScreenshotType screenshotType, Action onDone){
		this.StartCoroutine(this.rectshot(camera, rect, savePath, screenshotType, onDone));
	}
	IEnumerator rectshot(Camera camera, Rect rect, string savePath, ScreenshotType screenshotType, Action onDone) {

		Texture2D texture = new Texture2D((int)rect.width, (int)rect.height);

		print(rect.x + ", " + rect.y + ", " + rect.width + ", " + rect.height);

		// 等待1秒
		yield return new WaitForSeconds(10.0f);

		camera.Render();
		print(rect.x + ", " + rect.y + ", " + rect.width + ", " + rect.height);

		// 等待1秒
		yield return new WaitForSeconds(2.0f);

		// 擷取全部畫面的資訊
		texture.ReadPixels(this.getFixedRect(rect), 0, 0, false);
		texture.Apply();

		// 自己處理畫面資料的方法
		this.SaveTextureToFile(texture, savePath, screenshotType);

		if (onDone != null) onDone();
	}

	/** 儲存至檔案 */
	public void SaveTextureToFile (Texture2D texture, string savePath, ScreenshotType screenshotType) {
		byte[] bytes;
		
		if (screenshotType == ScreenshotType.PNG) {
			bytes = texture.EncodeToPNG();
		} else if (screenshotType == ScreenshotType.JPG) {
			bytes = texture.EncodeToJPG();
		} else {
			return;
		}
		string filePath = savePath;
		using (FileStream fs = File.Open(filePath, FileMode.Create)) {
			BinaryWriter binary = new BinaryWriter(fs);
			binary.Write(bytes);
		}
		// Debug.Log("Write To :"+filePath);
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

	/** 尺寸防呆 */
	private Rect getFixedRect(Rect rect){
		Rect fixedRect = new Rect(
			Mathf.RoundToInt(rect.x),
			Mathf.RoundToInt(rect.y),
			Mathf.RoundToInt(rect.width),
			Mathf.RoundToInt(rect.height)
		);
		return fixedRect;
	}

	/** 取得 RectTransform 在螢幕上的Rect */
	private Rect rectTransformToScreenSpace (RectTransform transform) {
		Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
		Rect rect = new Rect(transform.position.x, Screen.height - transform.position.y, size.x, size.y);
		rect.x -= (transform.pivot.x * size.x);
		rect.y -= ((1.0f - transform.pivot.y) * size.y);
		return rect;
	}

}



}