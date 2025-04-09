using DanielLochner.Assets.SimpleScrollSnap;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectWidget : MonoBehaviour
{
    public List<StageWidget> stagewidgets=new List<StageWidget>();
    
    public MainWidget mainWidget;
    public SimpleScrollSnap snap;
    void Start()
    {
        
           if (UFOLoadManager.Instance != null)
           {
             snap.SetStartingPanel(GameManager.Instance.userData.currentClearIndex+1);
           }

        
    }

   
   

    
}
