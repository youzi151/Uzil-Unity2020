using System.Collections.Generic;

using Uzil;
using Uzil.Misc;

namespace Uzil.Anim {


public class AnimClip_Custom : AnimClip {

	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 屬性軌 */
	public List<PropTrack> tracks = new List<PropTrack>();

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	/*========================================Interface==========================================*/

	/** [IMemoable] 紀錄為Json格式 */
	public override object ToMemo () {
		DictSO data = (DictSO) base.ToMemo();

		/* 軌 */
		List<DictSO> propTracks = new List<DictSO>();
		for (int idx = 0; idx < this.tracks.Count; idx++) {
			PropTrack track = this.tracks[idx];
			propTracks.Add((DictSO) track.ToMemo());
		}
		data.Set("tracks", propTracks);

		return data;
	}

	/** [IMemoable] 讀取Json格式 */
	public override void LoadMemo (object memoJson) {
		DictSO data = DictSO.Json(memoJson);
		if (data == null) return;

		base.LoadMemo(data);

		if (data.ContainsKey("tracks")) {
			this.tracks.Clear();
			List<DictSO> propTracks = data.GetList<DictSO>("tracks");
			
			for (int idx = 0; idx < propTracks.Count; idx++) {
				
				DictSO trackData = propTracks[idx];

				this.tracks.Add(new PropTrack(trackData));
				
			}
		}

	}

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/

	/*====================================Private Function=======================================*/


}

}