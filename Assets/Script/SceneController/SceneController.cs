using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] private bool isIgnoreAwakeSafetyCheck;
    [SerializeField] private AssetReferenceGameObject[] preloadAssetRef;

    [SerializeField] private bool isAllowToPause;

    protected GameManager GameManager => GameManager.Instance;
    protected SceneNavigator SceneNavigator => GameManager?.SceneNavigator;
    protected InputManager InputManager => GameManager?.InputManager;

    protected virtual void Awake()
    {
#if !UNITY_EDITOR
        isIgnoreAwakeSafetyCheck = false;
#endif
        if (isIgnoreAwakeSafetyCheck)
            return;

        if (GameManager == null)
        {
            SceneManager.LoadScene(0);
            throw new Exception("There is no GameManager in the scene");
        }
    }

    protected virtual void OnEnable()
    {
        AddInputActionEvent();
    }

    protected virtual void OnDisable()
    {
        RemoveInputActionEvent();
        UnloadPreloadAssetInScene();
    }

    protected virtual async UniTask LoadPreloadAssetInSceneAsync()
    {
        if (preloadAssetRef == null || preloadAssetRef.Length == 0)
            return;

        Log.Call();

        for (var i = 0; i < preloadAssetRef.Length; i++)
        {
            var assets = preloadAssetRef[i];
            await AddressableManager.LoadAssetToCacheAsync(assets);
        }
    }

    protected virtual void UnloadPreloadAssetInScene()
    {
        if (preloadAssetRef == null || preloadAssetRef.Length == 0)
            return;

        Log.Call();

        for (var i = 0; i < preloadAssetRef.Length; i++)
        {
            var assets = preloadAssetRef[i];
            AddressableManager.UnloadCacheAssets(assets);
        }
    }

    protected virtual void AddInputActionEvent()
    {
        if (InputManager == null)
            return;

        InputManager.OnUiCancelStartedEvent += OnUiCancelInvoke;
    }

    protected virtual void RemoveInputActionEvent()
    {
        if (InputManager == null)
            return;

        InputManager.OnUiCancelStartedEvent -= OnUiCancelInvoke;
    }

    protected virtual void OnUiCancelInvoke()
    {
        if (!isAllowToPause)
            return;
    }
}