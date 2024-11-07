using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Prediction : MonoBehaviour
{
    private static Prediction _instance;
    public static Prediction Instance => _instance;

    public List<TrainingDataWrapper> datas;
    private List<TrainingData> playerData = new List<TrainingData>();

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        datas = Database.LoadTrainingData();
        foreach (var data in datas)
        {
            if (data.botTitle.Contains("Friendly")) playerData.AddRange(data.trainingDataList);
        }
    }

    public GridPosition GetAveragePlayerPositionAtMove(int turn)
    {
        int x = 0, z = 0;
        IEnumerable<TrainingData> dataTurn = playerData.Where(x => x.turn == turn);
        int length = dataTurn.Count();
        foreach (var data in dataTurn)
        {
            x += data.position.x;
            z += data.position.z;
        }

        return new GridPosition((int)((float)x / length), (int)((float)z / length));
    }

    public int GetAverageTotalPlayerHealth(int turn)
    {
        int hp = 0;
        IEnumerable<TrainingData> dataTurn = playerData.Where(x => x.turn == turn);
        int length = dataTurn.Count();
        foreach (var data in dataTurn)
        {
            hp += data.heuristicDatas.Where(x => x.heuristic == HeuristicType.TotalHealth).Sum(x => x.value);
        }
        return (int)((float)hp / length);
    }
}
