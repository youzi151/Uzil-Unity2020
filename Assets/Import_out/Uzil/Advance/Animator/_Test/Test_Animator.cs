using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Uzil.Mod;
using Uzil.Anim;
using Uzil.Util;

namespace Uzil.Test {

public class Test_Animator : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	/*=====================================Static Funciton=======================================*/

	/*=========================================Members===========================================*/

	public string targetName = "";

	private Animator_Custom animator;

	/*========================================Components=========================================*/

	public TextAsset animatorJson;

	public List<GameObject> targetList = new List<GameObject>();

	private List<PropTarget_Comm> propTargets = new List<PropTarget_Comm>();

	/*==========================================Event============================================*/

	/*======================================Unity Function=======================================*/

	// Use this for initialization
	void Start () {
	

		ModMgr modMgr = ModMgr.Inst();
		modMgr.AddBaseLoader();
		modMgr.ReloadAll();

		this.animator = new Animator_Custom();

		this.animator.Load(DictSO.Json(this.animatorJson.text));

		Debug.Log(((DictSO) this.animator.ToMemo()).ToJson());

		Vector2[] points = new Vector2[]{
			new Vector2(0, 0), 
			new Vector2(0, 2),
			new Vector2(2, 2),
			new Vector2(2, 0)
		};

		// 建立 目標列表
		foreach (GameObject each in this.targetList) {
			PropTarget_Comm propTarget = new PropTarget_Comm();
			propTarget.transform = each.transform;
			this.propTargets.Add(propTarget);

			MeshFilter meshFilter = each.GetComponent<MeshFilter>();
			meshFilter.mesh = MeshUtil.SpriteMesh(points, points);

			propTarget.meshFilter = meshFilter;
			propTarget.renderers.Add(each.GetComponent<MeshRenderer>());
		}
		

		// 設置 動畫狀態機 取得目標方式
		this.animator.getTarget = (targetStr)=>{
			foreach (PropTarget_Comm each in this.propTargets) {
				if (each.transform.gameObject.name == targetStr) {
					return each;
				}
			}
			return null;
		};

		this.animator.Init();

		// 過一段時間後 開啟 動畫狀態機變數:移動
		Invoker.Inst().Once(()=>{
			this.animator.Param("isMove", true);
			Invoker.Inst().Once(()=>{
				Debug.Log(this.animator.layers[0].currentState.id);
			});
		}, 2);

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void LateUpdate() {
		this.animator.Update(Time.smoothDeltaTime);
	}

	
	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}