using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using Data = GoogleSheetEditorScriptableObject.Data;
using Cysharp.Threading.Tasks;
using System;
using System.IO;
using System.Linq;

public class GoogleSheetEditorWindow : EditorWindow
{
    private const string editorDataPath = "Assets/Script/GoogleSheet/Editor/GoogleSheetEditorWindowData.asset";

    public List<Data> dataList = new()
    {
        new Data(){StoredType = typeof(TownHallRawData[])},
    };

    private Vector2 scrollPosition;

    private GoogleSheetEditorScriptableObject editorData;
    private GoogleSheetManager googleSheetManager = new();

    [MenuItem("Tools/GoogleSheetEditorWindow")]
    public static void GoogleSheetEditorWindowOpen()
    {
        GetWindow<GoogleSheetEditorWindow>("Google Sheet Editor");
    }

    private void OnGUI()
    {
        if (editorData == null)
        {
            LoadEditorData();
            ValidateEditorData();
        }

        editorData = (GoogleSheetEditorScriptableObject)EditorGUILayout.ObjectField("Editor Data", editorData, typeof(GoogleSheetEditorScriptableObject), false);

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        for (int i = 0; i < editorData.dataList.Count; i++)
        {
            EditorGUILayout.LabelField("Type: " + editorData.dataList[i].StoredType.ToString());
            GUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            editorData.dataList[i].savePath = EditorGUILayout.TextField("Save Path: ", editorData.dataList[i].savePath);
            if (GUILayout.Button("Select Save Path"))
            {
                var path = EditorUtility.OpenFolderPanel("Select save location", "Assets", "") + $"/{editorData.dataList[i].StoredType.Name}.json";

                var index = path.IndexOf("Assets");
                if (index != -1)
                    editorData.dataList[i].savePath = path.Substring(index);
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(5);

            editorData.dataList[i].readerData = EditorGUILayout.ObjectField("Reader Data: ",
                editorData.dataList[i].readerData, typeof(GoogleSheetReaderScriptableObject), false) as GoogleSheetReaderScriptableObject;

            GUILayout.Space(5);
            if (GUILayout.Button("Load"))
            {
                LoadSheet(editorData.dataList[i]).Forget();
            }

            GUILayout.Space(20);
            EditorWindowsHelper.HorizontalLine();
            GUILayout.Space(20);
        }

        EditorGUILayout.EndScrollView();
    }

    private async UniTask LoadSheet(Data data)
    {
        var storedType = data.StoredType;
        var method = typeof(GoogleSheetManager).GetMethod("GetSheetData").MakeGenericMethod(storedType);
        dynamic task = method.Invoke(googleSheetManager, new object[] { data.readerData });

        var result = await task; // Works because 'await' handles dynamic UniTask<T>

        object typedResult = Convert.ChangeType(result, storedType);

        var directory = Path.GetDirectoryName(data.savePath);
        var fileNameWithoutExt = Path.GetFileNameWithoutExtension(data.savePath);
        FileManager.SaveJsonFile(directory, fileNameWithoutExt, typedResult);

        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(data.savePath, typeof(object)));

        AssetDatabase.Refresh();
    }

    private void LoadEditorData()
    {
        try
        {
            editorData = AssetDatabase.LoadAssetAtPath<GoogleSheetEditorScriptableObject>(editorDataPath);
        }
        catch { }
    }

    private void ValidateEditorData()
    {
        foreach (var item in dataList)
        {
            if (editorData.dataList.FindIndex(x => x.StoredType == item.StoredType) == -1)  // Using FindIndex instead of Contains
            {
                editorData.dataList.Add(item);
            }
        }

        var dataToRemove = editorData.dataList.Where(x => dataList.FindIndex(y => y.StoredType == x.StoredType) == -1).ToList();
        foreach (var item in dataToRemove)
        {
            editorData.dataList.Remove(item);
        }
    }
}
