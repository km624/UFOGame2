using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StageWidget : MonoBehaviour
{
    
    [SerializeField]
    private List<Image> enableStars = new List<Image>();
    [SerializeField]
    private List<ShapeWidget> ShapeCntWidgets = new List<ShapeWidget>();
    [SerializeField]
    private TMP_Text StageText;
    [SerializeField]
    private TMP_Text TimeText;

    //private int stageNum = 0;
    //private int clearStar = 0;
    
   
    void Start()
    {
        
    }

   
    void Update()
    {
        
    }

    private void ResetShapeWidget()
    {
        foreach (var shapewidget in ShapeCntWidgets)
        {
            shapewidget.gameObject.SetActive(false); 
        }

        foreach (var star in enableStars)
        {
            star.gameObject.SetActive(false);
        }

        TimeText.text = "";
    }

    public void SetupStageWidget(StageData stagedata, int stagenum, int clearstar, int cleartime)
    {

        ResetShapeWidget();

        //스테이지 표시
        StageText.text = "Stage" + stagenum;

        //별 표시
        //Debug.Log(clearstar);
        for (int i = 0; i < clearstar; i++)
        {
            enableStars[i].gameObject.SetActive(true);
        }

        //클리어 시간 표시
        if(cleartime!=0)
        {
            int min = cleartime / 60;
            int sec = cleartime % 60;
            
            TimeText.text = min + " : " + sec;
        }

        int cnt = 0;
        foreach(var stageobjectdata in stagedata.RequiredShapeCnt)
        {
            int shapecnt = stageobjectdata.Value;
            if (shapecnt == 0) continue;

            ShapeEnum shapeEnum = stageobjectdata.Key;
            Sprite shapeimg = null;
            if(ShapeManager.Instance)
            {
                shapeimg= ShapeManager.Instance.GetShapeSprite(shapeEnum);
            }
            //ShapeCntWidgets[cnt].SetShapeWidget(shapecnt, shapeimg);
            ShapeCntWidgets[cnt].gameObject.SetActive(true);
           // Debug.Log("도형 세팅중");
            cnt++;
        }
            
    }
}

