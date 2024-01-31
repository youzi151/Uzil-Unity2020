using UnityEngine;

using Uzil.Mod;

public class Test_ReloadMod : MonoBehaviour {
	// Start is called before the first frame update
	void Start() {
		
	}

	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.F1)) {
			// ModMgr.Inst().UnloadAll();
		} else if (Input.GetKeyDown(KeyCode.F2)) {
			ModMgr.Inst().LogRes();
		} else if (Input.GetKeyDown(KeyCode.F3)) {
			ModMgr.Inst().ReloadAll();
		}
	}
}
