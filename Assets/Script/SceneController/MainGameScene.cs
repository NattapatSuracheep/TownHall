using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MainGameScene : SceneController
{
    [SerializeField] private AssetReferenceComponent<TownHallPanel> townHallPanelAssetRef;
    [SerializeField] private Transform panelContainer;

    private TownHallPanel townHallPanel;

    public Transform PanelContainer => panelContainer;

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    private async void Start()
    {
        Log.Call();
        var loadSequence = new List<Func<UniTask>>
        {
            LoadPreloadAssetInSceneAsync,
        };
        await GameManager.GameDataManager.LoadSequenceAsync(loadSequence.ToArray(), "main game");

        townHallPanel = await AddressableManager.InstantiateComponentObjectAsync(townHallPanelAssetRef, panelContainer);
        await townHallPanel.InitializeAsync();


        await SceneNavigator.CloseLoadingPanelAsync();
    }

    public async UniTask BackToMainGame()
    {
        await SceneNavigator.NextAsync(SceneNavigator.SceneNavigatorStateEnum.Title);
    }
}