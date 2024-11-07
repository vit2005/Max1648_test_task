using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Database
{
    private static string baseFilePath = Application.dataPath + "/TrainingData/";
    private static string filePath = Application.dataPath + "/TrainingData/";

    public static void Init()
    {
        filePath += DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToLongTimeString().Replace(":", "_") + "/";
        Directory.CreateDirectory(filePath);
    }

    public static void SaveTrainingData(string botId, List<TrainingData> trainingLog)
    {
        string json = JsonUtility.ToJson(new TrainingDataWrapper(botId, trainingLog));
        File.WriteAllText(filePath + botId, json);
    }

    public static List<TrainingDataWrapper> LoadTrainingData()
    {
        List<TrainingDataWrapper> result = new List<TrainingDataWrapper>();

        var directories = Directory.GetDirectories(baseFilePath);
        foreach (var folder in directories)
        {
            var files = Directory.GetFiles(folder);
            foreach (var item in files)
            {
                if (item.Contains(".meta")) continue;

                string json = File.ReadAllText(item);
                TrainingDataWrapper dataWrapper = JsonUtility.FromJson<TrainingDataWrapper>(json);
                result.Add(dataWrapper);
            }
        }

        return result;
    }
}