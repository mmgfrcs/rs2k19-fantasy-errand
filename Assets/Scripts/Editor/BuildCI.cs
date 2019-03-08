using UnityEditor;
using UnityEditor.Build.Reporting;
using System;
using UnityEngine;
using System.IO;

public class BuildCI {
    public static void PerformBuild()
    {
        string[] args = Environment.GetCommandLineArgs(); 
        Debug.Log($"Executing build, {DateTime.Now} {args[1]}");
        string[] scenes = { "Assets/Scenes/SampleScene.unity" };
        BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions()
        {
            locationPathName = Path.Combine(args[1], "out.apk"),
            target = BuildTarget.Android,
            scenes = scenes,
            options = BuildOptions.StrictMode
        });


        if(report.summary.result != BuildResult.Succeeded) EditorApplication.Exit(1);
    }
}
