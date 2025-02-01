using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableTextAssetsManager
{
    public async UniTask<TextAsset> GetTextAssetAsync(string key, bool isCache = true)
    {
        if (AddressableCacheManager.IsAlreadyInCache(key))
        {
            Debug.Log($"Already cache text assets: {key}");

            return AddressableCacheManager.GetCache<TextAsset>(key);
        }

        var handle = Addressables.LoadAssetAsync<TextAsset>(key);

        while (handle.Status == AsyncOperationStatus.None)
            await UniTask.Yield();

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            if (isCache)
            {
                AddressableCacheManager.AddToCache(key, handle);
            }
            else
            {
                var result = handle.Result;
                handle.Release();

                return result;
            }
        }
        else if (handle.Status == AsyncOperationStatus.Failed)
        {
            Debug.LogError($"Failed to load text assets: {key}");
            return default;
        }

        return default;
    }

    public async UniTask<TextAsset[]> GetTextAssetLabelAsync(string label)
    {
        var handle = Addressables.LoadAssetAsync<TextAsset[]>(label);

        while (handle.Status == AsyncOperationStatus.None)
            await UniTask.Yield();

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            var result = handle.Result;
            handle.Release();

            return result;
        }
        else if (handle.Status == AsyncOperationStatus.Failed)
        {
            Debug.LogError($"Failed to load text assets label: {label}");
            return default;
        }

        return default;
    }
}