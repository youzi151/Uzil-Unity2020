using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// namespace XXX.XXX {

public class PropogateEventTrigger_ToScrollView : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 是否允許拖曳 */
	public bool isDragEnable = true;
	
	/** 是否允許捲動 */
	public bool isScrollEnable = true;

	/*========================================Components=========================================*/

	private ScrollRect scrollView;

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Start () {

		this.scrollView = this.transform.GetComponentInParent<ScrollRect>();
		if (this.scrollView == null) return;

		EventTrigger trigger = this.GetComponent<EventTrigger>();

		if (this.isDragEnable) {
			EventTrigger.Entry entryBegin = new EventTrigger.Entry();
			EventTrigger.Entry entryDrag = new EventTrigger.Entry();
			EventTrigger.Entry entryEnd = new EventTrigger.Entry();
			EventTrigger.Entry entryPotential = new EventTrigger.Entry();
	
			entryBegin.eventID = EventTriggerType.BeginDrag;
			entryBegin.callback.AddListener((data) => { this.scrollView.OnBeginDrag((PointerEventData)data); });
			trigger.triggers.Add(entryBegin);
	
			entryDrag.eventID = EventTriggerType.Drag;
			entryDrag.callback.AddListener((data) => { this.scrollView.OnDrag((PointerEventData)data); });
			trigger.triggers.Add(entryDrag);
	
			entryEnd.eventID = EventTriggerType.EndDrag;
			entryEnd.callback.AddListener((data) => { this.scrollView.OnEndDrag((PointerEventData)data); });
			trigger.triggers.Add(entryEnd);
			
			entryPotential.eventID = EventTriggerType.InitializePotentialDrag;
			entryPotential.callback.AddListener((data) => { this.scrollView.OnInitializePotentialDrag((PointerEventData)data); });
			trigger.triggers.Add(entryPotential);
		}
 
 		if (this.isScrollEnable) {
			EventTrigger.Entry entryScroll = new EventTrigger.Entry();
			entryScroll.eventID = EventTriggerType.Scroll;
			entryScroll.callback.AddListener((data) => { this.scrollView.OnScroll((PointerEventData)data); });
			trigger.triggers.Add(entryScroll);
		 }

	}
	// Update is called once per frame
	void Update () {
		
	}

	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


//}
