
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

    private List<UFOButtonWidget> UFOBtnWidgetList = new List<UFOButtonWidget>();

    [SerializeField]private SimpleScrollSnapType2 scrollSnap2;

    [SerializeField] private GameObject SelectUFOContent;

    public event Action<int/*ufoindex*/ ,bool/*bUnlock*/> FOnUFOSelected; 
    
    public event Action<bool/*bsuccess*/,int/*currentstarcnt*/> FOnUFOPurchased;

    private int selectUfoindex = 0;
    private int selectUfoPrice = 0;

    [SerializeField] private Button PurchaseButton;
    [SerializeField] private TMP_Text PriceText;

    public void InitializeSelectWidget(UFOAllWidget ufowidget, IReadOnlyList<UFOData> ufoList, 
        IReadOnlyDictionary<string, UserUFOData> userufo, int selectindex)
    {

        ufoallWidget = ufowidget;

        FOnUFOSelected += ufoallWidget.CallBAck_ChangePreviewUFOType;

        GameObject currentBox = null; // ���� UFOBOX Prefab �ν��Ͻ�
        int inBoxCounter = 0;          // ���� �ڽ� �ȿ� �� �� �־����� ī��Ʈ

       
        PriceText.text = string.Empty;

        PurchaseButton.interactable = false;

        for (int i = 0; i < ufoList.Count; i++)
        {
            if (currentBox == null || inBoxCounter >= InBoxCnt)
            {
                currentBox = Instantiate(UFOBOXPrefab, SelectUFOContent.transform, false);
                inBoxCounter = 0;
            }

            bool bunlock = userufo.ContainsKey(ufoList[i].UFOName);
            bool bselect = (selectindex == i);

           

            selectUfoindex = selectindex;

            int price = ufoList[i].UFOPrice;

            // UFOButtonWidget�� ���� Box�� ����
            UFOButtonWidget btnWidget = Instantiate(PrefabUFOBtnWidget, currentBox.transform, false);

            Sprite ufoicon = ufoList[i].UFOIcon;

            btnWidget.InitializeUFOButton(this, i, ufoicon, bunlock, bselect, price);

            UFOBtnWidgetList.Add(btnWidget);

            inBoxCounter++;
        }

        scrollSnap2.SetStartingPanel(selectindex / InBoxCnt);
    }
   
   UFOButtonWidget CreateUFOButtonWidget(int index, bool bselect , bool bunlock, int price)
   {
       
        UFOButtonWidget btnWidget = Instantiate(PrefabUFOBtnWidget, SelectUFOContent.transform, false);

      
        btnWidget.InitializeUFOButton(this,index,null, bunlock, bselect, price); 

       
        UFOBtnWidgetList.Add(btnWidget);

        return btnWidget;

        
    }

    public void SelectUFOType(int index,bool bunlock,int ufoprice)
    {
        for(int i = 0;i<UFOBtnWidgetList.Count;i++)
        {
            if(index == i) continue;

            UFOBtnWidgetList[i].UnSelect();
        }
        selectUfoindex = index;
       
        //��ȭ Ȯ��
        int starcnt = GameManager.Instance.userData.StarCnt;
      
        if (starcnt < ufoprice || bunlock)
        {
            PurchaseButton.interactable = false;
        }
        else
        {
            PurchaseButton.interactable = true;
        }
       

        selectUfoPrice = ufoprice;
        if (!bunlock)
        {
            PriceText.text = ufoprice.ToString();
        }
        else
        {
            PriceText.text = string.Empty;
        }

        FOnUFOSelected?.Invoke(index , bunlock);
       
    }

    public void PurchaseUFO()
    {
        bool bsuccess = GameManager.Instance.userData.CheckStarCnt(selectUfoPrice);
       
        int newcnt = GameManager.Instance.userData.MinusStartCnt(selectUfoPrice);
       
        FOnUFOPurchased.Invoke(bsuccess, newcnt);
        if (bsuccess)
        {
            //userdata�� �ش� ufo �߰�
            UFOData pruchaseUFOData = UFOLoadManager.Instance.LoadedUFODataList[selectUfoindex];
            UserUFOData newuserUFOData = new UserUFOData(pruchaseUFOData);
            GameManager.Instance.userData.serialUFOList.AddUFO(newuserUFOData);

            //���� �����ϸ� ��ư �ر�
            UFOBtnWidgetList[selectUfoindex].UnlockUFO();
            //UFO ������ ���� ����
            UFOBtnWidgetList[selectUfoindex].OnClickSelectBtn();

            //��ȭâ ����
            ufoallWidget.OnEnableColorReinForceWidget();

        }

    }

    
}
