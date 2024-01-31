
namespace Uzil.RNG {


/** 
 * 加權隨機
 * 因為每個選項有特定權重，所以為有限選擇，而只提供序號index即可，而非浮點數。
 */


public class RateRandom {
	public static int Get (float[] rate, string seed = null) {
		float total = 0;

		// 全部加總
		for (int i = 0; i < rate.Length; i++){
			total += rate[i];
		}
		
		// 取得隨機數
		System.Random random;
		if (seed != null) {
			random = new System.Random(seed.GetHashCode());
		} else {
			random = new System.Random();
		}
		
		float t = total * (float) random.NextDouble();

		// 隨機數 依序 減 每一個比例
		for (int i = 0; i < rate.Length; i++){

			t = t - rate[i];

			//若扣除後已為零，則回傳該序號的Value
			if (t < 0){
				return i;
			}
		}
		return 0;
	}

}


}