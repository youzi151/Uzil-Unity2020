using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using Uzil.Res;
using Uzil.BuiltinUtil;

namespace Uzil.ObjInfo {

public enum SizeMode {
	Fit, Custom, Native
}

/**
 * Image資訊
 * 提供設置通常Image所會用到的資訊，並且提供快捷功能將資訊應用在物件上
 */ 

public class ImageInfo : TransformInfo {

	/*======================================Constructor==========================================*/

	public ImageInfo () {

	}

	public ImageInfo (object jsonOrPath) {
		if (jsonOrPath == null) return;

		this.raw = jsonOrPath;

		DictSO data = DictSO.Json(jsonOrPath);
		
		if (data == null) {
			this.textureInfo = new TextureInfo(jsonOrPath);
			return;
		} else {
			this.LoadMemo(data);
		}

	}

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/**== 一般屬性 ===============*/

	/** 顏色 */
	public Color? color = null;

	/** UV */
	public List<Vector2> uv = null;

	/** 尺寸模式 */
	public SizeMode sizeMode = SizeMode.Custom;

    /** 尺寸 */
	public Vector3? size = null;

	/** 邊界 */
	public RectOffset border = null;


	/** 填滿 方式 */
	public Image.FillMethod? fillMethod = null;
	/** 填滿 順逆時針 */
	public bool? isFillClockwise = null;
	/** 填滿 原點 */
	public int? fillOrigin = null;
	/** 填滿 值 */
	public float? fillAmount = null;

	/**== 可指定Null ===============*/

    /** 貼圖 */
	public TextureInfo textureInfo = null;

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** [IMemoable] 紀錄為Json格式 */
	public override object ToMemo () {
		DictSO data = (DictSO) base.ToMemo();

		if (this.color != null) {
			data.Set("color", DictSO.ColorToHex((Color)this.color));
		}
	
		if (this.uv != null) {
			data.Set("uv", this.uv);
		}

		if (this.size != null) {
			if (this.sizeMode == SizeMode.Custom) {
				data.Set("size", DictSO.Vector3To((Vector3) this.size));
			} else {
				data.Set("size", DictSO.EnumTo<SizeMode>(this.sizeMode));
			}
		}

		if (this.border != null) {
			Vector4 v4 = new Vector4(this.border.left, this.border.right, this.border.top, this.border.bottom);
			data.Set("border", DictSO.Vector4To(v4));
		}

		if (this.fillMethod != null) {
			data.Set("fillMethod", DictSO.EnumTo<Image.FillMethod>((Image.FillMethod) this.fillMethod));
		}

		if (this.isFillClockwise != null) {
			data.Set("fillClockwise", (bool) this.isFillClockwise);
		}

		if (this.fillOrigin != null) {
			data.Set("fillOrigin", (int) this.fillOrigin);
		}

		if (this.fillAmount != null) {
			data.Set("fillAmount", (float) this.fillAmount);
		}

		if (this.textureInfo != null) {
			data.Set("texture", this.textureInfo.ToMemo());
		}

		return data;
	}

	/** [IMemoable] 讀取Json格式 */
	public override void LoadMemo (object memoJson) {
		DictSO data = DictSO.Json(memoJson);
		if (data == null) return;

		base.LoadMemo(data);

		if (data.ContainsKey("color")) {
			this.color = data.GetColor("color");
		}

		if (data.ContainsKey("uv")) {
			this.uv = data.GetList<Vector2>("uv");
		}

		if (data.ContainsKey("size")) {
			try {
				this.sizeMode = data.GetEnum<SizeMode>("size");
			} catch (System.ArgumentException) {
				this.sizeMode = SizeMode.Custom;
				this.size = data.GetVector3("size");	
			}
		}

		if (data.ContainsKey("border")) {
			Vector4 v4 = (Vector4) data.GetVector4("border");
			this.border = new RectOffset((int) v4.x, (int) v4.y, (int) v4.z, (int) v4.w);
		}

		if (data.ContainsKey("fillMethod")) {
			this.fillMethod = data.GetEnum<Image.FillMethod>("fillMethod");
		}

		if (data.ContainsKey("fillClockwise")) {
			this.isFillClockwise = data.GetBool("fillClockwise");
		}

		if (data.ContainsKey("fillOrigin")) {
			this.fillOrigin = data.GetInt("fillOrigin");
		}

		if (data.ContainsKey("fillAmount")) {
			this.fillAmount = data.GetFloat("fillAmount");
		}

		if (data.ContainsKey("texture")) {
			this.textureInfo = new TextureInfo(data.Get("texture"));
		}

	}

	/*=====================================Public Function=======================================*/

    /** 取得Sprite */
	public Sprite HoldSprite (GameObject gObj) {
		if (this.textureInfo == null) return null;
		if (this.textureInfo.isTextureExist == false) return null;
		
		ResReq req = new ResReq(this.textureInfo.texturePath);
		req.user = req;

		if (this.border != null) {
			req.Arg("border", this.border);
			req.isForceReload = true;
		}

		Sprite sp = null;
		ResInfo res = ResMgr.Hold<Sprite>(req);
		if (res != null) {
			sp = res.GetResult<Sprite>();
			res.ReplaceUser(req, gObj);
			ResMgr.UnholdOnDestroy(res, gObj, gObj);
		}

		return sp;
	}

    /** 取得貼圖 */
	public Texture2D HoldTexture (GameObject gObj) {
		if (this.textureInfo == null) return null;
		if (this.textureInfo.isTextureExist == false) return null;
		
		ResReq req = new ResReq(this.textureInfo.texturePath);
		req.user = req;

		Texture2D tex = null;
		ResInfo res = ResMgr.Hold<Texture2D>(req);
		if (res != null) {
			tex = res.GetResult<Texture2D>();
			res.ReplaceUser(req, gObj);
			ResMgr.UnholdOnDestroy(res, gObj, gObj);
		}
		
		return tex;
	}

    /** 應用在 */
	public virtual void ApplyOn (Image img) {
		if (img == null) return;

		RectTransform trans = img.GetComponent<RectTransform>();
		this.ApplyOn(trans);

		SetDataUtil.SetImage(img, (DictSO) this.ToMemo());

	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

}

}