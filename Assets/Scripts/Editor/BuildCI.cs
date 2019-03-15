using UnityEditor;
using UnityEditor.Build.Reporting;
using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class BuildCI {
    [MenuItem("Debugging/Print CI Built Scene")]
    public static void PrintBuildSceneInfo()
    {
        foreach (var scene in EditorBuildSettings.scenes)
        {
            Debug.Log("CI Included scene " + scene.path);
        }
    }
    
    public static void PerformBuild()
    {
        string[] args = Environment.GetCommandLineArgs(); 
        Debug.Log($"Executing build, {DateTime.Now} {args[1]}");
        List<string> scenes = new List<string>();
        
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                Debug.Log(" > Included scene " + scene.path);
                scenes.Add(scene.path);
            }
        }

        BuildReport report = BuildPipeline.BuildPlayer(new BuildPlayerOptions()
        {
            locationPathName = Path.Combine(args[1], "out.apk"),
            target = BuildTarget.Android,
            scenes = scenes.ToArray()
        });
        
        if(report.summary.result != BuildResult.Succeeded) EditorApplication.Exit(1);
    }
}
