using UnityEditor;

public class CreateAssetBundles
{
    [MenuItem("ID Finder/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        _ = BuildPipeline.BuildAssetBundles("../../mod/shaders", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
    }
}