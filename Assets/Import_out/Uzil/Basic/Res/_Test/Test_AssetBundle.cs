using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Uzil.Util;

using Uzil.Res;

namespace Uzil.Test {

public class Test_AssetBundle : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	public Image img_resource;
	public Image img_assetBundle;

	public AudioSource audioSource_assetBundle;
	public AudioSource audioSource_resource;

	private AssetBundle bundle;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Start () {

		Debug.Log("Start"); 

		this.TextTest();

		this.TextureTest();

		this.AudioTest();

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	public void TextTest () {

		string path = "Folder1/Test_ResText";
		string asbPath = System.IO.Path.GetFileName(path);

		AssetBundle asb = this.getBundle();
		
		Debug.Log(asb.LoadAsset<TextAsset>(asbPath).text);
		Debug.Log(Resources.Load<TextAsset>(path).text);
	}

	public void TextureTest () {

		string path = "Folder1/Test_ResTexture";
		string asbPath = System.IO.Path.GetFileName(path);

		AssetBundle asb = this.getBundle();
		
		Sprite sp_assetBundle = ResUtil.sprite.Create(asb.LoadAsset<Texture2D>(asbPath));
		this.img_assetBundle.sprite = sp_assetBundle;

		Sprite sp_resource =  ResUtil.sprite.Create(Resources.Load<Texture2D>(path));
		this.img_resource.sprite = sp_resource;
	}

	public void AudioTest () {

		string path = "Folder1/Test_ResAudio";
		string asbPath = System.IO.Path.GetFileName(path);

		AssetBundle asb = this.getBundle();
		
		AudioClip audioClip_assetBundle = asb.LoadAsset<AudioClip>(asbPath);
		this.audioSource_assetBundle.clip = audioClip_assetBundle;
		this.audioSource_assetBundle.Play();

		AudioClip audioClip_resource =  Resources.Load<AudioClip>(path);
		this.audioSource_resource.clip = audioClip_resource;
		this.audioSource_resource.Play();
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

	private AssetBundle getBundle () {
		if (this.bundle == null) {

#if	UNITY_EDITOR
			this.bundle = AssetBundle.LoadFromFile(PathUtil.GetDataPath()+"Assets/AssetBundles/TestBundle");
#else
			this.bundle = AssetBundle.LoadFromFile(PathUtil.GetDataPath()+"TestBundle");
#endif
		}
		return this.bundle;
	}
}


}
