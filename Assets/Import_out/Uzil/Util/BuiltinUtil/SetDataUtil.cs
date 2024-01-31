using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

using Uzil.Res;
using Uzil.Misc;
using Uzil.ObjInfo;

namespace Uzil.BuiltinUtil {

/** 
 * 依照 傳入資料 設置 各種物件
 */

public class SetDataUtil {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/** 設置 2D剛體 */
	public static void SetRigidbody2D (Rigidbody2D rigidbody, DictSO data) {

		data.TryGetEnum<CollisionDetectionMode2D>("collisionDetectMode", (res)=>{
			switch (res) {
				case CollisionDetectionMode2D.Continuous:
					rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
					break;
				default:
					rigidbody.collisionDetectionMode = CollisionDetectionMode2D.Discrete;
					break;
			}
		});
		
		data.TryGetFloat("gravityScale", (res)=>{
			rigidbody.gravityScale = res;
		});

		data.TryGetFloat("mass", (res)=>{
			rigidbody.mass = res;
		});

		data.TryGetVector2("velocity", (res)=>{
			rigidbody.velocity = res;
		});

		data.TryGetFloat("angularVelocity", (res)=>{
			rigidbody.angularVelocity = res;
		});

		data.TryGetBool("isFreezeRotation", (res)=>{
			rigidbody.freezeRotation = res;
		});

		if (data.ContainsKey("physicsMaterial")) {
			
			PhysicsMaterial2D physicsMat = null;
			DictSO physicsMatData = data.GetDictSO("physicsMaterial");
			
			if (physicsMatData != null) {
				physicsMat = new PhysicsMaterial2D();
				SetDataUtil.SetPhysicsMaterial2D(physicsMat, physicsMatData);
			} else {
				string path = data.GetString("physicsMaterial");
				physicsMat = ResMgr.Get<PhysicsMaterial2D>(new ResReq(path));
			}

			if (physicsMat != null) {
				rigidbody.sharedMaterial = physicsMat;
			}
		}

		if (data.ContainsKey("freezePosition")) {
			List<bool> freezePos = data.GetList<bool>("freezePosition");
			bool isFreezeX = freezePos[0];
			bool isFreezeY = freezePos[1];
			bool isFreezeR = rigidbody.freezeRotation;

			if (isFreezeX && isFreezeY && isFreezeR) {
				rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
			} else if (isFreezeX && isFreezeY && !isFreezeR) {
				rigidbody.constraints = RigidbodyConstraints2D.FreezePosition;
			} else if (isFreezeX && !isFreezeY && !isFreezeR) {
				rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX;
			} else if (!isFreezeX && isFreezeY && !isFreezeR) {
				rigidbody.constraints = RigidbodyConstraints2D.FreezePositionY;
			} else if (isFreezeX && !isFreezeY && isFreezeR) {
				rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
			} else if (!isFreezeX && isFreezeY && isFreezeR) {
				rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionY;
			} else {
				rigidbody.constraints = RigidbodyConstraints2D.None;
			}
			
		}

	}

	/** 設置 變形 */
	public static void SetTransform (Transform transform, DictSO data) {
		data.TryGetVector3("position", (pos)=>{
			transform.localPosition = pos;
		});

		data.TryGetVector3("rotation", (rot)=>{
			transform.localRotation = Quaternion.Euler(rot.x, rot.y, rot.z);
		});

		data.TryGetVector3("scale", (scale)=>{
			transform.localScale = scale;
		});

		data.TryGetVector3("lookAt", (res)=>{
			Vector3 faceTo = res - transform.position;
			transform.rotation = Quaternion.LookRotation(faceTo);
		});
	}

	/** 取得 變形 */
	public static DictSO GetTransform (Transform transform) {
		DictSO data = new DictSO();
		
		data.Set("position", DictSO.Vector3To(transform.localPosition));
		data.Set("rotation", DictSO.Vector3To(transform.localRotation.eulerAngles));
		data.Set("scale", DictSO.Vector3To(transform.localScale));

		data.Set("worldPosition", DictSO.Vector3To(transform.position));
		data.Set("worldRotation", DictSO.Vector3To(transform.rotation.eulerAngles));
		data.Set("worldScale", DictSO.Vector3To(transform.lossyScale));

		return data;
	}

	/** 設置 矩形變形 */
	public static void SetRectTransform (RectTransform transform, DictSO data) {

		data.TryGetVector2("pivot", (res)=>{
			transform.pivot = res;
		});

		data.TryGetVector2("anchorMin", (res)=>{
			transform.anchorMin = res;
		});

		data.TryGetVector2("anchorMax", (res)=>{
			transform.anchorMax = res;
		});

		data.TryGetVector2("position", (res)=>{
			transform.anchoredPosition = res;
		});
		
		data.TryGetVector2("sizeDelta", (res)=>{
			transform.sizeDelta = res;
		});

		data.TryGetVector2("size", (res)=>{
			transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, res.x);
			transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, res.y);
		});

		data.TryGetVector3("rotation", (rot)=>{
			transform.rotation = Quaternion.Euler(rot.x, rot.y, rot.z);
		});
		
		data.TryGetVector3("scale", (res)=>{
			transform.localScale = res;
		});
	}

	/** 取得 矩形變形 */
	public static DictSO GetRectTransform (RectTransform transform, List<string> requests) {
		DictSO res = new DictSO();

		if (requests.Contains("pivot")) {
			res.Set("pivot", DictSO.Vector2To(transform.pivot));
		}

		if (requests.Contains("anchorMin")) {
			res.Set("anchorMin", DictSO.Vector2To(transform.anchorMin));
		}

		if (requests.Contains("anchorMax")) {
			res.Set("anchorMax", DictSO.Vector2To(transform.anchorMax));
		}

		if (requests.Contains("position")) {
			res.Set("position", DictSO.Vector2To(transform.anchoredPosition));
		}
		
		if (requests.Contains("sizeDelta")) {
			res.Set("sizeDelta", DictSO.Vector2To(transform.sizeDelta));
		}

		if (requests.Contains("rotation")) {
			res.Set("rotation", DictSO.Vector3To(transform.rotation.eulerAngles));
		}
		
		if (requests.Contains("scale")) {
			res.Set("scale", DictSO.Vector3To(transform.localScale));
		}

		if (requests.Contains("rect")) {
			Rect rect = transform.rect;
			DictSO _rect = new DictSO().Set("x", rect.x).Set("y", rect.y).Set("width", rect.width).Set("height", rect.height);
			res.Set("rect", _rect);
		}

		return res;
	}

	/** 設置 圖像 */
	public static void SetImage (Image image, DictSO data) {

		RectOffset border = null;

		bool enabled = true;

		/** 啟用 */
		data.TryGetBool("active", (res)=>{
			enabled = res;
		});
		/** 類型 */
		data.TryGetEnum<Image.Type>("type", (res)=>{
			image.type = res;
		});
		
		/** 貼圖邊界 */
		data.TryGetVector4("border", (v4)=>{
			border = new RectOffset((int) v4.x, (int) v4.y, (int) v4.z, (int) v4.w);
			image.type = Image.Type.Sliced;
		});

		/** 貼圖 */
		data.TryGet("texture", (obj)=>{

			TextureInfo texInfo = new TextureInfo(obj);

			Sprite sprite = null;

			if (texInfo != null) {
				if (texInfo.isTextureExist && texInfo.texturePath != "") {
					
					ResReq req = new ResReq(texInfo.texturePath).User(image);

					// 若 Image層中 有 指定 filterMode
					data.TryGetEnum<FilterMode>("filterMode", (res)=>{
						texInfo.filterMode = res;
					});
					
					if (texInfo.isFilterModeExist) {
						req.Arg("filterMode", texInfo.filterMode);
					}

					if (border != null) {
						req.Arg("border", (RectOffset) border);
						req.isForceReload = true;
					}

					ResInfo res = ResMgr.Hold<Sprite>(req);
					if (res != null) {
						sprite = res.GetResult<Sprite>();
						ResMgr.UnholdOnDestroy(res, image, image.gameObject);
					}
					
				}
			}

			image.sprite = sprite;

			if (sprite == null) {
				enabled = false;
			}
		});

		/** 阻擋輸入 */
		data.TryGetBool("raycastTarget", (res)=>{
			image.raycastTarget = res;
		});

		/** 顏色 */
		data.TryGetColor("color", (res)=>{
			image.color = res;
		});

		// 是否有設置填滿
		bool isSetFill = false;
		
		/** 填滿 類型 */
		data.TryGetEnum<Image.FillMethod>("fillMethod", (res)=>{
			image.fillMethod = res;
			isSetFill = true;
		});

		/** 填滿 順逆時針 */
		data.TryGetBool("fillClockwise", (res)=>{
			image.fillClockwise = res;
			isSetFill = true;
		});

		/** 填滿 原點 */
		data.TryGetInt("fillOrigin", (res)=>{
			image.fillOrigin = res;
			isSetFill = true;
		});

		/** 填滿 值 */
		data.TryGetFloat("fillAmount", (res)=>{
			image.fillAmount = res;
			isSetFill = true;
		});

		if (isSetFill) {
			image.type = Image.Type.Filled;
		}

		data.TryGetEnum<SizeMode>("sizeMode", (res)=>{
			switch (res) {
				case SizeMode.Native:
					image.SetNativeSize();
					break;
				case SizeMode.Custom:
					data.TryGetVector2("size", (size)=>{
						image.rectTransform.sizeDelta = size;
					});
					break;
				default: 
					break;
			}
		});

		image.enabled = enabled;
	}

	/** 設置 貼圖 */
	public static void SetTexture (Texture tex, DictSO data) {

		data.TryGetEnum<FilterMode>("filterMode", (res)=>{
			tex.filterMode = res;
		});

	}

	/** 設置 遮罩 */
	public static void SetMask (Mask mask, DictSO data) {


	}

	/** 設置 2D物理材質 */
	public static void SetPhysicsMaterial2D (PhysicsMaterial2D mat, DictSO data) {

		data.TryGetFloat("friction", (res)=>{
			mat.friction = res;
		});

		data.TryGetFloat("bounciness", (res)=>{
			mat.bounciness = res;
		});

	}

	public static void SetCollider2D (Collider2D collider, DictSO data) {
		
		// 是否為觸發 (不碰撞)
		data.TryGetBool("isTrigger", (res)=>{
			collider.isTrigger = res;
		});
		data.TryGetDictSO("physicsMaterial", (physicsMatData)=>{

			PhysicsMaterial2D physicsMat = null;
			
			if (physicsMatData != null) {
				physicsMat = new PhysicsMaterial2D();
				SetDataUtil.SetPhysicsMaterial2D(physicsMat, physicsMatData);
			} else {
				string path = data.GetString("physicsMaterial");
				physicsMat = ResMgr.Get<PhysicsMaterial2D>(new ResReq(path));
			}


			if (physicsMat != null) {
				collider.sharedMaterial = physicsMat;
			}
		});

	}

	/** 設置 多邊形2D碰撞器 */
	public static void SetPolygonCollider2D (PolygonCollider2D collider, DictSO data) {
		bool isSetAnyPath = false;

		// 基底
		SetDataUtil.SetCollider2D(collider, data);

		// 單條 路徑
		data.TryGetList<Vector2>("path", (res)=>{
			List<Vector2> path = res;
			collider.SetPath(0, path);
			isSetAnyPath = true;
		});

		// 路徑列表
		data.TryGetList("pathList", (pathList)=>{

			collider.pathCount = pathList.Count;

			for (int idx = 0; idx < pathList.Count; idx++) {
				List<Vector2> path = DictSO.List<Vector2>(pathList[idx]);

				collider.SetPath(idx, path);

				isSetAnyPath = true;
			}
		});

		if (!isSetAnyPath) {
			collider.pathCount = 0;
		}
	}

	/** 設置 圓形2D碰撞器 */
	public static void SetCircleCollider2D (CircleCollider2D collider, DictSO data) {

		// 基底
		SetDataUtil.SetCollider2D(collider, data);

		data.TryGetFloat("radius", (res)=>{
			collider.radius = res;
		});

	}

	/** 設置 EventTrigger */
	public static void SetEventTrigger (EventTrigger evtTrigger, DictSO data) {
		
		evtTrigger.triggers.Clear();

		if (data.ContainsKey("onPointerClick")) {

			Action<DictSO> cb = (Action<DictSO>) data.Get("onPointerClick");
			UnityAction<BaseEventData> act = (evtData)=>{
				cb(null);
			};

			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerClick;
			entry.callback.AddListener(act);

			evtTrigger.triggers.Add(entry);
		}
	}

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}


}