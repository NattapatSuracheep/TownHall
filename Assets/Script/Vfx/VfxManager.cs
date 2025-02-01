using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class VfxManager : MonoBehaviour
{
    private const string AssetsPoolResourcesPath = "Utils/AssetsPool";
    private AssetsPool poolResourcesObject;

    [SerializeField] private Transform vfxContainer;
    [SerializeField] private Transform clickVfxCanvas;

    private Dictionary<string, AssetsPool> poolDictionary = new();

    private void OnDestroy()
    {
        RemoveBlindVfxToInputManager();
    }

    public void Initialize()
    {
        Log.Call();

        LoadResourcesVfx();
        BindClickVfxToInputManager();
    }

    private void LoadResourcesVfx()
    {
        Log.Call();

        LoadResourcePool();

        for (var i = 0; i < VfxConstant.ResourcesVfxDict.Count; i++)
        {
            var data = VfxConstant.ResourcesVfxDict.ElementAt(i);
            var key = data.Key;
            var resourcePath = data.Value;
            var pool = GameObject.Instantiate(poolResourcesObject, vfxContainer);
            pool.CreatePool(resourcePath);
            poolDictionary.Add(key, pool);
        }
    }

    public async UniTask LoadAddressbleVfxAsync()
    {
        Log.Call();

        await UniTask.CompletedTask;
    }

    private void LoadResourcePool()
    {
        if (poolResourcesObject == null)
            poolResourcesObject = Resources.Load<AssetsPool>(AssetsPoolResourcesPath);
    }

    private void BindClickVfxToInputManager()
    {
        GameManager.Instance.InputManager.OnUiLeftClickStartedEvent += OnUiClick;
    }

    private void RemoveBlindVfxToInputManager()
    {
        GameManager.Instance.InputManager.OnUiLeftClickStartedEvent -= OnUiClick;
    }

    private void OnUiClick()
    {
        var pos = GameManager.Instance.InputManager.CurrentPointPosition;
        var clickPos = new Vector3(pos.x, pos.y, GameManager.Instance.UiCamera.focusDistance);
        clickPos = GameManager.Instance.UiCamera.ScreenToWorldPoint(clickPos);
        PlayClickVfx(clickPos);
    }

    public void PlayClickVfx(Vector3 position)
    {
        PlayVfxAsync(VfxConstant.ClickVfx, position, clickVfxCanvas).Forget();
    }

    public async UniTask PlayVfxAsync(string key, Vector3 position, Transform parent = null)
    {
        if (poolDictionary.TryGetValue(key, out var pool))
        {
            var vfx = pool.Get<ParticleSystem>(parent);
            vfx.transform.position = position;
            vfx.Play();

            await UniTask.WaitForSeconds(vfx.main.startLifetime.constantMax);

            pool.Return(vfx.gameObject);
        }
        else
        {
            Debug.LogWarning($"Can't play vfx: {key}");
        }
    }
}