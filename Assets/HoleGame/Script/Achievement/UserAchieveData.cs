﻿using System;
using UnityEngine;

[Serializable]
public class UserAchieveData
{
    public string Id;
    public int CurrentValue;
    public int CurrentTierIndex;
    public int RewardTierIndex;
    public bool IsCompleted;
}
