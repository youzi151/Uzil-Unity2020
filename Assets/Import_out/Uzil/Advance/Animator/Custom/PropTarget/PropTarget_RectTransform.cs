using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Uzil.Res;
using Uzil.Lua;

namespace Uzil.Anim {

public class PropTarget_RectTransform : PropTarget {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 變形 */
	public RectTransform transform;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 設置 */
	public override void SetTo (string propName, object value, bool isAddtive) {
		
		switch (propName) {

			case "position.x":
				this.setLocalPosition(0, (float)value, isAddtive);
				return;
			case "position.y":
				this.setLocalPosition(1, (float)value, isAddtive);
				return;

			case "size.x":
				this.setSizeDelta(0, (float)value, isAddtive);
				return;
			case "size.y":
				this.setSizeDelta(1, (float)value, isAddtive);
				return;

			case "scale.x":
				this.setLocalScale(0, (float)value, isAddtive);
				return;
			case "scale.y":
				this.setLocalScale(1, (float)value, isAddtive);
				return;
			case "scale.z":
				this.setLocalScale(2, (float)value, isAddtive);
				return;

			case "rotation.x":
				this.setLocalRotation(0, (float)value, isAddtive);
				return;
			case "rotation.y":
				this.setLocalRotation(1, (float)value, isAddtive);
				return;
			case "rotation.z":
				this.setLocalRotation(2, (float)value, isAddtive);
				return;
		}

		base.SetTo(propName, value, isAddtive);
	}

	/** 應用 */
	protected override void applyTo (Animator animator, string propName, object value) {
		
		switch (propName) {
			case "localPosition":
			case "position":
				this.applyLocalPosition((float?[]) value);
				return;

			case "size":
			case "sizeDelta":
				this.applySizeDelta((float?[]) value);
				return;

			case "localScale":
			case "scale":
				this.applyLocalScale((float?[]) value);
				return;

			case "localRotation":
			case "rotation":
				this.applyLocalRotation((float?[]) value);
				return;

			case "script":
				this.doScript(value.ToString());
				return;

		}

		base.applyTo(animator, propName, value);
	}

	/*===================================Protected Function======================================*/

	/**== 設置到 ==================*/

	
	/** 設置 本地位置 */
	protected void setLocalPosition (int dimensionIdx, float value,  bool isAddtive) {
		string propName = "localPosition";
		this.setFloat2(propName, dimensionIdx, value, isAddtive);
	}

	/** 設置 本地位置 */
	protected void setSizeDelta (int dimensionIdx, float value,  bool isAddtive) {
		string propName = "sizeDelta";
		this.setFloat2(propName, dimensionIdx, value, isAddtive);
	}

	/** 設置 本地縮放 */
	protected void setLocalScale (int dimensionIdx, float value,  bool isAddtive) {
		string propName = "localScale";
		this.setFloat3(propName, dimensionIdx, value, isAddtive);
	}

	/** 設置 本地旋轉 */
	protected void setLocalRotation (int dimensionIdx, float value,  bool isAddtive) {
		string propName = "localRotation";
		this.setFloat3(propName, dimensionIdx, value, isAddtive);
	}


	/**== 應用到 ==================*/

	/** 應用 本地位置 */
	protected void applyLocalPosition (float?[] value) {
		if (this.transform == null) return;
		Vector2 localPos = this.transform.anchoredPosition;
		
		if (value[0] != null) localPos.x = (float) value[0];
		if (value[1] != null) localPos.y = (float) value[1];
		
		this.transform.localPosition = localPos;
	}

	/** 應用 尺寸 */
	protected void applySizeDelta (float?[] value) {
		if (this.transform == null) return;
		Vector2 sizeDelta = this.transform.sizeDelta;
		
		if (value[0] != null) sizeDelta.x = (float) value[0];
		if (value[1] != null) sizeDelta.y = (float) value[1];
		
		this.transform.sizeDelta = sizeDelta;
	}

	/** 應用 本地縮放 */
	protected void applyLocalScale (float?[] value) {
		if (this.transform == null) return;
		
		Vector3 localScale = this.transform.localScale;

		if (value[0] != null) localScale.x = (float) value[0];
		if (value[1] != null) localScale.y = (float) value[1];
		if (value[2] != null) localScale.z = (float) value[2];

		this.transform.localScale = localScale;
	}

	/** 應用 本地旋轉 */
	protected void applyLocalRotation (float?[] value) {
		if (this.transform == null) return;
		
		Vector3 localRotation = this.transform.localRotation.eulerAngles;

		if (value[0] != null) localRotation.x = (float) value[0];
		if (value[1] != null) localRotation.y = (float) value[1];
		if (value[2] != null) localRotation.z = (float) value[2];

		this.transform.localRotation = Quaternion.Euler(localRotation);
	}

	
	/*====================================Private Function=======================================*/


}

}