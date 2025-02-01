using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SceneNavigator : MonoBehaviour
{
    private const float fadeDuration = 0.2f;
    private const string loadingPanelResourcePath = "Prefab/UI/LoadingPanel"; //Assets/Resources/UI/LoadingPanel.prefab

    public enum SceneNavigatorStateEnum
    {
        TriggerGameStart,
        Title,
        MainGame,
    }

    [Header("Fade")]
    [SerializeField] private Image fadeImage;

    [Space(30)]
    [SerializeField] private AssetReferenceScene mainGame;

    private StateMachine<SceneNavigatorStateEnum> stateMachine;

    private LoadingPanel loadingPanelResourceObject;
    private LoadingPanel loadingPanel;

    public async UniTask InitializeAsync()
    {
        Log.Call();

        stateMachine = new(CreateState());
        await stateMachine.InitializeAsync();
    }

    private SceneNavigatorState[] CreateState()
    {
        var result = new List<SceneNavigatorState>
        {
            new SceneNavigatorState(SceneNavigatorStateEnum.MainGame, mainGame)
        };

        return result.ToArray();
    }

    public async UniTask NextAsync(SceneNavigatorStateEnum state)
    {
        GameManager.Instance.AllowApplicationToSleep();

        await FadeBackInAsync();
        await stateMachine.TransitionToStateAsync(state);
    }

    #region Fade

    public async UniTask FadeBackInAsync()
    {
        Log.Call();

        if (fadeImage.color.a == 1)
            return;

        fadeImage.raycastTarget = true;
        fadeImage.DOFade(1, fadeDuration);

        await UniTask.WaitForSeconds(fadeDuration);
    }

    public async UniTask FadeBackOutAsync()
    {
        Log.Call();

        if (fadeImage.color.a == 0)
            return;

        Sequence sequence = DOTween.Sequence();
        sequence.Append(fadeImage.DOFade(0, fadeDuration));
        sequence.AppendCallback(() =>
        {
            fadeImage.raycastTarget = false;
        });

        await UniTask.WaitForSeconds(fadeDuration);
    }

    #endregion Fade

    #region Loading Panel

    public async UniTask ShowLoadingPanelAsync(string info, float maxValue = 1f)
    {
        Log.Call();
        await FadeBackInAsync();

        if (loadingPanelResourceObject == null)
            loadingPanelResourceObject = Resources.Load<LoadingPanel>(loadingPanelResourcePath);

        if (loadingPanel == null)
            loadingPanel = Instantiate(loadingPanelResourceObject, GameManager.Instance.MainCanvas.transform);

        loadingPanel.InitializeWithLoadingBar(info, maxValue);
        await FadeBackOutAsync();
    }

    public async UniTask ShowLoadingPanelWithOutLoadBarAsync(string info)
    {
        Log.Call();
        await FadeBackInAsync();

        if (loadingPanelResourceObject == null)
            loadingPanelResourceObject = Resources.Load<LoadingPanel>(loadingPanelResourcePath);

        if (loadingPanel == null)
            loadingPanel = Instantiate(loadingPanelResourceObject, GameManager.Instance.MainCanvas.transform);

        loadingPanel.InitializeWithOutLoadingBar(info);
        await FadeBackOutAsync();
    }

    public void UpdateLoadingPanel(string info, float value = -1)
    {
        if (loadingPanel == null)
            return;

        loadingPanel.UpdateData(info, value);
    }

    public async UniTask CloseLoadingPanelAsync()
    {
        if (loadingPanel == null)
        {
            await FadeBackOutAsync();
            return;
        }

        await FadeBackInAsync();
        Destroy(loadingPanel.gameObject);
        loadingPanel = null;
        await FadeBackOutAsync();
    }

    #endregion Loading Panel
}