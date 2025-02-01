using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public static class AddressableCacheManager
{
    public static Dictionary<string, AsyncOperationHandle> cacheDict = new();

    public static bool IsAlreadyInCache(string key)
    {
        return cacheDict.ContainsKey(key);
    }

    public static void AddToCache(string key, AsyncOperationHandle handle)
    {
        if (IsAlreadyInCache(key))
        {
            Debug.LogError($"Already in cache: {key} / {handle.DebugName}");
            return;
        }

        handle.Destroyed += (value) =>
        {
            Debug.LogWarning($"Handle get destroyed or release: {key} / {handle.DebugName}");
            RemoveFromCache(key);
        };

        cacheDict.Add(key, handle);

        Debug.Log($"Added to cache: {key} / {handle.DebugName}");

        DisplayCurrentCacheCount();
    }

    public static void RemoveFromCache(string key)
    {
        if (!IsAlreadyInCache(key))
        {
            Debug.LogWarning($"Not in cache: {key}");
            return;
        }

        Log.Call();

        var handle = cacheDict[key];
        var debugName = handle.DebugName;
        cacheDict.Remove(key);

        if (handle.Status != AsyncOperationStatus.Succeeded)
            handle.Release();

        Debug.Log($"Removed from cache: {key} / {debugName}");

        DisplayCurrentCacheCount();
    }

    private static void DisplayCurrentCacheCount()
    {
        Log.Logging($"cache count: {cacheDict.Count}");
    }


    public static T GetCache<T>(string key)
    {
        if (cacheDict.TryGetValue(key, out var cacheHandle))
        {
            Debug.Log($"Get cache: {cacheHandle.DebugName}");

            return (T)cacheHandle.Result;
        }

        Debug.LogWarning($"Can't get gameobject from cache: {key}");
        return default;
    }

    public static T GetCacheComponent<T>(string key)
    {
        return GetCache<GameObject>(key).GetComponent<T>();
    }
}