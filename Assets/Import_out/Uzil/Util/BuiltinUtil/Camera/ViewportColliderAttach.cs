using System.Collections.Generic;

using UnityEngine;

using Uzil.Util;

namespace Uzil.BuiltinUtil {

[RequireComponent(typeof(Camera))]
public class ViewportColliderAttach : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	public bool isStatic = false;

	public bool is2D = false;

	public bool isTrigger = false;

	protected Camera cam {
		get {
			if (this._cam == null) this._cam = this.GetComponent<Camera>();
			return this._cam;
		}
	}
	protected Camera _cam;

	protected MeshCollider meshCollider {
		get {
			if (this._meshCollider == null) {
				this._meshCollider = this.GetComponent<MeshCollider>();
				if (this._meshCollider == null) {
					this._meshCollider = this.gameObject.AddComponent<MeshCollider>();
					this._meshCollider.isTrigger = this.isTrigger;
					this._meshCollider.convex = true;
				}
			}
			return this._meshCollider;
		}
	}
	protected MeshCollider _meshCollider;
	protected Mesh mesh;

	protected PolygonCollider2D polygonCollider {
		get {
			if (this._polygonCollider == null) {
				this._polygonCollider = this.GetComponent<PolygonCollider2D>();
				if (this._polygonCollider == null) {
					this._polygonCollider = this.gameObject.AddComponent<PolygonCollider2D>();
					this._polygonCollider.isTrigger = this.isTrigger;
				}
			}
			return this._polygonCollider;
		}
	}
	protected PolygonCollider2D _polygonCollider;


	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Awake () {
		if (this.is2D) {
			this.updatePolygon2D();
		} else {
			this.updateMesh3D();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (this.is2D) {
			this.polygonCollider.isTrigger = this.isTrigger;
			this.cam.orthographic = true;
		} else {
			this.meshCollider.isTrigger = this.isTrigger;
		}
	}

	void FixedUpdate() {
		if (this.isStatic) return;

		if (this.is2D) {
			this.updatePolygon2D();
		} else {
			this.updateMesh3D();
		}
	}

	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	protected void updateMesh3D () {

		Vector3 orinPos = this.transform.localPosition;
		Quaternion orinRot =this.transform.localRotation;
		Transform orinParent = this.transform.parent;

		this.transform.rotation = Quaternion.identity;
		this.transform.position = Vector3.zero;
		this.transform.SetParent(null, false);
		
		float near = this.cam.nearClipPlane;
		float far = this.cam.farClipPlane;

		Vector3 fdl = cam.ViewportToWorldPoint(new Vector3(0f, 0f, near));
		fdl -= this.transform.localPosition;
		Vector3 fur = cam.ViewportToWorldPoint(new Vector3(1f, 1f, near));
		fur -= this.transform.localPosition;

		float fu = fur.y;
		float fd = fdl.y;
		float fl = fdl.x;
		float fr = fur.x;

		Vector3 bdl = cam.ViewportToWorldPoint(new Vector3(0f, 0f, far));
		bdl -= this.transform.localPosition;
		Vector3 bur = cam.ViewportToWorldPoint(new Vector3(1f, 1f, far));
		bur -= this.transform.localPosition;

		float bu = bur.y;
		float bd = bdl.y;
		float bl = bdl.x;
		float br = bur.x;

		Vector3[] vertices = new Vector3[]{
			new Vector3(fl, fd, near),
			new Vector3(fr, fd, near),
			new Vector3(fr, fu, near),
			new Vector3(fl, fu, near),

			new Vector3(bl, bu, far),
			new Vector3(br, bu, far),
			new Vector3(br, bd, far),
			new Vector3(bl, bd, far),
		};

		this.mesh = MeshUtil.CubeMesh(vertices, null, this.mesh);
		this.meshCollider.sharedMesh = this.mesh;

		this.transform.localRotation = orinRot;
		this.transform.localPosition = orinPos;
		this.transform.SetParent(orinParent, false);
	}

	protected void updatePolygon2D () {

		Vector3 orinPos = this.transform.localPosition;
		Quaternion orinRot = this.transform.localRotation;
		Transform orinParent = this.transform.parent;

		this.transform.rotation = Quaternion.identity;
		this.transform.position = Vector3.zero;
		this.transform.SetParent(null, false);
		
		float far = this.cam.farClipPlane;

		Vector3 bdl = cam.ViewportToWorldPoint(new Vector3(0f, 0f, far));
		bdl -= this.transform.localPosition;
		Vector3 bur = cam.ViewportToWorldPoint(new Vector3(1f, 1f, far));
		bur -= this.transform.localPosition;

		float bu = bur.y;
		float bd = bdl.y;
		float bl = bdl.x;
		float br = bur.x;

		List<Vector2> path = new List<Vector2>(){
			new Vector2(bl, bd),
			new Vector2(br, bd),
			new Vector2(br, bu),
			new Vector2(bl, bu),
		};
		this.polygonCollider.SetPath(0, path);

		this.transform.localRotation = orinRot;
		this.transform.localPosition = orinPos;
		this.transform.SetParent(orinParent, false);
	}
	
	/*====================================Private Function=======================================*/
}


}
