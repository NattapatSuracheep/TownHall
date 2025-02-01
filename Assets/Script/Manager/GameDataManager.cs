using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class GameDataManager
{
    public const string configLabel = "config";

    private TownHallDataManager townHallDataManager = new();
    private SceneNavigator SceneNavigator => GameManager.Instance.SceneNavigator;

    public TownHallPrototypeData TownHallPrototypeData { get; private set; }

    public async UniTask InitializeAsync()
    {
        Log.Call();

        TownHallPrototypeData = await townHallDataManager.LoadDataAsync();

        Log.Array(TownHallPrototypeData.DataList);
    }

    public async UniTask LoadSequenceAsync(Func<UniTask>[] func, string info = null)
    {
        var totalFunc = func.Length;
        var tasks = new UniTask[totalFunc];

        for (var i = 0; i < totalFunc; i++)
        {
            var task = func[i];

            float progress = (float)(i + 1) / totalFunc;
            SceneNavigator.UpdateLoadingPanel($"loading {info ?? task.Method.Name}... ({i}/{totalFunc})", progress);

            tasks[i] = task.Invoke();
        }

        await UniTask.WhenAll(tasks);
    }
}