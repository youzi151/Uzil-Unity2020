using System;
using System.IO;
using System.Net;
using System.Collections.Generic;

namespace Uzil.Macro {


/** FTP上傳器 */

public class FTPUploader {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 帳號 */
	public string account = "";

	/** 密碼 */
	public string password = "";


	/** 上傳客戶端 與 完成上傳 */
	private Dictionary<WebClient, Action> webClient2OnUploadComplete = new Dictionary<WebClient, Action>();

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	public void UploadFile (string filePath, string uploadPath, Action onUploadComplete = null)	{
		
		byte[] bytes = File.ReadAllBytes(filePath);
		
        this.Upload(bytes, uploadPath, onUploadComplete);
	}

	public void Upload (byte[] bytes, string uploadPath, Action onUploadComplete = null) {

		// DateTime startTime = DateTime.Now;
		WebClient client = new WebClient();

		//client設置
		client.Credentials = new NetworkCredential(this.account, this.password);

		//事件 當上傳完畢
		client.UploadDataCompleted += (sender, args) => {

			// TimeSpan timeDelta = DateTime.Now - startTime;
			// Debug.Log("upload done, uploadTime: " + timeDelta.TotalSeconds);

			//執行 當上傳完畢	
			if (this.webClient2OnUploadComplete.ContainsKey(client)) {

				Action clientsOnUploadComplete = this.webClient2OnUploadComplete[client];
				if (clientsOnUploadComplete != null) {
					Invoker.Inst().Once(()=>{
						clientsOnUploadComplete();
					});
				}
				
			}
		
		};

		client.UploadDataAsync(new Uri("ftp://"+uploadPath), bytes);

		
		this.webClient2OnUploadComplete.Add(client, onUploadComplete);
	}

	public void Stop () {
		foreach (KeyValuePair<WebClient, Action> pair in this.webClient2OnUploadComplete) {
			pair.Key.Dispose();
		}
		this.webClient2OnUploadComplete.Clear();
	}


	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}
