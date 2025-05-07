using DanielLochner.Assets.SimpleScrollSnap;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class SelectPaletteWidget : MonoBehaviour
{
    private UFOAllWidget ufoallWidget;

    //private int Currentufoindex;
    private string CurrentUfoName;

    [SerializeField] private PaletteButtonWidget PrefabPaletteBtnWidget;
 
    private List<PaletteButtonWidget> PaletteBtnWidgetList = new List<PaletteButtonWidget>();

    [SerializeField] private SimpleScrollSnapType2 scrollsnap2;

    [SerializeField] private GameObject SelectPaletteContent;

    public event Action<int/*ufoindex*/,bool /*bunlock*/> FOnColorSelected;

    public event Action<bool/*bsuccess*/, int/*currentstarcnt*/> FOnColorPurchased;





    public void InitializeSelectWidget(UFOAllWidget ufowidget, string currentUFOname, IReadOnlyList<UFOColorData> UFOcolordataList,
        IReadOnlyList<int> userufocolorlist, int selectindex)
    {


        ufoallWidget = ufowidget;
        CurrentUfoName = currentUFOname;
        //�÷� ���� ���ε�
        FOnColorSelected += ufoallWidget.CallBack_ChangeColor;

        for (int i = 0; i < UFOcolordataList.Count; i++)
        {
            bool bunlock = false;
            bool bselect = false;

            if (userufocolorlist != null)
            {
                if (userufocolorlist.Contains(i))
                {
                    bunlock = true;
                    Debug.Log("��� �÷� : " + i);
                }
            }
            if (i == 0)
                bunlock = true;

            if (selectindex == i)
            {
                bselect = true;
                Debug.Log("���� �÷� : " + i);
            }


            int price = UFOcolordataList[i].ColorPrice;
            Color32 iconcolor = UFOcolordataList[i].IConColor;

            bool bisreward = UFOcolordataList[i].bIsreward;

            CreatePaletteButtonWidget(i, bselect, iconcolor, bunlock, price, bisreward);
        }

        scrollsnap2.SetStartingPanel(selectindex);





    }

    void CreatePaletteButtonWidget(int index, bool bselect,Color32 ufocolor , bool bunlock, int price ,bool bisreward)
    {

        PaletteButtonWidget btnWidget = Instantiate(PrefabPaletteBtnWidget, SelectPaletteContent.transform, false);


        btnWidget.InitializePaletteButton(this, index, ufocolor, price, bselect, bunlock, bisreward);


        PaletteBtnWidgetList.Add(btnWidget);

        scrollsnap2.AddInstantaiteBack(btnWidget.gameObject);
        
    }

    public void SelectColor(int index,bool bunlock)
    {
        for (int i = 0; i < PaletteBtnWidgetList.Count; i++)
        {
            if (index == i) continue;

            PaletteBtnWidgetList[i].UnSelect();
        }
        FOnColorSelected?.Invoke(index, bunlock);

    }

    public void PurchaseColor(int index, int price)
    {
        bool bsuccess = GameManager.Instance.userData.CheckStarCnt(price);

        int newcnt = GameManager.Instance.userData.MinusStartCnt(price);

        //MainWidget�Լ�
        FOnColorPurchased.Invoke(bsuccess, newcnt);

        if (bsuccess)
        {
            UFOData currentUFOData = UFOLoadManager.Instance.ReadLoadedUFODataDic[CurrentUfoName];
            UserUFOData userufodata = GameManager.Instance.userData.serialUFOList.Get(currentUFOData.UFOName);
            userufodata.AddColor(index);

            //���� �����ϸ� ��ư �ر�
            PaletteBtnWidgetList[index].UnlockPalette();
            //UFO ������ ���� ����
            PaletteBtnWidgetList[index].OnClickSelectBtn();

            if (GameManager.Instance.userData != null)
            {
                //( ���� )��Ų ���� ī��Ʈ 
                string CollectSkincntId = $"Collect_UFO_Skin_Cnt";
                AchievementManager.Instance.ReportProgress(AchieveEnum.Collect, CollectSkincntId, 1);


            }

        }
    }
 
    public void CallBack_ClearPalettetWidget()
    {
       for(int i = PaletteBtnWidgetList.Count-1; i >= 0;i--)
       {
            scrollsnap2.Remove(i);
       }
        PaletteBtnWidgetList.Clear();

        //init�Ҷ� �ߺ��Ǽ� ���� ����
        if (ufoallWidget!=null)
            FOnColorSelected -= ufoallWidget.CallBack_ChangeColor;

        Debug.Log("�ȷ�Ʈ Ŭ����");
    }

    public void RewardCompleteColor(string ufoname ,int colorindex)
    {
        if (CurrentUfoName ==ufoname)
        {
            PaletteBtnWidgetList[colorindex].UnlockPalette();
        }
        else
            Debug.Log(" ���� ����");
    }
}
