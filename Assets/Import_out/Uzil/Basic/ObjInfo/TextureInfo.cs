using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Uzil;
using Uzil.Res;

namespace Uzil.ObjInfo {

public class TextureInfo : ObjInfo {

	/*======================================Constructor==========================================*/

	public TextureInfo () {

	}

	public TextureInfo (object jsonOrPath) {
		this.raw = jsonOrPath;

		DictSO data = DictSO.Json(jsonOrPath);
		
		if (data == null) {
			this.texturePath = jsonOrPath.ToString();
			return;
		} else {
			this.LoadMemo(data);
		}

	}

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 貼圖 是否存在 */
	public bool isTextureExist = false;
    /** 貼圖路徑 */
	public string texturePath {
		get { return this._texturePath; }
		set {
			this.isTextureExist = true;
			this._texturePath = value;
		}
	}
	private string _texturePath;

	/** 過濾插值模式 是否存在 */
	public bool isFilterModeExist = false;
	/** 過濾插值模式 */
	public FilterMode filterMode {
		get { return this._filterMode; }
		set {
			this.isFilterModeExist = true;
			this._filterMode = value;
		}
	}
	private FilterMode _filterMode = FilterMode.Point;


	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** [IMemoable] 紀錄為Json格式 */
	public override object ToMemo () {
		DictSO data = (DictSO) base.ToMemo();

		if (this.isTextureExist) {
			data.Set("texture", this.texturePath);
		}

		if (this.isFilterModeExist) {
			data.Set("filterMode", this.filterMode);
		}

		return data;
	}

	/** [IMemoable] 讀取Json格式 */
	public override void LoadMemo (object memoJson) {
		DictSO data = DictSO.Json(memoJson);
		if (data == null) return;

		base.LoadMemo(data);

		if (data.ContainsKey("texture")) {
			this.isTextureExist = true;
			this.texturePath = data.GetString("texture");
		}

		if (data.ContainsKey("filterMode")) {
			this.isFilterModeExist = true;
			this.filterMode = data.GetEnum<FilterMode>("filterMode");
		}

	}

	/*=====================================Public Function=======================================*/

    
    /** 取得貼圖 */
	public Texture GetTexture () {
		if (this.texturePath == "" || this.texturePath == null) return null;
		ResReq req = new ResReq(this.texturePath);

		// 若有需要 一定得建立時期 使用的參數
		// if (this.isBorderExist) {
		// 	req.Arg("border", this.border);
		// 	req.isForceReload = true;
		// }

		Texture tex = ResMgr.Get<Texture>(req);

		this.ApplyOn(tex);

		return tex;
	}

    /** 應用在 */
	public virtual void ApplyOn (Texture target) {
		if (target == null) return;

		if (this.isFilterModeExist) {
			target.filterMode = this.filterMode;
		}
	}

    /** 從實體複製 */
	public void CopyFrom (Texture target) {
		this.filterMode = target.filterMode;
	}

	/** 是否等同 */
	public bool IsEqual (TextureInfo other) {
		if (this.isFilterModeExist || other.isFilterModeExist) {
			if (this.filterMode != other.filterMode) return false;
		}
		if (this.isTextureExist || other.isTextureExist) {
			if (this.texturePath != other.texturePath) return false;
		}
		return true;
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

}

}