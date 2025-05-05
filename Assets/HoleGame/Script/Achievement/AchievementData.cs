using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementData", menuName = "Scriptable Objects/AchievementData")]
public class AchievementData : ScriptableObject
{
    public string Id;
    public string Title;
   
    public AchieveEnum AchieveType;
    public List<AchievementTier> TiersList;
  
}

[System.Serializable]
public class AchievementTier
{
    public int TargetValue;    
    public int Rewardcnt;      
}