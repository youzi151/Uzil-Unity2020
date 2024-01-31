using System;
using System.Text;
using System.Security.Cryptography;

namespace Uzil.Util {

public class EncryptUtil {

    public static string ToMD5 (string content) {
        MD5 md5 = new MD5CryptoServiceProvider();
        // 將要加密的字串轉換為位元組陣列
        byte[] bytes = Encoding.Default.GetBytes(content);
        // 將字串加密後也轉換為字元陣列
        byte[] encryptdata = md5.ComputeHash(bytes);
        // 將加密後的位元組陣列轉換為加密字串
        return Convert.ToBase64String(encryptdata);
    }


}


}