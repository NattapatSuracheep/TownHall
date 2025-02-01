using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

public class TownHallDataManager
{
    private const string path = "Assets/Addressable/Configs/TownHallRawData.json";

    public async UniTask<TownHallPrototypeData> LoadDataAsync()
    {
        var data = (await AddressableManager.GetTextAssetAsync(path, false)).text;
        return Convert(data);
    }

    public TownHallPrototypeData Convert(string rawStringData)
    {
        var rawDatas = JsonConvert.DeserializeObject<TownHallRawData[]>(rawStringData);

        var result = new TownHallPrototypeData();

        for (var i = 0; i < rawDatas.Length; i++)
        {
            var data = rawDatas[i];
            result.TryAdd(data.Date, data.Topic, data.Message);
        }

        return result;
    }
}