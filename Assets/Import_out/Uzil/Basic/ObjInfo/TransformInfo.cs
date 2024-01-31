using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Uzil;
using Uzil.Res;

namespace Uzil.ObjInfo {


public class TransformInfo : ObjInfo {

	/*======================================Constructor==========================================*/

	public TransformInfo () {

	}

	public TransformInfo (object jsonOrPath) {
		this.raw = jsonOrPath;

		DictSO data = DictSO.Json(jsonOrPath);
		
		if (data == null) return;

		this.LoadMemo(data);

	}

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/
	
	/** 開啟/關閉  */
	public int active = -1;// -1:忽略/ 0:關閉 / 1:開啟

	/** 位移是否存在 */
	public bool isPositionExist = false;
    /** 位移 */
	public Vector3 position {
		get {
			return this._position;
		}
		set {
			this._position = value;
			this.isPositionExist = true;
		}
	}
	protected Vector3 _position;

	/** 旋轉是否存在 */
	public bool isRotationExist = false;
    /** 旋轉 */
	public Vector3 rotation {
		get {
			return this._rotation;
		}
		set {
			this._rotation = value;
			this.isRotationExist = true;
		}
	}
	protected Vector3 _rotation;

	/** 比例是否存在 */
	public bool isScaleExist = false;
    /** 比例 */
	public Vector3 scale {
		get {
			return this._scale;
		}
		set {
			this._scale = value;
			this.isScaleExist = true;
		}
	}
	protected Vector3 _scale;

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** [IMemoable] 紀錄為Json格式 */
	public override object ToMemo () {
		DictSO data = DictSO.New();

		if (this.active == 1) {
			data.Set("active", true);
		} else if (this.active == 0) {
			data.Set("active", false);
		}

		if (this.isPositionExist) {
			data.Set("position", DictSO.Vector3To(this.position));
		}

		if (this.isRotationExist) {
			data.Set("rotation", DictSO.Vector3To(this.rotation));
		}

		if (this.isScaleExist) {
			data.Set("scale", DictSO.Vector3To(this.scale));	
		}

		return data;
	}

	/** [IMemoable] 讀取Json格式 */
	public override void LoadMemo (object memoJson) {
		
		DictSO data = DictSO.Json(memoJson);
		if (data == null) return;

		if (data.ContainsKey("active")) {
			bool isActive = data.GetBool("active");
			if (isActive) {
				this.active = 1;
			} else {
				this.active = 0;
			}
		} else {
			this.active = -1;
		}

		data.TryGetVector3("position", (res)=>{
			this.isPositionExist = true;
			this.position = res;
		});
		data.TryGetVector3("rotation", (res)=>{
			this.isRotationExist = true;
			this.rotation = res;
		});
		data.TryGetVector3("scale", (res)=>{
			this.isScaleExist = true;
			this.scale = res;
		});
	}

	/*=====================================Public Function=======================================*/

    /** 應用在 */
	public virtual void ApplyOn (Transform trans) {

		if (this.active != -1) {
			trans.gameObject.SetActive(this.active > 0);
		}

		if (this.isPositionExist) {
			trans.localPosition = this.position;
		}

		if (this.isRotationExist) {
			trans.rotation = Quaternion.Euler(this.rotation);
		}

		if (this.isScaleExist) {
			trans.localScale = this.scale;
		}
	}

    /** 從實體複製 */
	public void CopyFrom (Transform trans) {
		this.position = trans.localPosition;
		this.rotation = trans.rotation.eulerAngles;
		this.scale = trans.localScale;
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

}

}