using UnityEngine;

namespace Uzil.FX {

public class FXObj_Anim : FXObj {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/* 是否已經銷毀 */
	private bool isDestroyed = false;

	/* 動畫系統 */
	public Animator anim;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	public void Awake () {
		if (this.anim == null) this.anim = this.gameObject.GetComponent<Animator>();
		if (this.anim == null) return;
	}

	public override void Update () {

		//取得animator


		//若還是沒有則返回
		if (this.anim == null) return;

		if (this.isPlaying == false) return;

		//取得當前動畫時間
		float time = 1f;
		time = this.anim.GetCurrentAnimatorStateInfo(0).normalizedTime;


		//若不使用
		if (this.lifeTime == -1) return;

		//播放完畢則銷毀
		else if (this.lifeTime == 0) {
			if (time >= 1f) {
				//停止
				this.Stop();
			}
			else { }
		}

		//若指定倒數
		else if (this.lifeTime > 0) {
			this.leftTime -= Time.deltaTime;
			if (this.leftTime > 0) return;
			this.Stop();
		}
		else { }

	}

	public void OnDestroy () {
		this.isDestroyed = true;
	}

	/*========================================Interface==========================================*/

	/** 播放 */
	public override void Play (DictSO playData) {
		this.leftTime = this.lifeTime;

		if (this.anim == null) return;

		this.gameObject.SetActive(true);

		string animName = null;
		if (playData != null) {
			if (playData.ContainsKey("anim")) {
				animName = playData.GetString("anim");
			}
		}
		if (animName == null) animName = "DEFAULT";

		this.anim.Play(animName);

		base.Play();
	}

	/** 停止 */
	public override void Stop () {

		if (this.anim == null) return;

		this.anim.Play("NONE");
		this.anim.Update(Time.deltaTime);

		if (!this.isDestroyed) {
			this.gameObject.SetActive(false);
		}

		base.Stop();
	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}

}