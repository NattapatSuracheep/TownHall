using UnityEditor;
using UnityEngine;

public class WindowCustomBuildPipeline : BuildPlatformMenu
{
    public static void BuildMenu()
    {
        GUILayout.Label("Window Build Setting", EditorStyles.boldLabel);
    }
}