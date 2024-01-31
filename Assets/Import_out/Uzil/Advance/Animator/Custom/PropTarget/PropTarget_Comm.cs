using System.Collections.Generic;

using UnityEngine;

using Uzil.ObjInfo;
using Uzil.Audio;

namespace Uzil.Anim {

public class PropTarget_Comm : PropTarget {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 變形 */
	public Transform transform;

	/** 模型網格 */
	public MeshFilter meshFilter;
	private Mesh _mesh = null;

	/** 渲染與材質 */
	public Dictionary<MeshRenderer, Material> renderer2Mat = new Dictionary<MeshRenderer, Material>();
	public List<MeshRenderer> renderers = new List<MeshRenderer>();
	
	
	/** 最後執行的腳本 */
	protected Prop_ScriptCMD lastScript = null;

	/** 最後的音效 */
	protected AudioInfo lastAudio = null;

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
			case "position.z":
				this.setLocalPosition(2, (float)value, isAddtive);
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
			
			case "color.r":
				this.setFloat4("color", 0, (float) value, isAddtive);
				return;
			case "color.g":
				this.setFloat4("color", 1, (float) value, isAddtive);
				return;
			case "color.b":
				this.setFloat4("color", 2, (float) value, isAddtive);
				return;
			case "color.a":
				this.setFloat4("color", 3, (float) value, isAddtive);
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

			case "localScale":
			case "scale":
				this.applyLocalScale((float?[]) value);
				return;

			case "localRotation":
			case "rotation":
				this.applyLocalRotation((float?[]) value);
				return;

			case "mesh":
				this.applyMesh((Vector3[]) value);
				return;

			case "uv":
				this.applyUV((Vector2[]) value);
				return;

			case "texture":
				TextureInfo texInfo = (TextureInfo) value;
				this.applyTexture(texInfo.GetTexture());
				return;

			case "script":
				Prop_ScriptCMD scriptCMD = (Prop_ScriptCMD) value;
				// 若 與 上一次的時間幀 一樣 則 忽略
				if (this.lastScript != null) {
					if (scriptCMD.frame == this.lastScript.frame) return;
				}

				this.lastScript = scriptCMD;
				this.doScript(scriptCMD.script);
				return;

			case "audio":
				AudioInfo audioInfo = (AudioInfo) value;
				
				if (this.lastAudio != null) {
					if (this.lastAudio.IsEqual(audioInfo)) return;
				}

				this.applyAudio(audioInfo, this.lastAudio);
				this.lastAudio = audioInfo;
				return;

		}

		base.applyTo(animator, propName, value);
	}

	/** 當關鍵幀改變 */
	public override void OnKeyframeChanged () {
		this.lastScript = null;
	}

	/*===================================Protected Function======================================*/

	/**== 設置到 ==================*/

	/** 設置 本地位置 */
	protected void setLocalPosition (int dimensionIdx, float value,  bool isAddtive) {
		string propName = "localPosition";
		this.setFloat3(propName, dimensionIdx, value, isAddtive);
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
		Vector3 localPos = this.transform.localPosition;
		
		if (value[0] != null) localPos.x = (float) value[0];
		if (value[1] != null) localPos.y = (float) value[1];
		if (value[2] != null) localPos.z = (float) value[2];
		
		this.transform.localPosition = localPos;
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

	/** 應用網格 */
	protected void applyMesh (Vector3[] points) {
		if (this.meshFilter == null) return;
		if (this._mesh == null) {
			this._mesh = this.meshFilter.mesh; // 取出
		}
		
		this._mesh.vertices = points;

		this.meshFilter.sharedMesh = this._mesh;
	}

	/** 應用UV */
	protected void applyUV (Vector2[] uv) {
		if (this.meshFilter == null) return;
		if (this._mesh == null) {
			this._mesh = this.meshFilter.mesh; // 取出
		}
		
		this._mesh.uv = uv;

		this.meshFilter.sharedMesh = this._mesh;
	}

	/** 應用貼圖 */
	protected void applyTexture (Texture tex) {

		foreach (MeshRenderer each in this.renderers) {
			Material mat = null;
			if (this.renderer2Mat.ContainsKey(each) == false) {
				mat = each.material;
				this.renderer2Mat.Add(each, mat);
			} else {
				mat = this.renderer2Mat[each];
			}

			mat.SetTexture("_MainTex", tex);

			each.sharedMaterial = mat;
		}
	}

	/** 應用網格 */
	protected void applyAudio (AudioInfo info, AudioInfo lastInfo) {
		info.Apply(lastAudio);
	}
	
	/*====================================Private Function=======================================*/


}

}