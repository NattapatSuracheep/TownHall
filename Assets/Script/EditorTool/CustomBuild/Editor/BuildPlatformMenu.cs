using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildPlatformMenu
{
    protected static GUIStyle MatchStyle => CustomBuildPipeline.MatchStyle;
    protected static GUIStyle NotMatchStyle => CustomBuildPipeline.NotMatchStyle;
    protected static GUIStyle WarningStyle => CustomBuildPipeline.UnableToChangeStyle;

    protected static CustomBuildConfigAssets BuildConfigAssets => CustomBuildPipeline.BuildConfigAssets;

    public static void OpenBuildFolderGUI()
    {
        if (GUILayout.Button("Open Build Folder", GUILayout.Height(30)))
        {
            OpenBuildFolder();
        }
    }

    public static void OpenBuildFolder()
    {
        OpenFolder($"{BuildConfigAssets.buildFolder}\\");
    }

    protected static void OpenFolder(string path)
    {
        if (!Directory.Exists(path))
        {
            Debug.LogError($"Build Folder Not Found on Path: {path}");
            return;
        }

        EditorUtility.RevealInFinder(path);
    }
}