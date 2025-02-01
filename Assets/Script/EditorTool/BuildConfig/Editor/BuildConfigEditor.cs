using UnityEditor;
using UnityEngine;
using System.Linq;
using System;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;

[CustomEditor(typeof(BuildConfig))]
public class BuildConfigEditor : Editor
{
    public const string AddressableDefaultProfile = "Default";
    public const string AddressableRemoteProfile = "Aws-Remote";
    private const int AddressableUseAssetDataBase = 0; //local
    private const int AddressableUseExistingBuild = 1; //load asset from remote  ** build asset bundle first **

    private AddressableAssetSettings addressableAssetSettings;

    public enum AddressableHost
    {
        Local,
        Dev,
        Stg,
        Prod,
    }

    public override void OnInspectorGUI()
    {
        var buildConfig = target as BuildConfig;
        addressableAssetSettings = AddressableAssetSettingsDefaultObject.Settings;

        EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(buildConfig.Data)));
        serializedObject.ApplyModifiedProperties();

        // serializedObject.pro

        if (buildConfig.Data == null)
        {
            EditorGUILayout.HelpBox("BuildConfigData is null", MessageType.Error);
            return;
        }

        EditorAddressableHost(buildConfig);
        EditorApplicationVersion(buildConfig);
        ValidateBuildConfigWithCurrentSetting(buildConfig);
    }

    private void EditorAddressableHost(BuildConfig buildConfig)
    {
        var addressableHosts = System.Enum.GetNames(typeof(AddressableHost)).ToList();
        var currentHostIndex = (int)(Enum.TryParse(buildConfig.Data.AddressableHost, out AddressableHost host) ? host : AddressableHost.Local);

        var selectedHostIndex = EditorGUILayout.Popup("Addressable Host", currentHostIndex, addressableHosts.ToArray());

        if (selectedHostIndex != currentHostIndex)
        {
            var selectedHost = (AddressableHost)selectedHostIndex;

            buildConfig.Data.AddressableHost = selectedHost.ToString();
            EditorUtility.SetDirty(buildConfig.Data);

            EditorChangeAddressableProfile(selectedHost);
            EditorChangeAddressablePlayMode(selectedHost);
        }
    }

    private void EditorChangeAddressableProfile(AddressableHost selectedHost)
    {
        string addressableProfile = AddressableDefaultProfile;
        switch (selectedHost)
        {
            case AddressableHost.Local:
                addressableProfile = AddressableDefaultProfile;
                break;
            case AddressableHost.Dev:
            case AddressableHost.Stg:
            case AddressableHost.Prod:
                addressableProfile = AddressableRemoteProfile;
                break;
        }

        string profileID = addressableAssetSettings.profileSettings.GetProfileId(addressableProfile);
        if (addressableAssetSettings.activeProfileId == profileID)
            return;

        addressableAssetSettings.activeProfileId = profileID;
        Debug.Log($"Change Addressable Profile: {addressableProfile}");
    }

    private void EditorChangeAddressablePlayMode(AddressableHost selectedHost)
    {
        int playModeIndex = AddressableUseAssetDataBase;
        switch (selectedHost)
        {
            case AddressableHost.Local:
                playModeIndex = AddressableUseAssetDataBase;
                break;
            case AddressableHost.Dev:
            case AddressableHost.Stg:
            case AddressableHost.Prod:
                playModeIndex = AddressableUseExistingBuild;
                break;
        }

        if (addressableAssetSettings.ActivePlayModeDataBuilderIndex == playModeIndex)
            return;

        addressableAssetSettings.ActivePlayModeDataBuilderIndex = playModeIndex;
        Debug.Log($"Change Addressable Play Mode Script to '{playModeIndex}'");
    }

    private void EditorApplicationVersion(BuildConfig buildConfig)
    {
        buildConfig.Data.Version = string.IsNullOrWhiteSpace(buildConfig.Data.Version) ? Application.version : buildConfig.Data.Version;

        var version = EditorGUILayout.TextField(nameof(buildConfig.Data.Version), buildConfig.Data.Version);
        buildConfig.Data.Version = version;

        if (Application.version != buildConfig.Data.Version)
        {
            PlayerSettings.bundleVersion = buildConfig.Data.Version;

            Debug.Log($"Change Application Version to '{Application.version}'");

            EditorUtility.SetDirty(buildConfig.Data);
        }
    }

    private void ValidateBuildConfigWithCurrentSetting(BuildConfig buildConfig)
    {
        if (Application.version != buildConfig.Data.Version)
        {
            PlayerSettings.bundleVersion = buildConfig.Data.Version;

            Debug.Log($"Change Application Version to '{Application.version}'");
        }

        var currentHost = Enum.TryParse(buildConfig.Data.AddressableHost, out AddressableHost host) ? host : AddressableHost.Local;

        EditorChangeAddressableProfile(currentHost);
        EditorChangeAddressablePlayMode(currentHost);
    }
}