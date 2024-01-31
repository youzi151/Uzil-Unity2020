using System.Data.SqlTypes;

namespace Uzil.Res {


/* 資源回傳結果 定義 */
public class ResResult {
	public virtual bool IsValueExist () { return false;}
}


/* 資源回傳結果 定義 */
public class ResResult<T> : ResResult {

	public T value;
	public void SetValue (T value) { this.value = value; }

	public override bool IsValueExist () { return value != null;}

	public ResResult (T value) {
		this.value = value;
	}

}

}