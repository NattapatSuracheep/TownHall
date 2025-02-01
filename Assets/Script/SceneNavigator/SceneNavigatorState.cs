using Cysharp.Threading.Tasks;
using UnityEngine;

public class SceneNavigatorState : State<SceneNavigator.SceneNavigatorStateEnum>
{
    private AssetReferenceScene sceneAssetRef;

    public SceneNavigatorState(SceneNavigator.SceneNavigatorStateEnum stateEnum) : base(stateEnum)
    {
    }

    public SceneNavigatorState(SceneNavigator.SceneNavigatorStateEnum stateEnum, AssetReferenceScene sceneAssetRef) : base(stateEnum)
    {
        this.sceneAssetRef = sceneAssetRef;
    }

    public override async UniTask OnEnter()
    {
        await base.OnEnter();

        await AddressableManager.LoadSceneAsync(sceneAssetRef);
    }

    public override async UniTask OnExit()
    {
        await base.OnExit();

        AddressableManager.UnloadScene(sceneAssetRef);

        await Resources.UnloadUnusedAssets();
    }
}