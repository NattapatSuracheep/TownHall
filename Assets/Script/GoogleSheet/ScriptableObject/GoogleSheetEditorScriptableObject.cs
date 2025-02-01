
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GoogleSheetEditorWindowData", menuName = "GoogleSheet/GoogleSheetEditorWindowData")]
public class GoogleSheetEditorScriptableObject : ScriptableObject
{
    [Serializable]
    public class Data
    {
        public string assemblyName;
        public GoogleSheetReaderScriptableObject readerData;
        public string savePath;

        public Type StoredType
        {
            get
            {
                if (!string.IsNullOrEmpty(assemblyName))
                {
                    return Type.GetType(assemblyName);
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    assemblyName = value.FullName; // Store the fully qualified name of the type
                }
                else
                {
                    assemblyName = null;
                }
            }
        }
    }

    public List<Data> dataList = new();
}