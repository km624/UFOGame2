using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievePointRewardWidget : MonoBehaviour
{
    private AchievePointWidget achievePointwidget;
    
    [SerializeField] private Button RewardButton;
 
    public int RewardTier { get; private set; }

    public bool bisCompleted { get; private set; }
    public bool bCanOpen { get; private set; }

    [SerializeField] private Image IconImage;
    [SerializeField] private Sprite OpenIcon;
    [SerializeField] private Sprite CloseIcon;
    [SerializeField] private TMP_Text PointText;

    [SerializeField] private Image ProgressBar;

    [SerializeField] private GameObject CanOpen;
    public float maxgaugebar { get; private set; } = 0;
    public float currentgaugebar { get; private set; } = 0;


    public void InitRewardWidget(AchievePointWidget pointwidget,int tier, bool bcomplete , float maxgague ,float realcurrentgaguge, float point)
    {
        achievePointwidget = pointwidget;

        RewardTier = tier;
        bisCompleted = bcomplete;

        RewardButton.interactable = !bisCompleted;

        IconImage.sprite = bisCompleted ? OpenIcon : CloseIcon;

        maxgaugebar = maxgague;
        currentgaugebar = CacluateCurrentGauge(realcurrentgaguge);
        
        //Debug.Log(RewardTier + " 맥스 게이지바 : "+ maxgaugebar + " 현재 게이지  " + currentgaugebar + "실제 게이지: " + realcurrentgaguge);
        
        ProgressBar.fillAmount = currentgaugebar / maxgaugebar;

        CheckCanOpen();

 
        PointText.text = point.ToString();

    }

    public float CacluateCurrentGauge(float realgauge)
    {
       float calculategauge = 0;
        float caculatemax = (RewardTier + 1) * maxgaugebar;
        if (realgauge - caculatemax >= 0)
        {
            calculategauge = maxgaugebar;
        }
        else
        {
            if (caculatemax - realgauge < maxgaugebar)
                calculategauge = maxgaugebar - (caculatemax - realgauge);
        }

        return calculategauge;

    }

    public void OnClickRewardButton()
    {
        if (!bCanOpen) return;
        RectTransform buttontransform = RewardButton.GetComponent<RectTransform>();
        bisCompleted = true;
        IconImage.sprite = OpenIcon;
        CheckCanOpen();
        achievePointwidget.RewardBoxOpen(RewardTier, buttontransform);
    }

   public void ChangeProgressbar(float currentgauge)
   {
        currentgaugebar = CacluateCurrentGauge(currentgauge);
        ProgressBar.fillAmount  = currentgaugebar/maxgaugebar;
        CheckCanOpen();
   }

   private void CheckCanOpen()
   {
        if (!bisCompleted)
            bCanOpen = maxgaugebar == currentgaugebar;
        else
            bCanOpen = !bisCompleted;
        CanOpen.SetActive(bCanOpen);
    }
   



}
