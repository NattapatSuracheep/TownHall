using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

public class AndroidCustomBuildPipeline : BuildPlatformMenu
{
    private static AndroidSdkVersions minimumAndroidSdkVersion = AndroidSdkVersions.AndroidApiLevel28;
    private static AndroidSdkVersions targetAndroidSdkVersion = AndroidSdkVersions.AndroidApiLevel35;
    private static AndroidArchitecture targetArchitecture = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;
    private static ApiCompatibilityLevel apiCompatibilityLevel = ApiCompatibilityLevel.NET_Standard;

    public static void BuildMenu()
    {
        GUILayout.Label("Android Build Setting", EditorStyles.boldLabel);
        GUILayout.Space(5);
        CustomBuildPipeline.HorizontalLine();
        GUILayout.Space(10);

        BuildSettingInfo();

        GUILayout.Space(10);
        CustomBuildPipeline.HorizontalLine();
        GUILayout.Space(10);

        KeystoreManager.KeystoreInfo();

        GUILayout.Space(10);
        CustomBuildPipeline.HorizontalLine();
        GUILayout.Space(10);

        ExtraBuildConfig();

        GUILayout.Space(10);
        CustomBuildPipeline.HorizontalLine();
        GUILayout.Space(10);

        if (GUILayout.Button("Update Android Build Setting - Manually", GUILayout.Height(30)))
        {
            UpdateBuildSetting();
        }
        if (GUILayout.Button("Set Build Path - Manually", GUILayout.Height(30)))
        {
            SetBuildPath();
        }
        if (GUILayout.Button("Build Android", GUILayout.Height(50)))
        {
            UpdateBuildSetting();
            SetBuildPath();
            Build();
            OpenBuildFolder();
        }
    }

    private static void BuildSettingInfo()
    {
        GUILayout.Label("Api Info", EditorStyles.boldLabel);
        GUILayout.Label("Minimum Android API: ");
        GUILayout.Label("\t" + minimumAndroidSdkVersion, PlayerSettings.Android.minSdkVersion == minimumAndroidSdkVersion ? MatchStyle : NotMatchStyle);
        GUILayout.Space(5);

        GUILayout.Label("Target Android API: ");
        GUILayout.Label("\t" + targetAndroidSdkVersion, PlayerSettings.Android.targetSdkVersion == targetAndroidSdkVersion ? MatchStyle : NotMatchStyle);
        GUILayout.Space(5);

        GUILayout.Label("Api Compatibility Level: ");
        GUILayout.Label("\t" + apiCompatibilityLevel, PlayerSettings.GetApiCompatibilityLevel(NamedBuildTarget.Android) == apiCompatibilityLevel ? MatchStyle : NotMatchStyle);
        GUILayout.Space(5);

        GUILayout.Label("Target Architectures: ");
        GUILayout.Label("\t" + targetArchitecture, PlayerSettings.Android.targetArchitectures == targetArchitecture ? MatchStyle : NotMatchStyle);
        GUILayout.Space(5);

        GUILayout.Label("If something is red that mean you setting is not match with android build setting, click update button and if should be fixed");
    }

    private static void ExtraBuildConfig()
    {
        GUILayout.Label("Extra Build Config", EditorStyles.boldLabel);
        GUILayout.Space(5);
        BuildConfigAssets.isDevBuild = GUILayout.Toggle(BuildConfigAssets.isDevBuild, "is Dev Build");
        GUILayout.Space(5);
        BuildConfigAssets.isGoogleStoreBuild = GUILayout.Toggle(BuildConfigAssets.isGoogleStoreBuild, "is Google Store Build");

        EditorUtility.SetDirty(BuildConfigAssets);
    }

    private static void UpdateBuildSetting()
    {
        Log.Call();

        PlayerSettings.SplashScreen.show = true;
        PlayerSettings.SplashScreen.showUnityLogo = false;

        PlayerSettings.Android.minSdkVersion = minimumAndroidSdkVersion;
        PlayerSettings.Android.targetSdkVersion = targetAndroidSdkVersion;
        PlayerSettings.Android.targetArchitectures = targetArchitecture;
        PlayerSettings.SetApiCompatibilityLevel(NamedBuildTarget.Android, apiCompatibilityLevel);

        KeystoreManager.UpdateKeystore();
    }

    private static void SetBuildPath()
    {
        Log.Call();

        BuildConfigAssets.buildName = BuildConfigAssets.isDevBuild == true ? $"android_v.{BuildConfigAssets.buildVersion}_devBuild" : $"android_v.{BuildConfigAssets.buildVersion}";
        BuildConfigAssets.buildVersion = Application.version;
        BuildConfigAssets.buildFolder = "Builds\\Android";
        BuildConfigAssets.buildNumber = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm");
        BuildConfigAssets.fileType = BuildConfigAssets.isGoogleStoreBuild == true ? "aab" : "apk";
        BuildConfigAssets.buildPath = $"{BuildConfigAssets.buildFolder}\\{BuildConfigAssets.buildName}_{BuildConfigAssets.buildNumber}.{BuildConfigAssets.fileType}";
        EditorUtility.SetDirty(BuildConfigAssets);

        EditorUserBuildSettings.buildAppBundle = BuildConfigAssets.isGoogleStoreBuild;
    }

    private static void Build()
    {
        Log.Call();

        Debug.Log($">>>>>Trial License: {UnityEngine.Windows.LicenseInformation.isOnAppTrial}");

        BuildPlayerOptions buildOpstions = new()
        {
            options = (BuildConfigAssets.isDevBuild == true ?
                BuildOptions.Development | BuildOptions.AllowDebugging | BuildOptions.AllowDebugging : BuildOptions.None) | BuildOptions.CompressWithLz4,
            scenes = CustomBuildPipeline.GetScenes(),
            locationPathName = BuildConfigAssets.buildPath,
            target = BuildTarget.Android,
            targetGroup = BuildTargetGroup.Android,
        };

        Debug.Log($"Result Path: {buildOpstions.locationPathName}");

        var result = BuildPipeline.BuildPlayer(buildOpstions);

        Debug.Log($"BuildAndroid - Result");
        CustomBuildPipeline.LogBuildResult(result.summary);
    }
}