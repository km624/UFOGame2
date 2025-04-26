
using System;
using System.Collections.Generic;



[Serializable]
public class UserData
{
    public string userId;
    public int CurrentUFO;
    //public int currentClearIndex;
    //public List<int> SkillCnt;
    public int BestScore;
    public int StarCnt;
   
    public SerialUFOList serialUFOList = new();
    
    public UserSettingData userSettingData = new(); 

    public UserData()
    {
        userId = "Guest";
        CurrentUFO = 0;
        //SkillCnt = new List<int> { 2, 3, 4, 5 };
        BestScore = 0;
        StarCnt = 100;
        userSettingData = new UserSettingData();

    }
    public UserData(string userid)
    {
        userId = userid;
        CurrentUFO = 0;
        //SkillCnt = new List<int> { 2, 3, 4, 5 };
        BestScore = 0;
        StarCnt = 100;
        userSettingData = new UserSettingData();

    }

    public void InitializeUserData()
    {
        serialUFOList.InitializeFromUFOList(); 
    }

    public void SetCurrentUFO(int ufoindex)
    {
        CurrentUFO = ufoindex;
    }

    public void AddStarCnt(int cnt)
    {
        StarCnt += cnt;
    }

    public int MinusStartCnt(int cnt)
    {
        if(cnt>StarCnt)
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

    /*public void UpdateSkillCnt(int index , int cnt)
    {
        int newcnt = 0;
        if (cnt>=0)
        {
            newcnt=cnt;
        }
        SkillCnt[index] = newcnt;
    }*/



    
}
