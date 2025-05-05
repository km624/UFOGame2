using DanielLochner.Assets.SimpleScrollSnap;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AllAchievementWidget : MonoBehaviour
{

    [SerializeField] private MainWidget mainWidget;

    [SerializeField] private AchievementWidget achivementPrefab;

    [SerializeField] private GameObject AchieveContent;

    [SerializeField] private SimpleScrollSnapType2 scrollSnapType2;
    
    
    private Dictionary<string, AchievementWidget> achievementWidgets =new Dictionary<string, AchievementWidget>();

    public event Action<int/*starcnt*/> FOnRewarded;


    [SerializeField] private AchievePointWidget achievePointWidget;
    
    public void InitAllArchiveWidget()
    {
        if (GameManager.Instance.userData == null) return;
        if (AchievementManager.Instance == null) return;
       
        AchievementManager.Instance.FOnProgressChanged -= CallBack_RenewalAchieve;
        AchievementManager.Instance.FOnProgressChanged += CallBack_RenewalAchieve;
        FOnRewarded += mainWidget.Callback_OnRewarded;
        
        CreateAllArchiveWidget();

        achievePointWidget.InitPointWidget();
    }

    private void CreateAllArchiveWidget()
    {
       
        Dictionary<string, AchievementWidget> Allcompleteachievement = new Dictionary<string, AchievementWidget>();
        Dictionary<string, AchievementWidget> Prgrosschievement = new Dictionary<string, AchievementWidget>();  
        Dictionary<string, AchievementWidget> tiercompleteachievement = new Dictionary<string, AchievementWidget>();
       
        foreach(var achieve in AchievementManager.Instance.ReadAchiveDict)
        {
            foreach (var achievement in achieve.Value) 
            {
                
                UserAchieveData userachievedata = AchievementManager.Instance.ReadprogressDict[achievement.Key];

                AchievementWidget achievewidget = Instantiate(achivementPrefab);
              

                int rewardtier = userachievedata.IsCompleted? (userachievedata.RewardTierIndex-1) : userachievedata.RewardTierIndex;
                int target = achievement.Value.TiersList[rewardtier].TargetValue;
                int reward = achievement.Value.TiersList[rewardtier].Rewardcnt;
          
                //bool tierone = (achievement.Value.TiersList.Count == 1);
               
                achievewidget.InitializeWidget(this, achieve.Key , achievement.Value.Id, achievement.Value.Title,
                   userachievedata.CurrentValue, target, reward, userachievedata.IsCompleted);


                if (userachievedata.IsCompleted)
                {
                    Allcompleteachievement.Add(userachievedata.Id, achievewidget);
                } 
                else if(target <= userachievedata.CurrentValue)
                {
                    tiercompleteachievement.Add(userachievedata.Id, achievewidget);
                }
                else
                {
                   
                    Prgrosschievement.Add(userachievedata.Id, achievewidget);
                   
                }

            }
        }
        foreach (var achievement in tiercompleteachievement)
        {
            achievement.Value.transform.SetParent(AchieveContent.transform, false);
            achievementWidgets.Add(achievement.Key, achievement.Value);
        }
        foreach (var achievement in Prgrosschievement)
        {
            achievement.Value.transform.SetParent(AchieveContent.transform, false);
            achievementWidgets.Add(achievement.Key, achievement.Value);
        }

        foreach (var achievement in Allcompleteachievement)
        {
            achievement.Value.transform.SetParent(AchieveContent.transform, false);
            achievementWidgets.Add(achievement.Key, achievement.Value);
        }
        
        tiercompleteachievement.Clear();
        Prgrosschievement.Clear();
        Allcompleteachievement.Clear();


        scrollSnapType2.SetStartingPanel(0);

    }

    public void RewardAchievement(AchieveEnum type , string id , int reward)
    {
        AchievementManager.Instance.rewadCompleted(type, id);
        
        int currentstarcnt = GameManager.Instance.userData.AddStarCnt(reward);

        FOnRewarded?.Invoke(currentstarcnt);

        if (AchievementManager.Instance.ReadprogressDict[id].IsCompleted)
        {
            AllWidgetClear();
            CreateAllArchiveWidget();
        }
        else
        {
            
            int newrewardTier = AchievementManager.Instance.ReadprogressDict[id].RewardTierIndex;

            var progressdic = AchievementManager.Instance.ReadprogressDict[id];
            int progress = progressdic.CurrentValue;

            var typedic = AchievementManager.Instance.ReadAchiveDict[type];
            int newtarget = typedic[id].TiersList[newrewardTier].TargetValue;
            int newreward = typedic[id].TiersList[newrewardTier].Rewardcnt;

            if(newtarget>progress)
            {
                AllWidgetClear();
                CreateAllArchiveWidget();
            }
            else
            {
                achievementWidgets[id].NextTierTarget(newtarget, newreward);
            }
               
        }
    }


    public void CallBack_RenewalAchieve(AchieveEnum type, string id,int progress)
    {
        achievementWidgets[id].RenewalProgress(progress);
    }

    private void AllWidgetClear()
    {
        foreach(var widget in achievementWidgets)
        {
            DestroyImmediate(widget.Value.gameObject);
        }
        achievementWidgets.Clear();

        FOnRewarded -= mainWidget.Callback_OnRewarded;
    }


    public void OnDisable()
    {
        if (AchievementManager.Instance != null)
            AchievementManager.Instance.FOnProgressChanged -= CallBack_RenewalAchieve;
    }



}
