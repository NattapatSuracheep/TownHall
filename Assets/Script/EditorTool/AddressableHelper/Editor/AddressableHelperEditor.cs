using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CustomEditor(typeof(AddressableHelper))]
public class AddressableHelperEditor : UnityEditor.Editor
{
    private List<string> addressableRemoteLabel = new()
    {
        "config",
        "stage",
        "localization",
        "scene",
        "prefab"
    };

    public override void OnInspectorGUI()
    {
        var addressableHelper = target as AddressableHelper;

        if (GUILayout.Button("Clear Cache"))
        {
            Debug.Log($"Caching.ClearCache");

            Caching.ClearCache();
        }

        if (GUILayout.Button("Clear Dependency Cache"))
        {
            Debug.Log($"Addressables.ClearDependencyCacheAsync");

            Addressables.ClearDependencyCacheAsync(addressableRemoteLabel);
        }

        if (GUILayout.Button("Clear Addressable Build Cache"))
        {
            Debug.Log($"Clear Addressable Build Cache");

            AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder.ClearCachedData();
        }

        GUILayout.Space(10);
        if (GUILayout.Button("List Current Addressable Cache"))
        {
            if (!Application.isPlaying)
            {
                Debug.Log("Not In Play Mode");
                return;
            }

            if (AddressableCacheManager.cacheDict.Count == 0)
            {
                Debug.Log("Addressable Cache is Empty");
                return;
            }

            foreach (var cacheItem in AddressableCacheManager.cacheDict)
                Debug.Log($"{cacheItem.Key} : {cacheItem.Value.DebugName}");
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Open Addressable Build Folder"))
        {
            var path = @$"ServerData\{EditorUserBuildSettings.activeBuildTarget}";
            if (!Directory.Exists(path))
            {
                Debug.LogError($"Addressable Build Folder Not Found on Path: {path}");
                return;
            }

            Debug.Log($"Addressables.OpenBuildFolder");

            Application.OpenURL(path);
        }
    }
}