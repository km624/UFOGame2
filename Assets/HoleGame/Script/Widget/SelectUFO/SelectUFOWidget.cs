
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

        GameObject currentBox = null; // 현재 UFOBOX Prefab 인스턴스
        int inBoxCounter = 0;          // 현재 박스 안에 몇 개 넣었는지 카운트

       
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

            // UFOButtonWidget을 현재 Box에 생성
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
       
        //재화 확인
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
            //userdata의 해당 ufo 추가
            UFOData pruchaseUFOData = UFOLoadManager.Instance.LoadedUFODataList[selectUfoindex];
            UserUFOData newuserUFOData = new UserUFOData(pruchaseUFOData);
            GameManager.Instance.userData.serialUFOList.AddUFO(newuserUFOData);

            //구매 성공하면 버튼 해금
            UFOBtnWidgetList[selectUfoindex].UnlockUFO();
            //UFO 선택한 로직 실행
            UFOBtnWidgetList[selectUfoindex].OnClickSelectBtn();

            //강화창 띄우기
            ufoallWidget.OnEnableColorReinForceWidget();

        }

    }

    
}
