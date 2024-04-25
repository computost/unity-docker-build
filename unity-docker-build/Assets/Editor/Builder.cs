using System;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class Builder
{
    // This function will be called from the build process
    public static void Build()
    {
        var buildPlayerOptions = new BuildPlayerOptions
        {
            scenes = new[] { "Assets/Scenes/SampleScene.unity" },
            locationPathName = $"../Build/unity-docker-build.{GetExtension()}",
            options =  BuildOptions.CleanBuildCache | BuildOptions.StrictMode,
            target = EditorUserBuildSettings.activeBuildTarget,
            subtarget = (int)EditorUserBuildSettings.standaloneBuildSubtarget,
            targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup
        };
        
        var report = BuildPipeline.BuildPlayer(buildPlayerOptions);

        if (report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Build successful - Build written to {buildPlayerOptions.locationPathName}");
        }
        else if (report.summary.result == BuildResult.Failed)
        {
            Debug.LogError($"Build failed");
        }
    }

    private static string GetExtension()
    {
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.StandaloneLinux64:
                return "x86_64";
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
                return "exe";
            default:
                throw new InvalidOperationException("unsupported build target");
        }
    }
}