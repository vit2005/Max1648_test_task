using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Database
{
    private static string filePath = Application.dataPath + "/TrainingData/";

    public static void Init()
    {
        filePath += DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToLongTimeString().Replace(":", "_") + "/";
        Directory.CreateDirectory(filePath);
    }

    public static void SaveTrainingData(string botId, List<TrainingData> trainingLog)
    {
        string json = JsonUtility.ToJson(new TrainingDataWrapper(trainingLog));
        File.WriteAllText(filePath + botId, json);
    }

    public static List<TrainingData> LoadTrainingData(string botId)
    {
        if (File.Exists(filePath + botId))
        {
            string json = File.ReadAllText(filePath + botId);
            TrainingDataWrapper dataWrapper = JsonUtility.FromJson<TrainingDataWrapper>(json);
            return dataWrapper.trainingDataList;
        }
        return new List<TrainingData>();
    }
}