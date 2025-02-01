using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableObjectManager
{
    public async UniTask<GameObject> InstantiateGameObjectAsync(AssetReferenceGameObject assetRef, Transform parent = null)
    {
        Debug.Log($"instantiate gameobject: {assetRef}");

        if (AddressableCacheManager.IsAlreadyInCache(assetRef.AssetGUID))
        {
            Debug.Log($"Already cache gameobject: {assetRef}");

            var objCache = AddressableCacheManager.GetCache<GameObject>(assetRef.AssetGUID);
            var objInst = GameObject.Instantiate(objCache, parent);

            return objInst;
        }

        var handle = Addressables.InstantiateAsync(assetRef, parent);

        while (handle.Status == AsyncOperationStatus.None)
            await UniTask.Yield();

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            AddressableCacheManager.AddToCache(assetRef.AssetGUID, handle);
            return handle.Result;
        }
        else if (handle.Status == AsyncOperationStatus.Failed)
        {
            Debug.LogError($"Failed to instantiate gameobject: {assetRef}");
            return default;
        }

        return default;
    }

    public async UniTask<T> InstantiateComponentObjectAsync<T>(AssetReferenceComponent<T> assetRef, Transform parent = null) where T : Component
    {
        Debug.Log($"instantiate component object: {assetRef}");

        if (AddressableCacheManager.IsAlreadyInCache(assetRef.AssetGUID))
        {
            Debug.Log($"Already cache component object: {assetRef}");

            var objCache = AddressableCacheManager.GetCacheComponent<T>(assetRef.AssetGUID);
            var objInst = GameObject.Instantiate(objCache, parent);

            return objInst;
        }

        var handle = assetRef.InstantiateAsync(parent);

        while (handle.Status == AsyncOperationStatus.None)
            await UniTask.Yield();

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            AddressableCacheManager.AddToCache(assetRef.AssetGUID, handle);
            return handle.Result.GetComponent<T>();
        }
        else if (handle.Status == AsyncOperationStatus.Failed)
        {
            Debug.LogError($"Failed to instantiate component object: {assetRef}");
            return default;
        }

        return default;
    }

    public void ReleaseGameObject(AssetReferenceGameObject assetRef)
    {
        var obj = AddressableCacheManager.GetCache<GameObject>(assetRef.AssetGUID);
        if (!Addressables.ReleaseInstance(obj))
            GameObject.Destroy(obj);

        AddressableCacheManager.RemoveFromCache(assetRef.AssetGUID);
    }

    public void ReleaseGameObject<T>(AssetReferenceComponent<T> assetRef) where T : Component
    {
        var obj = AddressableCacheManager.GetCache<GameObject>(assetRef.AssetGUID);
        if (!Addressables.ReleaseInstance(obj))
            GameObject.Destroy(obj);

        AddressableCacheManager.RemoveFromCache(assetRef.AssetGUID);
    }
}