
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatWidget : MonoBehaviour
{
    private ReinForceWidget reinForceWidget;
    private UFOStatEnum StatType;
    private int CurrentStat;
    private int PreviewStat;
    private int MaxStat;

    private float StatCnt = 5.0f;

    [SerializeField] private List<Image> BoxFrameList = new List<Image>();
    [SerializeField] private TMP_Text statText;
    [SerializeField] private Image StatProgressBar;
    [SerializeField] private Image StatPreviewBar;


    public void InitializeStatWidget(ReinForceWidget reinforce, UFOStatEnum stattype, string StatText, int basestat, int maxStat)
    {
        reinForceWidget = reinforce;
        StatType = stattype;

        statText.text = StatText;

        CurrentStat = basestat;
        PreviewStat = CurrentStat;
        MaxStat = maxStat;

        for (int i = 0; i < MaxStat; i++)
        {
            BoxFrameList[i].gameObject.SetActive(true);
        }

        
        StatProgressBar.fillAmount = CurrentStat/ StatCnt;
        StatPreviewBar.fillAmount = StatProgressBar.fillAmount;

    }

    public void OnClicknUpStat()
    {
        PreviewStat++;
        ChangePreviewStat(PreviewStat);
    }

    public void OnClickDownStat()
    {
        PreviewStat--;
        ChangePreviewStat(PreviewStat);
    }

    public void ApplyStat()
    {
        CurrentStat = PreviewStat;
        StatProgressBar.fillAmount = CurrentStat / StatCnt;
       
    }


    public void OnCancelStat()
    {
        ChangePreviewStat(CurrentStat);
    }

    private void ChangePreviewStat(int newstat)
    {
        if(MaxStat < newstat)
        {
            PreviewStat = MaxStat;
            return;
        }
           
        if(CurrentStat > newstat)
        {
            PreviewStat = CurrentStat;
            return;
        }
           

        if (newstat>=CurrentStat)
        {
            PreviewStat = newstat;

           // Debug.Log(PreviewStat);
            StatPreviewBar.fillAmount = PreviewStat / StatCnt;
        }

        reinForceWidget.OnChangePreviewStat(StatType, PreviewStat);
    }


    public void ClearStatWidget()
    {
        for (int i = 0; i < BoxFrameList.Count; i++)
        {
            BoxFrameList[i].gameObject.SetActive(false);
        }
    }

   
    
}
