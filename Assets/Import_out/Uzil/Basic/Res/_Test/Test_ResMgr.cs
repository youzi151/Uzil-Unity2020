using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Uzil.Res;

namespace Uzil.Test {

public class Test_ResMgr : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	public Image img;

	public AudioSource audioSource;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Start () {

		Debug.Log("Start"); 

		// Uzil.Invoker.main.Once(()=>{

			this.TextTest();

			this.TextureTest();

			this.AudioTest();

		// },1);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	public void TextTest () {
		Debug.Log(ResMgr.Get<string>(new ResReq("Res/Test_ResText", "ab:testbundle")));
	}

	public void TextureTest () {
		Sprite sprite = ResUtil.sprite.Create(ResMgr.Get<Texture2D>(new ResReq("Res/Test_ResTexture", "ab:testbundle")));
		this.img.sprite = sprite;
	}

	public void AudioTest () {
		AudioClip audioClip = ResMgr.Get<AudioClip>(new ResReq("Res/Test_ResAudio", "ab:testbundle"));
		Debug.Log(audioClip);
		this.audioSource.clip = audioClip;
		this.audioSource.Play();
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

}


}
