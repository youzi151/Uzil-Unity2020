using Uzil;

namespace UZAPI {

public class Flags {

	/** 取得 Flags */
	public static string GetFlags_str () {
		return DictSO.ToJson(Flags.flags);
	}

	
	/** 定義 Flags */
	public static readonly string[] flags = new string[]{

#if STEAM
		"steam",
#endif

		"moddable"

	};


}

}