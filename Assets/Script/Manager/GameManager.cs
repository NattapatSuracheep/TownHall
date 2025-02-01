using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using IngameDebugConsole;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There is more than one GameManager in the scene.");
            DestroyImmediate(this.gameObject);
            return;
        }

        Log.Call();
        Instance = this;

        UnityEngine.Rendering.DebugManager.instance.enableRuntimeUI = false;
        Debug.developerConsoleEnabled = false;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Debug.unityLogger.logEnabled = true;
        inGameDebugConsole.gameObject.SetActive(true);
        inGameDebugConsole.enabled = true;
#else
        inGameDebugConsole.gameObject.SetActive(false);
        inGameDebugConsole.enabled = false;
        Debug.unityLogger.logEnabled = false;
#endif
    }

    [Header("Debug")]
    [SerializeField] private DebugLogManager inGameDebugConsole;

    [Header("Config")]
    [SerializeField] private BuildConfig buildConfig;

    [Header("Camera")]
    [SerializeField] private CameraManager cameraManager;

    [Header("Main Canvas")]
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Transform mainCanvasSafeArea;

    [Header("Component")]
    [SerializeField] private GameDataManager gameDataManager = new();
    [SerializeField] private SceneNavigator sceneNavigator;
    [SerializeField] private VfxManager vfxManager;
    [SerializeField] private InputManager inputManager;

    [Header("Permanent Load Assets")]
    [SerializeField] private AssetReferenceGameObject[] permanentLoadAssets;

    public Camera MainCamera => cameraManager.MainCamera;
    public Camera UiCamera => cameraManager.UiCamera;
    public Canvas MainCanvas => mainCanvas;
    public Transform MainCanvasSafeArea => mainCanvasSafeArea;
    public CameraManager CameraManager => cameraManager;
    public SceneNavigator SceneNavigator => sceneNavigator;
    public GameDataManager GameDataManager => gameDataManager;
    public SubSceneController CurrentSubSceneController { get; private set; }
    public VfxManager VfxManager => vfxManager;
    public GoogleSheetManager GoogleSheetManager { get; private set; } = new();
    public BuildConfig BuildConfig => buildConfig;
    public InputManager InputManager => inputManager;

    private void OnValidate()
    {
        if (sceneNavigator == null && !TryGetComponent(out sceneNavigator))
            sceneNavigator = this.gameObject.AddComponent<SceneNavigator>();

        if (vfxManager == null && !TryGetComponent(out vfxManager))
            vfxManager = this.gameObject.AddComponent<VfxManager>();

        if (buildConfig == null && !TryGetComponent(out buildConfig))
            buildConfig = this.gameObject.AddComponent<BuildConfig>();

        if (inputManager == null && !TryGetComponent(out inputManager))
            inputManager = this.gameObject.AddComponent<InputManager>();
    }

    private async void Start()
    {
        Log.Call();

        SceneNavigator.FadeBackInAsync().Forget();

        PreventApplicationToSleep();
        vfxManager.Initialize();
        inputManager.Initialize();

        await AddressableManager.InitializeAsync();

        await InitializeComponentAfterAddressableInitializeAsync();

        await SceneNavigator.InitializeAsync();
    }

    public async UniTask InitializeComponentAfterAddressableInitializeAsync()
    {
        if (!AddressableManager.IsInitialize)
        {
            throw new Exception($"Addressable is not initialize");
        }

        var sequence = new List<Func<UniTask>>()
        {
            vfxManager.LoadAddressbleVfxAsync,
            LoadPermanentAssetAsync,
            GameDataManager.InitializeAsync,
        };
        await GameDataManager.LoadSequenceAsync(sequence.ToArray());
    }

    private async UniTask LoadPermanentAssetAsync()
    {
        await AddressableManager.LoadPermanentAssetAsync(permanentLoadAssets);
    }

    public void AddSubSceneController(SubSceneController subSceneController)
    {
        CurrentSubSceneController = subSceneController;
    }

    public void RemoveSubSceneController(SubSceneController subSceneController)
    {
        if (CurrentSubSceneController != subSceneController)
            return;

        CurrentSubSceneController = null;
    }

    public void AllowApplicationToSleep()
    {
        Log.Call();
        Screen.sleepTimeout = SleepTimeout.SystemSetting;
    }

    public void PreventApplicationToSleep()
    {
        Log.Call();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    public async void ReturnToTitleAsync()
    {
        await SceneNavigator.NextAsync(SceneNavigator.SceneNavigatorStateEnum.Title);
    }

    public async void QuitTheGameAsync()
    {
        await SceneNavigator.FadeBackInAsync();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
