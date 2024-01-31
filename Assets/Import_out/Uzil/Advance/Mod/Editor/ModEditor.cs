using UnityEditor;
using UnityEngine;

namespace Uzil.Mod {

public class ModEditor {

	[MenuItem ("Uzil/Mod/Log Zip Password")]
	protected static void logZipPassword () {
		Debug.Log(ModUtil.GetZipPassword());
	}

}

}