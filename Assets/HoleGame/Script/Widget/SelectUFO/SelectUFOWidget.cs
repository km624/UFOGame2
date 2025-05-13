
using DanielLochner.Assets.SimpleScrollSnap;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectUFOWidget : MonoBehaviour
{

    private UFOAllWidget ufoallWidget;

    [SerializeField]private UFOButtonWidget PrefabUFOBtnWidget;

    [SerializeField] private GameObject UFOBOXPrefab;

    private int InBoxCnt = 2;

    //private List<UFOButtonWidget> UFOBtnWidgetList = new List<UFOButtonWidget>();
    private Dictionary<string ,UFOButtonWidget> UFOBtnWidgetList = new Dictionary<string, UFOButtonWidget>();

    [SerializeField]private SimpleScrollSnapType2 scrollSnap2;

    [SerializeField] private GameObject SelectUFOContent;

    public event Action<string/*ufoname*/ ,bool/*bUnlock*/> FOnUFOSelected; 
    
    public event Action<bool/*bsuccess*/,int/*currentstarcnt*/> FOnUFOPurchased;

    private string selectUfoname = null;
    private int selectUfoPrice = 0;

    [SerializeField] private Button PurchaseButton;
    [SerializeField] private TMP_Text PriceText;

    [SerializeField] private Image PanelIamge;
    [SerializeField] private Color disableColor;
    [SerializeField] private Color enableColor;

    public void InitializeSelectWidget(UFOAllWidget ufowidget, IReadOnlyDictionary<string, UFOData> ufoList, 
        IReadOnlyDictionary<string, UserUFOData> userufo, string selectUFOname)
    {

        ufoallWidget = ufowidget;

        FOnUFOSelected += ufoallWidget.CallBAck_ChangePreviewUFOType;

        GameObject currentBox = null; // ���� UFOBOX Prefab �ν��Ͻ�
        int inBoxCounter = 0;          // ���� �ڽ� �ȿ� �� �� �־����� ī��Ʈ

       
        PriceText.text = string.Empty;

        PurchaseButton.interactable = false;

        int selectindex = 0;
        int index = 0;

        foreach (var ufodata in ufoList)
        {
            if (currentBox == null || inBoxCounter >= InBoxCnt)
            {
                currentBox = Instantiate(UFOBOXPrefab, SelectUFOContent.transform, false);
                inBoxCounter = 0;
            }

            bool bunlock = userufo.ContainsKey(ufodata.Key);
            bool bselect = (selectUFOname == ufodata.Key);

            if(bselect)
            {
                selectindex = index;
                selectUfoname = selectUFOname;
            }

            int price = ufodata.Value.UFOPrice;

            // UFOButtonWidget�� ���� Box�� ����
            UFOButtonWidget btnWidget = Instantiate(PrefabUFOBtnWidget, currentBox.transform, false);

            Sprite ufoicon = ufodata.Value.UFOIcon;

            bool bisreward = ufodata.Value.bIsReward;

            btnWidget.InitializeUFOButton(this, ufodata.Key, ufoicon, bunlock, bselect, price , bisreward);

            //UFOBtnWidgetList.Add(btnWidget);
            UFOBtnWidgetList.Add(ufodata.Key, btnWidget);

            inBoxCounter++;
            index++;
        }


        scrollSnap2.SetStartingPanel(selectindex / InBoxCnt);
    }
   
  

    public void SelectUFOType(string ufoname,bool bunlock,int ufoprice ,bool bisreward)
    {
        /*for(int i = 0;i<UFOBtnWidgetList.Count;i++)
        {
            if(index == i) continue;

            UFOBtnWidgetList[i].UnSelect();
        }*/
        foreach (var btn in UFOBtnWidgetList)
        {
            if (ufoname == btn.Key)
            {
                btn.Value.OnSelect();
                continue; 
            }

            btn.Value.UnSelect();
        }
        selectUfoname = ufoname;
       
        //��ȭ Ȯ��
        int starcnt = GameManager.Instance.userData.StarCnt;
        
       
        if (starcnt < ufoprice || bunlock )
        {
            PurchaseButton.interactable = false;
        }
        else
        {
            if(!bisreward)
                PurchaseButton.interactable = true;
            else
                PurchaseButton.interactable= false;
        }
       

        selectUfoPrice = ufoprice;
        if (!bunlock)
        {
            if (!bisreward)
                PriceText.text = ufoprice.ToString();
            else
                PriceText.text = "���� ����";
        }
        else
        {
            PriceText.text = string.Empty;
        }



        FOnUFOSelected?.Invoke(ufoname, bunlock);
       
    }

    public void PurchaseUFO()
    {
        bool bsuccess = GameManager.Instance.userData.CheckStarCnt(selectUfoPrice);
       
        int newcnt = GameManager.Instance.userData.MinusStartCnt(selectUfoPrice);
       
        FOnUFOPurchased.Invoke(bsuccess, newcnt);
        if (bsuccess)
        {
            //userdata�� �ش� ufo �߰�
            UFOData pruchaseUFOData = UFOLoadManager.Instance.ReadLoadedUFODataDic[selectUfoname];
            UserUFOData newuserUFOData = new UserUFOData(pruchaseUFOData);
            GameManager.Instance.userData.serialUFOList.AddUFO(newuserUFOData);

            //���� �����ϸ� ��ư �ر�
            UFOBtnWidgetList[selectUfoname].UnlockUFO();
            //UFO ������ ���� ����
            UFOBtnWidgetList[selectUfoname].OnClickSelectBtn();

            //��ȭâ ����
            ufoallWidget.OnEnableColorReinForceWidget();

            if (GameManager.Instance.userData != null)
            {
                //( ���� ) UFO Ÿ�� ����
                string collectufoid = $"Collect_{pruchaseUFOData.UFOName}";
                AchievementManager.Instance.ReportProgress(AchieveEnum.Collect, collectufoid, 1);

                //���� UFO ���� 
              
            }

        }

    }

    public void RewardCompleteUFO(string ufoname)
    {
        if (UFOBtnWidgetList.ContainsKey(ufoname))
        {
            Debug.Log(ufoname + "UFO �ر�");
            UFOBtnWidgetList[ufoname].UnlockUFO();
        }
        else
            Debug.Log("UFO ����");
    }

    public void EnablePanel(bool benable)
    {
        PanelIamge.color = benable ? enableColor :disableColor;
    }

}
