using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

using Uzil;
using Uzil.Audio;
using Uzil.Options;

[RequireComponent(typeof(Slider))]
public class VolumeSlider : MonoBehaviour {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	public bool isInited = false;

	/* 混音器 */
	public AudioMixer mixer;

	/* 參數名稱 */
	public string paramName;
	private string _paramName = null;

	/* 註冊音量變化 */
	private EventListener _listener;

	/* Slide r*/
	private Slider slider;

	private bool isPreventEvent = false;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	void Awake () {
		if (this.mixer == null) return;
		if (this.getSlider() == null) return;
		this.getSlider().normalizedValue = AudioUtil.GetVolumeLinear(this.paramName);
		this.isInited = true;
	}

	void Update () {
		this.updateParam();
	}

	void OnDestroy() {
		if (this._listener != null) {
			AudioUtil.OffVolumeChange(this._paramName, this._listener);
		}
	}

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/* 設定音量(百分比) */
	public void SetVolume (float i) {
		if (this.mixer == null) return;

		//取得
		Slider slider = this.getSlider();
		if (slider != null) {
			i = slider.normalizedValue;
		}

		this.isPreventEvent = true;
		
		// 設置
		Option.SetVolumeLinear(this.paramName, i, this.isInited);

		this.isPreventEvent = false;
	}

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

	private Slider getSlider () {
		if (this.slider == null)
			this.slider = this.GetComponent<Slider>();
		return this.slider;
	}

	private void updateParam () {
		if (this.paramName == this._paramName) return;
		if (this._listener != null) {
			AudioUtil.OffVolumeChange(this._paramName, this._listener);
		} else {
			this._listener = new EventListener((data)=>{
				if (this.isPreventEvent) return;
				this.getSlider().normalizedValue = AudioUtil.DecibelToLinear(data.GetFloat("val"));
			});
		}

		this._paramName = this.paramName;

		AudioUtil.OnVolumeChange(this._paramName, this._listener);
	}

}