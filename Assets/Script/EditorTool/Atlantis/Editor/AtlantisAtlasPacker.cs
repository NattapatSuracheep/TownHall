using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

public class AtlantisAtlasPacker : EditorWindow
{
    private Vector2 scrollPosition;

    public Texture2D[] SelectedTexture2DToPack => getTexture2DModules.SelectedTexture2DToPack;
    public IReadOnlyList<AtlantisValidationController.MergeTexture2DContainer> MergeTexture2DList => validationController.MergeList;
    public IReadOnlyList<SpriteAtlas> AllAtlasInProject => spriteAtlasController.AllAtlasInProject;

    public bool DefaultSelectToPack => behaviorController.DefaultSelectToPack;
    public AtlantisValidationController.DuplicateOperation DefaultDuplicateOperation => behaviorController.DefaultDuplicateOperation;

    private AtlantisGetTexture2DModule getTexture2DModules = new();
    private AtlantisValidationController validationController = new();
    private AtlantisSpriteAtlasController spriteAtlasController = new();
    private AtlantisBehaviorController behaviorController = new();

    private static bool isInitialized;

    [MenuItem("Tools/Atlantis | Atlas Packer")]
    public static void ShowWindow()
    {
        isInitialized = false;

        GetWindow<AtlantisAtlasPacker>("Atlantis : The Atlas Packer");
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void OnScriptReload()
    {
        isInitialized = false;
    }

    private void OnGUI()
    {
        if (!isInitialized)
        {
            Clear();
            ClearValidationData();

            getTexture2DModules.OnSelectTypeChangeEvent = ClearValidationData;
            getTexture2DModules.OnGetAllSpriteButtonClickEvent = ClearValidationData;
            getTexture2DModules.OnUpdateSelectedSpriteIndexToPackEvent = ClearValidationData;

            getTexture2DModules.Initialize(this);
            validationController.Initialize(this);
            spriteAtlasController.Initialize(this);
            behaviorController.Initialize(this);

            isInitialized = true;
        }

        behaviorController.GUI();

        GUILayout.Space(10);
        EditorWindowsHelper.HorizontalLine();
        GUILayout.Space(10);

        if (getTexture2DModules.GUI() == false)
            return;

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        GUILayout.Space(10);
        EditorWindowsHelper.HorizontalLine();
        GUILayout.Space(10);

        if (validationController.GUI() == false)
        {
            EditorGUILayout.EndScrollView();
            return;
        }

        GUILayout.Space(10);
        EditorWindowsHelper.HorizontalLine();
        GUILayout.Space(10);

        spriteAtlasController.GUI();

        GUILayout.Space(10);
        EditorWindowsHelper.HorizontalLine();
        GUILayout.Space(10);

        EditorGUILayout.EndScrollView();
    }

    public void GetAllAtlasInProject()
    {
        spriteAtlasController.GetAllAtlasInProject();
    }

    public void ProcessDuplicateOperation()
    {
        validationController.ProcessDuplicateOperation();
        for (var i = 0; i < MergeTexture2DList.Count; i++)
        {
            var atlasIndex = MergeTexture2DList[i].atlasIndex;
            var texture2DList = MergeTexture2DList[i].texture2DList;
            spriteAtlasController.RemoveTexture2DFromAtlas(atlasIndex, texture2DList.ToArray());
        }
    }

    public Texture2D[] FilterSkipSprite()
    {
        return validationController.FilterSkipSprite(SelectedTexture2DToPack);
    }

    private void Clear()
    {
        getTexture2DModules.Clear();
        spriteAtlasController.Clear();
    }

    public void ClearValidationData()
    {
        spriteAtlasController.Clear();
        validationController.Clear();
    }
}