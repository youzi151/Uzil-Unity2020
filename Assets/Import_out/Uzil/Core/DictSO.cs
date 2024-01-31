using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Uzil.Util;

namespace Uzil {

	public class DictSO : Dictionary<string, object> {


		/*=====================================Static Funciton=======================================*/

		/** 快速新建 */
		private static DictSO prepareNew = new DictSO();
		public static DictSO New () {
			// 若沒有 預備好的 則 及時建立
			if (DictSO.prepareNew == null) return new DictSO();

			// 取用 預備好的
			DictSO newOne = DictSO.prepareNew;

			// 清空 預備好的
			DictSO.prepareNew = null;

			// 延遲產生新的
			Invoker.Inst().Once(() => {
				DictSO.prepareNew = new DictSO();
			});

			return newOne;
		}

		/** 是否為json格式物件 */
		public static bool IsJson (object _json) {
			if (_json == null) return false;
			if (_json is DictSO) return true;
			if (_json is Newtonsoft.Json.Linq.JObject) return true;
			try {
				DictSO.Json(_json.ToString());
			}
			catch (Exception) {
				return false;
			}
			return true;
		}

		/** 是否可以為Json記載 */
		public static bool IsJsonable (object target) {
			if (target is int ||
				target is float ||
				target is string) {
				return true;
			}
			else if (target is DictSO) {
				DictSO dictSO = ((DictSO) target);
				foreach (KeyValuePair<string, object> pair in dictSO) {
					if (IsJsonable(pair.Value) == false) {
						return false;
					}
				}
				return true;
			}
			else {
				return false;
			}
		}

		public static bool IsLuaable (object target) {
			if (target == null) {
				return true;
			} else if (target is int ||
				target is float ||
				target is string) {
				return true;
			} else {
				return false;
			}
		}

		/** 轉換為Json字串 */
		public static string ToJson (object obj) {
			if (obj == null) return null;
			try {

				if (obj is string) {
					return (string) obj;
				}

				else if (obj is Vector2) {
					List<object> list = new List<object>();
					Vector2 v2 = (Vector2) obj;
					list.Add(v2.x);
					list.Add(v2.y);
					obj = list;
				}

				else if (obj is Vector3) {
					List<object> list = new List<object>();
					Vector3 v3 = (Vector3) obj;
					list.Add(v3.x);
					list.Add(v3.y);
					list.Add(v3.z);
					obj = list;
				}

				else if (DictSO.IsNumeric(obj)) {
					return obj.ToString();
				}

				return JsonConvert.SerializeObject(obj, Formatting.Indented);
			}
			catch (Exception e) {
				Debug.Log("[DictSO] : SerializeObject Fail in object:" + obj);
				Debug.Log(e);
				return null;
			}
		}

		/** 解析 Json字串或Object 轉成 DictSO */
		public static DictSO Json (object _json) {
			if (_json == null) return null;
			if (_json is DictSO) return (DictSO) _json;
			if (_json is Newtonsoft.Json.Linq.JValue) {
				_json = _json.ToString();
			}
			if (_json is Newtonsoft.Json.Linq.JObject) {
				return DictSO.FromJObj((Newtonsoft.Json.Linq.JObject) _json);
			}
			if (_json is string) {
				return DictSO.Json(_json.ToString());
			}
			Debug.Log("[DictSO] : can't convert type:"+_json.GetType().ToString());
			Debug.Log(_json);
			return null;
		}

		/** 解析 Json字串 轉成 DictSO */
		public static DictSO Json (string json, bool isLogError = false) {
			if (json == null) return null;
			// json = Regex.Replace(json, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");
			try {
				return DictSO.FromJObj(JObject.Parse(json));
			}
			catch (Exception e) {
				if (isLogError) {
					Debug.Log("[DictSO] : DeserializeObject Fail in string:" + json);
					Debug.Log(e);
				}
				return null;
			}

		}

		protected static DictSO FromJObj (JObject jObj) {
			DictSO data = new DictSO();
			IEnumerable ienumerable = jObj.Properties();
			foreach (JProperty prop in ienumerable) {
				try {
					object val = DictSO.GetObjFromJToken(prop.Value);
					data.Set(prop.Name, val);
				} catch (Exception e) {
					Debug.Log(e);
				}
			}
			return data;
		}

		/*轉換成各類型別================*/

		/** 列舉 */
		public static T Enum<T> (string enumStr) {
			return EnumUtil.Parse<T>(enumStr);
		}

		/** 布林 */
		public static bool Bool (object obj) {
			bool res = default(bool);
			try {
				if (obj is bool) {
					res = (bool) obj;
				}
				else if (obj is string) {
					res = bool.Parse((string) obj);
				}
				else if (DictSO.IsNumeric(obj)) {
					res = Convert.ToInt32(obj) > 0 ? true : false;
				}
				else {
					res = bool.Parse(obj.ToString());
				}
			}
			catch (Exception e) {
				Debug.Log(e);
			}
			return res;
		}

		/** 
		 * 解析Json字串或Array轉成List
		 * T限定類型: string, float, int, Vector2, Vector3, DictSO
		 */
		public static List<T> List<T> (object json) {
			if (json is List<T>) return (List<T>) json;

			List<object> objList = DictSO.List(json);
			if (objList == null) return null;

			List<T> list = new List<T>();

			Func<object, object> getToAdd = null;

			//轉為string
			if (typeof(T) == typeof(string)) {
				getToAdd = (each) => {
					if (each == null) return null;
					else return each.ToString();
				};
			}
			
			else if (typeof(T) == typeof(bool)) {
				getToAdd = (each) => {
					return DictSO.Bool(each);
				};
			}

			else if (typeof(T) == typeof(float)) {
				getToAdd = (each) => {
					return DictSO.Float(each);
				};
			}

			else if (typeof(T) == typeof(double)) {
				getToAdd = (each) => {
					return DictSO.Double(each);
				};
			}

			else if (typeof(T) == typeof(int)) {
				getToAdd = (each) => {
					return DictSO.Int(each);
				};
			}

			else if (typeof(T) == typeof(Vector2)) {
				getToAdd = (each) => {
					return DictSO.Vector2(each);
				};
			}

			else if (typeof(T) == typeof(Vector3)) {
				getToAdd = (each) => {
					return DictSO.Vector3(each);
				};
			}

			else if (typeof(T) == typeof(DictSO)) {
				getToAdd = (each) => {
					return DictSO.Json(each);
				};
			}
			else {
				getToAdd = (each) => { return each; };
			}

			foreach (object each in objList) {
				object toAdd = getToAdd(each);
				try {
					list.Add((T) toAdd);
				}
				catch (Exception e) {
					Debug.Log(e);
					Debug.Log("[DictSO]: List<" + typeof(T).ToString() + "> with " + each.GetType().ToString());
					break;
				}
			}


			return list;
		}

		/** 解析 Json字串或Object 轉成 List<object> */
		public static List<object> List (object json) {
			if (json == null) return null;
			if (json is List<object>) return (List<object>) json;
			if (json is IEnumerable && !(json is string)) {
				IEnumerator orinList = (json as IEnumerable).GetEnumerator();
				List<object> res = new List<object>();
				while (orinList.MoveNext()) {
					res.Add(orinList.Current);
				}
				return res;
			}

			if (!(json is string)) return null;
			
			string jsonStr = DictSO.ToJson(json);

			// 特例	
			if (jsonStr == "") {
				return null;
			} else if (jsonStr == "{}" || jsonStr == "[]") {
				return new List<object>();
			}

			// 效能消耗
			// _json = Regex.Replace(_json, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1");

			List<object> list;

			try {
				list = JsonConvert.DeserializeObject<List<object>>(jsonStr);
			}
			catch (Exception) {
				return null;
			}
			return list;
		}


		/*==將Json轉為其他型別=====================*/

		/** 將Json轉為Int */
		public static int Int (object _json) {
			if (DictSO.IsNumeric(_json)) {
				try {
					return Convert.ToInt32(_json);
				} catch (Exception) {}
			}

			Int32 num = default(int);
			string str = _json.ToString();
			while (Int32.TryParse(str, out num) == false) {
				str = str.Substring(0, str.Length-1);
			}
			return num;
		}

		public static Int64 Int64 (object _json) {
			if (DictSO.IsNumeric(_json)) {
				try {
					return Convert.ToInt64(_json);
				} catch (Exception) {}
			}

			Int64 num = default(Int64);
			string str = _json.ToString();
			while (System.Int64.TryParse(str, out num) == false && str.Length > 0) {
				str = str.Substring(0, str.Length-1);
			}
			return num;
		}

		/** 將Json轉為float */
		public static float Float (object _json) {
			if (DictSO.IsNumeric(_json)) {
				try {
					return Convert.ToSingle(_json);
				} catch (Exception) {}
			}

			float num = default(float);
			string str = _json.ToString();
			while (float.TryParse(str, out num) == false && str.Length > 0) {
				str = str.Substring(0, str.Length-1);
			}
			return num;
		}

		/** 將Json轉為double */
		public static double Double (object _json) {
			if (DictSO.IsNumeric(_json)) {
				try {
					return Convert.ToDouble(_json);
				} catch (Exception) {}
			}
			
			double num = default(double);
			string str = _json.ToString();
			while (double.TryParse(str, out num) == false && str.Length > 0) {
				str = str.Substring(0, str.Length-1);
			}
			return num;
		}

		/** 將json陣列轉為Vector2 */
		public static Vector2? Vector2 (object _json) {
			if (_json is Vector2) { return (Vector2) _json; } 

			List<object> vecList = null;
			
			if (_json is List<object>) { vecList = (List<object>) _json; }
			else if (_json is string || _json is JArray) { vecList = DictSO.List(_json); }
			else if (DictSO.IsNumeric(_json)) {
				float num = DictSO.Float(_json);
				return new Vector2(num, num);
			} 
			
			else {
				Debug.Log(_json.GetType());
			}

			if (vecList == null) { return null; }

			float x = 0, y = 0;
			if (vecList.Count > 0) { x = DictSO.Float(vecList[0]); }
			if (vecList.Count > 1) { y = DictSO.Float(vecList[1]); }

			return new Vector2(x, y);
		}

		/** 將json陣列轉為Vector3 */
		public static Vector3? Vector3 (object _json) {
			if (_json is Vector3) { return (Vector3) _json; } 
			else if (_json is Vector2) { return (Vector3) ((Vector2) _json); }

			List<object> vecList = null;
			
			if (_json is List<object>) { vecList = (List<object>) _json; }
			else if (_json is string || _json is JArray) { vecList = DictSO.List(_json); }
			else if (DictSO.IsNumeric(_json)) {
				float num = DictSO.Float(_json);
				return new Vector3(num, num, num);
			} 
			
			else {
				Debug.Log(_json.GetType());
			}

			if (vecList == null) { return null; }

			float x = 0, y = 0, z = 0;
			if (vecList.Count > 0) { x = DictSO.Float(vecList[0]); }
			if (vecList.Count > 1) { y = DictSO.Float(vecList[1]); }
			if (vecList.Count > 2) { z = DictSO.Float(vecList[2]); }

			return new Vector3(x, y, z);
		}

		/** 將json陣列轉為Vector4 */
		public static Vector4? Vector4 (object _json) {
			if (_json is Vector4) { return (Vector4) _json; } 
			else if (_json is Vector3) { return (Vector4) ((Vector3) _json); }
			else if (_json is Vector2) { return (Vector4) ((Vector2) _json); }
			else if (_json is RectOffset) { 
				RectOffset rectOffset = (RectOffset) _json;
				return new Vector4(rectOffset.left, rectOffset.right, rectOffset.top, rectOffset.bottom); 
			}

			List<object> vecList = null;
			
			if (_json is List<object>) { vecList = (List<object>) _json; }
			else if (_json is string || _json is JArray) { vecList = DictSO.List(_json); }
			else if (DictSO.IsNumeric(_json)) {
				float num = DictSO.Float(_json);
				return new Vector4(num, num, num, num);
			} 
			
			else {
				Debug.Log(_json.GetType());
			}

			if (vecList == null) { return null; }

			float x = 0, y = 0, z = 0, w = 0;
			if (vecList.Count > 0) { x = DictSO.Float(vecList[0]); }
			if (vecList.Count > 1) { y = DictSO.Float(vecList[1]); }
			if (vecList.Count > 2) { z = DictSO.Float(vecList[2]); }
			if (vecList.Count > 3) { w = DictSO.Float(vecList[3]); }

			return new Vector4(x, y, z, w);
		}

		public static RectOffset RectOffset (object _json) {
			if (_json is RectOffset) { return (RectOffset) _json; }
			List<object> rectOffsetList = null;
			
			if (_json is List<object>) { rectOffsetList = (List<object>) _json; }
			else if (_json is string || _json is JArray) { rectOffsetList = DictSO.List(_json); }
			else if (_json is Vector4 || _json is Vector4?) {
				Vector4 vec4 = (Vector4) _json;
				Vector4? _vec4 = DictSO.Vector4(_json);
				if (_vec4 == null) return null;
				vec4 = (Vector4) _vec4;
				rectOffsetList = new List<object>(){vec4[0], vec4[1], vec4[2], vec4[3]};
			}
			
			int left = 0, right = 0, top = 0, bottom = 0;
			if (rectOffsetList.Count > 0) { left = DictSO.Int(rectOffsetList[0]); }
			if (rectOffsetList.Count > 1) { right = DictSO.Int(rectOffsetList[1]); }
			if (rectOffsetList.Count > 2) { top = DictSO.Int(rectOffsetList[2]); }
			if (rectOffsetList.Count > 3) { bottom = DictSO.Int(rectOffsetList[3]); }
			return new RectOffset(left, right, top, bottom);
		}

		/** 將json字串 (color hex) 轉為 Color顏色 */
		public static Color Color (object _json) {
			if (_json is Color) return (Color) _json;

			bool isGot = false;
			Color color = new Color();

			if (_json is string) {
				string str = _json.ToString();
				if (str.StartsWith("#")) {
					isGot = ColorUtility.TryParseHtmlString(_json.ToString(), out color);
				}
			}
			
			if (!isGot) {
				List<float> list = DictSO.List<float>(_json);
				if (list != null) {
					if (list.Count > 0) color.r = list[0];
					if (list.Count > 1) color.g = list[1];
					if (list.Count > 2) color.b = list[2];
					if (list.Count > 3) color.a = list[3];
					isGot = true;
				}
			}

			return color;
		}

		/*==從其他型別轉為可Json化的型別===========*/

		/** 列舉 轉為 string */
		public static string EnumTo<T> (T _enum) where T : struct, IConvertible {
			if (typeof(T).IsEnum == false) return null;

			return (_enum as Enum).ToString("F");
		}

		/** Vector2 轉為 List<object> */
		public static List<object> Vector2To (Vector2 v2) {
			List<object> list = new List<object>();
			list.Add(v2.x);
			list.Add(v2.y);
			return list;
		}
		public static string Vector2ToStr (Vector2 v2) {
			return "["+v2.x+","+v2.y+"]";
		}

		/** Vector3 轉為 List<object> */
		public static List<object> Vector3To (Vector3 v3) {
			List<object> list = new List<object>();
			list.Add(v3.x);
			list.Add(v3.y);
			list.Add(v3.z);
			return list;
		}
		public static string Vector3ToStr (Vector3 v3) {
			return "["+v3.x+","+v3.y+","+v3.z+"]";
		}
		
		

		/** Vector4 轉為 List<object> */
		public static List<object> Vector4To (Vector4 v4) {
			List<object> list = new List<object>();
			list.Add(v4.x);
			list.Add(v4.y);
			list.Add(v4.z);
			list.Add(v4.w);
			return list;
		}
		public static string Vector4ToStr (Vector4 v4) {
			return "["+v4.x+","+v4.y+","+v4.z+","+v4.w+"]";
		}

		/** RectOffset 轉為 List<object> */
		public static List<object> RectOffsetTo (RectOffset rectOffset) {
			return new List<object>{rectOffset.left, rectOffset.right, rectOffset.top, rectOffset.bottom};
		}


		/** Color 轉為 String */
		public static string ColorToHex (Color color) {
			return "#" + ColorUtility.ToHtmlStringRGBA(color);
		}

		/** Color 轉為 陣列 */
		public static List<object> ColorTo (Color color) {
			return new List<object>(){color.r, color.g, color.b, color.a};
		}


		/** 從文字檔中讀取Json */
		public static string GetJsonFromFile (string path) {
			try {
				string result = "";
				foreach (string eachline in File.ReadAllLines(path)) {
					result += eachline;
				}
				return result;
			}
			catch (Exception e) {
				Debug.Log(e);
				return null;
			}
		}


		/*=====================================Public Function=======================================*/


		/** 轉換為Json字串 */
		private bool isDoingToString = false;
		public override string ToString () {
			// 防止指向自己的Loop
			if (this.isDoingToString) return "";

			this.isDoingToString = true;
			string json = this.ToJson();
			this.isDoingToString = false;

			return json;
		}

		/*==主要功能====================*/

		/** 轉成Json string */
		public string ToJson (bool isMinimize = true) {
			try {
				return JsonConvert.SerializeObject(this, isMinimize ? Formatting.None:Formatting.Indented);
			}
			catch (Exception e) {
				Debug.Log(e);
				Debug.Log("[DictSO] : ToJson Error, maybe is a complex jsonObject");
				this.LogMembers();
				return null;
			}
		}

		/** 設置 */
		public DictSO Set (string _key, object _value) {
			if (_value == null) {
				this.Remove(_key);
				return this;
			}

			if (this.ContainsKey(_key)) {
				this[_key] = _value;
			}
			else {
				this.Add(_key, _value);
			}

			return this;
		}

		/** 設置(較快速) (需確保該key不存在) */
		public DictSO Ad (string _key, object _value) {
			this.Add(_key, _value);
			return this;
		}


		/** 取得 */
		public object Get (string _key) {
			if (this.ContainsKey(_key)) {
				return this[_key];
			}
			else {
				return null;
			}
		}

		/** 試著取得 */
		public void TryGet (string _key, Action<object> res) {
			if (this.ContainsKey(_key) == false) return;
			res(this.Get(_key));
		}

		/** 合併 */
		public DictSO Merge (DictSO data) {
			if (data == null) return this;
			foreach (KeyValuePair<string, object> pair in data) {
				if (pair.Value == null) {
					this.Remove(pair.Key);
					continue;
				}

				// 若為 DictSO 則 巢狀融合 
				if (this.ContainsKey(pair.Key)) {
					object exist = this[pair.Key];
					if (exist is DictSO && pair.Value is DictSO) {
						(exist as DictSO).Merge((DictSO) pair.Value);
						continue;
					}
				}

				this.Set(pair.Key, pair.Value);

			}
			return this;
		}

		/*==直接取得轉換過的====================*/

		/** 取得列舉 */
		public T GetEnum<T> (string key) {
			string str = this.GetString(key);
			if (str == null) return default(T);
			T res = DictSO.Enum<T>(str);
			return res;
		}
		/** 取得列舉 */
		public bool TryGetEnum<T> (string key, Action<T> res) {
			if (this.ContainsKey(key) == false) return false;
			res(this.GetEnum<T>(key));
			return true;
		}

		/** 取得布林 */
		public bool GetBool (string key) {
			object obj = this.Get(key);
			if (obj == null) return default(bool);
			bool res = DictSO.Bool(obj);
			return res;
		}
		/** 取得布林 */
		public bool TryGetBool (string key, Action<bool> res) {
			if (this.ContainsKey(key) == false) return false;
			res(this.GetBool(key));
			return true;
		}


		/** 取得字串 */
		public string GetString (string key) {
			object obj = this.Get(key);
			if (obj == null) return null;
			return obj.ToString();
		}
		/** 取得字串 */
		public bool TryGetString (string key, Action<string> res) {
			if (this.ContainsKey(key) == false) return false;
			res(this.GetString(key));
			return true;
		}

		/** 取得整數 */
		public int GetInt (string key) {
			object obj = this.Get(key);
			if (obj == null) return default(int);
			return DictSO.Int(obj);
		}

		/** 取得整數64 */
		public Int64 GetInt64 (string key) {
			object obj = this.Get(key);
			if (obj == null) return default(int);
			return DictSO.Int64(obj);
		}

		/** 取得整數 */
		public bool TryGetInt (string key, Action<int> res) {
			if (this.ContainsKey(key) == false) return false;
			res(this.GetInt(key));
			return true;
		}

		/** 取得整數 */
		public bool TryGetInt64 (string key, Action<Int64> res) {
			if (this.ContainsKey(key) == false) return false;
			res(this.GetInt64(key));
			return true;
		}

		/** 取得浮點數 */
		public float GetFloat (string key) {
			object obj = this.Get(key);
			if (obj == null) return default(float);
			return DictSO.Float(obj);
		}
		/** 取得浮點數 */
		public bool TryGetFloat (string key, Action<float> res) {
			if (this.ContainsKey(key) == false) return false;
			res(this.GetFloat(key));
			return true;
		}

		/** 取得浮點數 */
		public double GetDouble (string key) {
			object obj = this.Get(key);
			if (obj == null) return default(double);
			return DictSO.Double(obj);
		}
		/** 取得浮點數 */
		public bool TryGetDouble (string key, Action<double> res) {
			if (this.ContainsKey(key) == false) return false;
			res(this.GetDouble(key));
			return true;
		}

		/** 取得二維向量 */
		public Vector2? GetVector2 (string key) {
			object obj = this.Get(key);
			if (obj == null) return null;
			return DictSO.Vector2(obj);
		}
		/** 取得二維向量 */
		public bool TryGetVector2 (string key, Action<Vector2> res) {
			if (this.ContainsKey(key) == false) return false;
			Vector2? vec2 = this.GetVector2(key);
			if (vec2 == null) return false;
			res((Vector2)vec2);
			return true;
		}
		

		/** 取得三維向量 */
		public Vector3? GetVector3 (string key) {
			object obj = this.Get(key);
			if (obj == null) return null;
			return DictSO.Vector3(obj);
		}
		/** 取得三維向量 */
		public bool TryGetVector3 (string key, Action<Vector3> res) {
			if (this.ContainsKey(key) == false) return false;
			Vector3? vec3 = this.GetVector3(key);
			if (vec3 == null) return false;
			res((Vector3)vec3);
			return true;
		}

		/** 取得四維向量 */
		public Vector4? GetVector4 (string key) {
			object obj = this.Get(key);
			if (obj == null) return null;
			return DictSO.Vector4(obj);
		}
		/** 取得四維向量 */
		public bool TryGetVector4 (string key, Action<Vector4> res) {
			if (this.ContainsKey(key) == false) return false;
			Vector4? vec4 = this.GetVector4(key);
			if (vec4 == null) return false;
			res((Vector4)vec4);
			return true;
		}

		/** 取得矩形偏移 */
		public RectOffset GetRectOffset (string key) {
			object obj = this.Get(key);
			if (obj == null) return null;
			return DictSO.RectOffset(obj);
		}
		/** 取得四維向量 */
		public bool TryGetRectOffset (string key, Action<RectOffset> res) {
			if (this.ContainsKey(key) == false) return false;
			RectOffset rectOffset = this.GetRectOffset(key);
			if (rectOffset == null) return false;
			res(rectOffset);
			return true;
		}


		/** 取得顏色 */
		public Color GetColor (string key) {
			object obj = this.Get(key);
			if (obj == null) return default(Color);
			return DictSO.Color(obj);
		}
		/** 取得四維向量 */
		public bool TryGetColor (string key, Action<Color> res) {
			if (this.ContainsKey(key) == false) return false;
			res(this.GetColor(key));
			return true;
		}

		/** 取得DictSO */
		public DictSO GetDictSO (string key) {
			object obj = this.Get(key);
			if (obj == null) return null;
			return DictSO.Json(obj);
		}
		/** 取得DictSO */
		public bool TryGetDictSO (string key, Action<DictSO> res) {
			if (this.ContainsKey(key) == false) return false;
			res(this.GetDictSO(key));
			return true;
		}

		/** 取得List */
		public List<object> GetList (string key) {
			object obj = this.Get(key);
			if (obj == null) return null;
			return DictSO.List(obj);
		}
		/** 取得List */
		public bool TryGetList (string key, Action<List<object>> res) {
			if (this.ContainsKey(key) == false) return false;
			res(this.GetList(key));
			return true;
		}

		/** 取得List */
		public List<T> GetList<T> (string key) {
			object obj = this.Get(key);
			if (obj == null) return null;
			return DictSO.List<T>(obj);
		}
		/** 取得List */
		public bool TryGetList<T> (string key, Action<List<T>> res) {
			if (this.ContainsKey(key) == false) return false;
			res(this.GetList<T>(key));
			return true;
		}


		/** 取得副本 */
		// TODO : 待優化
		public DictSO GetCopy () {
			DictSO newOne = DictSO.New();
			foreach (KeyValuePair<string, object> pair in this) {
				if (pair.Value is DictSO) {
					newOne.Add(pair.Key, (pair.Value as DictSO).GetCopy());
				} else {
					newOne.Add(pair.Key, pair.Value);
				}
			}
			return newOne;
		}


		/** 取得成員 */
		public T GetVal<T> (string key) {
			try {
				return (T) this[key];
			}
			catch (Exception e) {
				throw e;
				// return default (T);
			}
		}

		/*==其他用途===========================*/

		/** Debug用，列出所有成員 */
		public void LogMembers () {
			foreach (KeyValuePair<string, object> eachPair in this) {
				Debug.Log(eachPair.Key + " : " + eachPair.Value);
			}
		}

		/* 取得類型 */
		public Type GetType (string key) {
			if (this.ContainsKey(key) == false) return null;
			object value = this[key];
			if (value is JValue) {
				JTokenType jType = ((JValue) value).Type;
				if (jType == JTokenType.Boolean) return typeof(bool);
				if (jType == JTokenType.String) return typeof(string);
				if (jType == JTokenType.Float) return typeof(double);
				if (jType == JTokenType.Integer) return typeof(int);
				if (jType == JTokenType.Null) return null;
			}
			return value.GetType();
		}

		/** 是否為數字 */
		public static bool IsNumeric (object o) {

			bool res = DictSO.IsNumericType(o.GetType());
			if (o is Newtonsoft.Json.Linq.JValue) {
				try {
					object val = ((Newtonsoft.Json.Linq.JValue)o).Value<double>();
					res = DictSO.IsNumericType(val.GetType());
				} catch (Exception) {}
			}
			
			res = DictSO.IsNumericType(o.GetType());

			return res;
		}
		public static bool IsNumericType (Type tp) {
			switch (Type.GetTypeCode(tp)) {
				case TypeCode.Byte:
				case TypeCode.SByte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Decimal:
				case TypeCode.Double:
				case TypeCode.Single:
					return true;
				default:
					return false;
			}
		}

		public static object GetObjFromJToken (JToken token) {
			switch (token.Type) {
				case JTokenType.None:
				case JTokenType.Null:
					return null;
				case JTokenType.String:
					return token.Value<string>();
				case JTokenType.Boolean:
					return token.Value<bool>();
				case JTokenType.Float:
					try {
						return token.Value<double>();
					} catch (Exception) {
						return DictSO.Float(token);
					}
				case JTokenType.Integer:
					try {
						return token.Value<int>();
					} catch (Exception) {
						return DictSO.Int(token);
					}
				case JTokenType.Array:
					return DictSO.List(token.Values<object>());
				case JTokenType.Object:
					return DictSO.Json(token.Value<JObject>());
			}
			return token;
		}

	}

}