
using System.Collections.Generic;

using TMPro;
using Unity.Collections;
using UnityEngine;

using UnityEngine.UI;

public class StatWidget : MonoBehaviour
{
    private ReinForceWidget reinForceWidget;
    private UFOStatEnum StatType;
    private int CurrentStat;
    private int MaxStat;

    private int currentStatPrice;
    [SerializeField] private List<Image> ActiveList = new List<Image>();
    [SerializeField] private List<Image> BoxFrameList = new List<Image>();
    [SerializeField] private TMP_Text statText;
    [SerializeField] private TMP_Text statPrice;
    [SerializeField] private Image StatIcon;

 
    [SerializeField] Button UpButton;

    [SerializeField]private List<int>StatPriceList = new List<int>();

    public void InitializeStatWidget(ReinForceWidget reinforce, UFOStatEnum stattype, string StatText, int basestat, int maxStat, int starcnt,
       Sprite staticon )
    {
        reinForceWidget = reinforce;
        StatType = stattype;

        statText.text = StatText;

        CurrentStat = basestat;

        for (int i = 0; i < CurrentStat; i++)
        {
            ActiveList[i].enabled=true;
        }

        currentStatPrice = StatPriceList[CurrentStat - 1];

        CheckStatPrice(starcnt);

        statPrice.text = currentStatPrice.ToString();

       
        MaxStat = maxStat;

        for (int i = 0; i < MaxStat; i++)
        {
            //ActiveList[i].enabled=true;
            BoxFrameList[i].enabled = true;
        }

        if (CurrentStat == MaxStat)
        {
            statPrice.text = "MAX";
            UpButton.interactable = false;
            

        }

        if (staticon != null)
        {
            StatIcon.sprite = staticon;
        }

    }

    public void OnClickUpStat()
    {
        CurrentStat++;
        if (CurrentStat == MaxStat)
        {
            statPrice.text = "MAX";
            UpButton.interactable = false;

        }
        else
        {
            ActiveList[CurrentStat-1].enabled = true;
            currentStatPrice = StatPriceList[CurrentStat - 1];
            statPrice.text = currentStatPrice.ToString();
        }
        
        reinForceWidget.OnClickApply(currentStatPrice, StatType);
      
    }
   
    public void CheckStatPrice(int starcnt)
    {
        if (CurrentStat == MaxStat) return;

         bool buttonactive = starcnt >= currentStatPrice;
         UpButton.interactable = buttonactive;
    }
   
    public void ClearStatWidget()
    {
        for (int i = 0; i < ActiveList.Count; i++)
        {
            ActiveList[i].enabled = false;
            BoxFrameList[i].enabled = false;

        }
    }

   
    
}
