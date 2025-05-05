using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    //private float fillSpeed = 2.0f;

    private float TargetfillPercent = 0.0f;

    private int currentStep = 0;

    private float NormalPointgauge = 0;
  
    private UserAchievePointData userpointData;
  
    public void InitPointWidget()
    {
        if (GameManager.Instance.userData == null) return;
        userpointData = GameManager.Instance.userData.userAchievePointData;

        AchievementManager.Instance.FOnPointChanged -= CallBack_UpdatePoint;
        AchievementManager.Instance.FOnPointChanged += CallBack_UpdatePoint;
        
        CreatePointWidget();
    }

    /* private void CreatePointWidget()
     {
         currentStep = userpointData.Step;

         //���� Ƽ���� �°� ����
         int tier = 0;
         var pointDataList = AchievementManager.Instance.ReadPointRewardDataList[currentStep];
         //int maxtier = pointDataList.Count - 1;
         MaxPointGauge = pointDataList.MaxPoint;

         float barWidth = ProgressBarTransform.rect.width;
         foreach (var achievetier in AchievementManager.Instance.ReadPointRewardDataList[currentStep].PointRewardDatas)
         {

             // 1) ��� ��ġ ���
             float normalized = achievetier.AchievePoint / MaxPointGauge;
             float posX = barWidth * normalized;

             // 2) ������ ����
             var rewardWidget = Instantiate(pointrewardPrefab, ProgressBarTransform);
             rewardWidget.GetComponent<RectTransform>().anchoredPosition = new Vector2(posX, 0f);

             // 3) �ʿ��ϴٸ� ������ ���� (��: �ؽ�Ʈ, ���� ��)
             bool rewardComplete = userpointData.Tier <= tier;
            // rewardWidget.SetData(achievetier, rewardComplete); // �װ� ������ �޼����

             RewardButtons.Add(rewardWidget);
             tier++;


         }



         //���� ����Ʈ ���
         for (int i = 0; i < userpointData.Step; i++)
         {
             int lastpoint = AchievementManager.Instance.ReadPointRewardDataList[i].PointRewardDatas.Count - 1;
             int stepmax = AchievementManager.Instance.ReadPointRewardDataList[i].PointRewardDatas[lastpoint].AchievePoint;
             NormalPointgauge += stepmax;
         }
         CurrentPointGauge = userpointData.Point;
         CurrentPointGauge -= NormalPointgauge;

         TargetfillPercent = CurrentPointGauge / MaxPointGauge;
         PointProgressBar.fillAmount = TargetfillPercent;
     }*/

    private void CreatePointWidget()
    {
        currentStep = userpointData.Step;

        //���� Ƽ���� �°� ����
        int tier = 0;
        var pointDataList = AchievementManager.Instance.ReadPointRewardDataList[currentStep];
    
        MaxPointGauge = pointDataList.MaxPoint;

        float eachMaxgauge = MaxPointGauge / (pointDataList.PointRewardDatas.Count);


        foreach (var achievetier in AchievementManager.Instance.ReadPointRewardDataList[currentStep].PointRewardDatas)
        {

          
            // 2) ������ ����
            var rewardWidget = Instantiate(pointrewardPrefab, ProgressBarArea);
           
          
            bool rewardComplete = userpointData.Tier <= tier;
            // rewardWidget.SetData(achievetier, rewardComplete); // �װ� ������ �޼����

            RewardWidgets.Add(rewardWidget);
            tier++;


        }

        //���� ����Ʈ ���
      for (int i = 0; i < userpointData.Step; i++)
        {
            int stepmaxtpoint = AchievementManager.Instance.ReadPointRewardDataList[i].MaxPoint;
           
            NormalPointgauge += stepmaxtpoint;
        }
        CurrentPointGauge = userpointData.Point;
        CurrentPointGauge -= NormalPointgauge;
       
        currentPointText.text = CurrentPointGauge.ToString();   
       // TargetfillPercent = CurrentPointGauge / MaxPointGauge;
        //PointProgressBar.fillAmount = TargetfillPercent;
    }
    private void Update()
    {
        if (CurrentPointGauge == 0 && TargetfillPercent == 0) return;
        UpdateGaugeBar();
    }

    public void CallBack_UpdatePoint(int point)
    {
        CurrentPointGauge = point - NormalPointgauge;
        currentPointText.text = CurrentPointGauge.ToString();
        TargetfillPercent = CurrentPointGauge / MaxPointGauge;
    }

    private void UpdateGaugeBar()
    {

        //PointProgressBar.fillAmount = Mathf.Lerp(PointProgressBar.fillAmount, TargetfillPercent, Time.deltaTime * fillSpeed);
    }

    public void OnDisable()
    {
        AchievementManager.Instance.FOnPointChanged -= CallBack_UpdatePoint;
    }

    //public bool Next
}
