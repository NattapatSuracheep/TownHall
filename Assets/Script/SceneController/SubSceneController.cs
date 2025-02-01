using Cysharp.Threading.Tasks;
using UnityEngine;

public class SubSceneController : SceneController
{
    protected override void OnEnable()
    {
        base.OnEnable();

        GameManager.Instance.AddSubSceneController(this);
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        GameManager.Instance.RemoveSubSceneController(this);
        GameManager.Instance.CameraManager.ChangeCinemachineBlendSetting(null);
    }

    public virtual async UniTask InitializeAsync()
    {
        await LoadPreloadAssetInSceneAsync();
    }
}