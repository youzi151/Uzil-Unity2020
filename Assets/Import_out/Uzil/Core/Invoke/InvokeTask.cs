using System;
using System.Collections.Generic;

namespace Uzil {

/* 一般任務 */
public class InvokeTask {

	/*== 建構子 ==*/
	public InvokeTask () {}

	public InvokeTask (Action act) {
		this.SetAct(act);
	}

	public InvokeTask (Action<DictSO> act) {
		this.SetAct(act);
	}

	
	/* 識別 */
	public List<string> tags = new List<string>();

	/* 執行內容 */
	public Action<DictSO> act;

	/* 執行 */
	public void Do (DictSO data = null) {
		if (this.act == null) return;
		this.act(data);
	}

	public void SetAct (Action<DictSO> act) {
		this.act = act;
	}

	public void SetAct (Action act) {
		this.act = (data)=>{
			act();
		};
	}

	public virtual InvokeTask Tag (string tag) {
		if (this.IsTag(tag) == false) {
			this.tags.Add(tag);
		}
		return this;
	}

	public bool IsTag (string tag) {
		return this.tags.Contains(tag);
	}
}

/* 含 優先度 任務 */
public class InvokeTask_Priority : InvokeTask {
	
	/* 優先度 (越小越先) */
	public float priority = 0f;

	public InvokeTask_Priority Priority (float priority) {
		this.priority = priority;
		return this;
	}

}

/* 含 呼叫時間 任務 */
public class InvokeTask_Delay : InvokeTask {
	
	/* 呼叫次數 */
	public float callTime = 0f;

}

/* 含 呼叫時間 任務 */
public class InvokeTask_Call : InvokeTask {
	
	/* 呼叫次數 */
	public int callTimes = -1;

	/* 設置 呼叫次數 */
	public InvokeTask_Call Times (int times) {
		this.callTimes = times;
		return this;
	}

	public new InvokeTask_Call Tag (string tag) {
		base.Tag(tag);
		return this;
	}

}


}
