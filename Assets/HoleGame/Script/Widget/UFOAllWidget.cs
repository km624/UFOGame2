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

        // 블록 끄고 → 끝나면 켜기
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
        // 블록 끄고 → 끝나면 비활성화
        // SetInteractable(false);
       
        int currentufo = GameManager.Instance.userData.CurrentUFO;

        //판넬 닺힐때 강제 리셋
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
        Debug.Log("UFO 선택/강화/색깔 세팅 시작");

        if (UFOLoadManager.Instance == null) return;
        if (UFOLoadManager.Instance.LoadedUFODataList.Count <= 0)
        {
            Debug.Log("데이터 없음");
            return;
        }
        if (GameManager.Instance == null) return;
        if (GameManager.Instance.userData == null)
        {
            Debug.Log("UserData 없음");
            return;
        }

       int currentUFOindex = GameManager.Instance.userData.CurrentUFO;

        
        //UFO 선택 란 생성
        CreateSelectWidget(currentUFOindex);

        CallBAck_ChangePreviewUFOType(currentUFOindex, true);

    }

    private void CreateSelectWidget(int currnetUFOIndex)
    {
        //UFO 종류 위젯 세팅
        //int currentUFOindex = GameManager.Instance.userData.CurrentUFO;
        IReadOnlyList<UFOData> ufoDataList = UFOLoadManager.Instance.LoadedUFODataList;
        UFOData currentUFOdata = ufoDataList[currnetUFOIndex];

        IReadOnlyDictionary<string, UserUFOData> ufomap =  GameManager.Instance.userData.serialUFOList.UFOMap;
        selectUFOWidget.InitializeSelectWidget(this, ufoDataList,
            ufomap, currnetUFOIndex);

        //구매했을때 바인딩
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

            if (!bunlock) // 언락 안 된 경우
            {
                int count = currentUFOdata.UFOColorDataList[0].Materials.Count;
                mats = new Material[count];
                for (int i = 0; i < count; i++)
                {
                    mats[i] = new Material(currentUFOdata.UFOColorDataList[0].Materials[i]); // 복사
                    mats[i].color = MTlockedColor;
                }
            }
            else // 언락된 경우
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
                        mats[i] = new Material(colorSet.Materials[i]); // 복사본
                    }
                }

                SaveSelectUFO(ufoindex);
            }

            // 머터리얼 배열 통째로 적용
            renderer.materials = mats;

            

            // UFO 새로 선택하면 위젯 갱신
            if(selecetUFOindex != ufoindex)
            {
                selecetUFOindex = ufoindex;
                RenewalWidget(selecetUFOindex, bunlock);
            }
            //구매해서 다시 프리뷰 세팅했다는 뜻
            //색깔 , 강화 위젯 활성화
            else
            {
                OnEnableSelectWidget(bunlock);
                Debug.Log("색깔 , 강화 가능");
            }
              
        }

            Debug.Log("Set : " + selecetUFOindex + "  " + currentUFOdata);

    }

    public void SaveSelectUFO(int selectindex)
    {
        if (GameManager.Instance.userData == null)
        {
            Debug.Log("UserData 없음");
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
                mats[i] = new Material(colorSet.Materials[i]); // 복사본
            }

            // 머터리얼 배열 통째로 적용
            renderer.materials = mats;
            
            if(bunclock)
                SaveSelectColor(colorindex, currentUFOdata);

        }
    }
    private void SaveSelectColor(int selectindex, UFOData selectufodata)
    {
        if (GameManager.Instance.userData == null)
        {
            Debug.Log("UserData 없음");
            return;
        }

        UserUFOData userufodata = GameManager.Instance.userData.serialUFOList.Get(selectufodata.UFOName);
        userufodata.CurrentColorIndex = selectindex;

        GameManager.Instance.SaveUserData();
    }

    private void RenewalWidget(int UFOindex, bool bUnlock)
    {

        //기본 데이터 참조
        IReadOnlyList<UFOData> ufoDataList = UFOLoadManager.Instance.LoadedUFODataList;
        IReadOnlyDictionary<string, UserUFOData> ufomap = GameManager.Instance.userData.serialUFOList.UFOMap;
        UFOData currentUFOdata = ufoDataList[UFOindex];

        //컬러 위젯 세팅
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


        //UFO 스텟 세팅
        IReadOnlyList<UFOStatData> statdatalist = currentUFOdata.StatList;
       
        if (bUnlock && ufomap.ContainsKey(currentUFOdata.UFOName))
        {
            statdatalist = ufomap[currentUFOdata.UFOName].StatReinforceList;
            
        }

        reinForceWidget.ClearStat();

        reinForceWidget.InitializeStatWidgetList(this, currentUFOdata.UFOName, statdatalist);

        OnEnableSelectWidget(bUnlock);

        Debug.Log("새로 생성 : " + Time.time);

    }

    public void OnEnableSelectWidget(bool bunlock)
    {
        selectPaletteWidget.OnSelectPaletteWdiget(bunlock);
        reinForceWidget.OnSelectPaletteWdiget(bunlock);
    }



}
