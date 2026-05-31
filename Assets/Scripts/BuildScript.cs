using UnityEditor;
using UnityEngine;

public class BuildScript
{
    [MenuItem("Build/Release Build")]
    public static void BuildGame()
    {
        string path = "Builds/Release/AetherShift.exe";

        BuildPipeline.BuildPlayer(
            EditorBuildSettings.scenes,
            path,
            BuildTarget.StandaloneWindows64,
            BuildOptions.None
        );

        Debug.Log("Build finished!");
    }
}