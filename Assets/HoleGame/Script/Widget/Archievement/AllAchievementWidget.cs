using DanielLochner.Assets.SimpleScrollSnap;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AllAchievementWidget : MonoBehaviour
{

    [SerializeField] private MainWidget mainWidget;

    [SerializeField] private AchievementWidget achivementPrefab;

    [SerializeField] private GameObject AchieveContent;
    [SerializeField] private GameObject tempParent;



    [SerializeField] private SimpleScrollSnapType2 scrollSnapType2;
    
    private List<AchievementWidget> Instancewidgets = new List<AchievementWidget>();
    private Dictionary<string, AchievementWidget> achievementWidgets =new Dictionary<string, AchievementWidget>();

    public event Action<int/*starcnt*/ ,RectTransform> FOnRewarded;


    [SerializeField] private AchievePointWidget achievePointWidget;

    [SerializeField] private GameObject AchieveMark;

    private bool AchieveMarkActive = false;

    public event Action<string /*ufoname*/> FOnUfoRewarded;
    public event Action<string /*ufoname*/,int/*colorindex*/> FOnUfoColorRewarded;
    
    public void InitAllArchiveWidget()
    {
        if (GameManager.Instance.userData == null) return;
        if (AchievementManager.Instance == null) return;
       
        AchieveMark.SetActive(false);
        AchievementManager.Instance.FOnProgressChanged -= CallBack_RenewalAchieve;
        AchievementManager.Instance.FOnProgressChanged += CallBack_RenewalAchieve;
      
        CreateInstancewidgets();

        ArrayAllArchiveWidget();
        FOnRewarded += mainWidget.Callback_OnRewarded;

        achievePointWidget.InitPointWidget();
        achievePointWidget.FonMoneyRewarded += mainWidget.Callback_OnRewarded;

        if(achievePointWidget.CheckrewardMark())
            AchieveMark.SetActive(true);
    }

    public void CallBack_RewardMaskFalse()
    {
       if(!AchieveMarkActive)
            AchieveMark.SetActive(false);
    }

    private void CreateInstancewidgets()
    {
        foreach (var achieve in AchievementManager.Instance.ReadAchiveDict)
        {
            foreach (var achievement in achieve.Value)
            {
                AchievementWidget achievewidget = Instantiate(achivementPrefab, tempParent.transform,false);
                Instancewidgets.Add(achievewidget);
                achievewidget.gameObject.SetActive(false);
            }
        }
    }


    private void ArrayAllArchiveWidget()
    {
       
        Dictionary<string, AchievementWidget> Allcompleteachievement = new Dictionary<string, AchievementWidget>();
        Dictionary<string, AchievementWidget> Prgrosschievement = new Dictionary<string, AchievementWidget>();  
        Dictionary<string, AchievementWidget> tiercompleteachievement = new Dictionary<string, AchievementWidget>();

        int index = 0;

        AchieveMarkActive = false;

        Debug.Log("Àç¹è¿­");
        foreach (var achieve in AchievementManager.Instance.ReadAchiveDict)
        {
            foreach (var achievement in achieve.Value) 
            {
               
                UserAchieveData userachievedata = AchievementManager.Instance.ReadprogressDict[achievement.Key];

                //AchievementWidget achievewidget = Instantiate(achivementPrefab);
                AchievementWidget achievewidget = Instancewidgets[index];

                
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
                    AchieveMarkActive = true;
                    AchieveMark.SetActive(AchieveMarkActive);
                    tiercompleteachievement.Add(userachievedata.Id, achievewidget);
                }
                else
                {
                   
                    Prgrosschievement.Add(userachievedata.Id, achievewidget);
                   
                }
                index++;
            }
        }

        if (!AchieveMarkActive)
            AchieveMark.SetActive(false);

        foreach (var achievement in tiercompleteachievement)
        {
            achievement.Value.transform.SetParent(AchieveContent.transform, false);
            achievement.Value.gameObject.SetActive(true);
            achievementWidgets.Add(achievement.Key, achievement.Value);
        }
        foreach (var achievement in Prgrosschievement)
        {
            achievement.Value.transform.SetParent(AchieveContent.transform, false);
            achievement.Value.gameObject.SetActive(true);
            achievementWidgets.Add(achievement.Key, achievement.Value);
        }

        foreach (var achievement in Allcompleteachievement)
        {
            achievement.Value.transform.SetParent(AchieveContent.transform, false);
            achievement.Value.gameObject.SetActive(true);
            achievementWidgets.Add(achievement.Key, achievement.Value);
        }
        
        tiercompleteachievement.Clear();
        Prgrosschievement.Clear();
        Allcompleteachievement.Clear();

        

        scrollSnapType2.SetStartingPanel(0);

    }

    public void RewardAchievement(AchieveEnum type , string id , int reward ,RectTransform buttontransform)
    {
       
        AchievementManager.Instance.rewadCompleted(type, id);
        
        int currentstarcnt = GameManager.Instance.userData.AddStarCnt(reward);

        FOnRewarded?.Invoke(currentstarcnt , buttontransform);

        if (AchievementManager.Instance.ReadprogressDict[id].IsCompleted)
        {
            AllWidgetClear();
            ArrayAllArchiveWidget();
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
                ArrayAllArchiveWidget();
            }
            else
            {
                achievementWidgets[id].NextTierTarget(newtarget, newreward);
            }
               
        }
    }


    public void CallBack_RenewalAchieve(AchieveEnum type, string id,int progress)
    {
        if(achievementWidgets[id].RenewalProgress(progress))
        {
            AllWidgetClear();
            ArrayAllArchiveWidget();
        }
    }

    private void AllWidgetClear()
    {
        foreach(var widget in achievementWidgets)
        {
            //DestroyImmediate(widget.Value.gameObject);
            widget.Value.gameObject.SetActive(false);   
            widget.Value.transform.SetParent(tempParent.transform, false);

        }

        achievementWidgets.Clear();

    }

    public void OnEvenetRewardComplete(PointRewardEnum rewardtype ,string UFOName , int colorindex)
    {
        if (rewardtype == PointRewardEnum.UFO)
            FOnUfoRewarded?.Invoke(UFOName);
        else
            FOnUfoColorRewarded?.Invoke(UFOName,colorindex);
    }


    public void OnDisable()
    {
        if (AchievementManager.Instance != null)
            AchievementManager.Instance.FOnProgressChanged -= CallBack_RenewalAchieve;
    }



}
