using System;
using System.Collections.Generic;


[Serializable]
public class UserData
{
    public string userId;
    public int CurrentUFO;
    //public int currentClearIndex;
    public List<int> SkillCnt;
    public int BestScore;
    public int StarCnt;
    //public List<int> stageClearTimes;
    //public List<int> stageStars;

    public UserData()
    {
        userId = "Guest";
        CurrentUFO = 0;
        SkillCnt = new List<int> { 2, 3, 4, 5 };
        BestScore = 0;
        StarCnt = 0;
        //currentClearIndex = -1;
        // stageStars = new List<int>();

        //stageClearTimes = new List<int>();
    }
    public UserData(string userid)
    {
        userId = userid;
        CurrentUFO = 0;
        SkillCnt = new List<int> { 2, 3, 4, 5 };
        BestScore = 0;
        StarCnt = 0;
        //currentClearIndex = -1;
        //stageStars = new List<int>();
        //stageClearTimes = new List<int>();
    }

    public void AddStarCnt(int cnt)
    {
        StarCnt += cnt;
    }

    public void UpdateBestScore(int score)
    {
        if (BestScore < score)
        {
            BestScore = score;  
        }
       
    }

    public void UpdateSkillCnt(int index , int cnt)
    {
        int newcnt = 0;
        if (cnt>=0)
        {
            newcnt=cnt;
        }
        SkillCnt[index] = newcnt;
    }
}
