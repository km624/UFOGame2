
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

    private int currentStatPrice = 0;
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
       
        MaxStat = maxStat;

        for (int i = 0; i < MaxStat; i++)
        {
           
            BoxFrameList[i].enabled = true;
        }



        if (CheckStatPrice(starcnt))
        {
            currentStatPrice = StatPriceList[CurrentStat - 1];
            statPrice.text = currentStatPrice.ToString();
        }
        else
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
        reinForceWidget.OnClickApply(currentStatPrice, StatType);
        CurrentStat++;
     
        if (CurrentStat == MaxStat)
        {
            statPrice.text = "MAX";
            UpButton.interactable = false;

        }
        else
        {
           
            currentStatPrice = StatPriceList[CurrentStat - 1];
            statPrice.text = currentStatPrice.ToString();
        }
        ActiveList[CurrentStat - 1].enabled = true;
       
      
    }
   
    public bool CheckStatPrice(int starcnt)
    {
        if (CurrentStat == MaxStat) return false;

         bool buttonactive = starcnt >= currentStatPrice;
         UpButton.interactable = buttonactive;

        return true;
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
