using UnityEditor;
using System;

public class BuildCI {
    public static void PerformBuild()
    {
        string[] args = Environment.GetCommandLineArgs(); 
        string[] scenes = { "Assets/Scenes/SampleScene.unity" };
        BuildPipeline.BuildPlayer(new BuildPlayerOptions()
        {
            locationPathName = args[0] + "/out.apk",
            target = BuildTarget.Android,
            scenes = scenes,
            options = BuildOptions.AcceptExternalModificationsToPlayer
        });
    }
}
