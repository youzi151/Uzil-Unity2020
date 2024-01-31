using System.Collections.Generic;

using UnityEngine;

using Uzil.Res;
using Uzil.Util;

namespace Uzil.InputPipe {
public class LoadInputSetting : MonoBehaviour {

	
	/*======================================Constructor==========================================*/

	/*=====================================Static Members========================================*/

	public static string KEYBINDING_CONFIG_PATH {
        get {
            return PathUtil.Combine(PathUtil.GetDataPath(), "Config/keybinding.cfg");
        }
    }

	public static bool isLoaded = false;

	/*=====================================Static Funciton=======================================*/

	public static void LoadSetting () {
		if (LoadInputSetting.isLoaded) return;
		LoadInputSetting.isLoaded = true;

		string json = ResUtil.text.Read(KEYBINDING_CONFIG_PATH);
		List<DictSO> bindings = DictSO.List<DictSO>(json);
		if (bindings == null) return;

		foreach (DictSO inst in bindings) {
			LoadInputSetting.loadInst(inst);
		}
	}

	protected static void loadInst (DictSO instData) {
		if (!instData.ContainsKey("inst") || !instData.ContainsKey("layers")) {
			return;
		}

		string instID = instData.GetString("inst");
		List<DictSO> layers = instData.GetList<DictSO>("layers");

		InputMgr inputMgr = InputMgr.Inst(instID);

		foreach (DictSO layerData in layers) {
			LoadInputSetting.loadLayer(inputMgr, layerData);
		}
	}

	protected static void loadLayer (InputMgr inst, DictSO layerData) {

		// if (!layerData.ContainsKey("layer") || !layerData.ContainsKey("pairs")) {
		// 	return;
		// }

		// string layerID = layerData.GetString("layer");
		// List<DictSO> pairs = layerData.GetList<DictSO>("pairs");

		// inst.AddLayer(layerID);

		// foreach (DictSO pair in pairs) {
		// 	if (!pair.ContainsKey("dst") || !pair.ContainsKey("src")) {
		// 		return;
		// 	}

		// 	int dst = pair.GetInt("dst");
		// 	List<int> src = pair.GetList<int>("src");

		// 	inst.AddHandler(layerID, new InputHandler_Convert(dst, src));
		// }
	}

	/*=========================================Members===========================================*/

	/*========================================Components=========================================*/

	/*==========================================Event============================================*/

	/*========================================Interface==========================================*/

	/*=====================================Public Function=======================================*/

	public void Awake () {
		LoadInputSetting.LoadSetting();
	}

	/*===================================Protected Function======================================*/
	
	/*====================================Private Function=======================================*/
}


}
