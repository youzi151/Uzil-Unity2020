using System;

using UnityEngine;

using Uzil;

namespace Uzil.Anim {

public class PropKeyframe : IMemoable {

	/** 影格 */
	public int frame = 0;

	/** 屬性名稱 */
	public string propName = null;

	/** 值 */
	public object value = null;
	private float? valTemp_float = null;

	/** 緩入 */
	public object easeIn = Vector2.zero;
	
	/** 緩出 */
	public object easeOut = Vector2.zero;


	/** [IMemoable] 紀錄為Json格式 */
	public object ToMemo () {
		DictSO data = DictSO.New();

		/** 影格 */
		data.Set("f", this.frame);

		PropHandler propHandler = PropHandler.GetHandler(this.propName);
		if (propHandler == null) {
			propHandler = PropHandler.defaultHandler;
		}

		/** 數值 */
		data.Set("v", propHandler.ValueTo(this.value));

		/** 緩入 */
		data.Set("i", propHandler.EaseInTo(this.easeIn));

		/** 緩出 */
		data.Set("o", propHandler.EaseOutTo(this.easeOut));

		return data;
	}

	/** [IMemoable] 讀取Json格式 */
	public void LoadMemo (object memoJson) {
		DictSO data = DictSO.Json(memoJson);

		/** 影格 */
		if (data.ContainsKey("f")) {
			this.frame = data.GetInt("f");
		}

		PropHandler propHandler = PropHandler.GetHandler(this.propName);
		if (propHandler == null) {
			propHandler = PropHandler.defaultHandler;
		}

		/** 數值 */
		if (data.ContainsKey("v")) {
			this.value = propHandler.ValueFrom(data.Get("v"));
		}

		/** 緩入 */
		if (data.ContainsKey("i")) {
			this.easeIn = propHandler.EaseInFrom(data.Get("i"));
		}

		/** 緩出 */
		if (data.ContainsKey("o")) {
			this.easeOut = propHandler.EaseOutFrom(data.Get("o"));
		}

	}

	public float GetVal<Float> () {
		if (this.valTemp_float == null) {
			this.valTemp_float = Convert.ToSingle(this.value);
		}
		return (float) this.valTemp_float;
	}

}

}