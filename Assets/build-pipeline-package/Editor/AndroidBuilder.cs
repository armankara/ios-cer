using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;
using System.Text.RegularExpressions;

// Output the build size or a failure depending on BuildPlayer.


public class AndroidBuilder : MonoBehaviour
{
    private static string[] args;
    private static BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

    [MenuItem("Azerion/Build Android Development")]
    private static void Build()
    {
        // Place all your scenes here
        string[] scenes = new string[EditorBuildSettings.scenes.Length];

        for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }

        
        
        
        buildPlayerOptions.scenes = scenes;

        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.None;

        PlayerSettings.Android.keystoreName = "user.keystore";
        PlayerSettings.Android.keystorePass = "armankara";
        PlayerSettings.Android.keyaliasName = "release";
        PlayerSettings.Android.keyaliasPass = "armankara";
       // PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevelAuto;
        PlayerSettings.Android.bundleVersionCode = 1;

        if (GetArg("branchName") != null)
        {
            string regexString = FilterBranchVersion(GetArg("branchName"));
            string trimmedString = regexString.Replace("release/v", "");

            PlayerSettings.bundleVersion = trimmedString;
        }
        else
        {
            PlayerSettings.bundleVersion = GetArg("buildVersion");
        }

        EditorUserBuildSettings.development = false;
        EditorUserBuildSettings.allowDebugging = false;
       // PlayerSettings.Android.useCustomKeystore = true;
        

        //BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildReport report = BuildPipeline.BuildPlayer(scenes, "builds/android", BuildTarget.Android, BuildOptions.None);
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

    public static void BuildAPK()
    {
        buildPlayerOptions.locationPathName = "builds/android/apk-" + GetArg("buildVersion");
        Build();
    }

    public static void BuildAAB()
    {
        EditorUserBuildSettings.buildAppBundle = true;
        EditorUserBuildSettings.androidCreateSymbolsZip = true;
        buildPlayerOptions.locationPathName = "builds/android/appBundle-" + GetArg("buildVersion");
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

    private static string FilterBranchVersion(string branchName)
    {
        string newBranchName = string.Empty;
        MatchCollection matches = Regex.Matches(branchName, @"([0-9]+\.[0-9]+\.[0-9]+)");

        foreach (Match match in matches)
        {
            newBranchName += match.Value;
        }
        return newBranchName;
    }
}
