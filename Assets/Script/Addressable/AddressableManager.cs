using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class AddressableManager
{
    public static bool IsInitialize { get; private set; }

    private static AddressableSceneManager addressableSceneManager = new();
    private static AddressableObjectManager addressableObjectManager = new();
    private static AddressableTextAssetsManager addressableTextAssetsManager = new();

    public static async UniTask InitializeAsync()
    {
        if (IsInitialize)
            return;

        Log.Call();

        var handle = Addressables.InitializeAsync(false);
        await handle;

        IsInitialize = handle.Status == AsyncOperationStatus.Succeeded;

        handle.Release();

        Debug.Log($"Addressable initialize: {IsInitialize}");
    }

    public static async UniTask LoadPermanentAssetAsync(AssetReferenceGameObject[] permanentAssetRef)
    {
        List<UniTask> taskList = new();

        for (var i = 0; i < permanentAssetRef.Length; i++)
        {
            var assetRef = permanentAssetRef[i];
            taskList.Add(LoadAssetToCacheAsync(assetRef));
        }

        await UniTask.WhenAll(taskList);
    }

    public static async UniTask LoadSceneAsync(AssetReferenceScene sceneAssetRef, LoadSceneMode mode = LoadSceneMode.Additive, bool isActiveOnLoad = true)
    {
        await addressableSceneManager.LoadSceneAsync(sceneAssetRef, mode, isActiveOnLoad);
    }

    public static void UnloadScene(AssetReferenceScene sceneAssetRef)
    {
        addressableSceneManager.UnLoadScene(sceneAssetRef);
    }

    public static async UniTask LoadAssetToCacheAsync<T>(AssetReferenceComponent<T> assetRef) where T : Component
    {
        if (AddressableCacheManager.IsAlreadyInCache(assetRef.AssetGUID))
            return;

        var handle = assetRef.LoadAssetAsync();

        while (handle.Status == AsyncOperationStatus.None)
            await UniTask.Yield();

        AddressableCacheManager.AddToCache(assetRef.AssetGUID, handle);
    }

    public static async UniTask LoadAssetToCacheAsync(AssetReferenceGameObject assetRef)
    {
        if (AddressableCacheManager.IsAlreadyInCache(assetRef.AssetGUID))
            return;

        var handle = assetRef.LoadAssetAsync();

        while (handle.Status == AsyncOperationStatus.None)
            await UniTask.Yield();

        AddressableCacheManager.AddToCache(assetRef.AssetGUID, handle);
    }

    public static void UnloadCacheAssets(AssetReferenceGameObject assetRef) => AddressableCacheManager.RemoveFromCache(assetRef.AssetGUID);

    public static async UniTask<GameObject> InstantiateGameObjectAsync(AssetReferenceGameObject assetRef, Transform parent = null)
    {
        return await addressableObjectManager.InstantiateGameObjectAsync(assetRef, parent);
    }

    public static async UniTask<T> InstantiateComponentObjectAsync<T>(AssetReferenceComponent<T> assetRef, Transform parent = null) where T : Component
    {
        return await addressableObjectManager.InstantiateComponentObjectAsync(assetRef, parent);
    }

    public static void ReleaseGameObject(AssetReferenceGameObject assetRef)
    {
        addressableObjectManager.ReleaseGameObject(assetRef);
    }

    public static UniTask<TextAsset> GetTextAssetAsync(string key, bool isCache = true)
    {
        return addressableTextAssetsManager.GetTextAssetAsync(key, isCache);
    }

    public static UniTask<TextAsset[]> GetTextAssetLabelAsync(string label)
    {
        return addressableTextAssetsManager.GetTextAssetLabelAsync(label);
    }
}