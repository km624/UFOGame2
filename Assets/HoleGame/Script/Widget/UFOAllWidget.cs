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

        // ��� ���� �� ������ �ѱ�
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
        // ��� ���� �� ������ ��Ȱ��ȭ
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

        int currentindex = GameManager.Instance.userData.CurrentUFO;

        //�̸� ������ ����
        CallBAck_ChangePreviewUFOType(currentindex, true);

        //UFO ���� �� ����
        CreateSelectWidget();
    }

    private void CreateSelectWidget()
    {
        

        IReadOnlyDictionary<string, UserUFOData> ufomap =  GameManager.Instance.userData.serialUFOList.UFOMap;
        selectUFOWidget.InitializeSelectWidget(this, UFOLoadManager.Instance.LoadedUFODataList, 
            ufomap, GameManager.Instance.userData.CurrentUFO);

        //���������� ���ε�
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

            //��ϵ��� ������ 
            if (!bunlock)
            {
                mat = new Material(selectUFOdata.UFOMaterials[0]);
                mat.color = MTlockedColor;
            }
            //��� ����ִٸ� ������ �÷��� ����
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
            Debug.Log("UserData ����");
            return;
        }

         UserData userdata = GameManager.Instance.userData;
        userdata.SetCurrentUFO(selectindex);
    }



}
