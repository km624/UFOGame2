using DG.Tweening;
using EasyTransition;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHudWidget : MonoBehaviour
{
    [Header("ĵ���� ����")]
    public RectTransform canvasRect;
    [Header("���� ���� ������")]
    public GameObject FallingObjectWidgetPrefab;
    [Header("������ ���� ������")]
    public Image uiImagePrefab;
    [Header("��ų ���� ������")]
    public GameObject SkillWidgetPrefab;
 

    public GameObject AllShapeWidget;
    public Dictionary<ShapeEnum, ShapeWidget> AllBonusWidget { get; private set; } = new Dictionary<ShapeEnum, ShapeWidget>();

    private int EndBounsWidgetCnt = 0;
    private bool bIsFroceRenewal = false;
    public event Action<bool /*bounusClear*/> FOnAllBounusAnimEnded;
    

    public List<SkillWidget> AllSkillWidgets { get; private set; } = new List<SkillWidget>();
    public GameObject AllSkillWidgetObject;


    public TimerWidget timerWidget;

    [SerializeField]
    private TMP_Text CurrentScoreText;


    public GameStateWidget gameStateWidget;

    public TransitionSettings transition;
    
   /* [SerializeField]
    private ThermometerWidget thermometerWidget;*/

    [SerializeField]
    private TMP_Text StarCntText;
   
   [SerializeField]
    private DetectWidget detectWidget;




    public void CallBack_CreateShapeWidget(ShapeEnum shapetype , int count)
    {
        GameObject newWidgetObj = Instantiate(FallingObjectWidgetPrefab, AllShapeWidget.transform);
       
        if (newWidgetObj != null)
        {
           
           ShapeWidget shapeWidget = newWidgetObj.GetComponent<ShapeWidget>();

            if(shapeWidget!=null)
            {
                Sprite ShapeImage = ShapeManager.Instance.GetShapeSprite(shapetype);
                shapeWidget.SetShapeWidget(count, ShapeImage,this);
                AllBonusWidget.Add(shapetype, shapeWidget);
            }
       
        }
    }
    public void UpdateAllShapeCount(ShapeEnum shapetype,int count)
    {
        if (AllBonusWidget.ContainsKey(shapetype))
        {
            AllBonusWidget[shapetype].UpdateShapeCount(count);
        }
       
    }

    
    public void SwalloweShapeCount(ShapeEnum shapetype)
    {
        if (AllBonusWidget.ContainsKey(shapetype))
        {

            AllBonusWidget[shapetype].MinusShapeCount();
        }

    }
    public void CallBack_SpawnUIImageAtWorldPos(Vector3 worldPos, ShapeEnum shape)
    {
        // 1. ���� ��ǥ �� ��ũ�� ��ǥ ��ȯ
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        // 2. ��ũ�� ��ǥ �� ĵ���� ���� ��ǥ ��ȯ
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out Vector2 canvasPos);

        Sprite icon = ShapeManager.Instance.GetShapeSprite(shape);

        // 3. UI �̹��� ������ �ν��Ͻ� ���� �� �ʱ� ��ġ ����
        Image spawnedImage = Instantiate(uiImagePrefab, canvasRect);
        spawnedImage.sprite = icon;
        spawnedImage.rectTransform.anchoredPosition = canvasPos;


       
        // �̹��� �ʱ� ũ�⸦ �۰� ����
        spawnedImage.rectTransform.localScale = Vector3.one * 0.5f;
        
      
        RectTransform targetUIAnchor = AllBonusWidget[shape].gameObject.GetComponent<RectTransform>();
        Vector3 endPos = targetUIAnchor.position;

        Sequence seq = DOTween.Sequence()
        .Append(spawnedImage.rectTransform.DOScale(Vector3.one, 1.5f).SetEase(Ease.OutBack))
        .Join(spawnedImage.rectTransform.DOMove(endPos, 0.8f).SetEase(Ease.OutCubic))
        .OnComplete(() => OnIconArrived(spawnedImage, shape));

    }

    public void OnIconArrived(Image icon, ShapeEnum shapetype)
    {
        Destroy(icon);
        SwalloweShapeCount(shapetype);
    }

    public void CallBack_WidgetAnimationEnd()
    {
        EndBounsWidgetCnt++;
        if (AllBonusWidget.Count <= EndBounsWidgetCnt)
        {

            foreach (var widget in AllBonusWidget)
            {
                Destroy(widget.Value.gameObject);

            }

            AllBonusWidget.Clear();
            EndBounsWidgetCnt = 0;


            //Debug.Log("���ʽ� ���� �ʱ�ȭ");
            FOnAllBounusAnimEnded?.Invoke(bIsFroceRenewal);
            bIsFroceRenewal = false;
        }

    }


    public void OnForceRenewalBounusWidget()
    {
        bIsFroceRenewal = true;

        Debug.Log("���� ���� �ʱ�ȭ");
        foreach (var widget in AllBonusWidget.ToList())
        {
            if (!widget.Value.CheckDisable())
            {
                widget.Value.OnShpaeWidgetAnimation();

            }
               
        }

    }



    public void CreateSkillWidget(SkillBase skillData,SkillManager skillmanager)
    {
        GameObject newskillWidgetObj = Instantiate(SkillWidgetPrefab, AllSkillWidgetObject.transform);

        if (newskillWidgetObj != null)
        {

          
            SkillWidget skillwidget = newskillWidgetObj.GetComponent<SkillWidget>();

            if (skillwidget != null)
            {
                skillwidget.SetSkillWidget(skillmanager,skillData.SkillIcon, skillData.SkillCount, skillData.SkillDuration, AllSkillWidgets.Count);
                AllSkillWidgets.Add(skillwidget);
            }

        }
    }

    public void ActiveSkill(int num ,int skillcount)
    {

        AllSkillWidgets[num].ActivateSkillWidget(skillcount);
    }

    public void SetTimeWidget(float totaltime,GameState gamestate)
    {
       
        timerWidget.SetTime(totaltime);
        //���ε���
        gamestate.FOnTotalTimeChanged += timerWidget.CallBack_UpdateTimeText;
    }
    public void SetScoreText(int score)
    {
        CurrentScoreText.text = score.ToString();
    }

   /* public void CallBack_SetThermometerWidget(GenerationObjects generationdata)
    {
        
        if(thermometerWidget != null)
        {
            thermometerWidget.SetThermometerData(generationdata);
        }
    }
    public void CallBack_AddEXPThermometerWidget(float addgauge)
    {
       
        if (thermometerWidget != null)
        {
            //Debug.Log(addgauge);
            thermometerWidget.AddEXPGauge(addgauge);
        }
    }*/

    public void ChangeStarCntText(int cnt)
    {
        StarCntText.text = cnt.ToString();
    }
    
    public void SetDetectStandardTarget(IDetctable standard)
    {
        detectWidget.AddstandardTarget(standard);
    }

    public void CallBack_AddDetectTarget(IDetctable star)
    {
        detectWidget.AddTarget(star);   
    }

    public void CallBack_RemoveDetectTarget(IDetctable star) 
    {
        detectWidget.RemoveTarget(star);
    }





    public void UpdateGameState(int totaltime, int totalscore)
    {

        gameStateWidget.SetGameState(totaltime, totalscore);
       
    }

    public void GoMainScreen()
    {
        
        TransitionManager.Instance().Transition("MainScene", transition, 0.0f);
    }

   
}

