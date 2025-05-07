
using DG.Tweening;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;


public class UFOAllWidget : MonoBehaviour
{
    [SerializeField]private MainWidget mainWidget;
   

    [SerializeField] private RectTransform UFOAllWidgetTransform;
    [SerializeField] private Button UFOSelectButton;
    [SerializeField] private Button UFOReinforceButton;
    
    [SerializeField] private GameObject PreviewUFO;
    
    private List<GameObject> AddObjectInstanceList =new List<GameObject>();
    private List<GameObject> FullStatObjectInstanceList =new List<GameObject>();

    [SerializeField] private float slideDuration = 0.6f;
   
    private Vector2 shownPosition;
    private Vector2 hiddenPosition;
    private bool isVisible = false;

    

    [SerializeField] private SelectUFOWidget selectUFOWidget;

    [SerializeField] private GameObject ColorReinforcePanel;

    [SerializeField] private SelectPaletteWidget selectPaletteWidget;
    [SerializeField] private ReinForceWidget reinForceWidget;

    [SerializeField] private Color MTLockedColor = new Color32(34, 34, 34, 255);

    private string selecetUFOname = null;

    void Start()
    {
        ColorReinforcePanel.SetActive(false);
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
        isVisible = true;
        
        Vector2 shownPos = new Vector2(0, 0f);

        UFOSelectButton.gameObject.SetActive(true);
        UFOReinforceButton.gameObject.SetActive(true);
       

        UFOAllWidgetTransform.DOAnchorPos(shownPos, slideDuration)
            .SetEase(Ease.OutQuart)
            .OnComplete(() =>
            { 
                //isVisible = true;
            });
    }

    private void HidePanel()
    {

        isVisible = false;

        string currentufo = GameManager.Instance.userData.SelectUFOName;

        //�ǳ� ������ ���� ����
        selectUFOWidget.SelectUFOType(currentufo, true, 0,false);


        UFOSelectButton.gameObject.SetActive(false);
        UFOReinforceButton.gameObject.SetActive(false);

        Vector2 hiddenPos = new Vector2(0.0f, -1100.0f);
        UFOAllWidgetTransform.DOAnchorPos(hiddenPos, slideDuration)
            .SetEase(Ease.InQuart)
            .OnComplete(() =>
            {
                //isVisible = false;
                
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
        if (UFOLoadManager.Instance.ReadLoadedUFODataDic.Count <= 0)
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

       string currentUFOname = GameManager.Instance.userData.SelectUFOName;

        
        //UFO ���� �� ����
        CreateSelectWidget(currentUFOname);
        
        //���������� ���ε�
        selectUFOWidget.FOnUFOPurchased += mainWidget.CallBack_OnPurchased;
        selectPaletteWidget.FOnColorPurchased += mainWidget.CallBack_OnPurchased;
        reinForceWidget.FOnReinforceApplied += mainWidget.CallBack_OnPurchased;
        reinForceWidget.FOnFullStated += CreateFullStatObject;


        CallBAck_ChangePreviewUFOType(currentUFOname, true);

    }

    private void CreateSelectWidget(string currentufoname)
    {
       
        IReadOnlyDictionary<string, UFOData> ufodataList = UFOLoadManager.Instance.ReadLoadedUFODataDic;
         UFOData currentUFOdata = UFOLoadManager.Instance.ReadLoadedUFODataDic[currentufoname];

        IReadOnlyDictionary<string, UserUFOData> ufomap =  GameManager.Instance.userData.serialUFOList.UFOMap;
        selectUFOWidget.InitializeSelectWidget(this, ufodataList,
            ufomap, currentufoname);

        reinForceWidget.ReinforceInitWidget();

    }

    public void CallBAck_ChangePreviewUFOType(string ufoname, bool bunlock)
    {
        foreach (var addobject in AddObjectInstanceList)
        {
            Destroy(addobject);
        }
        AddObjectInstanceList.Clear();
        foreach (var addobject in FullStatObjectInstanceList)
        {
            Destroy(addobject);
        }
        FullStatObjectInstanceList.Clear();
 

        UFOData currentUFOdata = UFOLoadManager.Instance.ReadLoadedUFODataDic[ufoname];

        MeshFilter meshFilter = PreviewUFO.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            meshFilter.mesh = currentUFOdata.UFOMesh;
        }

        MeshRenderer renderer = PreviewUFO.GetComponent<MeshRenderer>();
        
        if (renderer != null)
        {
            Material[] mats = null;
            Texture baseMap = null;

            if (!bunlock) // ��� �� �� ���
            {
                int count = currentUFOdata.UFOColorDataList[0].Materials.Count;
                mats = new Material[count];
                for (int i = 0; i < count; i++)
                {
                    mats[i] = new Material(currentUFOdata.UFOColorDataList[0].Materials[i]); // ����
                    mats[i].color = MTLockedColor;
                    if (mats[i].HasProperty("_BaseColor"))
                    {
                        mats[i].SetColor("_BaseColor", MTLockedColor);
                    }
                    if (mats[i].HasProperty("_Color"))
                    {
                        mats[i].SetColor("_Color", MTLockedColor);
                    }
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

                    baseMap = colorSet.Materials[0].GetTexture("_BaseMap");

                    for (int i = 0; i < count; i++)
                    {
                        mats[i] = new Material(colorSet.Materials[i]); // ���纻
                    }

                    //Ǯ�� Ȯ�� �� �߰�
                    if(userufo.AllStat())
                     CreateFullStatObject(currentUFOdata,baseMap);

                }

                SaveSelectUFO(ufoname);
            }

            // ���͸��� �迭 ��°�� ����
            renderer.materials = mats;
            if(isVisible)
                UFOReinforceButton.gameObject.SetActive(bunlock);

            //�߰� ������Ʈ ����
            CreateAddObject(currentUFOdata, bunlock, baseMap);
            

            // UFO ���� �����ϸ� ���� ����
            if (selecetUFOname != ufoname)
            {
                selecetUFOname = ufoname;
                RenewalWidget(selecetUFOname, bunlock);
            }
         
        }

            Debug.Log("Set : " + ufoname + "  " + currentUFOdata);

    }

    private void CreateAddObject(UFOData currentUFOdata, bool bunlock, Texture currentBaseMap)
    {
       

        // �� �ν��Ͻ� ����
        foreach (var addobject in currentUFOdata.AddUFObject)
        {
            GameObject addobjectInstance = Instantiate(addobject, PreviewUFO.transform);

            if (addobjectInstance != null)
            {
                MeshRenderer renderer = addobjectInstance.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    Material[] originalMaterials = renderer.sharedMaterials;
                    Material[] newMaterials = new Material[originalMaterials.Length];

                    for (int i = 0; i < originalMaterials.Length; i++)
                    {
                        // ���� ���͸��� ����
                        newMaterials[i] = new Material(originalMaterials[i]);

                        if (!bunlock)
                        {
                            if (newMaterials[i].HasProperty("_BaseColor"))
                            {
                                newMaterials[i].SetColor("_BaseColor", MTLockedColor);
                            }
                            if (newMaterials[i].HasProperty("_Color"))
                            {
                                newMaterials[i].SetColor("_Color", MTLockedColor);
                            }
                            newMaterials[i].color = MTLockedColor;
                        }
                        else
                        {
                            // ��� ���� �� BaseMap(Texture) ��ü
                            if (newMaterials[i].HasProperty("_BaseMap"))
                            {
                                newMaterials[i].SetTexture("_BaseMap", currentBaseMap);
                            }
                           
                        }
                    }

                    // �� ���͸��� �迭 ����
                    renderer.materials = newMaterials;
                }

                AddObjectInstanceList.Add(addobjectInstance);
            }
        }
    }

    private void CreateFullStatObject(UFOData currentUFOdata ,Texture BaseMap)
    {

        // �� �ν��Ͻ� ����
        foreach (var addobject in currentUFOdata.AddFullStatObject)
        {
            GameObject addobjectInstance = Instantiate(addobject, PreviewUFO.transform);

            if (addobjectInstance != null)
            {
                MeshRenderer renderer = addobjectInstance.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    Material[] originalMaterials = renderer.sharedMaterials;
                    Material[] newMaterials = new Material[originalMaterials.Length];



                    for (int i = 0; i < originalMaterials.Length; i++)
                    {
                        // ���� ���͸��� ����
                        newMaterials[i] = new Material(originalMaterials[i]);

                        if (newMaterials[i].HasProperty("_BaseMap"))
                        {
                            newMaterials[i].SetTexture("_BaseMap", BaseMap);
                        }
                   
                    }

                    // �� ���͸��� �迭 ����
                    renderer.materials = newMaterials;
                }

                FullStatObjectInstanceList.Add(addobjectInstance);
            }
        }

    }
    public void SaveSelectUFO(string ufoname)
    {
        if (GameManager.Instance.userData == null)
        {
            Debug.Log("UserData ����");
            return;
        }

         UserData userdata = GameManager.Instance.userData;
        userdata.SetCurrentUFO(ufoname);

        
        GameManager.Instance.SaveUserData();
    }
    
    public void CallBack_ChangeColor(int colorindex,bool bunclock)
    {
        foreach (var addobject in AddObjectInstanceList)
        {
            Destroy(addobject);
        }
        AddObjectInstanceList.Clear();
        foreach (var addobject in FullStatObjectInstanceList)
        {
            Destroy(addobject);
        }
        FullStatObjectInstanceList.Clear();

        string currentUFOname = GameManager.Instance.userData.SelectUFOName;
        UFOData currentUFOdata = UFOLoadManager.Instance.ReadLoadedUFODataDic[currentUFOname];


        MeshRenderer renderer = PreviewUFO.GetComponent<MeshRenderer>();

        if (renderer != null)
        {
            Material[] mats = null;


            var colorSet = currentUFOdata.UFOColorDataList[colorindex];
            int count = colorSet.Materials.Count;

            Texture baseMap = colorSet.Materials[0].GetTexture("_BaseMap");

            mats = new Material[count];
            for (int i = 0; i < count; i++)
            {
                mats[i] = new Material(colorSet.Materials[i]); // ���纻
            }

            // ���͸��� �迭 ��°�� ����
            renderer.materials = mats;
            
            if(bunclock)
                SaveSelectColor(colorindex, currentUFOdata);

            UserUFOData userufodata = GameManager.Instance.userData.serialUFOList.Get(currentUFOdata.UFOName);
            if (userufodata.AllStat())
                CreateFullStatObject(currentUFOdata, baseMap);


            CreateAddObject(currentUFOdata, true, baseMap);

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

    private void RenewalWidget(string UFOname, bool bUnlock)
    {

        //�⺻ ������ ����
        IReadOnlyDictionary<string, UFOData> ufoDataList = UFOLoadManager.Instance.ReadLoadedUFODataDic;
        IReadOnlyDictionary<string, UserUFOData> ufomap = GameManager.Instance.userData.serialUFOList.UFOMap;
        UFOData currentUFOdata = ufoDataList[UFOname];

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

        selectPaletteWidget.InitializeSelectWidget(this, UFOname, currentUFOColorList, OwnUserColorList, selectcolorindex);


        //UFO ���� ����
        IReadOnlyList<UFOStatData> statdatalist = currentUFOdata.StatList;
       
        if (bUnlock && ufomap.ContainsKey(currentUFOdata.UFOName))
        {
            statdatalist = ufomap[currentUFOdata.UFOName].StatReinforceList;
            
        }

        reinForceWidget.ClearStat();

        reinForceWidget.InitializeStatWidgetList(this, currentUFOdata, statdatalist, currentUFOdata.Skilltype);

        //OnEnableColorReinForceWidget();

        Debug.Log("���� ���� : " + Time.time);

    }

    public void OnEnableColorReinForceWidget()
    {

        ColorReinforcePanel.SetActive(true);
        selectUFOWidget.gameObject.SetActive(false);
    }

    public void OnEnableUFOSelectWdiget()
    {
        selectUFOWidget.gameObject.SetActive(true);
        ColorReinforcePanel.SetActive(false);

    }

    public void CallBack_UFORewardCompleted(string ufoname)
    {
        Debug.Log(ufoname + "UFO");
        selectUFOWidget.RewardCompleteUFO(ufoname);
    }
    public void CallBack_UFOColorCompleted(string ufoname, int colorindex)
    {
        selectPaletteWidget.RewardCompleteColor(ufoname, colorindex);   
    }


}
