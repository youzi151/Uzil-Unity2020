using UnityEngine;

namespace Uzil.Macro {

public class LinkParent : MonoBehaviour {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/* 是否在Update同步 */
	public bool isLinkOnUpdate = true;
    
    /* 是否在LateUpdate同步 */
	public bool isLinkOnLateUpdate = true;

	/* 是否連結 */
	public bool isLinked = true;

	/* 子物件 */
	public Transform child;

	/* 親物件 */
	public Transform parent;

	/* 啟用狀態 */
	public bool isActiveLinked = true;

	/* 位置 */
	public bool isPositionLinked = true;

	/* 旋轉 */
	public bool isRotationLinked = true;
	
	/* 比例 */
	//運作起來不會和真實Parent一致
	// public bool isScaleLinked = false;

    /* 位置偏移 */
	public Vector3 positionOffset = Vector3.zero;

    /* 旋轉偏移 */
	public Vector3 rotationOffset = Vector3.zero;
	// public Vector3 scaleOffset = Vector3.zero;

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Awake () {
		this.Link(this.parent, this.child);
	}
	
	// Update is called once per frame
	void Update(){
		if (this.isLinkOnUpdate) this.UpdateLink();
	}
	void LateUpdate(){
		if (this.isLinkOnLateUpdate) this.UpdateLink();
	}

	public void UpdateLink () {
		if (!this.isLinked) return;

		if (this.child == null || this.parent == null) return;

		if (this.isActiveLinked){
			this.child.gameObject.SetActive(this.parent.gameObject.activeInHierarchy);
		}

		if (this.isPositionLinked){
			this.child.position = this.parent.position + this.positionOffset;
		}

		if (this.isRotationLinked){
			this.child.rotation = Quaternion.Euler(this.parent.rotation.eulerAngles + this.rotationOffset);
		}

		// if (this.isScaleLinked){
		// 	Vector3 newScale = new Vector3(
		// 		this.parent.localScale.x * this.scaleOffset.x,
		// 		this.parent.localScale.y * this.scaleOffset.y,
		// 		this.parent.localScale.z * this.scaleOffset.z
		// 	);
		// 	this.child.localScale = newScale;
		// }
	}
	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

    /* 連結 */
	public void Link (Transform parent, Transform child, bool keepInWolrd = true) {
		if (parent == null || child == null) return;

		this.parent = parent;
		this.child = child;

		if (keepInWolrd) {
			this.positionOffset = this.child.position - this.parent.position;
			this.rotationOffset = this.child.rotation.eulerAngles - this.parent.rotation.eulerAngles;
			// this.scaleOffset = this.child.localScale - this.parent.localScale;
		} else {
			this.positionOffset = this.child.localPosition;
			this.rotationOffset = this.child.localRotation.eulerAngles;
			// this.scaleOffset = this.child.localScale - this.parent.localScale;
		}

		this.Link();
	}

    /* 連結 */
	public void Link () {
		this.isLinked = true;
	}

    /* 解除連結 */
	public void Unlink () {
		this.isLinked = false;
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

}


}