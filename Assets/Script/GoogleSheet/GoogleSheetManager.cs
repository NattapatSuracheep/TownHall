using Cysharp.Threading.Tasks;

public class GoogleSheetManager
{
    private GoogleSheetReader reader = new();
    private GoogleSheetParser parser = new();

    public async UniTask<T> GetSheetData<T>(GoogleSheetReaderScriptableObject readerScriptableObject)
    {
        var rawData = await reader.ReadAsync(readerScriptableObject);

        return parser.ParseToClass<T>(rawData, readerScriptableObject.dataBeginRow);
    }
}