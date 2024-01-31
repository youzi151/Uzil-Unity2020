using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Uzil.PageCard {

public class PageCtrl : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	/** 當前頁面 */
	private string _currentPage = "";

	/** 頁面列表 */
	public List<Page> pages  = new List<Page>();

	/** 卡片列表 */
	public List<Card> cards  = new List<Card>();

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Awake () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*== 註冊 ==============================*/

	
	/** 註冊頁面 */
	public void AddPage (Page page) {
		if (this.pages.Contains(page)) return;
		this.pages.Add(page);
	}


	/** 註冊卡片 */
	public void AddCard (Card card) {
		if (this.cards.Contains(card)) return;
		this.cards.Add(card);
		card.deactive();
	}

	/** 清除頁面 */
	public void ClearPage () {
		this.pages.Clear();
	}

	/** 清除卡片 */
	public void ClearCard () {
		this.cards.Clear();
	}

	public void Clear () {
		this.pages.Clear();
		this.cards.Clear();
	}

	/*== 取得 ==============================*/

	/** 取得頁面 */
	public List<Page> GetPages (string id) {
		List<Page> result = new List<Page>();

		foreach (Page each in this.pages) {
			if (each.id == id) {
				result.Add(each);
			}
		}

		if (result.Count == 0) {
			return null;
		} else {
			return result;
		}
	}

	/** 取得卡片 */
	public List<Card> GetCards (string id) {
		List<Card> result = new List<Card>();

		foreach (Card each in this.cards) {
			if (each.id == id) {
				result.Add(each);
			}
		}

		if (result.Count == 0) {
			return null;
		} else {
			return result;
		}
	}


	
	/*== 切換頁面 =============================*/

	public void GoPage (string pageID) {

		List<string> showCards = this._getCardIDsInPage(pageID);

		// 每一張卡片
		foreach (Card eachCard in this.cards) {

			// 若 在要顯示的卡片中 則
			if (showCards.Contains(eachCard.id)) {
				// 啟用
				eachCard.active();
			} 

			// 否則關閉
			else {
				eachCard.deactive();
			}

		}

		this._currentPage = pageID;

	}

	public void ShowPage (string pageID) {

		List<string> showCards = this._getCardIDsInPage(pageID);
		// 每一張卡片
		foreach (Card eachCard in this.cards) {

			// 若 在要顯示的卡片中 則
			if (showCards.Contains(eachCard.id)) {
				// 啟用
				eachCard.active();
			}

		}

	}

	public void HidePage (string pageID) {
	
		List<string> hideCards = this._getCardIDsInPage(pageID);

		// 每一張卡片
		foreach (Card eachCard in this.cards) {

			// 若 在要顯示的卡片中 則
			if (hideCards.Contains(eachCard.id)) {
				// 啟用
				eachCard.deactive();
			}

		}
	}


	/*== 啟用卡片 =============================*/


	/**
	 * 啟用
	 * @param cardID 卡片名稱
	 * @param isForceReactive 若已經啟用，是否強制重新啟用
	 */
	public void activeCard (string cardID, bool isForceReactive = false) {
		List<Card> matchCards = this.GetCards(cardID);
		foreach (Card each in matchCards) {
			each.active(isForceReactive);
		}
	}

	/**
	 * 關閉
	 * @param cardID 卡片名稱
	 * @param isForceReDeactive 若已經關閉，是否強制重新關閉
	 */
	public void deactiveCard (string cardID, bool isForceReDeactive = false) {
		List<Card> matchCards = this.GetCards(cardID);
		foreach (Card each in matchCards) {
			each.deactive(isForceReDeactive);
		}
	}
	
	/*== Protected Function =======================================*/

	/*== Private Function =========================================*/

	private List<string> _getCardIDsInPage (string pageID) {
		List<string> cards = new List<string>();

		// 每一個頁面
		foreach (Page eachPage in this.pages) {

			// 若非指定頁面 則 忽略
			if (eachPage.id != pageID) continue;

			// 該頁面的每一張卡
			foreach (string eachCard in eachPage.cards) {

				// 不重複的加入 回傳卡片 中
				if (cards.Contains(eachCard) == false) {
					cards.Add(eachCard);
				}

			}

		}

		return cards;
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}
