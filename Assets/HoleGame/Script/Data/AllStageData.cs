using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AllStageData", menuName = "Scriptable Objects/AllStageData")]
public class AllStageData : ScriptableObject
{
    public List<StageData> allStageData;
}
