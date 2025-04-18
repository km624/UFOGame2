using System.Collections.Generic;
using UnityEngine;

public class ReinForceWidget : MonoBehaviour
{
    private UFOAllWidget  ufoAllWidget;
    List<StatWidget> StatWidgetList = new List<StatWidget>();

    void InitializeStatWidgetList(UFOAllWidget allwidget , IReadOnlyList<UFOStatData> ufostatlist , IReadOnlyList<UFOStatData> Userufostatlist)
    {
        ufoAllWidget = allwidget;

        if (StatWidgetList.Count != 3) Debug.Log("stat ∫Œ¡∑");
        
        for(int i=0;i<3;i++)
        {

        }

    }


}
