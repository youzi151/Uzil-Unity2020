namespace Uzil.Res {

/* 錯誤：找不到相關資源 */
public class ResNotFoundException : System.Exception {

	private ResReq req;

	public ResNotFoundException (ResReq req) {
		this.req = req;
	}

	public override string Message {
        get {
			return string.Format("ResNotFoundException: not found res: {0}", this.req.path);
        }
    }
}


/* 錯誤：資源類型不支援 */
public class ResTypeNotSupportException : System.Exception {

	System.Type type;
	
	public ResTypeNotSupportException (System.Type type) {
		this.type = type;
	}

	public override string Message {
        get {
			return string.Format("ResTypeNotSupportException: not support res type: {0}.", this.type.ToString());
        }
    }
}


}