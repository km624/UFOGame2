using DanielLochner.Assets.SimpleScrollSnap;
using DG.Tweening;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEditor;
using UnityEngine;
using static Unity.Burst.Intrinsics.X86;

public class UFOAllWidget : MonoBehaviour
{
    [SerializeField]private MainWidget mainWidget;

    [SerializeField] private RectTransform UFOAllWidgetTransform;
    //[SerializeField] private RectTransform UFOSlideButtonTransform;
    //[SerializeField] private RectTransform UFOAllWidgetTransform;
    
    [SerializeField] private GameObject PreviewUFO;


    [SerializeField] private float slideDuration = 0.6f;
   
    private Vector2 shownPosition;
    private Vector2 hiddenPosition;
    private bool isVisible = false;

    [SerializeField] private SelectUFOWidget selectUFOWidget;
    [SerializeField] private SelectPaletteWidget selectPaletteWidget;
    [SerializeField] private ReinForceWidget reinForceWidget;

    [SerializeField] private Color MTlockedColor = new Color32(34, 34, 34, 255);

    private int selecetUFOindex = -1;

    void Start()
    {
       
    }

    public void TogglePanel()
    {
        if (isVisible)
            HidePanel();
        else
            ShowPanel();

       
    }
    private void ShowPanel()
    {

        // ��� ���� �� ������ �ѱ�
        //SetInteractable(false);

        Vector2 shownPos = new Vector2(0, 50f);
       
  
        UFOAllWidgetTransform.DOAnchorPos(shownPos, slideDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                //SetInteractable(true);
                isVisible = true;
            });
    }

    private void HidePanel()
    {
        // ��� ���� �� ������ ��Ȱ��ȭ
        // SetInteractable(false);
       
        int currentufo = GameManager.Instance.userData.CurrentUFO;

        //�ǳ� ������ ���� ����
        CallBAck_ChangePreviewUFOType(currentufo, true);
        RenewalWidget(currentufo,true);


        Vector2 hiddenPos = new Vector2(800f, 50f);
        UFOAllWidgetTransform.DOAnchorPos(hiddenPos, slideDuration)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                isVisible = false;
                
            });
    }

    private void SetInteractable(bool state)
    {
        CanvasGroup cg = UFOAllWidgetTransform.GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.interactable = state;
            cg.blocksRaycasts = state;
        }
    }

    public void InitializedUFOAllWidget()
    {
        Debug.Log("UFO ����/��ȭ/���� ���� ����");

        if (UFOLoadManager.Instance == null) return;
        if (UFOLoadManager.Instance.LoadedUFODataList.Count <= 0)
        {
            Debug.Log("������ ����");
            return;
        }
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.userData == null)
        {
            Debug.Log("UserData ����");
            return;
        }

       int currentUFOindex = GameManager.Instance.userData.CurrentUFO;

        
        //UFO ���� �� ����
        CreateSelectWidget(currentUFOindex);

        CallBAck_ChangePreviewUFOType(currentUFOindex, true);

    }

    private void CreateSelectWidget(int currnetUFOIndex)
    {
        //UFO ���� ���� ����
        //int currentUFOindex = GameManager.Instance.userData.CurrentUFO;
        IReadOnlyList<UFOData> ufoDataList = UFOLoadManager.Instance.LoadedUFODataList;
        UFOData currentUFOdata = ufoDataList[currnetUFOIndex];

        IReadOnlyDictionary<string, UserUFOData> ufomap =  GameManager.Instance.userData.serialUFOList.UFOMap;
        selectUFOWidget.InitializeSelectWidget(this, ufoDataList,
            ufomap, currnetUFOIndex);

        //���������� ���ε�
        selectUFOWidget.FOnUFOPurchased += mainWidget.CallBack_OnPurchased;
        selectPaletteWidget.FOnColorPurchased += mainWidget.CallBack_OnPurchased;
        reinForceWidget.FOnReinforceApplied += mainWidget.CallBack_OnPurchased;


    }

    public void CallBAck_ChangePreviewUFOType(int ufoindex, bool bunlock)
    {
        
       UFOData currentUFOdata = UFOLoadManager.Instance.LoadedUFODataList[ufoindex];
    
        MeshFilter meshFilter = PreviewUFO.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            meshFilter.mesh = currentUFOdata.UFOMesh;
        }

        MeshRenderer renderer = PreviewUFO.GetComponent<MeshRenderer>();
        
        if (renderer != null)
        {
            Material[] mats = null;

            if (!bunlock) // ��� �� �� ���
            {
                int count = currentUFOdata.UFOColorDataList[0].Materials.Count;
                mats = new Material[count];
                for (int i = 0; i < count; i++)
                {
                    mats[i] = new Material(currentUFOdata.UFOColorDataList[0].Materials[i]); // ����
                    mats[i].color = MTlockedColor;
                }
            }
            else // ����� ���
            {
                UserUFOData userufo = GameManager.Instance.userData.serialUFOList.Get(currentUFOdata.UFOName);
                if (userufo != null)
                {
                    int colorIndex = userufo.CurrentColorIndex;

                    var colorSet = currentUFOdata.UFOColorDataList[colorIndex]; 
                    int count = colorSet.Materials.Count;
                    mats = new Material[count];
                    for (int i = 0; i < count; i++)
                    {
                        mats[i] = new Material(colorSet.Materials[i]); // ���纻
                    }
                }

                SaveSelectUFO(ufoindex);
            }

            // ���͸��� �迭 ��°�� ����
            renderer.materials = mats;

            

            // UFO ���� �����ϸ� ���� ����
            if(selecetUFOindex != ufoindex)
            {
                selecetUFOindex = ufoindex;
                RenewalWidget(selecetUFOindex, bunlock);
            }
            //�����ؼ� �ٽ� ������ �����ߴٴ� ��
            //���� , ��ȭ ���� Ȱ��ȭ
            else
            {
                OnEnableSelectWidget(bunlock);
                Debug.Log("���� , ��ȭ ����");
            }
              
        }

            Debug.Log("Set : " + selecetUFOindex + "  " + currentUFOdata);

    }

    public void SaveSelectUFO(int selectindex)
    {
        if (GameManager.Instance.userData == null)
        {
            Debug.Log("UserData ����");
            return;
        }

         UserData userdata = GameManager.Instance.userData;
        userdata.SetCurrentUFO(selectindex);

        GameManager.Instance.SaveUserData();
    }
    
    public void CallBack_ChangeColor(int colorindex,bool bunclock)
    {
        int currentUFOIndex = GameManager.Instance.userData.CurrentUFO;
        UFOData currentUFOdata = UFOLoadManager.Instance.LoadedUFODataList[currentUFOIndex];


        MeshRenderer renderer = PreviewUFO.GetComponent<MeshRenderer>();

        if (renderer != null)
        {
            Material[] mats = null;


            var colorSet = currentUFOdata.UFOColorDataList[colorindex];
            int count = colorSet.Materials.Count;

            mats = new Material[count];
            for (int i = 0; i < count; i++)
            {
                mats[i] = new Material(colorSet.Materials[i]); // ���纻
            }

            // ���͸��� �迭 ��°�� ����
            renderer.materials = mats;
            
            if(bunclock)
                SaveSelectColor(colorindex, currentUFOdata);

        }
    }
    private void SaveSelectColor(int selectindex, UFOData selectufodata)
    {
        if (GameManager.Instance.userData == null)
        {
            Debug.Log("UserData ����");
            return;
        }

        UserUFOData userufodata = GameManager.Instance.userData.serialUFOList.Get(selectufodata.UFOName);
        userufodata.CurrentColorIndex = selectindex;

        GameManager.Instance.SaveUserData();
    }

    private void RenewalWidget(int UFOindex, bool bUnlock)
    {

        //�⺻ ������ ����
        IReadOnlyList<UFOData> ufoDataList = UFOLoadManager.Instance.LoadedUFODataList;
        IReadOnlyDictionary<string, UserUFOData> ufomap = GameManager.Instance.userData.serialUFOList.UFOMap;
        UFOData currentUFOdata = ufoDataList[UFOindex];

        //�÷� ���� ����
        IReadOnlyList<UFOColorData> currentUFOColorList = currentUFOdata.UFOColorDataList;
        IReadOnlyList<int> OwnUserColorList = null;
        int selectcolorindex = 0;
        if (bUnlock && ufomap.ContainsKey(currentUFOdata.UFOName))
        {
            OwnUserColorList = ufomap[currentUFOdata.UFOName].OwnedColorIndexes;
            selectcolorindex = ufomap[currentUFOdata.UFOName].CurrentColorIndex;
        }
        selectPaletteWidget.CallBack_ClearPalettetWidget();

        selectPaletteWidget.InitializeSelectWidget(this, UFOindex, currentUFOColorList, OwnUserColorList, selectcolorindex);


        //UFO ���� ����
        IReadOnlyList<UFOStatData> statdatalist = currentUFOdata.StatList;
       
        if (bUnlock && ufomap.ContainsKey(currentUFOdata.UFOName))
        {
            statdatalist = ufomap[currentUFOdata.UFOName].StatReinforceList;
            
        }

        reinForceWidget.ClearStat();

        reinForceWidget.InitializeStatWidgetList(this, currentUFOdata.UFOName, statdatalist);

        OnEnableSelectWidget(bUnlock);

        Debug.Log("���� ���� : " + Time.time);

    }

    public void OnEnableSelectWidget(bool bunlock)
    {
        selectPaletteWidget.OnSelectPaletteWdiget(bunlock);
        reinForceWidget.OnSelectPaletteWdiget(bunlock);
    }



}
