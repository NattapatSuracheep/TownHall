
using UnityEngine;

[CreateAssetMenu(fileName = "GoogleSheetReader", menuName = "GoogleSheet/GoogleSheetReader")]
public class GoogleSheetReaderScriptableObject : ScriptableObject
{
    public string spreadsheetId;
    public string sheetName;
    public int dataBeginRow = 2;
}