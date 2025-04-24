
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReinForceWidget : MonoBehaviour
{
    private UFOAllWidget  ufoAllWidget;
    private string UFOName;

    [SerializeField]List<StatWidget> StatWidgetList = new List<StatWidget>();

    //�������� ������ ����
   

    //[SerializeField] private Button CancelButton;
    [SerializeField] private Button ApplyButton;

    private int TotalPrcie = 0;

    private int statPrcie = 25;

    [SerializeField] private TMP_Text priceText;

    private Dictionary<UFOStatEnum , int> OriginStatDic = new Dictionary<UFOStatEnum , int>();
    private Dictionary<UFOStatEnum , int> PreviewStatDic = new Dictionary<UFOStatEnum , int>();

    public event Action<bool/*bsuccess*/, int/*currentstarcnt*/> FOnReinforceApplied;
    public void InitializeStatWidgetList(UFOAllWidget allwidget ,string ufoname,  IReadOnlyList<UFOStatData> ufostatlist)
    {
        ufoAllWidget = allwidget;
        UFOName = ufoname;
        if (StatWidgetList.Count != 3) Debug.Log("stat ����");

        PreviewStatDic.Clear();
        OriginStatDic.Clear();

        ApplyButton.interactable = false;

        for (int i=0;i< ufostatlist.Count; i++)
        {
            UFOStatEnum statenum = ufostatlist[i].StatType;
            string statstring = statenum.ToString();
            int basestat = ufostatlist[i].BaseValue;
            int maxstat = ufostatlist[i].MaxValue;
            StatWidgetList[i].InitializeStatWidget(this, statenum, statstring, basestat, maxstat);

          
            PreviewStatDic[statenum] = basestat;
            OriginStatDic[statenum] =basestat;
        }

    }

    public void OnSelectPaletteWdiget(bool bactive)
    {
        //ĵ���� �׷� ��Ȱ��ȭ
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null)
        {
            //Debug.Log("ĵ���� ��Ȱ��ȭ" + bactive);
            cg.interactable = bactive;

        }
    }

    public void OnChangePreviewStat(UFOStatEnum stattype , int value)
    {
        int startcnt = GameManager.Instance.userData.StarCnt;
        
        PreviewStatDic[stattype] = value;

        CalculateTotalPrice();

        // Debug.Log(TotalPrcie);
        ChangeTotalPrcieText(TotalPrcie);
        if (startcnt < TotalPrcie)
            ApplyButton.interactable = false;
        else
            ApplyButton.interactable = true;
    }

    private void CalculateTotalPrice()
    {
        TotalPrcie = 0;
        foreach (var orignstat  in OriginStatDic)
        {
            int cnt = PreviewStatDic[orignstat.Key] - orignstat.Value;


            TotalPrcie += statPrcie * cnt;
        }
        
        Debug.Log(TotalPrcie);
    }

    private void ChangeTotalPrcieText(int totalprice)
    {
        priceText.text = totalprice.ToString();
    }


    public void OnClickApply()
    {
        UserUFOData userufodata = GameManager.Instance.userData.serialUFOList.Get(UFOName);
       int currentcnt =  GameManager.Instance.userData.MinusStartCnt(TotalPrcie);
        ChangeTotalPrcieText(currentcnt);
        
        TotalPrcie = 0;
        

        foreach (var preview in PreviewStatDic)
        {
            OriginStatDic[preview.Key] = preview.Value;
            userufodata.SetReinforce(preview.Key, preview.Value);
        }
        
        //Ȯ��
        foreach (var Origin in OriginStatDic)
        {
           Debug.Log("userdata : " + Origin.Key.ToString() + " : " + userufodata.GetReinforceValue(Origin.Key)+  " ���� ");
        }

        GameManager.Instance.SaveUserData();

       FOnReinforceApplied?.Invoke(true, TotalPrcie);

        foreach(var widget in StatWidgetList)
        {
            widget.ApplyStat();
        }

    }

    public void OnClickCancel()
    {
        foreach(var stat in StatWidgetList)
        {
            stat.OnCancelStat();
        }
    }



    public void ClearStat()
    {
        foreach (var stat in StatWidgetList)
        {
            stat.ClearStatWidget();
        }
    }


}
