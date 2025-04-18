
using DanielLochner.Assets.SimpleScrollSnap;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SelectUFOWidget : MonoBehaviour
{

    private UFOAllWidget ufoallWidget;

    [SerializeField]private UFOButtonWidget PrefabUFOBtnWidget;
    //[SerializeField]private UFOButtonWidget SelectUFOContent;

    private List<UFOButtonWidget> UFOBtnWidgetList = new List<UFOButtonWidget>();

    [SerializeField]private SimpleScrollSnapType2 scrollsnap2;

    [SerializeField] private GameObject SelectUFOContent;

    public event Action<int/*ufoindex*/ ,bool/*bUnlock*/> FOnUFOSelected; 
    
    public event Action<bool/*bsuccess*/,int/*currentstarcnt*/> FOnUFOPurchased;

    public void InitializeSelectWidget(UFOAllWidget ufowidget, IReadOnlyList<UFOData> ufoList, 
        IReadOnlyDictionary<string, UserUFOData> userufo,int selectindex)
    {
       

        ufoallWidget = ufowidget;

        //ufo 타입 바인딩
        FOnUFOSelected+= ufoallWidget.CallBAck_ChangePreviewUFOType;

        for (int i = 0; i < ufoList.Count; i++)
        {
            bool bunlock =false;
            bool bselect = false;

            if (userufo.ContainsKey(ufoList[i].UFOName))
            {
                bunlock = true;
                Debug.Log("언록 UFO : " + i);
            }
            if(selectindex==i)
            {
                bselect = true;
                Debug.Log("선택 UFO : " + i);
            }
                

            int price = ufoList[i].UFOPrice;

            CreateUFOButtonWidget(i,bselect, bunlock, price);
        }
        
        scrollsnap2.SetStartingPanel(selectindex);
    }
   
   void CreateUFOButtonWidget(int index, bool bselect , bool bunlock, int price)
   {
       
        UFOButtonWidget btnWidget = Instantiate(PrefabUFOBtnWidget, SelectUFOContent.transform, false);

      
        btnWidget.InitializeUFOButton(this,index,null, bunlock, bselect, price); 

       
        UFOBtnWidgetList.Add(btnWidget);

        scrollsnap2.AddInstantaiteBack(btnWidget.gameObject);
    }

    public void SelectUFOType(int index,bool bunlock)
    {
        for(int i = 0;i<UFOBtnWidgetList.Count;i++)
        {
            if(index == i) continue;

            UFOBtnWidgetList[i].UnSelect();
        }
        FOnUFOSelected?.Invoke(index , bunlock);
       
    }

    public void PurchaseUFO(int index,int price)
    {
        bool bsuccess = GameManager.Instance.userData.CheckStarCnt(price);
       
        int newcnt = GameManager.Instance.userData.MinusStartCnt(price);
       
        FOnUFOPurchased.Invoke(bsuccess, newcnt);
        if (bsuccess)
        {
            //userdata의 해당 ufo 추가
            UFOData pruchaseUFOData = UFOLoadManager.Instance.LoadedUFODataList[index];
            UserUFOData newuserUFOData = new UserUFOData(pruchaseUFOData);
            GameManager.Instance.userData.serialUFOList.AddUFO(newuserUFOData);

            //구매 성공하면 버튼 해금
            UFOBtnWidgetList[index].UnlockUFO();
            //UFO 선택한 로직 실행
            UFOBtnWidgetList[index].OnClickSelectBtn();


        }

    }

    
}
