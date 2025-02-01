using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TownHallObj : MonoBehaviour
{
    public class Data
    {
        public List<TownHallPrototypeData.Data> PrototypeData { get; private set; }
        public bool IsFirst { get; private set; }
        public bool IsLast { get; private set; }

        public Data(List<TownHallPrototypeData.Data> data, bool isFirst, bool isLast)
        {
            PrototypeData = data;
            IsFirst = isFirst;
            IsLast = isLast;
        }
    }

    [SerializeField] private GameObject topLine;
    [SerializeField] private GameObject midLine;
    [SerializeField] private GameObject botLine;
    [SerializeField] private TownHallObj_2 obj_2;
    [SerializeField] private Transform container;
    [SerializeField] private TMP_Text dateTime;

    public void Initialize(string date, Data data)
    {
        dateTime.text = date;

        var data2 = data.PrototypeData;
        for (var i = 0; i < data2.Count; i++)
        {
            var item = data2.ElementAt(i);
            var obj = GameObject.Instantiate(obj_2, container);
            obj.Initialize(i, item.Topic, item.Message.ToArray());
        }

        if (data.IsFirst)
            topLine.SetActive(true);
        else if (data.IsLast)
            botLine.SetActive(true);
        else
            midLine.SetActive(true);
    }
}
