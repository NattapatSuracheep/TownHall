using System.Collections.Generic;
using com.cyborgAssets.inspectorButtonPro;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class TestGoogleSheetReader : MonoBehaviour
{
    [SerializeField] private GoogleSheetReaderScriptableObject testGoogleSheetReader;
    private GoogleSheetReader googleSheetReader = new();
    private GoogleSheetParser googleSheetParser = new();

    private string rawData;

    [ProButton]
    public async UniTask TestRead()
    {
        rawData = await googleSheetReader.ReadAsync(testGoogleSheetReader);
        Debug.Log(rawData);
    }

    [ProButton]
    public void TestParseJson(string rawData)
    {
        var b = googleSheetParser.ParseToClass<TestClass[]>(rawData, testGoogleSheetReader.dataBeginRow);
        Debug.Log(JsonConvert.SerializeObject(b, Formatting.Indented));
    }

    [ProButton]
    public async UniTask FullTest()
    {
        await TestRead();
        TestParseJson(rawData);
    }

    private class TestClass
    {
        public class StatusPrototype2
        {
            public List<int> hp { get; set; }
            public float[] atk { get; set; }
            public float def { get; set; }
            public float matk { get; set; }
            public float mdef { get; set; }
            public float spd { get; set; }
            public float crit_rate { get; set; }
            public float crit_dmg { get; set; }
        }

        public class Aa
        {
            public string[] aa1 { get; set; }
            public int[] aa2 { get; set; }
            public string aa3 { get; set; }
        }

        public string id { get; set; }
        public StatusPrototype2 status { get; set; } = new();

        public string data1 { get; set; }
        public string data2 { get; set; }

        public Aa aa { get; set; } = new();

        public int[] data3 { get; set; }
        public List<int> data4 { get; set; }
        public float[] data5 { get; set; }
    }

    public class TestLocalization
    {
        public string key { get; set; }
        public string content { get; set; }
    }
}