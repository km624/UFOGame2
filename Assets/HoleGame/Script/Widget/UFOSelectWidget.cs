using DanielLochner.Assets.SimpleScrollSnap;

using UnityEngine;

public class UFOSelectWidget : MonoBehaviour
{

   

    public GameObject UfoWidgetPrefab;
    public RectTransform UFOContent;
    public SimpleScrollSnap Snap;
   
    
    void Start()
    {
        CreateUfoWidget();
        SetStartPanelUFO();
       
    }

   

    void CreateUfoWidget()
    {
        if (UFOLoadManager.Instance != null)
        {
            foreach (UFOData data in UFOLoadManager.Instance.LoadedUFODataList)
            {
                GameObject ufowidgetobject = Instantiate(UfoWidgetPrefab, UFOContent,false);
               UFOWidget ufowidget = ufowidgetobject.GetComponent<UFOWidget>();
                if(ufowidget != null )
                {
                    ufowidget.SetUfoWidget(data);
                }
                Snap.AddInstantaiteBack(ufowidgetobject);
            }
        }
      
    }

    void SetStartPanelUFO()
    {
        if (UFOLoadManager.Instance != null)
        {
            Snap.SetStartingPanel(GameManager.Instance.userData.CurrentUFO);
        }
      
    }
        

    
}
