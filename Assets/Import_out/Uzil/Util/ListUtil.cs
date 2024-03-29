using System.Collections.Generic;

namespace Uzil.Util {

public class ListUtil {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/* 交集 */
	public static List<T> Intersect<T> (List<T> a, List<T> b) {
		List<T> c = new List<T>();

		if (a == null || b == null) return c;

		for (int i = 0; i < a.Count; i++) {

			T eachA = a[i];
			if (c.Contains(eachA)) continue;

			for (int i2 = 0; i2 < b.Count; i2++) {
				if (EqualityComparer<T>.Default.Equals(eachA, b[i2])) {
					c.Add(eachA);
					break;
				}
			}
		}
		return c;
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