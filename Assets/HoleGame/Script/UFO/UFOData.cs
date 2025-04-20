using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UFOData", menuName = "Scriptable Objects/UFOData")]
public class UFOData : ScriptableObject
{
    public string UFOName;
    public Mesh UFOMesh;
    public List<UFOColorData> UFOColorDataList = new List<UFOColorData>();

    public Mesh AllStatUFOMesh;
    public Material AllStatUFOMaterial;

    public int UFOPrice;

    public List<UFOStatData> StatList = new();

#if UNITY_EDITOR
    private void OnValidate()
    {
        // 모든 enum 값 체크
        foreach (UFOStatEnum statType in System.Enum.GetValues(typeof(UFOStatEnum)))
        {
            if (!StatList.Exists(s => s.StatType == statType))
            {
                UFOStatData newStat = new UFOStatData
                {
                    StatType = statType,
                    BaseValue = 1,
                    MaxValue = 1
                };
                StatList.Add(newStat);
                Debug.Log($"[UFOData] 누락된 StatType '{statType}' 추가됨");
            }
        }
    }
#endif
}



