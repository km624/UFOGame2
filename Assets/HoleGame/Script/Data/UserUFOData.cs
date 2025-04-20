using System.Collections.Generic;

using System.Linq;
using UnityEngine;


[System.Serializable]
public class UserUFOData
{
    public string UFOName;
    //public bool IsUnlocked;          
    public List<UFOStatData> StatReinforceList = new();

    public List<int> OwnedColorIndexes = new(); 
    public int CurrentColorIndex = 0;

   
    /*public UserUFOData()
    {
        UFOName = "UFONormal";

        StatReinforceList = new List<StatReinforceData>
        {
            new() {
                StatType = UFOStatEnum.MoveSpeed,
                ReinforceValue = 0,
                MaxReinforceValue = 1
            },
            new() {
                StatType = UFOStatEnum.LiftSpeed,
                ReinforceValue = 0,
                MaxReinforceValue = 1
            },
            new() {
                StatType = UFOStatEnum.BeamRange,
                ReinforceValue = 0,
                MaxReinforceValue = 1
            }
        };

        OwnedColorIndexes.Add(0);

        CurrentColorIndex = 0;

        //AllStat = false;
    }*/
    public UserUFOData(UFOData ufoData)
    {
        UFOName = ufoData.UFOName;

        StatReinforceList = new List<UFOStatData>();

      foreach(var stat in ufoData.StatList)
        {
            StatReinforceList.Add(stat);
            //Debug.Log(stat.BaseValue);
        }
       
     


        OwnedColorIndexes.Add(0);

        CurrentColorIndex = 0;

        //AllStat = false;
    }

    public void AddColor(int colorIndex)
    {
        if (!OwnedColorIndexes.Contains(colorIndex))
        {
            OwnedColorIndexes.Add(colorIndex);
        }
        CurrentColorIndex = colorIndex;

    }

    public int GetReinforceValue(UFOStatEnum type)
    {
        return StatReinforceList.Find(s => s.StatType == type)?.BaseValue ?? 0;
    }

    /*public void AddReinforce(UFOStatEnum type)
    {
        var stat = StatReinforceList.Find(s => s.StatType == type);
        if (stat != null && stat.BaseValue < stat.MaxValue)
        {
            stat.BaseValue++;
        }
    }*/

    public void SetReinforce(UFOStatEnum type, int statcnt)
    {
        var stat = StatReinforceList.Find(s => s.StatType == type);
        if (stat != null && stat.BaseValue < stat.MaxValue)
        {
            stat.BaseValue = statcnt;
        }
    }

    public bool AllStat()
    {
        return StatReinforceList.All(s => s.BaseValue >= s.MaxValue);
    }



}



