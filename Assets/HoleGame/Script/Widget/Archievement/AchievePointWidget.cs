
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

       
        //���� ����Ʈ ���
        for (int i = 0; i < userpointData.Step; i++)
        {
            int stepmaxtpoint = AchievementManager.Instance.ReadPointRewardDataList[i].MaxPoint;

            NormalPointgauge += stepmaxtpoint;
        }
        //Debug.Log("currentgague ���� " + NormalPointgauge);
        CurrentPointGauge = userpointData.Point- NormalPointgauge;

        currentPointText.text = CurrentPointGauge.ToString();

        int tier = 0;
        foreach (var achievetier in pointDataList.PointRewardDatas)
        {
            // 2) ������ ����
            var rewardWidget = Instantiate(pointrewardPrefab, ProgressBarArea);
            
            //���� ������ ���
          
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
                Debug.Log("���ο� ����Ʈ ������ ����");

                foreach(var rewardwidget in RewardWidgets)
                {
                    Destroy(rewardwidget.gameObject);
                }
                RewardWidgets.Clear();
                CreatePointWidget();
                
            }
            else
            {
                Debug.Log("���� ����Ʈ ������ ���� ����");
            }
        }

        if(!CheckrewardMark())
        {
            allAchievementWidget.CallBack_RewardMaskFalse();
        }

        //������ ������ ����
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
                        //Debug.Log($"�� ȹ��: {moneyAmount}��");
                       int currentStar =  GameManager.Instance.userData.AddStarCnt(moneyAmount);
                        GameManager.Instance.soundManager.PlaySfx(SoundEnum.AddMoney,  1.0f);
                      FonMoneyRewarded?.Invoke(currentStar, boxtransform);
                    }
                    else
                    {
                        Debug.LogWarning($"Money ����ȯ ����: {reward}");
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
                            allAchievementWidget.OnEvenetRewardComplete(rewardtype, ufoname, ColorIndex);
                            UFOData currentUFOData = UFOLoadManager.Instance.ReadLoadedUFODataDic[ufoname];
                            UserUFOData userufodata = GameManager.Instance.userData.serialUFOList.Get(currentUFOData.UFOName);
                            if (userufodata != null)
                            {
                                userufodata.AddColor(colorIndex);
                                lastRewardWidget.SetLastRewardWidget(rewardtype, ufoname, colorIndex);
                            }
                            else
                            {
                                Debug.Log($"{ufoname} �ر� ���� ����");
                            }
         
                        }
                        else
                        {
                            Debug.LogWarning($"Color Index �Ľ� ����: {split[1]}");
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Color ���� ���ڿ� ���� ����: {reward}");
                    }
                  
                    break;
                }

            case PointRewardEnum.UFO:
                {
                    Debug.Log($"UFO �ر�: {reward}");
                    allAchievementWidget.OnEvenetRewardComplete(rewardtype, reward, 0);
                    
                    UFOData rewardUFOData = UFOLoadManager.Instance.ReadLoadedUFODataDic[reward];
                    UserUFOData newuserUFOData = new UserUFOData(rewardUFOData);
                    GameManager.Instance.userData.serialUFOList.AddUFO(newuserUFOData);
                    lastRewardWidget.SetLastRewardWidget(rewardtype, reward,0);
                    break;
                }

            default:
                Debug.LogWarning($"�� �� ���� ���� Ÿ��: {rewardtype}");
                break;
        }
    }

   

    public void OnDisable()
    {
        AchievementManager.Instance.FOnPointChanged -= CallBack_UpdatePoint;
    }

  
}
