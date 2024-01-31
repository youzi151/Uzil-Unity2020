using UnityEngine;

namespace Uzil.Util {

public class MathUtil {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/* 比較 (比較子 為 字串) */
	public static bool Compare (int a, int b, string comparer) {
		return MathUtil.Compare((float)a, (float)b, comparer);
	}

	/* 比較 (比較子 為 字串) */
	public static bool Compare (float a, float b, string comparer) {
		switch (comparer) {
			case ">":
				return a > b;
			case ">=":
				return a >= b;
			case "<":
				return a < b;
			case "<=":
				return a <= b;
			case "==":
				return a == b;
			case "!=":
				return a != b;
			default:
				return false;
		}
	}

	/** 絕對值 */
	public static float Abs (float a) {
		return a < 0 ? a * -1: a;
	}

	/** 最小值 */
	public static float Min (params float[] arr) {
		float a = arr[0];
		for (int idx = 1; idx < arr.Length; idx++) {
			float b = arr[idx];
			if (b < a) a = b;
		}
		return a;
	}

	/** 最大值 */
	public static float Max (params float[] arr) {
		float a = arr[0];
		for (int idx = 1; idx < arr.Length; idx++) {
			float b = arr[idx];
			if (b > a) a = b;
		}
		return a;
	}

	
	/** 最小 */
	public static float MinAbs (params float[] nums) {
		float min = nums[0];
		float minAbs = MathUtil.Abs(nums[0]);
		foreach (float each in nums) {
			float eachAbs = MathUtil.Abs(each);
			if (eachAbs < minAbs) {
				minAbs = eachAbs;
				min = each;
			}
		}
		return min;
	}

	/** 循環數 */
	public static float Loop (float num, float min, float max) {
		if (num == max) return min;
		if (num >= min && num <= max) return num;

		if (num < 0) {
			num = Mathf.Ceil(num * 1000f) * 0.001f;
		} else {
			num = Mathf.Floor(num * 1000f) * 0.001f;
		}
		
		// 防呆
		float start, end;
		if (min < max) {
			start = min; end = max;
		} else {
			start = max; end = min;
		}

		// 循環長度
		float length = (end - start);

		float newNum = num;
		if (num < min) {
			// 從終點 往前算 (終點到指定數 餘 循環長度)
			newNum = end - Mathf.Abs((num-end) % length);
		} else if (num > max) {
			// 從起點 往後算 (起點到指定數 餘 循環長度)
			newNum = start + Mathf.Abs((num-start) % length);
		}

		if (newNum == max) {
			newNum = 0;
		}

		return newNum;
	}

	/** 循環數 距離 */
	public static float[] GetOffsetsLoop (float from, float to, float min, float max) {
		float length = max - min;
		
		from = MathUtil.Loop(from, min, max);
		to = MathUtil.Loop(to, min, max);
		if (from == to) return new float[]{0, 0};

		float offset = to - from;

		float offset_alt;
		if (offset > 0) offset_alt = (to - length) - from;
		else offset_alt = (to + length) - from;

		float[] offsets = new float[]{
			MathUtil.Min(offset, offset_alt),
			MathUtil.Max(offset, offset_alt)
		};

		return offsets;
	}

	/* 除 */
	public static float Division (float a, float b) {
		if (a == 0 && b == 0) {
			return 0;
		} else {
			return a / b;
		}
	}

		
	/* 確保角度 */
	public static float ValidAngle (float angle) {
		while (angle < 0){
			angle += 360;
		}
		while (angle > 360){
			angle -= 360;
		}
		return angle;
	}

	/* 旋轉至 */
	public static float RotateToward (float from, float to, float movement) {

		float a = MathUtil.ValidAngle(from);
		float b = MathUtil.ValidAngle(to);

		if (a == b) return to;

		float way1 = b - a;
		float way2 = (b + (b > a? -360 : 360)) - a;
		
		float way1Abs = MathUtil.Abs(way1);
		float way2Abs = MathUtil.Abs(way2);

		float length = 0;
		bool isClockwise = false;
		if (way1Abs < way2Abs){
			length = way1Abs;
			isClockwise = way1 > 0;
		}else{
			length = way2Abs;
			isClockwise = way2 > 0;
		}
		float normal = isClockwise? 1 : -1;

		return from + normal * (MathUtil.Min(length, movement));

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