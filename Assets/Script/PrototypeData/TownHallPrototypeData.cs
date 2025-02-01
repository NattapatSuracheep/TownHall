using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TownHallPrototypeData
{
    public class Data
    {
        public string Topic;
        public List<string> Message = new();

        public Data(string topic, string message)
        {
            Topic = topic;
            Message.Add(message);
        }
    }

    public Dictionary<string, List<Data>> DataList { get; private set; } = new();

    private List<Data> currentListData;
    private int currentTopicIndex;

    public void TryAdd(string date, string topic, string message)
    {
        if (string.IsNullOrEmpty(date) && string.IsNullOrEmpty(topic))
        {
            AddDataToCurrentDataList(message);
            return;
        }

        else if (string.IsNullOrEmpty(date) && !string.IsNullOrEmpty(topic))
        {
            currentListData.Add(new Data(topic, message));
            return;
        }

        else if (string.IsNullOrEmpty(date) && !string.IsNullOrEmpty(topic))
        {
            Debug.LogError($"Your data is in wrong format ¯\\_(ツ)_/¯");
            return;
        }

        if (DataList.TryGetValue(date, out currentListData))
        {
            currentTopicIndex = currentListData.FindIndex(x => x.Topic == topic);
            if (currentTopicIndex == -1)
            {
                currentListData.Add(new Data(topic, message));
                currentTopicIndex = currentListData.Count - 1;
            }
            else
            {
                currentListData[currentTopicIndex].Message.Add(message);
            }
        }
        else
        {
            DataList.Add(date, new() { new Data(topic, message) });
            currentListData = DataList[date];
            currentTopicIndex = 0;
        }
    }

    private void AddDataToCurrentDataList(string message)
    {
        currentListData[currentTopicIndex].Message.Add(message);
    }
}