using System.Collections.Generic;

namespace Uzil.Util {

public class Comparer {
	public const string EQUAL = "==";
	public const string NOT_EQUAL = "!=";
	public const string GREATER = ">";
	public const string GREATER_EQUAL = ">=";
	public const string LESS = "<";
	public const string LESS_EQUAL = "<=";
}

public class CompareUtil {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/** 比較 */
	public static bool Compare (bool a, bool b, string comparer) {
		switch (comparer) {
			case Comparer.EQUAL:
				return a == b;
			case Comparer.NOT_EQUAL:
				return a != b;
		}
		return false;
	}

	/** 比較 */
	public static bool Compare (string a, string b, string comparer) {
		switch (comparer) {
			case Comparer.EQUAL:
				return a == b;
			case Comparer.NOT_EQUAL:
				return a != b;
		}
		return false;
	}

	/** 比較 */
	public static bool Compare (float a, float b, string comparer) {
		switch (comparer) {
			case Comparer.EQUAL:
				return a == b;
			case Comparer.NOT_EQUAL:
				return a != b;
			case Comparer.GREATER:
				return a > b;
			case Comparer.GREATER_EQUAL:
				return a >= b;
			case Comparer.LESS:
				return a < b;
			case Comparer.LESS_EQUAL:
				return a <= b;
		}
		return false;
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