using UnityEngine;

using System.Text;
using System.Collections;
using System.Collections.Generic;

/**
 * 縮短字串
 * 來源: 不詳
 */

namespace Uzil.Macro {

public class URLShortner {
	
	/** 數字->字元映射 */
	private static string DICT = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
	
	/** 字元->數字映射 */
	private static char[] CHARS = DICT.ToCharArray();

	/** 數據庫 */
	private static Dictionary<char, int> NUMBERS = new Dictionary<char, int>();

	/** 長度 */
	private static int length;

	/** 初始化 */
	public static void Init () {
		URLShortner.length = URLShortner.CHARS.Length;
		for (int i = 0; i < length; i++) {
			URLShortner.NUMBERS.Add(CHARS[i], i);
		}
	}

	/**
	 * 根據從數據庫回傳的紀錄ID產生對應的短網址編碼
	 * @param id (1-56.8billion)
	 */
	public static string Encode(ulong id) {
		StringBuilder shortURL = new StringBuilder();
		while (id > 0) {
			int r = (int) (id % 62);
			shortURL.Insert(0, URLShortner.CHARS[r]);
			id = id / 62;
		}
		int length = shortURL.Length;
		while (length < 6) {
			shortURL.Insert(0, URLShortner.CHARS[0]);
			length++;
		}
		return shortURL.ToString();
	}

	/**
	 * 根據獲得的短網址編碼解析出數據庫中對應的紀錄ID
	 * @param key 短網址 eg. RwTji8, GijT7Y等等
	 */
	public static long Decode(string key) {
		char[] shorts = key.ToCharArray();
		int len = shorts.Length;
		ulong id = 0L;
		for (int i = 0; i < len; i++) {
			id = id + (ulong) (URLShortner.NUMBERS[shorts[i]] * Mathf.Pow(62, len-i-1));
		}
		return (long)id;
	}

}

}