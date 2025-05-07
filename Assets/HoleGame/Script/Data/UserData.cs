
using System;
using System.Collections.Generic;



[Serializable]
public class UserData
{
    public string userId;
    //public int CurrentUFO;
    public string SelectUFOName;
    public int BestScore;
    public int StarCnt;

    public SerialUFOList serialUFOList = new();

    public UserSettingData userSettingData = new();

    public List<UserAchieveData> Achievements = new List<UserAchieveData>();

    public UserAchievePointData userAchievePointData = new();
    public UserData()
    {
        userId = "Guest";
        SelectUFOName = "UFONormal";

        BestScore = 0;
        StarCnt = 100;
        userSettingData = new UserSettingData();
        userAchievePointData = new UserAchievePointData();

    }
    public UserData(string userid)
    {
        userId = userid;
        SelectUFOName = "UFONormal";

        BestScore = 0;
        StarCnt = 100;
        userSettingData = new UserSettingData();
        userAchievePointData = new UserAchievePointData();
    }

    public void InitializeUserData()
    {
        serialUFOList.InitializeFromUFOList();
    }

    public void SetCurrentUFO(string ufoname)
    {
        SelectUFOName = ufoname;
    }

    public int AddStarCnt(int cnt)
    {
        StarCnt += cnt;
        return StarCnt;
    }

    public int MinusStartCnt(int cnt)
    {
        if (cnt > StarCnt)
            return -1;

        StarCnt -= cnt;
        return StarCnt;
    }

    public bool CheckStarCnt(int cnt)
    {
        if (cnt > StarCnt)
            return false;
        else
            return true;
    }

    public void UpdateBestScore(int score)
    {
        if (BestScore < score)
        {
            BestScore = score;
        }

    }

   


    


    
}
