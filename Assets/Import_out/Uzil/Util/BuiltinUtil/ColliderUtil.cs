using UnityEngine;

namespace Uzil.BuiltinUtil {

public class ColliderUtil {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/


	/** 在Collider中取得隨機位置 */
	public static Vector3 GetRandomPosInCollider (Collider collider) {
        
		Bounds bounds = collider.bounds;
		Vector3 point = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            Random.Range(bounds.min.y, bounds.max.y),
            Random.Range(bounds.min.z, bounds.max.z)
        );
 
        if (point != collider.ClosestPoint(point)) {
            // Debug.Log("Out of the collider! Looking for other point...");
            point = ColliderUtil.GetRandomPosInCollider(collider);
        }
 
        return point;
    }

	/** 是否 位置 位於 Collider中 */
	public static bool IsPosInCollider (Collider collider, Vector3 position) {
		return collider.bounds.Contains(position);
	}

	/** 是否相交 */
	public static bool IsIntersects (Collider a, Collider b) {
		return a.bounds.Intersects(b.bounds);
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