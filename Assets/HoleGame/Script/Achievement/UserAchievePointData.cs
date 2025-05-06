using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class UserAchievePointData 
{
    public int Point = 0;

    public int Step = 0;
    //public int Tier = -1;

    public List<bool> TierCompleted = new List<bool>();

    public void InitTierList(int tier)
    {
        TierCompleted.Clear();
        TierCompleted = Enumerable.Repeat(false, tier).ToList();
    }

    public void CompleteTier(int tier)
    {
        if (TierCompleted.Count > tier)
        {
            TierCompleted[tier] = true;
        }
    }

    public bool CheckAllTierCompleted()
    {
        foreach (var tier in TierCompleted)
        {
            if (!tier)
                return false;

        }

        return true;

    }
    public int AddAchievePoint(int point)
    {
        Point += point;
        return Point;
    }

    public void ChangeRewradPointStep(int step,int tier)
    {
        Step = step;
        InitTierList(tier);
    }

   /* public void ChangeRewradPointTier(int tier)
    {
        Tier = tier;
    }*/
}
