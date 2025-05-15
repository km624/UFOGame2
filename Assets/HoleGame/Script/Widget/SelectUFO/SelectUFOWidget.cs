
using DanielLochner.Assets.SimpleScrollSnap;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectUFOWidget : MonoBehaviour
{

    private UFOAllWidget ufoallWidget;

    [SerializeField]private UFOButtonWidget PrefabUFOBtnWidget;

    [SerializeField] private GameObject UFOBOXPrefab;

    private int InBoxCnt = 2;

    private List<UFOButtonWidget> UFOBtnWidgetInstances = new List<UFOButtonWidget>();
    private Dictionary<string ,UFOButtonWidget> UFOBtnWidgetList = new Dictionary<string, UFOButtonWidget>();

    private List<GameObject> UFOBoxList = new List<GameObject>();   


    [SerializeField]private SimpleScrollSnapType2 scrollSnap2;

    [SerializeField] private GameObject SelectUFOContent;
    [SerializeField] private GameObject TempContent;

    public event Action<string/*ufoname*/ ,bool/*bUnlock*/> FOnUFOSelected; 
    
    public event Action<bool/*bsuccess*/,int/*currentstarcnt*/> FOnUFOPurchased;

    private string selectUfoname = null;
    private int selectUfoPrice = 0;

    [SerializeField] private Button PurchaseButton;
    [SerializeField] private Image ButtonBackgroundImage;
    [SerializeField] private TMP_Text PriceText;

    [SerializeField] private Image PanelIamge;
    [SerializeField] private Color disableColor;
    [SerializeField] private Color enableColor;

    IReadOnlyDictionary<string, UFOData> UfoListDic;
    IReadOnlyDictionary<string, UserUFOData> UserufoDic;

    public void InitializeSelectWidget(UFOAllWidget ufowidget, IReadOnlyDictionary<string, UFOData> ufoList, 
        IReadOnlyDictionary<string, UserUFOData> userufo, string selectUFOname)
    {

        ufoallWidget = ufowidget;

        FOnUFOSelected += ufoallWidget.CallBAck_ChangePreviewUFOType;

       /* GameObject currentBox = null; // ���� UFOBOX Prefab �ν��Ͻ�
        int inBoxCounter = 0;          // ���� �ڽ� �ȿ� �� �� �־����� ī��Ʈ
*/
       
        PriceText.text = string.Empty;

        //PurchaseButton.interactable = false;
        PurchaseButton.gameObject.SetActive(false);

        UfoListDic = ufoList;
        UserufoDic = userufo;
        selectUfoname = selectUFOname;

        CreateInstances(UfoListDic);
        ArrayUFOButton(UfoListDic, UserufoDic, selectUfoname);


     /*   foreach (var ufodata in sortedUfoList)
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


        scrollSnap2.SetStartingPanel(selectindex / InBoxCnt);*/
    }

    private void CreateInstances(IReadOnlyDictionary<string, UFOData> ufoList)
    {

        GameObject currentBox = null; // ���� UFOBOX Prefab �ν��Ͻ�
        int inBoxCounter = 0;          // ���� �ڽ� �ȿ� �� �� �־����� ī��Ʈ


      
        foreach (var ufodata in ufoList)
        {
            if (currentBox==null ||inBoxCounter >= InBoxCnt)
            {
                currentBox = Instantiate(UFOBOXPrefab, TempContent.transform, false);
                inBoxCounter = 0;
                UFOBoxList.Add(currentBox);
                
            }
            Debug.Log("�ڼ� ���� " + UFOBoxList.Count);
            UFOButtonWidget btnWidget = Instantiate(PrefabUFOBtnWidget, currentBox.transform, false);

            UFOBtnWidgetInstances.Add(btnWidget);

            inBoxCounter++;
        }
    }

    private void ArrayUFOButton(IReadOnlyDictionary<string, UFOData> ufoList, 
        IReadOnlyDictionary<string, UserUFOData> userufo, string selectUFOname)
    {
        GameObject currentBox = null; // ���� UFOBOX Prefab �ν��Ͻ�
        int inBoxCounter = 0;          // ���� �ڽ� �ȿ� �� �� �־����� ī��Ʈ
        int currentBoxcounter = 0;
        int selectindex = 0;
        int index = 0;
       
        var sortedUfoList = ufoList
     .OrderBy(kv => !userufo.ContainsKey(kv.Key))  // unlock ����
     .ThenBy(kv => kv.Value.UFOPrice)              // ���� ��������
     .ToList();

        foreach (var ufodata in sortedUfoList)
        {
            if (currentBox == null || inBoxCounter >= InBoxCnt)
            {
                //currentBox = Instantiate(UFOBOXPrefab, SelectUFOContent.transform, false);
                Debug.Log(index);
                Debug.Log(currentBoxcounter);
                currentBox = UFOBoxList[currentBoxcounter];
                currentBox.transform.SetParent(SelectUFOContent.transform, false);
                currentBox.SetActive(true);
                inBoxCounter = 0;
                currentBoxcounter++;
            }

            bool bunlock = userufo.ContainsKey(ufodata.Key);
            bool bselect = (selectUFOname == ufodata.Key);

            if (bselect)
            {
                selectindex = index;
                selectUfoname = selectUFOname;
            }

            int price = ufodata.Value.UFOPrice;

          
            UFOButtonWidget btnWidget = UFOBtnWidgetInstances[index];
            btnWidget.transform.SetParent(currentBox.transform, false);
            Sprite ufoicon = ufodata.Value.UFOIcon;

            bool bisreward = ufodata.Value.bIsReward;

            btnWidget.InitializeUFOButton(this, ufodata.Key, ufoicon, bunlock, bselect, price, bisreward);

            //UFOBtnWidgetList.Add(btnWidget);
            UFOBtnWidgetList.Add(ufodata.Key, btnWidget);

            inBoxCounter++;
            index++;
        }


        scrollSnap2.SetStartingPanel(selectindex / InBoxCnt);
    }

    public void RenewalUFOButtons()
    {
        AllWidgetClear();
      
        ArrayUFOButton(UfoListDic, UserufoDic, selectUfoname);
    }
  

    public void SelectUFOType(string ufoname,bool bunlock,int ufoprice ,bool bisreward)
    {
        
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
        
        if (bunlock)
        {
            PurchaseButton.gameObject.SetActive(false);
        }
        else
        {
            PurchaseButton.gameObject.SetActive(true);
        }

        if (starcnt < ufoprice )
        {
            PurchaseButton.interactable = false;
            ButtonBackgroundImage.color = disableColor;
        }
        else
        {
            if(!bisreward)
            {
                PurchaseButton.interactable = true;
                ButtonBackgroundImage.color = enableColor;


            }
            else
            {
                PurchaseButton.interactable = false;
                ButtonBackgroundImage.color = disableColor;

            }
               
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

            if (GameManager.Instance.userData != null)
            {
                //( ���� ) UFO Ÿ�� ����
                string collectufoid = $"Collect_{pruchaseUFOData.UFOName}";
                AchievementManager.Instance.ReportProgress(AchieveEnum.Collect, collectufoid, 1);

                //���� UFO ���� 
              
            }

            RenewalUFOButtons();
           /* //���� �����ϸ� ��ư �ر�
            UFOBtnWidgetList[selectUfoname].UnlockUFO();*/
            //UFO ������ ���� ����
           
            UFOBtnWidgetList[selectUfoname].OnClickSelectBtn();
            //��ȭâ ����
            ufoallWidget.OnEnableColorReinForceWidget();

        }

    }

    public void RewardCompleteUFO(string ufoname)
    {
        if (UFOBtnWidgetList.ContainsKey(ufoname))
        {
            Debug.Log(ufoname + "UFO �ر�");
            //UFOBtnWidgetList[ufoname].UnlockUFO();
            RenewalUFOButtons();


        }
        else
            Debug.Log("UFO ����");
    }

    private void AllWidgetClear()
    {
        foreach (var widget in UFOBoxList)
        {
            
            widget.gameObject.SetActive(false);
            widget.transform.SetParent(TempContent.transform, false);

        }

        UFOBtnWidgetList.Clear();

    }



    public void EnablePanel(bool benable)
    {
        PanelIamge.color = benable ? enableColor :disableColor;
    }

}
