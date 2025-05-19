
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

    [SerializeField] private LastRewardWidget lastRewardWidget;

    [SerializeField] private NextRewardWidget nextRewardWidget;

    //SerializeField] private GameObject WorkInProgressPanel;


    public void InitPointWidget()
    {
        if (GameManager.Instance.userData == null) return;
        userpointData = GameManager.Instance.userData.userAchievePointData;

        AchievementManager.Instance.FOnPointChanged -= CallBack_UpdatePoint;
        AchievementManager.Instance.FOnPointChanged += CallBack_UpdatePoint;
       // WorkInProgressPanel.SetActive(false);



        CreatePointWidget();
    }
    private void InitUserTierAllCompleted()
    {
        currentStep = userpointData.Step;
        if(userpointData.CheckAllTierCompleted())
        {
            if (!AchievementManager.Instance.CheckPossibleNextStep(currentStep + 1))
            {
               
                var nextpointDataList = AchievementManager.Instance.ReadPointRewardDataList[currentStep + 1];
                userpointData.ChangeRewradPointStep(currentStep + 1, nextpointDataList.PointRewardDatas.Count);
               
            }
        }
      
        
    }
   

    private void CreatePointWidget()
    {
        currentStep = userpointData.Step;

       
        NormalPointgauge = 0;
        //누적 포인트 계산
        int enabletier = currentStep;
        //Debug.Log(currentStep + " 현재 스텝 세팅");
      
         for (int i = 0; i < enabletier; i++)
         {
             //Debug.Log(i + "번째");
             int stepmaxtpoint = AchievementManager.Instance.ReadPointRewardDataList[i].MaxPoint;
             //Debug.Log("stepmaxtpoint  " + stepmaxtpoint);
             NormalPointgauge += stepmaxtpoint;
         }
      
        CurrentPointGauge = userpointData.Point - NormalPointgauge;

        currentPointText.text = CurrentPointGauge.ToString();

       /* if (!AchievementManager.Instance.CheckPossibleNextStep(currentStep))
        {
           // WorkInProgressPanel.SetActive(true);
            return;
        }*/

        var pointDataList = AchievementManager.Instance.ReadPointRewardDataList[currentStep];

        //이부분 (위험 ) Danger
        if(userpointData.TierCompleted.Count== 0 || userpointData.TierCompleted == null)
        {

            userpointData.InitTierList(pointDataList.PointRewardDatas.Count);
            Debug.Log("포인트 현재 상황 데이터 없어서 세팅");
        }

        MaxPointGauge = pointDataList.MaxPoint;

        EachMaxGaguge = MaxPointGauge / (pointDataList.PointRewardDatas.Count);


        int tier = 0;
        foreach (var achievetier in pointDataList.PointRewardDatas)
        {
            // 2) 프리팹 생성
            var rewardWidget = Instantiate(pointrewardPrefab, ProgressBarArea);
            
            //현재 게이지 계산
          
            float eachpoint = (tier + 1) * EachMaxGaguge;
           
           
            bool rewardComplete = userpointData.TierCompleted[tier];

            bool lastier = (tier == pointDataList.PointRewardDatas.Count - 1);


            rewardWidget.InitRewardWidget(this, tier, rewardComplete, EachMaxGaguge, CurrentPointGauge, eachpoint, lastier);
           
            
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
            nextRewardWidget.SetNextRewardWidget(currentStep+1);

            if (AchievementManager.Instance.CheckPossibleNextStep(currentStep + 1))
            {
                var nextpointDataList = AchievementManager.Instance.ReadPointRewardDataList[currentStep + 1];
                
                userpointData.ChangeRewradPointStep(currentStep + 1, nextpointDataList.PointRewardDatas.Count);
                Debug.Log((currentStep+1) +  "새로운 포인트 리워드 세팅");
                foreach (var rewardwidget in RewardWidgets)
                {
                    Destroy(rewardwidget.gameObject);
                }
                RewardWidgets.Clear();
                CreatePointWidget();
            }
            else
            {
                //userpointData.ChangeRewradPointStep(currentStep + 1, 0);
                Debug.Log((currentStep + 1) + "세팅없엉 세팅");
            }

 
           /* foreach (var rewardwidget in RewardWidgets)
            {
                Destroy(rewardwidget.gameObject);
            }
            RewardWidgets.Clear();
            CreatePointWidget();*/

            


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
                    string ufoname = null;
                    int ColorIndex = 0;
                    string[] split = reward.Split('_');

                    if (split.Length == 2)
                    {
                        ufoname = split[0];
                        if (int.TryParse(split[1], out int colorIndex))
                        {
                            ColorIndex = colorIndex;
                           
                            UFOData currentUFOData = UFOLoadManager.Instance.ReadLoadedUFODataDic[ufoname];
                            UserUFOData userufodata = GameManager.Instance.userData.serialUFOList.Get(currentUFOData.UFOName);
                            if (userufodata != null)
                            {
                                userufodata.AddColor(colorIndex);
                                GameManager.Instance.soundManager.PlaySfx(SoundEnum.GetReward, 1.0f);
                                lastRewardWidget.SetLastRewardWidget(rewardtype, ufoname, colorIndex);

                                allAchievementWidget.OnEvenetRewardComplete(rewardtype, ufoname, ColorIndex);
                            }
                            else
                            {
                                Debug.Log($"{ufoname} 해금 되지 않음");
                            }
         
                        }
                        else
                        {
                            Debug.LogWarning($"Color Index 파싱 실패: {split[1]}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Color 보상 문자열 형식 오류: {reward}");
                    }
                  
                    break;
                }

            case PointRewardEnum.UFO:
                {
                    Debug.Log($"UFO 해금: {reward}");
                    
                    UFOData rewardUFOData = UFOLoadManager.Instance.ReadLoadedUFODataDic[reward];
                    UserUFOData newuserUFOData = new UserUFOData(rewardUFOData);
                    GameManager.Instance.userData.serialUFOList.AddUFO(newuserUFOData);
                    GameManager.Instance.soundManager.PlaySfx(SoundEnum.GetReward, 1.0f);
                    lastRewardWidget.SetLastRewardWidget(rewardtype, reward,0);

                    allAchievementWidget.OnEvenetRewardComplete(rewardtype, reward, 0);
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
