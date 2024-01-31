using System.Collections.Generic;

using Uzil;

namespace Uzil.Anim {

public class PropTrack : IMemoable {

	/*======================================Constructor==========================================*/

	public PropTrack () {}
	public PropTrack (DictSO data) {
		this.LoadMemo(data);
	}

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 設置對象 */
	public string target = null;

	/** 屬性名稱 */
	public string prop = null;

	/** 是否為附加 (否則為覆蓋) */
	public bool isAddtive = false;

	/** 關鍵幀 */
	public List<PropKeyframe> keyframes = new List<PropKeyframe>();

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

    /** [IMemoable] 紀錄為Json格式 */
	public object ToMemo () {
		DictSO data = DictSO.New();

		/** 目標 */
		data.Set("target", this.target);

		/** 屬性名稱 */
		data.Set("prop", this.prop);

		/* 是否為附加 */
		data.Set("isAddtive", this.isAddtive);

        /** 關鍵影格 */
        List<DictSO> keyframes = new List<DictSO>();
        for (int idx = 0; idx < this.keyframes.Count; idx++) {
            DictSO each = (DictSO) this.keyframes[idx].ToMemo();
            keyframes.Add(each);
        }
        data.Set("keyframes", keyframes);

		return data;
	}

	/** [IMemoable] 讀取Json格式 */
	public void LoadMemo (object memoJson) {
		DictSO data = DictSO.Json(memoJson);

		/** 目標 */
		if (data.ContainsKey("target")) {
			this.target = data.GetString("target");
		}

		/** 屬性名稱 */
		if (data.ContainsKey("prop")) {
			this.prop = data.GetString("prop");
		}

		/** 是否為附加 */
		if (data.ContainsKey("isAddtive")) {
			this.isAddtive = data.GetBool("isAddtive");
		}

        /** 關鍵影格 */
		if (data.ContainsKey("keyframes")) {
            this.keyframes.Clear();
            List<DictSO> keyframeDatas = data.GetList<DictSO>("keyframes");
			for (int idx = 0; idx < keyframeDatas.Count; idx++) {
                PropKeyframe keyframe = new PropKeyframe();
				keyframe.propName = this.prop;
                keyframe.LoadMemo(keyframeDatas[idx]);
                this.keyframes.Add(keyframe);
            }
		}

	}

	/*=====================================Public Function=======================================*/
	
	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/

}

}