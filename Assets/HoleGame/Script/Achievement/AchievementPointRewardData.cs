using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AcievementPointRewardData", menuName = "Scriptable Objects/AcievementPointRewardData")]
public class AchievementPointRewardData : ScriptableObject
{
    public List<PointRewardData> PointRewardDatas = new List<PointRewardData>();
    public int MaxPoint;

    [System.Serializable]
    public struct PointRewardData
    {
        public PointRewardEnum type;
        
        public string Reward;
    }
}
