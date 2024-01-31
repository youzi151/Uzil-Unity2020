using System;
using System.Collections.Generic;

using UnityEngine;

namespace Uzil.FX {

public class FXObj_Particle : FXObj {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/* 粒子系統 */
	public ParticleSystem mainPS;
	public List<ParticleSystem> otherPS = new List<ParticleSystem>();

	/* 是否強制在前景 */
	public bool isForeground = false;

	/* 是否已經呼叫停止 */
	public bool isCallStop = false;

	protected Color originalColor;

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	public virtual void Awake () {
		if (this.mainPS == null) this.mainPS = this.gameObject.GetComponent<ParticleSystem>();
		if (this.mainPS == null) return;

		if (this.isForeground){
			this.doToAllPS((eachPS)=>{
				Renderer renderer = eachPS.GetComponent<Renderer>();
				if (renderer == null) return;
				renderer.sortingLayerName = "Foreground";
			});
		}

		this.originalColor = this.mainPS.main.startColor.color;
	
	}

	public override void Update () {

		if (this.isPlaying == false) return;

		bool isMainPSAlive = false;
		if (this.mainPS != null) {
			isMainPSAlive = this.mainPS.IsAlive(/*withChildren*/true);
		}


		// 若 已經呼叫停止
		if (this.isCallStop){
			if (isMainPSAlive == false) {
				this.realStop();
				return;
			}
		}


		// 以下為Alive狀態
		if (this.lifeTime == -1) {
			return;
		}
		else if (this.lifeTime == 0) {
			if (this.mainPS != null) {
				if (isMainPSAlive == false) {
					this.Stop();
				}
			}
		}
		else if (this.lifeTime > 0) {
			base.Update();
		}

	}
	
	/*========================================Interface==========================================*/

	/** 初始化 */
	public override void Init (DictSO initData) {
		
		base.Init(initData);

		if (initData != null) {
			if (initData.ContainsKey("color")) {
				Color toColor = initData.GetColor("color");
				this.setAllPSColor(toColor);
			}
		}
	}

	/** 播放 */
	public override void Play (DictSO playData) {
		if (this.mainPS == null) return;

		this.gameObject.SetActive(true);

		this.Init(playData);

		this.mainPS.Play();

		this.isCallStop = false;

		base.Play(); // 已經Init過了 不需要傳參數data
	}

	/** 停止 */
	public override void Stop () {
		if (this.isCallStop) return;
		if (!this.isPlaying) return;

		ParticleSystem.MainModule main = this.mainPS.main;

		this.mainPS.Stop();

		this.isCallStop = true;

	}

	public override void Terminate () {
		this.realStop();
	}

	public override void LoadMemo (object memoJson) {
		
	}


	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	protected virtual void setAllPSColor (Color color) {
		ParticleSystem.MinMaxGradient psColor = new ParticleSystem.MinMaxGradient(color);
		this.doToAllPS((eachPS)=>{
			ParticleSystem.MainModule main = eachPS.main;
			main.startColor = psColor;
		});
	}

	protected virtual void doToAllPS (Action<ParticleSystem> toDo) {
		toDo(this.mainPS);
		foreach (ParticleSystem eachPS in this.otherPS){
			if (eachPS == null) continue;
			toDo(eachPS);
		}		
	}

	protected virtual void realStop () {
		this.mainPS.Stop();
		this.mainPS.Clear();
		
		this.setAllPSColor(this.originalColor);
		
		this.fxObj_Stop();
	}

	protected void fxObj_Stop () {
		base.Stop();
	}


	/*====================================Private Function=======================================*/


}

}