using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

[InitializeOnLoad]
public class BuildVersionIncrement {
    [PostProcessBuild(0)]
    public static void OnPostProcessBuild()
    {
        IncrementVersion();
    }

    public static void IncrementVersion()
    {
        string version = PlayerSettings.bundleVersion;
        string[] parts = version.Split('.');
        int lastNum = int.Parse(parts[2]);
        lastNum++;
        string newVersion = string.Join(".", parts[0], parts[1]);
        newVersion += ("." + lastNum);
        Debug.Log("Built version " + version + ", next version is " + newVersion + "\nVersion code " + ++PlayerSettings.Android.bundleVersionCode);
        PlayerSettings.bundleVersion = newVersion;
        
    }

    [MenuItem("Tools/Versioning/Increment Major")]
    public static void IncrementMajor()
    {
        string version = PlayerSettings.bundleVersion;
        string[] parts = version.Split('.');
        parts[0]= (int.Parse(parts[0]) + 1).ToString();
        parts[1] = "0";
        parts[2] = "1";
        string newVersion = string.Join(".", parts);
        Debug.Log("Version changed from " + version + " to " + newVersion + "\nVersion code " + ++PlayerSettings.Android.bundleVersionCode);
        PlayerSettings.bundleVersion = newVersion;

    }

    [MenuItem("Tools/Versioning/Increment Minor")]
    public static void IncrementMinor()
    {
        string version = PlayerSettings.bundleVersion;
        string[] parts = version.Split('.');
        parts[1] = (int.Parse(parts[1]) + 1).ToString();
        parts[2] = "1";
        string newVersion = string.Join(".", parts);
        Debug.Log("Version changed from " + version + " to " + newVersion + "\nVersion code " + ++PlayerSettings.Android.bundleVersionCode);
        PlayerSettings.bundleVersion = newVersion;

    }
}
