using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.U2D;

public class AtlantisSpriteAtlasController : AtlantisComponent
{
    private string atlasName;
    private List<SpriteAtlas> allAtlasInProject = new();
    private List<string> allAtlasAssetPathInProject = new();

    private Texture2D[] selectedTexture2DToPack => atlantis.SelectedTexture2DToPack;

    public IReadOnlyList<SpriteAtlas> AllAtlasInProject => allAtlasInProject;
    public IReadOnlyList<string> AllAtlasAssetPathInProject => allAtlasAssetPathInProject;

    public void GUI()
    {
        CreateNewAtlasGUI();

        AddSpriteToExistingAtlasGUI();
    }

    private void CreateNewAtlasGUI()
    {
        atlasName = EditorGUILayout.TextField("Atlas Name", atlasName);
        GUILayout.Space(5);

        if (GUILayout.Button("Create New Atlas"))
        {
            if (string.IsNullOrWhiteSpace(atlasName))
            {
                Debug.Log($"Atlas name is empty");
                return;
            }

            var folderPath = EditorUtility.OpenFolderPanel("Select save location", "Assets", "");
            if (string.IsNullOrEmpty(folderPath))
                return;

            if (selectedTexture2DToPack == null || selectedTexture2DToPack.Length == 0)
            {
                Debug.Log($"No Sprite to packed");
                return;
            }

            atlantis.ProcessDuplicateOperation();

            var filterSelectedTexture2D = atlantis.FilterSkipSprite();

            var atlas = new SpriteAtlasAsset();
            var path = folderPath + "/" + atlasName + ".spriteatlasv2";
            if (File.Exists(path))
                atlas = SpriteAtlasAsset.Load(path);

            AddTexture2DToAtlas(atlas, filterSelectedTexture2D);

            var spriteAtlasSavePath = folderPath;
            CreateAtlas(atlas, spriteAtlasSavePath, atlasName);
        }
    }

    private void AddSpriteToExistingAtlasGUI()
    {
        if (GUILayout.Button("Add Sprite To The Existing Atlas"))
        {
            var filePath = EditorUtility.OpenFilePanelWithFilters($"Select sprite atlas", "", new[] { "SpriteAtlas", "spriteatlasv2" });
            if (string.IsNullOrEmpty(filePath))
                return;

            atlantis.ProcessDuplicateOperation();

            var filterSelectedTexture2D = atlantis.FilterSkipSprite();

            var atlas = SpriteAtlasAsset.Load(filePath);
            AddTexture2DToAtlas(atlas, filterSelectedTexture2D);

            CreateAtlas(atlas, filePath);
        }
    }

    public void CreateAtlas(SpriteAtlasAsset spriteAtlasAssets, string saveLocation, string atlasName)
    {
        if (!Directory.Exists(saveLocation))
        {
            Directory.CreateDirectory(saveLocation);
            AssetDatabase.Refresh();
        }

        var index = saveLocation.IndexOf("Assets");
        if (index != -1)
            saveLocation = saveLocation.Substring(index);

        var fullPath = saveLocation + "/" + atlasName + ".spriteatlasv2";
        SpriteAtlasAsset.Save(spriteAtlasAssets, fullPath);
        AssetDatabase.Refresh();

        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(fullPath, typeof(object)));
    }

    public void CreateAtlas(SpriteAtlasAsset spriteAtlasAssets, string fullPath)
    {
        var index = fullPath.IndexOf("Assets");
        if (index != -1)
            fullPath = fullPath.Substring(index);

        SpriteAtlasAsset.Save(spriteAtlasAssets, fullPath);
        AssetDatabase.Refresh();

        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(fullPath, typeof(object)));
    }

    public void AddTexture2DToAtlas(SpriteAtlasAsset spriteAtlasAssets, Object[] texture2Ds)
    {
        spriteAtlasAssets.Add(texture2Ds);
    }

    public void AddTexture2DToAtlas(int atlasIndex, Object[] texture2Ds)
    {
        var atlasAssetsPath = allAtlasAssetPathInProject[atlasIndex];
        var atlasAssets = SpriteAtlasAsset.Load(atlasAssetsPath);
        atlasAssets.Add(texture2Ds);

        SpriteAtlasAsset.Save(atlasAssets, atlasAssetsPath);
        AssetDatabase.Refresh();
    }

    public void RemoveTexture2DFromAtlas(int atlasIndex, Object[] texture2Ds)
    {
        var atlasAssetsPath = allAtlasAssetPathInProject[atlasIndex];
        var atlasAssets = SpriteAtlasAsset.Load(atlasAssetsPath);
        atlasAssets.Remove(texture2Ds);

        SpriteAtlasAsset.Save(atlasAssets, atlasAssetsPath);
        AssetDatabase.Refresh();
    }

    public void GetAllAtlasInProject()
    {
        string[] atlasGUIDs = AssetDatabase.FindAssets("t:SpriteAtlas");

        for (var i = 0; i < atlasGUIDs.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(atlasGUIDs[i]);
            allAtlasInProject.Add(AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path));
            allAtlasAssetPathInProject.Add(path);
        }
    }

    public void Clear()
    {
        atlasName = null;
        allAtlasInProject.Clear();
        allAtlasAssetPathInProject.Clear();
    }
}