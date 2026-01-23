using System.IO;
using UnityEditor;

public class CreateAssetBundles
{
    [MenuItem("ID Finder/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        _ = BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        File.Copy("Assets/AssetBundles/idfinder", "../../mod/shaders/idfinder", true);
    }
}