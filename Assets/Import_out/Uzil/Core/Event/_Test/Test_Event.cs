using UnityEngine;

namespace Uzil.Test {

public class Test_Event : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Start () {
		
		/*== 單獨使用 ================*/

		// 建立 Event
		Event evt = new Event();
		
		// 註冊 偵聽者 (順序:2)
		evt.AddListener(new EventListener(()=>{
			Debug.Log("2");
		}).Sort(2));

		// 註冊 偵聽者 (順序:1)
		evt.AddListener(new EventListener(()=>{
			Debug.Log("1");
		}).Sort(1));

		// 2秒後 呼叫該事件
		Invoker.main.Once(()=>{
			evt.Call();
		}, 2);

		/*== EvetBus(全域)使用 =======*/

		// 取得 域"test"的EventBus
		EventBus bus = EventBus.Inst("test");

		// 註冊 "onCall" 事件 之 偵聽者 (ID: "test2", 順序:2)
		bus.On("onCall", new EventListener(()=>{
			Debug.Log("onCall second");
		}).ID("test2").Sort(2));

		// 註冊 "onCall" 事件 之 偵聽者 (ID: "test1")
		bus.On("onCall", new EventListener(()=>{
			Debug.Log("onCall");
		}).ID("test1"));

		// 註冊 "onCall" 事件 之 偵聽者 (ID: "test1", 順序:1)
		// 同ID取代
		bus.On("onCall", new EventListener(()=>{
			Debug.Log("onCall first");
		}).ID("test1").Sort(1));

		// 呼叫 "onCall" 事件
		bus.Post("onCall");

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
