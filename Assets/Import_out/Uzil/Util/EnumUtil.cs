using System;

namespace Uzil.Util {

    public class EnumUtil {


        /* 從文字解析 */
        public static T Parse<T> (string str) {
            return (T) Enum.Parse(typeof(T), str, /* ignoreCase */true);
        }

        /* 轉換為文字 */
        public static string ToString (Enum en) {
            return en.ToString("f");
        }

    }


}