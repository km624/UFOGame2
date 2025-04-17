using DanielLochner.Assets.SimpleScrollSnap;
using DG.Tweening;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEditor;
using UnityEngine;

public class UFOAllWidget : MonoBehaviour
{
    [SerializeField]private MainWidget mainWidget;

    [SerializeField] private RectTransform UFOSelectWidgetTransform;
    [SerializeField] private RectTransform UFOAllWidgetTransform;
    
    [SerializeField] private GameObject PreviewUFO;
    
    [SerializeField] private float slideDuration = 0.6f;
   
    private Vector2 shownPosition;
    private Vector2 hiddenPosition;
    private bool isVisible = false;

    [SerializeField] private SelectUFOWidget selectUFOWidget;
    [SerializeField] private SelectPalletWidget selectPalletWidget;
    [SerializeField] private ReinForceWidget reinForceWidget;

    [SerializeField] private Color MTlockedColor = new Color32(34, 34, 34, 255);

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
        Vector2 shownPos = new Vector2(-800f, 50f);
        UFOSelectWidgetTransform.DOAnchorPos(shownPos, slideDuration)
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
        Vector2 hiddenPos = new Vector2(0f, 50f);
        UFOSelectWidgetTransform.DOAnchorPos(hiddenPos, slideDuration)
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

        int currentindex = GameManager.Instance.userData.CurrentUFO;

        //미리 프리뷰 세팅
        CallBAck_ChangePreviewUFOType(currentindex, true);

        //UFO 선택 란 생성
        CreateSelectWidget();
    }

    private void CreateSelectWidget()
    {
        

        IReadOnlyDictionary<string, UserUFOData> ufomap =  GameManager.Instance.userData.serialUFOList.UFOMap;
        selectUFOWidget.InitializeSelectWidget(this, UFOLoadManager.Instance.LoadedUFODataList, 
            ufomap, GameManager.Instance.userData.CurrentUFO);

        //구매했을때 바인딩
        selectUFOWidget.FOnUFOPurchased += mainWidget.CallBack_OnPurchased;


    }

    public void CallBAck_ChangePreviewUFOType(int ufoindex, bool bunlock)
    {
        UFOData selectUFOdata = UFOLoadManager.Instance.LoadedUFODataList[ufoindex];

        MeshFilter meshFilter = PreviewUFO.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            meshFilter.mesh = selectUFOdata.UFOMesh;
        }

       
        MeshRenderer renderer = PreviewUFO.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            Material mat = null;

            //언록되지 않을때 
            if (!bunlock)
            {
                mat = new Material(selectUFOdata.UFOMaterials[0]);
                mat.color = MTlockedColor;
            }
            //언록 됬어있다면 선택한 컬러로 설정
            else
            {
                UserUFOData userufo = GameManager.Instance.userData.serialUFOList.Get(selectUFOdata.name);
                if(userufo!=null)
                {
                    int colorindex = userufo.CurrentColorIndex;
                    mat = new Material(selectUFOdata.UFOMaterials[colorindex]);
                }
              
            }
            renderer.material = mat;

            SetSelectUFO(ufoindex);
        }

        Debug.Log("Change : " + ufoindex + "  " + selectUFOdata);

    }

    public void SetSelectUFO(int selectindex)
    {
        if (GameManager.Instance.userData == null)
        {
            Debug.Log("UserData 없음");
            return;
        }

         UserData userdata = GameManager.Instance.userData;
        userdata.SetCurrentUFO(selectindex);
    }



}
