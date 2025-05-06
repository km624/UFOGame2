
using System;
using System.Collections.Generic;
using TMPro;

using UnityEngine;


public class AchievePointWidget : MonoBehaviour
{
    [SerializeField] private AllAchievementWidget allAchievementWidget;

    //[SerializeField] private Image PointProgressBar;
    [SerializeField] private RectTransform ProgressBarArea;

    [SerializeField] private AchievePointRewardWidget pointrewardPrefab;
    private List<AchievePointRewardWidget> RewardWidgets = new List<AchievePointRewardWidget>();

    [SerializeField] private TMP_Text currentPointText;

    private float CurrentPointGauge;

    private float MaxPointGauge;

    private float EachMaxGaguge;

    private int currentStep = 0;

    private float NormalPointgauge = 0;
  
    private UserAchievePointData userpointData;

    public event Action<int /*currentmoney*/, RectTransform> FonMoneyRewarded;

   
    public void InitPointWidget()
    {
        if (GameManager.Instance.userData == null) return;
        userpointData = GameManager.Instance.userData.userAchievePointData;

        AchievementManager.Instance.FOnPointChanged -= CallBack_UpdatePoint;
        AchievementManager.Instance.FOnPointChanged += CallBack_UpdatePoint;
        
        CreatePointWidget();
    }

    private void CreatePointWidget()
    {
        currentStep = userpointData.Step;

       
        var pointDataList = AchievementManager.Instance.ReadPointRewardDataList[currentStep];
    
        MaxPointGauge = pointDataList.MaxPoint;

        EachMaxGaguge = MaxPointGauge / (pointDataList.PointRewardDatas.Count);

       
        //누적 포인트 계산
        for (int i = 0; i < userpointData.Step; i++)
        {
            int stepmaxtpoint = AchievementManager.Instance.ReadPointRewardDataList[i].MaxPoint;

            NormalPointgauge += stepmaxtpoint;
        }
        Debug.Log("currentgague 오차 " + NormalPointgauge);
        CurrentPointGauge = userpointData.Point- NormalPointgauge;

        currentPointText.text = CurrentPointGauge.ToString();

        int tier = 0;
        foreach (var achievetier in pointDataList.PointRewardDatas)
        {
            // 2) 프리팹 생성
            var rewardWidget = Instantiate(pointrewardPrefab, ProgressBarArea);
            
            //현재 게이지 계산
          
            float eachpoint = (tier + 1) * EachMaxGaguge;
           
           
            bool rewardComplete = userpointData.TierCompleted[tier];
          
            rewardWidget.InitRewardWidget(this, tier, rewardComplete, EachMaxGaguge, CurrentPointGauge, eachpoint);
           
            
            RewardWidgets.Add(rewardWidget);
            tier++;

        }
    }

    public bool CheckrewardMark()
    {
       
        foreach(var widdget in RewardWidgets)
        {
            if(widdget.bCanOpen)
            {

                return true;
            }
        }
        return false;
    }
  

    public void CallBack_UpdatePoint(int point)
    {
        CurrentPointGauge = point - NormalPointgauge;
        currentPointText.text = CurrentPointGauge.ToString();
        foreach(var widget in RewardWidgets)
        {
            widget.ChangeProgressbar(CurrentPointGauge);
        }
        
     
    }

    public void RewardBoxOpen(int tier , RectTransform BoxTransform)
    {
        userpointData.CompleteTier(tier);
        var pointDataList = AchievementManager.Instance.ReadPointRewardDataList[currentStep];

        PointRewardEnum type = pointDataList.PointRewardDatas[tier].type;
        string reward = pointDataList.PointRewardDatas[tier].Reward;

        ReceivedReward(type, reward, BoxTransform);

      

        if (userpointData.CheckAllTierCompleted())
        {
           if(AchievementManager.Instance.CheckPossibleNextStep(currentStep + 1))
            {
                var nextpointDataList = AchievementManager.Instance.ReadPointRewardDataList[currentStep + 1];

                userpointData.ChangeRewradPointStep(currentStep + 1, nextpointDataList.PointRewardDatas.Count);
                Debug.Log("새로운 포인트 리워드 세팅");

                foreach(var rewardwidget in RewardWidgets)
                {
                    Destroy(rewardwidget.gameObject);
                }
                RewardWidgets.Clear();
                CreatePointWidget();
                
            }
            else
            {
                Debug.Log("다음 포인트 리워드 세팅 없음");
            }
        }

        if(!CheckrewardMark())
        {
            allAchievementWidget.CallBack_RewardMaskFalse();
        }

        //리워드 받으면 저장
        GameManager.Instance.SaveUserData();
    }

    private void ReceivedReward(PointRewardEnum rewardtype ,string reward,RectTransform boxtransform)
    {
        switch (rewardtype)
        {
            case PointRewardEnum.Money:
                {
                    if (int.TryParse(reward, out int moneyAmount))
                    {
                        //Debug.Log($"돈 획득: {moneyAmount}원");
                       int currentStar =  GameManager.Instance.userData.AddStarCnt(moneyAmount);
                        GameManager.Instance.soundManager.PlaySfx(SoundEnum.AddMoney,  1.0f);
                      FonMoneyRewarded?.Invoke(currentStar, boxtransform);
                    }
                    else
                    {
                        Debug.LogWarning($"Money 형변환 실패: {reward}");
                    }
                    break;
                }

            case PointRewardEnum.Color:
                {
                    if (int.TryParse(reward, out int colorIndex))
                    {
                        Debug.Log($"컬러 인덱스 해금: {colorIndex}");
                        // GameManager.Instance.userData.UnlockColor(colorIndex);
                    }
                    else
                    {
                        Debug.LogWarning($"Color 형변환 실패: {reward}");
                    }
                    break;
                }

            case PointRewardEnum.UFO:
                {
                    Debug.Log($"UFO 해금: {reward}");
                    // GameManager.Instance.userData.UnlockUFO(reward); // reward는 UFO 이름
                    break;
                }

            default:
                Debug.LogWarning($"알 수 없는 보상 타입: {rewardtype}");
                break;
        }
    }

   

    public void OnDisable()
    {
        AchievementManager.Instance.FOnPointChanged -= CallBack_UpdatePoint;
    }

  
}
