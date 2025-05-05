using System;
using UnityEngine;

[Serializable]
public class UserAchievePointData 
{
    public int Point = 0;

    public int Step = 0;
    public int Tier = -1;

    public int AddAchievePoint(int point)
    {
        Point += point;
        return Point;
    }

    public void ChangeRewradPointStep(int step)
    {
        Step = step;
    }

    public void ChangeRewradPointTier(int tier)
    {
        Tier = tier;
    }
}
