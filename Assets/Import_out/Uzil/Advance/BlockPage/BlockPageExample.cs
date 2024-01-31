using UnityEngine;

using Uzil;

namespace Uzil.BlockPage.Test {

public class BlockPageExample : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	public BlockPageObj page = null;

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Start () {
		
		BlockObj empty1 = BlockPageUtil.CreateBlock("Empty");
		this.page.AddBlock("empty", empty1);
		empty1.SetData(DictSO.Json("{\"height\":250}").Set("background", "BlockPage/empty.jpg"));

		BlockObj btn = BlockPageUtil.CreateBlock("Button");
		this.page.AddBlock("btn", btn);
		btn.SetData(DictSO.New().Set("onClick", "print(\"click\")"));

		BlockObj img = BlockPageUtil.CreateBlock("Image");
		this.page.AddBlock("img", img);
		img.SetData(DictSO.Json("{\"height\":250}").Set("onPointerEnter", "print(\"enter\")").Set("onPointerExit", "print(\"exit\")"));

		BlockObj empty2 = BlockPageUtil.CreateBlock("Empty");
		this.page.AddBlock("empty2", empty2);
		empty2.SetData(DictSO.Json("{\"fillHeight\":500}"));

		BlockObj title = BlockPageUtil.CreateBlock("Text");
		this.page.AddBlock("title", title);
		title.SetData(DictSO.Json("{\"text\":\"Test\"}"));

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
