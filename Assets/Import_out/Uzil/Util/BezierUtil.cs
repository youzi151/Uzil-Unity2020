
using System;
using System.IO;
using UnityEngine;

namespace Uzil.Util {

public class BezierUtil {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/
	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

    /** 以 x軸 取得 線段T值(百分比) */
    public static float GetT (float p2, float p3, float x, int sample, float sampleStep = -1f) {
        // 若沒有指定
        if (sampleStep <= 0) {
            sampleStep = 1f / Mathf.Pow(2f, sample);
        }

		// 線段T值
        float t = 0f;
		// 中間點
		float mid = 0f;

		for (float n = sample; n > 0; n--) {
			// 設 中間點 為 樣本最小值 * 2的n次方
			mid = t + sampleStep * Mathf.Pow(2, n);
			// 若 取中間點的X 小於 目標的X 則 設 線段T值 為 該中間點
			if (BezierUtil.GetPoint01(p2, p3, mid) < x) t = mid;
		}
    
        return t;
    }

    /** 取得線段值 (百分比) */
    public static float GetPoint01 (float p2, float p3, float t) {
		if (t == 0f) return 0f;
        if (t == 1f) return 1f;

		// float t2 = Mathf.Pow(t, 2);
		// float t3 = t2 * t;
		// return (((t2 - t3) * (p2/t -p2 +p3 )) * 3) + (t3);
		
		float omt = 1f-t;
		float omt2 = omt * omt;
		float t2 = t * t;

		return p2*(3f*omt2*t) + p3*(3f*omt*t2) + t2*t;

		// float a = (t*p2);
		// float b = (omt*p2) + (t*p3);
		// float c = (omt*p3) + t;
		
		// float d = (omt*a) + (t*b);
		// float e = (omt*b) + (t*c);

		// return (omt*d) + (t*e);
	}

    /** 取得線段值 */
	public static float GetPoint (float p1, float p2, float p3, float p4, float t) {
		if (t == 0f) return p1;
        if (t == 1f) return p4;

		float omt = 1f-t;
		float omt2 = omt * omt;
		float t2 = t * t;

		return p1*(omt*omt2) + p2*(3f*omt2*t) + p3*(3f*omt*t2) + p4*(t2*t);
	
		// float omt3 = omt2 * omt;
		// float t3 = t2 * t;

		// float res = (omt3*p1) + (((t2 - t3) * (p2/t -p2 +p3 )) * 3) + (t3*p4);
		// return res;

		// float a = (omt*p1) + (t*p2);
		// float b = (omt*p2) + (t*p3);
		// float c = (omt*p3) + (t*p4);
		
		// float d = (omt*a) + (t*b);
		// float e = (omt*b) + (t*c);

		// return (omt*d) + (t*e);
	}


	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}

