using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AtlantisGetTexture2DModule : AtlantisComponent
{
    private enum GetSpriteType
    {
        FromScene,
        FromGameObject
    }

    private class SelectedToPackContainer
    {
        public bool previousIsSelectToPack { get; private set; }
        public bool isSelectToPack { get; private set; }
        public int texture2DIndex { get; private set; }

        public void SetData(bool isSelectToPack, int texture2DIndex)
        {
            this.isSelectToPack = isSelectToPack;
            this.texture2DIndex = texture2DIndex;
        }

        public bool Update(bool isSelectToPack)
        {
            this.isSelectToPack = isSelectToPack;

            var isValueChange = previousIsSelectToPack != isSelectToPack;
            previousIsSelectToPack = isSelectToPack;

            return isValueChange;
        }
    }

    private SceneAsset sceneAsset;
    private GameObject[] gameObjectArray = new GameObject[1];
    private GetSpriteType currentGetSpriteType;
    private Vector2 gameObjectScrollPosition;
    private Vector2 scrollPosition;

    private List<string> optionList = new();
    private List<SelectedToPackContainer> selectedTexture2DToPackList = new();

    private Texture2D[] allTexture2D;
    private string[] allTexture2DAssetPath;

    private bool defaultSelectToPack => atlantis.DefaultSelectToPack;

    public Texture2D[] SelectedTexture2DToPack { get; private set; }
    public string[] SelectedTexture2DToPackAssetPath { get; private set; }

    private bool isGetAllSprite => allTexture2D != null;
    private bool isHaveTexture2D => allTexture2D != null && allTexture2D.Length > 0;
    private bool isHaveSelectedTexture2D => SelectedTexture2DToPack != null && SelectedTexture2DToPack.Length > 0;

    public Action OnSelectTypeChangeEvent;
    public Action OnGetAllSpriteButtonClickEvent;
    public Action OnUpdateSelectedSpriteIndexToPackEvent;

    public override void Initialize(AtlantisAtlasPacker atlantis)
    {
        base.Initialize(atlantis);

        CreateOption();
    }

    private void CreateOption()
    {
        optionList.Clear();

        var getSpriteTypeEnumLength = Enum.GetValues(typeof(GetSpriteType)).Length;
        for (var i = 0; i < getSpriteTypeEnumLength; i++)
            optionList.Add(((GetSpriteType)i).ToString());
    }

    public bool GUI()
    {
        OptionGUI();

        if (Application.isPlaying && currentGetSpriteType == GetSpriteType.FromScene)
            EditorGUILayout.LabelField($"This option is not available in play mode.");

        GUILayout.Space(5);
        EditorWindowsHelper.HorizontalLine();
        GUILayout.Space(10);

        ObjectGUI();

        GUILayout.Space(10);
        EditorWindowsHelper.HorizontalLine();
        GUILayout.Space(10);

        ButtonGUI();

        if (isGetAllSprite == false)
        {
            return false;
        }
        else if (isHaveTexture2D == false)
        {
            EditorGUILayout.LabelField($"Not found any sprite.");
            return false;
        }

        GUILayout.Space(10);
        EditorWindowsHelper.HorizontalLine();
        GUILayout.Space(10);

        DisplayAllTexture2D();

        if (isHaveSelectedTexture2D == false)
        {
            GUILayout.Space(10);
            EditorWindowsHelper.HorizontalLine();
            GUILayout.Space(10);

            EditorGUILayout.LabelField($"Please select some sprite to pack");

            GUILayout.Space(10);
            EditorWindowsHelper.HorizontalLine();
            GUILayout.Space(10);

            return false;
        }

        return true;
    }

    private void OptionGUI()
    {
        var selectedType = (GetSpriteType)GUILayout.Toolbar((int)currentGetSpriteType, optionList.ToArray());
        if (selectedType != currentGetSpriteType)
        {
            Clear();
            OnSelectTypeChangeEvent?.Invoke();

            currentGetSpriteType = selectedType;
        }
    }

    private void ObjectGUI()
    {
        switch (currentGetSpriteType)
        {
            case GetSpriteType.FromScene:
                EditorGUI.BeginDisabledGroup(Application.isPlaying);
                sceneAsset = (SceneAsset)EditorGUILayout.ObjectField("Scene", sceneAsset, typeof(SceneAsset), true);
                EditorGUI.EndDisabledGroup();
                break;

            case GetSpriteType.FromGameObject:
                var elementCount = gameObjectArray.Length;
                var estimatedHeight = (elementCount * 20f) + 20f; // Approximate per-item height
                var scrollHeight = Mathf.Clamp(estimatedHeight, 50, 150);

                gameObjectScrollPosition = EditorGUILayout.BeginScrollView(gameObjectScrollPosition, GUILayout.Height(scrollHeight));

                if (gameObjectArray.Length == 0)
                {
                    EditorGUILayout.LabelField("No game object selected.");
                    EditorGUILayout.LabelField("Click \"Add GameObject\" to add a game object.");
                }
                else
                {
                    for (int i = 0; i < gameObjectArray.Length; i++)
                    {
                        EditorGUILayout.BeginHorizontal();

                        gameObjectArray[i] = (GameObject)EditorGUILayout.ObjectField(gameObjectArray[i], typeof(GameObject), true);
                        if (GUILayout.Button("Remove", GUILayout.Width(100)))
                            RemoveElementAt(i);

                        EditorGUILayout.EndHorizontal();
                    }
                }

                EditorGUILayout.EndScrollView();

                if (GUILayout.Button("Add GameObject"))
                    AddNewElement();

                break;
        }
    }

    private void AddNewElement()
    {
        // Resize the array and add a new null element
        int newSize = gameObjectArray.Length + 1;
        System.Array.Resize(ref gameObjectArray, newSize);
    }

    private void RemoveElementAt(int index)
    {
        // Create a new array excluding the removed element
        for (int i = index; i < gameObjectArray.Length - 1; i++)
        {
            gameObjectArray[i] = gameObjectArray[i + 1];
        }

        // Resize the array to remove the last element
        System.Array.Resize(ref gameObjectArray, gameObjectArray.Length - 1);
    }

    private void ButtonGUI()
    {
        EditorGUI.BeginDisabledGroup(Application.isPlaying && currentGetSpriteType == GetSpriteType.FromScene);
        if (GUILayout.Button("Get All Sprite"))
        {
            Clear();
            gameObjectArray = gameObjectArray.Where(x => x != null).ToArray();
            OnGetAllSpriteButtonClickEvent?.Invoke();

            switch (currentGetSpriteType)
            {
                case GetSpriteType.FromScene:
                    var scenePath = AssetDatabase.GetAssetPath(sceneAsset);
                    var openedScene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
                    GetAllTexture2DInScene(openedScene);
                    EditorSceneManager.CloseScene(openedScene, true);
                    break;

                case GetSpriteType.FromGameObject:
                    GetAllTexture2DInGameObject(gameObjectArray);
                    break;
            }
        }
        EditorGUI.EndDisabledGroup();
    }

    private void GetAllTexture2DInScene(Scene scene)
    {
        var allGameObjectInScene = scene.GetRootGameObjects();
        var allImageList = new List<Image>();
        for (var i = 0; i < allGameObjectInScene.Length; i++)
        {
            var gameObject = allGameObjectInScene[i];
            var image = gameObject.GetComponentsInChildren<Image>(true);
            allImageList.AddRange(image);
        }

        Debug.Log($"Found {allImageList.Count} Image components in the scene: {scene.name}");

        ProcessGetAllSprite(allImageList.ToArray());
    }

    private void GetAllTexture2DInGameObject(GameObject[] gameObjects)
    {
        var allImageList = new List<Image>();
        for (var i = 0; i < gameObjects.Length; i++)
        {
            var gameObject = gameObjects[i];
            var image = gameObject.GetComponentsInChildren<Image>(true);
            allImageList.AddRange(image);
        }

        Debug.Log($"Found {allImageList.Count} Image components");

        ProcessGetAllSprite(allImageList.ToArray());
    }

    private void ProcessGetAllSprite(Image[] images)
    {
        var spriteList = GetSpriteFromImages(images);
        var filterSpriteList = RemoveDuplicateAndNullSprite(spriteList);

        allTexture2DAssetPath = GetSpriteAssetPath(filterSpriteList);

        var texture2d = new List<Texture2D>();
        for (var i = 0; i < allTexture2DAssetPath.Length; i++)
        {
            var texture = AssetDatabase.LoadAssetAtPath<Texture2D>(allTexture2DAssetPath[i]);
            texture2d.Add(texture);
        }

        allTexture2D = texture2d.ToArray();

        for (var i = 0; i < allTexture2D.Length; i++)
        {
            var spriteIndex = i;
            var pack = new SelectedToPackContainer();
            pack.SetData(defaultSelectToPack, spriteIndex);
            selectedTexture2DToPackList.Add(pack);
        }

        if (defaultSelectToPack == false)
            return;

        SelectedTexture2DToPack = allTexture2D;
        SelectedTexture2DToPackAssetPath = allTexture2DAssetPath;
    }

    private Sprite[] GetSpriteFromImages(Image[] images)
    {
        var spriteList = new List<Sprite>();
        foreach (var image in images)
            spriteList.Add(image.sprite);

        Debug.Log($"Found {spriteList.Count} Sprite");

        return spriteList.ToArray();
    }

    private Sprite[] RemoveDuplicateAndNullSprite(Sprite[] sprites)
    {
        var filterNullOrDuplicateSpriteList = sprites.Where(sprite => sprite != null).ToArray(); //remove null
        filterNullOrDuplicateSpriteList = filterNullOrDuplicateSpriteList.Distinct().ToArray(); //remove duplicate

        var filterSpriteNotInAssets = new List<Sprite>();
        for (var i = 0; i < filterNullOrDuplicateSpriteList.Length; i++)
        {
            var assets = filterNullOrDuplicateSpriteList[i];
            var path = AssetDatabase.GetAssetPath(assets.GetInstanceID());

            if (!path.Contains("Assets"))
                continue;

            filterSpriteNotInAssets.Add(assets);
        }

        Debug.Log($"Remove duplicate or null sprite down to {filterSpriteNotInAssets.Count} Sprite");

        return filterSpriteNotInAssets.ToArray();
    }

    private string[] GetSpriteAssetPath(Sprite[] sprites)
    {
        var assetPathList = new List<string>();
        foreach (var assets in sprites)
        {
            var path = AssetDatabase.GetAssetPath(assets.GetInstanceID());
            if (string.IsNullOrWhiteSpace(path))
            {
                Debug.Log($"Invalid asset path for sprite: {assets}");
                continue;
            }

            assetPathList.Add(path);

            Debug.Log($"Path: {path}");
        }

        return assetPathList.ToArray();
    }

    private void DisplayAllTexture2D()
    {
        EditorGUILayout.LabelField("All Sprite List", EditorStyles.boldLabel);

        var elementCount = allTexture2D.Length;
        var estimatedHeight = (elementCount * 30f) + 10f; // Approximate per-item height
        var scrollHeight = Mathf.Clamp(estimatedHeight, 50, 200);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(scrollHeight));

        for (int i = 0; i < allTexture2D.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();

            var isPack = EditorGUILayout.ToggleLeft($"{allTexture2DAssetPath[i]}", selectedTexture2DToPackList[i].isSelectToPack);
            UpdateSelectedSpriteIndexToPack(i, isPack);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(allTexture2D[i], typeof(Texture2D), false);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(3);
            EditorWindowsHelper.BreakLine();
            GUILayout.Space(5);
        }

        EditorGUILayout.EndScrollView();
    }

    private void UpdateSelectedSpriteIndexToPack(int index, bool isPack)
    {
        var isUpdate = selectedTexture2DToPackList[index].Update(isPack);
        if (!isUpdate)
            return;

        UpdateSelectedTexture2DToPack();
        OnUpdateSelectedSpriteIndexToPackEvent?.Invoke();
    }

    private void UpdateSelectedTexture2DToPack()
    {
        var selectToPack = new List<Texture2D>();
        var selectToPackAssetPath = new List<string>();
        for (var i = 0; i < selectedTexture2DToPackList.Count; i++)
        {
            if (!selectedTexture2DToPackList[i].isSelectToPack)
                continue;

            selectToPack.Add(allTexture2D[selectedTexture2DToPackList[i].texture2DIndex]);
            selectToPackAssetPath.Add(allTexture2DAssetPath[selectedTexture2DToPackList[i].texture2DIndex]);
        }

        SelectedTexture2DToPack = selectToPack.ToArray();
        SelectedTexture2DToPackAssetPath = selectToPackAssetPath.ToArray();
    }

    public void Clear()
    {
        allTexture2D = null;
        allTexture2DAssetPath = null;
        SelectedTexture2DToPack = null;
        SelectedTexture2DToPackAssetPath = null;
        selectedTexture2DToPackList = new();
    }
}