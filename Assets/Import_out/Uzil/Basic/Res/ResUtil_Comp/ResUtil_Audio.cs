using System;
using System.IO;

using UnityEngine;

namespace Uzil.Res {

public class ResUtil_Audio {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/
	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/* 以byte[]建立Audio */
	public AudioClip Create (byte[] byteData, string format, DictSO args = null) {
		if (byteData == null) return null;

		string clipName = "_anonymous";

		if (args != null) {

			args.TryGetString("name", (res)=>{
				clipName = res;
			});

		}

		AudioClip audioClip = null;
		switch (format){
			case "wav":
			case ".wav":
				audioClip = AudioConvert.FromData(byteData, AudioFormat.WAV, args);
				break;
			case "mp3":
			case ".mp3":
				audioClip = AudioConvert.FromData(byteData, AudioFormat.MP3, args);
				break;
			default: 
				return null;
		}

		audioClip.name = clipName;
		
    	return audioClip;
	}

	/* 以byte[]建立Audio */
	public void CreateAsync (byte[] byteData, string format, DictSO args, Action<AudioClip> cb) {
		if (byteData == null) {
			cb(null);
			return;
		}

		string clipName = "_anonymous";

		if (args != null) {
			args.TryGetString("name", (res)=>{
				clipName = res;
			});
		}

		Action<AudioClip> _cb = (audioClip)=>{
			audioClip.name = clipName;
			cb(audioClip);
		};

		switch (format){
			case "wav":
			case ".wav":
				AudioConvert.FromDataAsync(byteData, AudioFormat.WAV, _cb, args);
				break;
			case "mp3":
			case ".mp3":
				AudioConvert.FromDataAsync(byteData, AudioFormat.MP3, _cb, args);
				break;
			default: 
				cb(null);
				return;
		}
	}

	/* 讀取 */
	public AudioClip Read (string filePath) {
		
		string targetPath = null;
		byte[] fileData = null;
		
		for (int i = 0;; i++){
			if (i >= ResUtil.audioFormat.Length) return null;

			string withExt = filePath + ResUtil.audioFormat[i];
			if (File.Exists(withExt)){
				targetPath = withExt;
				break;
			}
		}

		if (targetPath != null) {
			fileData = File.ReadAllBytes(targetPath);
		}

		string ext = Path.GetExtension(targetPath);

		if (fileData == null) return null;
		return ResUtil.audio.Create(fileData, ext);
	}

	public void ReadAsync (string filePath, Action<AudioClip> cb) {
		
		string targetPath = null;
		byte[] fileData = null;
		
		for (int i = 0;; i++){
			if (i >= ResUtil.audioFormat.Length) {
				cb(null);
				return;
			}

			string withExt = filePath + ResUtil.audioFormat[i];
			if (File.Exists(withExt)){
				targetPath = withExt;
				break;
			}
		}

		if (targetPath != null) {
			fileData = File.ReadAllBytes(targetPath);
		}

		string ext = Path.GetExtension(targetPath);

		if (fileData == null) {
			cb(null);
			return;
		}
		ResUtil.audio.CreateAsync(fileData, ext, null, cb);
	}


	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/


}



}