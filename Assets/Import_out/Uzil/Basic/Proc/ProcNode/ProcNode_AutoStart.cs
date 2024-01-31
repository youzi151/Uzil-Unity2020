namespace Uzil.Proc {

public class ProcNode_AutoStart : ProcNode {


	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/
	
	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Start () {
		// this.Begin(); // 改由ProcMgr.StartFirstNode()呼叫
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	/*========================================Interface==========================================*/


	/* 開始此節點進程，通常受上一節點呼叫 */
	public override void Begin () {
		ProcMgr procMgr = this.getMgr();

		foreach (string eachID in this.nextNodeList){
			ProcNode node = procMgr.GetNode(eachID);
			node.Begin();
		}

		this.state = ProcNodeState.Complete;
	}


	/* 結束此節點進程 */
	public override void End () {
		
	}

	
	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/

}


}