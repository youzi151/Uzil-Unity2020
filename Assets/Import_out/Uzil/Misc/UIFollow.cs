using UnityEngine;

using Uzil.BuiltinUtil;

namespace Uzil.Misc {

public class UIFollow : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 跟隨者 */
	public RectTransform follower {
		get;
		protected set;
	}

	/** 跟隨者的Canvas */
	protected Canvas canvas;

	/** 跟隨對象 */
	public Transform target {
		get;
		protected set;
	}

	/** 攝影機 (若目標在世界座標時使用) */
	public Camera worldMode_camera = null;
	
	/** 是否 在 世界空間 */
	protected bool isInWorld = false;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		// 若 目標存在
		if (this.target != null) {

			// 轉換 到 螢幕上的位置
			Vector2 screenPos;

			
			if (this.isInWorld) {
				Camera camera = this.worldMode_camera != null? this.worldMode_camera : CameraUtil.GetMain();
				// 將 目標 世界座標 轉換到 跟隨者Canvas的座標內
				screenPos = RectTransformUtil.WorldToScreenPoint(this.canvas, camera, this.target.position);
			} else {
				RectTransform targetTrans = (this.target as RectTransform);
				// 將 目標的矩形 轉換到 跟隨者容器的座標內
				Rect rect = RectTransformUtil.GetRectIn(targetTrans, (this.follower.parent as RectTransform));
				// 以矩形轉換為要設置的位置
				screenPos = (rect.min + Vector2.Scale(rect.size, targetTrans.pivot));
			}

			// 設置位置
			this.SetPosition(screenPos);
		}
	}

	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/** 設置跟隨者 */
	public void SetFollower (RectTransform follower) {
		this.canvas = RectTransformUtil.FindRootCanvas(follower);
		if (this.canvas == null) {
			this.follower = null;
			return;
		}
		this.follower = follower;
	}
	
	/** 設置目標 */
	public void SetTarget (Transform target) {
		this.target = target;

		if (this.target != null) {

			Canvas canvas = RectTransformUtil.FindRootCanvas(this.target.gameObject);

			if (canvas != null) {
				if (canvas.renderMode != RenderMode.WorldSpace) {
					this.isInWorld = false;
				} else {
					this.isInWorld = true;
				}
			} else {
				this.isInWorld = true;
			}

		}
	}

	
	/** 設置位置 */
	public void SetPosition (Vector2 targetPos) {
		RectTransform followerTrans = (this.follower as RectTransform);
		followerTrans.anchorMin = Vector2.zero;
		followerTrans.anchorMax = Vector2.zero;
		followerTrans.anchoredPosition = targetPos;
	}


	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}

}