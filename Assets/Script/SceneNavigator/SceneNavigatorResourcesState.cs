using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigatorResourcesState : SceneNavigatorState
{
    private int sceneIndex;

    public SceneNavigatorResourcesState(SceneNavigator.SceneNavigatorStateEnum stateEnum, int sceneIndex) : base(stateEnum, null)
    {
        this.sceneIndex = sceneIndex;
    }

    public override async UniTask OnEnter()
    {
        var handle = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
        await UniTask.WhenAll(handle.ToUniTask());

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(sceneIndex));
    }

    public override async UniTask OnExit()
    {
        var handle = SceneManager.UnloadSceneAsync(sceneIndex);

        await UniTask.WhenAll(handle.ToUniTask());

        await Resources.UnloadUnusedAssets();
    }
}