using System.Collections.Generic;

using UnityEngine;

namespace Uzil.BuiltinUtil {

public class RectTransformUtil {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	
	/** 尋找 所屬根Canvas */
	public static Canvas FindRootCanvas (GameObject target) {
		RectTransform rect = target.GetComponent<RectTransform>();
		if (rect == null) return null;
		return RectTransformUtil.FindRootCanvas(rect); 
	}
	/** 尋找 所屬根Canvas */
	public static Canvas FindRootCanvas (RectTransform target) {
		// 下個搜尋對象
		Transform nextFind = target.transform;
		if (nextFind == null) return null;
		
		// 結果
		Canvas result = null;

		// 持續搜尋
		int safe = 50;
		while (nextFind != null) {

			if (safe-- < 0) break;
			Canvas canvas = nextFind.gameObject.GetComponent<Canvas>();
			if (canvas != null) {
				result = canvas;
			}

			// 找下一個父物件
			nextFind = nextFind.parent;
		}

		if (result == null) return null;
		return result;
	}

	/** 尋找 所屬Canvas */
	public static Canvas FindParentCanvas (GameObject target) {
		RectTransform rect = target.GetComponent<RectTransform>();
		if (rect == null) return null;
		return RectTransformUtil.FindParentCanvas(rect);
	}

	/** 尋找 所屬Canvas */
	public static Canvas FindParentCanvas (RectTransform target) {

		// 下個搜尋對象
		Transform nextFind = target.transform.parent;
		if (nextFind == null) return null;
		
		// 結果
		Canvas result = null;

		// 持續搜尋
		int safe = 50;
		while (result == null) {

			if (safe-- < 0) break;
			result = nextFind.gameObject.GetComponent<Canvas>();
			// 找下一個父物件
			nextFind = nextFind.parent;
			// 若 已無父物件 則 返回null
			if (nextFind == null) break;
		}

		if (result == null) return null;
		return result;
	}

	/** 取得 RectTransform在Canvas 中的 Rect */
	public static Rect GetRectInCanvas (Canvas canvas, RectTransform transform) {
		return RectTransformUtil.GetRectIn(transform, (canvas.transform as RectTransform));
	}

	/** 
	 * 取得 目標 在 另一處Rectransform底下的位置
	 * 
	 * 使用 localPosition 逐個parent回推，同時排除scale影響
	 * e.g.
	 * Obj3(inner)[0]{
	 *   localPos[100] + (pSize[800] * (0 - anchorMin[0])) + (size[200] * (0 - pivot[0.5]))
	 * }
	 * + Obj2[200]{
	 *   localPos[200] + (pSize[800] * (0 - anchorMin[0]))
	 * }
	 * + Obj1[400] {
	 *   localPos[-400] + (pSize[1600] * (0 - anchorMin[0.5]))
	 * }
	 * = final[500]
	 */
	public static Rect GetRectIn (RectTransform trans, RectTransform targetParent) {
		Canvas canvas;
		RectTransform canvasTrans;
		RectTransform nextTrans;

		// 要轉移的目標 ==========
		canvas = RectTransformUtil.FindRootCanvas(trans);
		canvasTrans = (canvas.transform as RectTransform);
		
		Vector2 scale = trans.lossyScale / (Vector2)canvasTrans.localScale;

		// 尺寸 ======
		Vector2 size = trans.rect.size;
		// 排除 Scale 影響 (不含CanvasScale)
		size = Vector2.Scale(size, scale);

		// 位置 ======
		Vector2 pos = ((Vector2)trans.localPosition) + trans.rect.min;
		// 邏輯上是 加 Vector2.Scale(transform.sizeDelta, (Vector2.zero - transform.pivot))
		// 但 改加 transform.rect.min 也等同
		
		// 排除 Scale 影響 (不含CanvasScale)
		pos = Vector2.Scale(pos, scale);

		// 每一個要回推的parent
		nextTrans = trans.parent as RectTransform;
		// 當 目標存在 且 不是Canvas
		while (nextTrans != null && nextTrans != canvasTrans) {
			// 若再往上 不含有parent則跳出
			if (nextTrans.parent == null) break;

			// 排除 Scale 影響 (不含CanvasScale)
			scale = nextTrans.parent.lossyScale / (Vector2)canvasTrans.localScale;
			// 回推 localPosition
			pos += Vector2.Scale( (Vector2)nextTrans.localPosition, scale);
			// 指定下一個回推目標
			nextTrans = (nextTrans.parent as RectTransform);
		}
		
		// 最後Canvas座標處理 (因為Canvas的pivot強制於中心，要將其改為以左下角為基準點)
		pos.x += canvasTrans.rect.width * 0.5f;
		pos.y += canvasTrans.rect.height * 0.5f;


		// 要轉移到的目標 ==========
		
		canvas = RectTransformUtil.FindRootCanvas(targetParent);
		canvasTrans = (canvas.transform as RectTransform);
		nextTrans = (targetParent as RectTransform);

		// 要轉移到的目標 的 回推parent
		Stack<RectTransform> targetParentToCanvas = new Stack<RectTransform>();
		while (nextTrans != null && nextTrans != canvasTrans) {
			// 若再往上 不含有parent則跳出
			if (nextTrans.parent == null) break;
			targetParentToCanvas.Push(nextTrans);
			nextTrans = (nextTrans.parent as RectTransform);
		}

		// 復原Canvas座標處理
		if (targetParentToCanvas.Count > 0) {
			pos.x -= canvasTrans.rect.width * 0.5f;
			pos.y -= canvasTrans.rect.height * 0.5f;
		}

		// 從 到Canvas前的最後一個parent往 要轉移到的目標 內推
		while (targetParentToCanvas.Count > 0) {
			nextTrans = targetParentToCanvas.Pop();
			// 排除 Scale 影響 (不含CanvasScale)
			scale = nextTrans.parent.lossyScale / (Vector2)canvasTrans.localScale;
			// 內推 localPosition
			pos -= Vector2.Scale(nextTrans.localPosition, scale);
		}

		return new Rect(pos.x, pos.y, size.x, size.y);
	}


	/** 是否重疊 */
	public static bool IsOverlaps (RectTransform a, RectTransform b) {
		Canvas canvasA = RectTransformUtil.FindRootCanvas(a);
		Canvas canvasB = RectTransformUtil.FindRootCanvas(b);

		Rect rectA = RectTransformUtil.GetRectInCanvas(canvasA, a);
		Rect rectB = RectTransformUtil.GetRectInCanvas(canvasB, b);
		return rectA.Overlaps(rectB);
	}

	/** 世界座標 轉 Canvas座標 */
	public static Vector2 WorldToScreenPoint (Canvas canvas, Camera camera, Vector3 worldPos) {

		Vector2 viewportPoint = camera.WorldToViewportPoint(worldPos);
		RectTransform canvasTrans = canvas.GetComponent<RectTransform>();

		Vector2 rectSize = canvasTrans.rect.size;

		return new Vector2(rectSize.x * viewportPoint.x, rectSize.y * viewportPoint.y);
	}

	/** 螢幕座標 轉 RectTransform內的本地座標 */
	public static Vector2 ScreenPointToLocalPointInRectangle (Vector2 screenPoint, RectTransform parent, Camera camera) {
		Vector2 res;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, screenPoint, camera, out res);
		return res;
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