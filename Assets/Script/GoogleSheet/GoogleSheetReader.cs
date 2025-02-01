using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;


public class GoogleSheetReader
{
    private const string baseUrl = "https://sheets.googleapis.com/v4/spreadsheets";
    private string apiKey = "AIzaSyDKyvJenSB3qEdCRIi7NZeTHaByI_AG4fQ";
    private string apiUrl;

    public async UniTask<string> ReadAsync(GoogleSheetReaderScriptableObject readerScriptableObject)
    {
        var spreadsheetId = readerScriptableObject.spreadsheetId;
        var sheetName = readerScriptableObject.sheetName;

        apiUrl = $"{baseUrl}/{spreadsheetId}/values/{sheetName}?key={apiKey}";

        Log.Logging($"Reading sheet: {readerScriptableObject.sheetName}");
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        await request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var rawData = request.downloadHandler.text;
            Debug.Log("Response: " + rawData);

            return rawData;
        }
        else
        {
            Debug.LogError("Error fetching data: " + request.error);

            return null;
        }
    }
}
