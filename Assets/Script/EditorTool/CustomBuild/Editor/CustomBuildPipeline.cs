using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

public class CustomBuildPipeline : EditorWindow
{
    private const string CustomBuildDataPath = "Assets/BuildConfig/Editor/CustomBuildData.json";
    private const string CustomBuildConfigAssetsPath = "Assets/BuildConfig/Editor/CustomBuildConfigAssets.asset";

    public class CustomBuildData
    {
        public KeystoreManager.KeystoreConfig keystore;
    }

    public static CustomBuildConfigAssets BuildConfigAssets { get; private set; }
    public static CustomBuildData Config { get; private set; }

    public static GUIStyle MatchStyle { get; private set; } = new GUIStyle();
    public static GUIStyle NotMatchStyle { get; private set; } = new GUIStyle();
    public static GUIStyle UnableToChangeStyle { get; private set; } = new GUIStyle();

    private static BuildTarget[] supportBuildTarget = new[] { BuildTarget.Android, BuildTarget.iOS, BuildTarget.StandaloneWindows64 };
    private static BuildTargetGroup[] supportBuildTargetGroup = new[] { BuildTargetGroup.Android, BuildTargetGroup.iOS, BuildTargetGroup.Standalone };

    private Vector2 scrollPosition;

    [MenuItem("Build/Custom Build Pipeline")]
    private static void BuildMenu()
    {
        LoadBuildConfigAssets();
        LoadConfig();

        GetWindow<CustomBuildPipeline>();
    }

    private void OnGUI()
    {
        SetGUIStyle();
        ValidateConfig();

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        GUILayout.Space(10);
        switch (EditorUserBuildSettings.activeBuildTarget)
        {
            case BuildTarget.StandaloneWindows64:
                WindowCustomBuildPipeline.BuildMenu();
                break;
            case BuildTarget.Android:
                AndroidCustomBuildPipeline.BuildMenu();
                break;
            case BuildTarget.iOS:
                iOSCustomBuildPipeline.BuildMenu();
                break;
            default:
                GUILayout.Space(50);
                GUILayout.Label($"Build Target Not Supported: {EditorUserBuildSettings.activeBuildTarget}", NotMatchStyle);
                GUILayout.Space(50);

                GUILayout.EndScrollView();
                return;
        }

        GUILayout.Space(10);
        HorizontalLine();
        GUILayout.Space(10);

        SupportBuildTargetMenu();

        GUILayout.Space(10);
        HorizontalLine();
        GUILayout.Space(10);

        LoadedConfig();

        GUILayout.Space(10);
        HorizontalLine();
        GUILayout.Space(10);

        if (GUILayout.Button("Open Build Folder", GUILayout.Height(30)))
        {
            BuildPlatformMenu.OpenBuildFolder();
        }

        GUILayout.EndScrollView();
    }

    private static void LoadBuildConfigAssets()
    {
        BuildConfigAssets = AssetDatabase.LoadAssetAtPath<CustomBuildConfigAssets>(CustomBuildConfigAssetsPath);
    }

    private static void LoadConfig()
    {
        Log.Call();

        if (File.Exists(CustomBuildDataPath))
        {
            BuildConfigAssets.customBuildConfigData = File.ReadAllText(CustomBuildDataPath);
            Config = JsonConvert.DeserializeObject<CustomBuildData>(BuildConfigAssets.customBuildConfigData);

            EditorUtility.SetDirty(BuildConfigAssets);  // Save changes
        }
        else
        {
            Debug.LogError($"Config file not found: {CustomBuildDataPath}");
        }

        // EditorUtility.SetDirty();
    }

    private void ValidateConfig()
    {
        if (Config == null)
        {
            if (BuildConfigAssets == null)
                LoadBuildConfigAssets();

            Config = JsonConvert.DeserializeObject<CustomBuildData>(BuildConfigAssets.customBuildConfigData);
        }
    }

    private static void SupportBuildTargetMenu()
    {
        GUILayout.Label($"Build Support", EditorStyles.boldLabel);
        GUILayout.Space(5);

        var supportBuildTargetList = new List<string>();
        for (var i = 0; i < supportBuildTarget.Length; i++)
        {
            var buildTarget = supportBuildTarget[i];
            supportBuildTargetList.Add($"{buildTarget}");
        }

        var currentBuildTargetIndex = Array.FindIndex(supportBuildTarget, x => x == EditorUserBuildSettings.activeBuildTarget);
        var buildTargetIndex = GUILayout.Toolbar(currentBuildTargetIndex, supportBuildTargetList.ToArray());

        if (buildTargetIndex >= 0)
        {
            var selectTargetGroup = supportBuildTargetGroup[buildTargetIndex];
            var selectBuildTarget = supportBuildTarget[buildTargetIndex];

            if (!EditorUserBuildSettings.SwitchActiveBuildTarget(selectTargetGroup, selectBuildTarget))
                throw new Exception("Failed to switch build target");
        }
    }

    private static void LoadedConfig()
    {
        GUILayout.Label("Loaded Config", EditorStyles.boldLabel);
        GUILayout.Space(5);
        BuildConfigAssets = (CustomBuildConfigAssets)EditorGUILayout.ObjectField("Build Config Assets", BuildConfigAssets, typeof(CustomBuildConfigAssets), false);
    }

    [MenuItem("Build/Run - Force Unity To Recompile")]
    private static void ForceUnityToRecompile()
    {
        Debug.Log($"Force Unity To Recompile...");

        AssetDatabase.Refresh(ImportAssetOptions.ImportRecursive);
        UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
    }

    [MenuItem("Build/Go to - Main.unity")]
    private static void GoToMainScene()
    {
        Debug.Log($"Go to - Main.unity");

        EditorSceneManager.OpenScene("Assets/Resources/Scenes/Main.unity");
    }

    public static string[] GetScenes()
    {
        List<string> scenes = new List<string>();
        foreach (var scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
                scenes.Add(scene.path);
        }
        return scenes.ToArray();
    }

    public static void LogBuildResult(BuildSummary summary)
    {
        if (summary.result != BuildResult.Succeeded)
        {
            Debug.Log(
                $"{Environment.NewLine}" +
                $"xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx{Environment.NewLine}" +
                $"x      !! Build Failed !!       x{Environment.NewLine}" +
                $"xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx{Environment.NewLine}" +
                $"{Environment.NewLine}" +
                $"Duration: {summary.totalTime}{Environment.NewLine}" +
                $"Warnings: {summary.totalWarnings}{Environment.NewLine}" +
                $"Errors: {summary.totalErrors}{Environment.NewLine}" +
                $"Size: {summary.totalSize} bytes{Environment.NewLine}" +
                $"{Environment.NewLine}");
            return;
        }
        // This format is required by the game-ci build action
        Debug.Log(
                $"{Environment.NewLine}" +
                $"###########################{Environment.NewLine}" +
                $"#      Build Success!     #{Environment.NewLine}" +
                $"###########################{Environment.NewLine}" +
                $"{Environment.NewLine}" +
                $"Duration: {summary.totalTime}{Environment.NewLine}" +
                $"Warnings: {summary.totalWarnings}{Environment.NewLine}" +
                $"Errors: {summary.totalErrors}{Environment.NewLine}" +
                $"Size: {summary.totalSize} bytes{Environment.NewLine}" +
                $"{Environment.NewLine}"
        );
    }

    private static void SetGUIStyle()
    {
        ColorUtility.TryParseHtmlString("#ff3d6e", out Color red);
        ColorUtility.TryParseHtmlString("#3dff84", out Color green);
        ColorUtility.TryParseHtmlString("#ffb83d", out Color yellow);
        ColorUtility.TryParseHtmlString("#354fa6", out Color blue);
        MatchStyle.normal.textColor = green;
        MatchStyle.margin.left = 5;
        NotMatchStyle.normal.textColor = red;
        NotMatchStyle.margin.left = 5;
        UnableToChangeStyle.normal.textColor = yellow;
        UnableToChangeStyle.margin.left = 5;
    }

    public static void HorizontalLine()
    {
        Rect rect = EditorGUILayout.GetControlRect(false, 1);
        rect.height = 1;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
    }
}