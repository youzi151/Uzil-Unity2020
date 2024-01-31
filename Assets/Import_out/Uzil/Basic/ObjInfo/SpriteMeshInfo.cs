using System;
using System.Collections.Generic;

using UnityEngine;

using Uzil.Res;
using Uzil.Util;

namespace Uzil.ObjInfo {

/** 
 * 尚無使用處
 */

public class SpriteMeshInfo : TransformInfo {

	/*======================================Constructor==========================================*/

	public SpriteMeshInfo () {

	}

	public SpriteMeshInfo (object jsonOrPath) {
		this.raw = jsonOrPath;

		DictSO data = DictSO.Json(jsonOrPath);
		
		if (data == null) {
			this.texturePath = jsonOrPath.ToString();
			return;
		} else {
			this.LoadMemo(data);
		}

	}

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 貼圖是否存在 */
	public bool isTextureExist = false;
    /** 貼圖路徑 */
	public string texturePath {
		get {
			return this._texturePath;
		}
		set {
			this.isTextureExist = true;
			this._texturePath = value;
		}
	}
	protected string _texturePath;

	protected Material _material;

	/** Mesh是否存在 */
	public bool isMeshExist = false;
	public Vector2[] meshPoints {
		get {
			return this._meshPoints;
		}
		set {
			this.isMeshExist = true;
			this._meshPoints = value;
		}
	}
	protected Vector2[] _meshPoints;
	protected Mesh _mesh;


	/** UV是否存在 */
	public bool isUVExist = false;
	public Vector2[] uv {
		get {
			return this._uv;
		}
		set {
			this.isUVExist = true;
			this._uv = value;
		}
	}
	protected Vector2[] _uv;

	/** PixelUV是否存在 */
	public bool isPixelUVExist = false;
	public Vector2[] pixelUV {
		get {
			return this._pixelUV;
		}
		set {
			this.isPixelUVExist = true;
			this._pixelUV = value;
		}
	}
	protected Vector2[] _pixelUV;


	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** [IMemoable] 紀錄為Json格式 */
	public override object ToMemo () {
		DictSO data = (DictSO) base.ToMemo();

		if (this.isTextureExist) {
			data.Set("texture", this.texturePath);
		}

		if (this.isMeshExist) {
			List<List<object>> mesh = new List<List<object>>();
			for (int idx = 0; idx < this.meshPoints.Length; idx++) {
				Vector2 each = this.meshPoints[idx];
				mesh.Add(DictSO.Vector2To(each));
			}
			data.Set("mesh", mesh);
		}

		if (this.isUVExist) {
			List<List<object>> uv = new List<List<object>>();
			for (int idx = 0; idx < this.uv.Length; idx++) {
				Vector2 each = this.uv[idx];
				uv.Add(DictSO.Vector2To(each));
			}
			data.Set("uv", uv);
		}

		if (this.isPixelUVExist) {
			List<List<object>> pixelUV = new List<List<object>>();
			for (int idx = 0; idx < this.pixelUV.Length; idx++) {
				Vector2 each = this.pixelUV[idx];
				pixelUV.Add(DictSO.Vector2To(each));
			}
			data.Set("pixelUV", pixelUV);
		}

		return data;
	}

	/** [IMemoable] 讀取Json格式 */
	public override void LoadMemo (object memoJson) {
		
		DictSO data = DictSO.Json(memoJson);
		if (data == null) return;

		base.LoadMemo(data);

		if (data.ContainsKey("texture")) {
			this.isTextureExist = true;
			this.texturePath = data.GetString("texture");
		}

		if (data.ContainsKey("mesh")) {
			this.isMeshExist = true;
			this.meshPoints = data.GetList<Vector2>("mesh").ToArray();
		}

		if (data.ContainsKey("uv")) {
			this.isUVExist = true;
			this.uv = data.GetList<Vector2>("uv").ToArray();
		}

		if (data.ContainsKey("pixelUV")) {
			this.isPixelUVExist = true;
			this.pixelUV = data.GetList<Vector2>("pixelUV").ToArray();
		}


	}

	/*=====================================Public Function=======================================*/

    /** 取得貼圖 */
	public Texture2D GetTexture () {
		if (this.texturePath == "" || this.texturePath == null) return null;
		Texture2D tex = ResMgr.Get<Texture2D>(new ResReq(this.texturePath));
		return tex;
	}

	public virtual void ApplyOn (MeshFilter meshFilter, MeshRenderer meshRenderer) {
		
		Texture tex = this.GetTexture();
		Material mat = this._material;
		

		if (meshRenderer != null) {

			if (this.isTextureExist) {
				
				if (mat == null) {
					mat = meshRenderer.material; // 取得副本
					this._material = mat;
				}

				mat.SetTexture("_MainTex", tex);
				mat.SetTexture("_EmissionMap", tex);
			}

		}
		
		if (meshFilter != null) {

			Vector2[] uv = null;
			if (this.isUVExist) {
				uv = this._uv;
			}

			Vector2[] meshPoints = null;
			if (this.isMeshExist) {
				meshPoints = this._meshPoints;
			}

			// 若 像素UV存在
			if (this.isPixelUVExist) {

				Vector2[] pixelUV = this.pixelUV;
				Vector2[] cauculateUV = new Vector2[pixelUV.Length];

				// 若 貼圖 不存在 則 從 目標物件上 取得
				if (tex == null) {
					tex = mat.GetTexture("_MainTex");
				}

				// 若 貼圖 存在
				if (tex != null) {

					// 取得尺寸
					float width = tex.width;
					float height = tex.height;
					
					// 依序計算每一個像素UV 設為 百分比UV
					for (int idx = 0; idx < pixelUV.Length; idx++) {

						Vector2 point_pixel = pixelUV[idx];
						Vector2 point_percent = new Vector2(
							point_pixel.x / width,
							point_pixel.y / height
						);

						cauculateUV[idx] = point_percent;
					}

					uv = cauculateUV;
				}


			}


			if (uv == null && meshFilter.sharedMesh != null) {
				uv = new Vector2[meshFilter.sharedMesh.uv.Length];
				Array.Copy(meshFilter.sharedMesh.uv, uv, meshFilter.sharedMesh.uv.Length);
			}
			 
			if (this.isMeshExist || this.isUVExist) {

				if (this._mesh == null) {
					this._mesh = meshFilter.mesh;
				}

				// 只有改UV
				if (this.isUVExist && !this.isMeshExist) {
					this._mesh.uv = uv;	
				} else {
					this._mesh = MeshUtil.SpriteMesh(meshPoints, uv, this._mesh);
				}
				// meshFilter.sharedMesh = this._mesh;

			}
		}

	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

}

}