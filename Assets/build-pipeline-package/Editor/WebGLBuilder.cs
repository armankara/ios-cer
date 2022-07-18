using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class WebGLBuilder : MonoBehaviour {

    [MenuItem("Azerion/Build WebGL Development")]
    public static void Build()
    {   
        // Place all your scenes here
        string[] scenes = new string[EditorBuildSettings.scenes.Length];

        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }

        string pathToDeploy = "builds/webgl";

        PlayerSettings.SplashScreen.showUnityLogo = false;

        BuildReport report = BuildPipeline.BuildPlayer(scenes, pathToDeploy, BuildTarget.WebGL, BuildOptions.None);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
            EditorApplication.Exit(0);
        }
        else if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
            EditorApplication.Exit(1);
        }
    }
}
