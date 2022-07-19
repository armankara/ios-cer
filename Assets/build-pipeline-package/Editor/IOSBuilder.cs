using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;
using System; 
using System.Linq;

// Output the build size or a failure depending on BuildPlayer.

public class IOSBuilder : MonoBehaviour
{
    private static string[] args;
    private static BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

    [MenuItem("Azerion/Build iOS Development")]
    private static void Build()
    {
        // Place all your scenes here
		//Build Ios
        string[] scenes = new string[EditorBuildSettings.scenes.Length];

        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }

        buildPlayerOptions.scenes = scenes;
        buildPlayerOptions.target = BuildTarget.iOS;
        buildPlayerOptions.options = BuildOptions.None;

        buildPlayerOptions.locationPathName = "builds/ios";
        EditorUserBuildSettings.development = false;
        EditorUserBuildSettings.allowDebugging = false;

        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
            EditorApplication.Exit(0);
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build failed");
            EditorApplication.Exit(1);
        }
    }
    public static void BuildIOS()
    {
        Build();
    }

    // Helper function for getting the command line arguments
    public static string GetArg(string name)
    {
        args = System.Environment.GetCommandLineArgs();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].Contains(name) && args.Length > i + 1)
            {
                return args[i + 1];
            }
        }
        return null;
    }
}
