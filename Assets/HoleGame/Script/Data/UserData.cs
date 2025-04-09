using System;
using System.Collections.Generic;


[Serializable]
public class UserData
{
    public string userId;
    public int CurrentUFO;
    public int currentClearIndex;
    public List<int> stageClearTimes;
    public List<int> stageStars;

    public UserData()
    {
        userId = "Guest";
        CurrentUFO = 0;
        currentClearIndex = -1;
        stageStars = new List<int>();
        stageClearTimes = new List<int>();
    }
    public UserData(string userid)
    {
        userId = userid;
        CurrentUFO = 0;
        currentClearIndex = -1;
        stageStars = new List<int>();
        stageClearTimes = new List<int>();
    }
}
