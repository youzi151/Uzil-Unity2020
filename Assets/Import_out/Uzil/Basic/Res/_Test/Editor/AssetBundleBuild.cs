using UnityEditor;

using Uzil.Util;

public class AssetBundleBuild {

	[MenuItem ("Assets/Build AssetBundles")]
	static void BuildAllAssetBundles () {
		BuildPipeline.BuildAssetBundles(PathUtil.Combine(PathUtil.GetDataPath(), "Bundles"), BuildAssetBundleOptions.None, BuildTarget.StandaloneOSX);
	}

}