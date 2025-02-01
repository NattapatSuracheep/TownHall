using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class AddressableSceneManager
{
    public async UniTask LoadSceneAsync(AssetReferenceScene sceneAssetRef, LoadSceneMode mode = LoadSceneMode.Additive, bool isActiveOnLoad = true)
    {
        Debug.Log($"Load scene: {sceneAssetRef.AssetGUID}");

        if (AddressableCacheManager.IsAlreadyInCache(sceneAssetRef.AssetGUID))
        {
            Debug.Log($"Already loaded scene: {sceneAssetRef.AssetGUID}");

            var scene = AddressableCacheManager.GetCache<Scene>(sceneAssetRef.AssetGUID);
            SceneManager.SetActiveScene(scene);
            return;
        }

        var handle = sceneAssetRef.LoadSceneAsync(mode, isActiveOnLoad);

        while (handle.Status == AsyncOperationStatus.None)
            await UniTask.Yield();

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            AddressableCacheManager.AddToCache(sceneAssetRef.AssetGUID, handle);
        }
        else if (handle.Status == AsyncOperationStatus.Failed)
        {
            Debug.LogError($"Failed to load scene: {sceneAssetRef.AssetGUID}");
        }
    }

    public void UnLoadScene(AssetReferenceScene sceneAssetRef)
    {
        Debug.Log($"Unload scene: {sceneAssetRef.AssetGUID}");

        sceneAssetRef.UnLoadScene();
        AddressableCacheManager.RemoveFromCache(sceneAssetRef.AssetGUID);
    }
}