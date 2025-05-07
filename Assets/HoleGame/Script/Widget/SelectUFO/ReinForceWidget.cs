
using System;
using System.Collections.Generic;
using UnityEngine;



public class ReinForceWidget : MonoBehaviour
{
    private UFOAllWidget  ufoAllWidget;
  
    private UFOData currentUFOData;

    [SerializeField]List<StatStructWidget> StatWidgetList = new List<StatStructWidget>();

    private Dictionary<UFOStatEnum, StatWidget> statWidgetMap = new();

    public event Action<bool/*bsuccess*/, int/*currentstarcnt*/> FOnReinforceApplied;

    public event Action<UFOData,Texture/*basemap*/> FOnFullStated;

    [System.Serializable]
    public struct StatStructWidget
    {
        public UFOStatEnum stattype;
        public StatWidget statwidget;
    }

    private void InitializeDictionary()
    {
        statWidgetMap.Clear();
        foreach (var item in StatWidgetList)
        {
            if (!statWidgetMap.ContainsKey(item.stattype))
            {
                statWidgetMap.Add(item.stattype, item.statwidget);
            }
        }
    }

    public void ReinforceInitWidget()
    {
        InitializeDictionary();
    }
    
    public void InitializeStatWidgetList(UFOAllWidget allwidget ,UFOData ufodata,  IReadOnlyList<UFOStatData> ufostatlist,SkillEnum skilltype)
    {

        ufoAllWidget = allwidget;
        currentUFOData = ufodata;
        if (StatWidgetList.Count != 4) Debug.Log("stat 부족");

      
       int currentstarcnt = GameManager.Instance.userData.StarCnt;

        for (int i=0;i< ufostatlist.Count; i++)
        {
           
            UFOStatEnum statenum = ufostatlist[i].StatType;

            Sprite staticon = StatIconManager.Instance.GetStatSprite(statenum);

            if(statenum ==UFOStatEnum.SkillCount) 
                staticon = SkillIconManager.Instance.GetSkillIconSprite(skilltype);


            string statstring = statenum.ToString();
            int basestat = ufostatlist[i].BaseValue;
            int maxstat = ufostatlist[i].MaxValue;
            if (statWidgetMap.TryGetValue(statenum, out var widget))
                widget.InitializeStatWidget(this, statenum, statstring, basestat, maxstat, currentstarcnt , staticon);
          
        }

    }



    public void OnClickApply(int price , UFOStatEnum statenum)
    {
        UserUFOData userufodata = GameManager.Instance.userData.serialUFOList.Get(currentUFOData.UFOName);
        userufodata.AddReinforce(statenum);

        if (userufodata.AllStat())
        {
            int colorIndex = userufodata.CurrentColorIndex;

            var colorSet = currentUFOData.UFOColorDataList[colorIndex];
           
            Texture baseMap = colorSet.Materials[0].GetTexture("_BaseMap");

            FOnFullStated?.Invoke(currentUFOData, baseMap);

            if (GameManager.Instance.userData != null)
            {
                //( 업적 )시대 이동 누적 카운트 
                string FullStatcntId = $"Collect_Stat_AllStat_Cnt";
                AchievementManager.Instance.ReportProgress(AchieveEnum.Collect, FullStatcntId, 1);


            }

        }
           

        int currentcnt =  GameManager.Instance.userData.MinusStartCnt(price);
       
        FOnReinforceApplied?.Invoke(true, currentcnt);
        GameManager.Instance.SaveUserData();

        foreach (var widget in statWidgetMap)
        {
            widget.Value.CheckStatPrice(currentcnt);
        }

    }

    public void OnClickCancel()
    {
        foreach(var stat in statWidgetMap)
        {
            stat.Value.ClearStatWidget();
        }
    }



    public void ClearStat()
    {
        foreach (var stat in statWidgetMap)
        {
            stat.Value.ClearStatWidget();
        }
    }


}
