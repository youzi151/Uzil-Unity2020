using System;

namespace Uzil.Util {


	public class TypeUtil {

		/* 要尋找的命名空間 */
		public static string [] nameSpaces = new string[]{
			"", "Uzil", "Uzil.Proc", "Uzil.Anim"
		};

		/* 尋找類別 */
		public static Type FindType (string typeName) {
			
			Type tp = null;

			// 每個命名空間中 搜尋
			for (int idx = 0; idx < TypeUtil.nameSpaces.Length; idx++) {
				tp = Type.GetType(nameSpaces[idx]+"."+typeName);
				if (tp != null) break;
			} 

			return tp;
		}

	}


}