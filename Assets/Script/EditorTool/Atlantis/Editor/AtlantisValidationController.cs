using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

public class AtlantisValidationController : AtlantisComponent
{
    public enum DuplicateOperation
    {
        PackAnyway,
        MergeToNewAtlas,
        Skip,
    }

    public class MergeTexture2DContainer
    {
        public int atlasIndex;
        public List<Texture2D> texture2DList;

        public MergeTexture2DContainer(int atlasIndex, List<Texture2D> texture2DList)
        {
            this.atlasIndex = atlasIndex;
            this.texture2DList = texture2DList;
        }
    }

    private class DuplicateTexture2DContainer
    {
        public int atlasIndex;
        public int texture2DIndex;
        public DuplicateOperation operation;

        public DuplicateTexture2DContainer(int atlasIndex, int texture2DIndex, DuplicateOperation operation)
        {
            this.atlasIndex = atlasIndex;
            this.texture2DIndex = texture2DIndex;
            this.operation = operation;
        }

        public void UpdateOperation(DuplicateOperation operation)
        {
            this.operation = operation;
        }
    }

    private Vector2 scrollPosition;

    private List<string> duplicateOperationList = new();
    private List<DuplicateTexture2DContainer> duplicateTexture2DList;
    private List<MergeTexture2DContainer> mergeList;
    private List<Texture2D> skipList;
    private GUIContent operationTooltips;

    private IReadOnlyList<SpriteAtlas> allAtlasInProject => atlantis.AllAtlasInProject;
    private Texture2D[] selectedToPackTexture2D => atlantis.SelectedTexture2DToPack;
    private DuplicateOperation defaultDuplicateOperation => atlantis.DefaultDuplicateOperation;
    private bool isProcessDuplicateOperation;

    public IReadOnlyList<MergeTexture2DContainer> MergeList => mergeList;
    private bool isValidate => duplicateTexture2DList != null;
    private bool isHaveDuplicateTexture2D => duplicateTexture2DList != null && duplicateTexture2DList.Count > 0;

    public override void Initialize(AtlantisAtlasPacker atlantis)
    {
        base.Initialize(atlantis);

        CreateOperationTooltips();
        CreateOperationList();
    }

    private void CreateOperationTooltips()
    {
        var toolTipString = new StringBuilder();
        toolTipString.Append($"PackAnyway - Ignore the duplicate sprite and pack to the new atlas anyway\n\n");
        toolTipString.Append($"Skip - Didn't pack the duplicate sprite\n\n");
        toolTipString.Append($"MergeToNewAtlas - Remove the sprite out of the duplicate atlas and merge into a new atlas");
        operationTooltips = new GUIContent
        (
            "Operation",
            $"{toolTipString}"
        );
    }

    private void CreateOperationList()
    {
        duplicateOperationList.Clear();

        var duplicateOperationEnumLength = Enum.GetValues(typeof(DuplicateOperation)).Length;
        for (var i = 0; i < duplicateOperationEnumLength; i++)
            duplicateOperationList.Add(((DuplicateOperation)i).ToString());
    }

    public bool GUI()
    {
        if (GUILayout.Button("Validate Duplicate Sprite In Other Atlas"))
            ValidateDuplicateTexture2DInOtherAtlas();

        if (isValidate == false)
            return false;

        if (isHaveDuplicateTexture2D == false)
        {
            EditorGUILayout.LabelField($"No duplicate sprite in other atlas");
        }

        GUILayout.Space(10);
        EditorWindowsHelper.HorizontalLine();
        GUILayout.Space(10);

        DisplayDuplicateGUI();

        if (isValidate == false)
        {
            return false;
        }

        return true;
    }

    private void ValidateDuplicateTexture2DInOtherAtlas()
    {
        atlantis.ClearValidationData();
        atlantis.GetAllAtlasInProject();

        duplicateTexture2DList = new();
        for (var i = 0; i < allAtlasInProject.Count; i++)
        {
            var atlasIndex = i;
            var atlas = allAtlasInProject[atlasIndex];
            for (var j = 0; j < selectedToPackTexture2D.Length; j++)
            {
                var texture2DIndex = j;
                var texture2D = selectedToPackTexture2D[texture2DIndex];
                if (atlas.GetSprite(texture2D.name) == null)
                    continue;

                duplicateTexture2DList.Add
                (
                    new
                    (
                        atlasIndex,
                        texture2DIndex,
                        defaultDuplicateOperation
                    )
                );
            }
        }
    }

    private void DisplayDuplicateGUI()
    {
        if (isHaveDuplicateTexture2D == false)
            return;

        EditorGUILayout.LabelField("Duplicate Sprite In Other Atlas", EditorStyles.boldLabel);

        var elementCount = duplicateTexture2DList.Count;
        var estimatedHeight = (elementCount * 30f) + 10f; // Approximate per-item height
        var scrollHeight = Mathf.Clamp(estimatedHeight, 50, 200);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(scrollHeight));

        for (int i = 0; i < duplicateTexture2DList.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField(selectedToPackTexture2D[duplicateTexture2DList[i].texture2DIndex], typeof(Texture2D), false);
            EditorGUILayout.ObjectField(allAtlasInProject[duplicateTexture2DList[i].atlasIndex], typeof(SpriteAtlas), false);
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(isProcessDuplicateOperation);
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField(operationTooltips, GUILayout.ExpandWidth(false));
            var operation = (DuplicateOperation)EditorGUILayout.Popup
            (
                (int)duplicateTexture2DList[i].operation,
                duplicateOperationList.ToArray(),
                GUILayout.MinWidth(100)
            );
            duplicateTexture2DList[i].UpdateOperation(operation);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(3);
            EditorWindowsHelper.BreakLine();
            GUILayout.Space(5);
        }

        EditorGUILayout.EndScrollView();
    }

    public void ProcessDuplicateOperation()
    {
        mergeList = new();
        skipList = new();
        for (var i = 0; i < duplicateTexture2DList.Count; i++)
        {
            var atlasIndex = duplicateTexture2DList[i].atlasIndex;
            var texture2D = selectedToPackTexture2D[duplicateTexture2DList[i].texture2DIndex];
            var operation = duplicateTexture2DList[i].operation;

            switch (operation)
            {
                case DuplicateOperation.PackAnyway:
                    continue;
                case DuplicateOperation.MergeToNewAtlas:
                    var mergeListIndex = mergeList.FindIndex(merge => merge.atlasIndex == atlasIndex);
                    if (mergeListIndex == -1)
                    {
                        mergeList.Add(new(atlasIndex, new List<Texture2D> { texture2D }));
                    }
                    else
                    {
                        mergeList[mergeListIndex].texture2DList.Add(texture2D);
                    }

                    break;
                case DuplicateOperation.Skip:
                    skipList.Add(texture2D);
                    continue;
            }
        }

        isProcessDuplicateOperation = true;
    }

    public Texture2D[] FilterSkipSprite(Texture2D[] texture2D)
    {
        return texture2D.Where(sprite => !skipList.Contains(sprite)).ToArray();
    }

    public void Clear()
    {
        duplicateTexture2DList = null;
        isProcessDuplicateOperation = false;
    }
}