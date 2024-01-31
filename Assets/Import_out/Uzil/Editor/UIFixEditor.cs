using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

namespace Uzil.BuiltinUtil {

public class UIFixEditor : EditorWindow {

	[MenuItem("Uzil/UI/Convert Transform to RectTransform", false, 0)]
	public static void ConvertTransfrom2RectTransform () {
		if (Selection.activeGameObject == null) {
			EditorUtility.DisplayDialog("Select Error", "You must select a object!", "OK");
			return;
		}

		bool isSelectedAreTransform = Selection.activeGameObject.transform.GetType() == (typeof(Transform));

		GameObject oldGObj = Selection.activeGameObject;

		int option = EditorUtility.DisplayDialogComplex("Convert Option", "chose a convert mode", "only selected", "include children", "cancel");
		switch (option) {
			case 0:
				if (isSelectedAreTransform) {
					UIFixEditor.convertTransfrom2RectTransform(oldGObj);
				}
				break;
			case 1:
				Transform oldTrans = oldGObj.transform;
				Transform[] oldTransArray = oldTrans.GetComponentsInChildren<Transform>();
				foreach (Transform each in oldTransArray) {
					UIFixEditor.convertTransfrom2RectTransform(each.gameObject, false);
				}
				break;
			case 2:
				return;
		}

		EditorUtility.DisplayDialog("msg", "Done", "OK");
	}

	private static void convertTransfrom2RectTransform (GameObject oldGObj, bool isWarning = true) {
		Transform oldTrans = oldGObj.transform;

		//只能轉換空的Transform
		foreach (Component each in oldGObj.GetComponents<Component>()) {
			if (each.GetType() != typeof(Transform)) {
				if (isWarning) {
					EditorUtility.DisplayDialog("Transform Error", "Can not convert transform that has other Components.", "OK");
				}
				return;
			}
		}


		//紀錄共通數值===========

		//大小位置
		Vector3 pos = oldTrans.position;
		Vector3 scale = oldTrans.localScale;
		Quaternion rot = oldTrans.rotation;
		int siblingIdx = oldTrans.GetSiblingIndex();

		//子物件
		List<Transform> childs = new List<Transform>();
		foreach (Transform each in oldTrans) {
			childs.Add(each);
		}


		//新物件
		GameObject newGObj = new GameObject(oldGObj.name, typeof(RectTransform));
		RectTransform newTrans = (newGObj.transform as RectTransform);
		newTrans.SetParent(oldTrans.parent);

		//轉移內容
		newTrans.position = pos;
		newTrans.localScale = scale;
		newTrans.rotation = rot;
		newTrans.sizeDelta = Vector2.zero;

		foreach (Transform each in childs) {
			each.SetParent(newTrans, false);
		}

		newTrans.SetSiblingIndex(siblingIdx);

		//移除 舊物件
		UnityEngine.Object.DestroyImmediate(oldGObj);
	}

}

}