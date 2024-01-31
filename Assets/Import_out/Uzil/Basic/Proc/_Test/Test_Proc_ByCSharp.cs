using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Uzil.Proc;

namespace Uzil.Test {

public class Test_Proc_ByCSharp : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Start () {
		
		ProcMgr pMgr = ProcMgr.Inst();

        // 初始節點
        ProcNode_AutoStart startNode = ProcFactory.CreateNode<ProcNode_AutoStart>();
        pMgr.AddNode(startNode);

        // Log節點 第一個
        ProcNode_General logNode = ProcFactory.CreateNode<ProcNode_General>();
        pMgr.AddNode(logNode);

        ProcEvent_Log logEvent = ProcFactory.CreateEvent<ProcEvent_Log>();
        logEvent.msg = "logEvent";
        pMgr.AddEvent(logEvent);
        logNode.AddEvent(logEvent.id);

        ProcGate_WaitForSeconds waitGate = ProcFactory.CreateGate<ProcGate_WaitForSeconds>();
        waitGate.timeToWait = 2f;
        pMgr.AddGate(waitGate);
        logNode.AddGate(waitGate.id);

        // Log節點 第二個
        ProcNode_General logNode2 = ProcFactory.CreateNode<ProcNode_General>();
        pMgr.AddNode(logNode2);

        ProcEvent_Log logEvent2 = ProcFactory.CreateEvent<ProcEvent_Log>();
        logEvent2.msg = "done";
        pMgr.AddEvent(logEvent2);
        logNode2.AddEvent(logEvent2.id);


        // 串聯關係
        startNode.AddNext(logNode.id);
        logNode.AddNext(logNode2.id);

        Debug.Log(logEvent.typeName);

        // 可產出 提供給 Test_Proc_ByJson使用的json
        Debug.Log(pMgr.ToMemo());

        // 開始
        pMgr.StartFirstNode();


	}
	
	// Update is called once per frame
	void Update () {
		
	}

	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}
