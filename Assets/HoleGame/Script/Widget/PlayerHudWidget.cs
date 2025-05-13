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
    [Header("캔버스 설정")]
    public RectTransform canvasRect;
    [Header("도형 위젯 프리팹")]
    public GameObject FallingObjectWidgetPrefab;
    [Header("아이콘 생성 프리팹")]
    public Image uiImagePrefab;
    [Header("스킬 위젯 프리팹")]
    public GameObject SkillWidgetPrefab;
 

    public GameObject AllShapeWidget;
    public Dictionary<ShapeEnum, ShapeWidget> AllBonusWidget { get; private set; } = new Dictionary<ShapeEnum, ShapeWidget>();

    private int EndBounsWidgetCnt = 0;
    private bool bIsFroceRenewal = false;
    public event Action<bool /*bounusClear*/> FOnAllBounusAnimEnded;
    

    public List<SkillWidget> AllSkillWidgets { get; private set; } = new List<SkillWidget>();
    public GameObject AllSkillWidgetObject;

    [SerializeField]
    private RectTransform UpPanel;
    [SerializeField]
    private RectTransform DownPanel;
    [SerializeField]
    private RectTransform PauseButton;
    [SerializeField]
    private Joystick joystick;

    public TimerWidget timerWidget;

    [SerializeField]
    private TMP_Text CurrentScoreText;
   
    [SerializeField]
    private TMP_Text currentGenerationText;


    public GameStateWidget gameStateWidget;

    public TransitionSettings transition;
    
    [SerializeField]
    private FuelTankWidget fuelTankWidget;

    [SerializeField]
    private TMP_Text StarCntText;
   
   [SerializeField]
    private DetectWidget detectWidget;
    
    public TMP_Text fpsText;
    float deltaTime = 0.0f;


    public void InitDirectionPostion()
    {
        float upheight = UpPanel.rect.height;

        UpPanel.anchoredPosition += Vector2.up * upheight;

        float Downheight = DownPanel.rect.height;
       
        DownPanel.anchoredPosition -= Vector2.up * Downheight;

        //예외
        PauseButton.anchoredPosition += Vector2.up * upheight;

        SetJoystick(false);


    }

    public void OnUIDirection()
    {
        Vector2 UPcurrentPos = UpPanel.anchoredPosition;
        float UPheight = UpPanel.rect.height;
        Vector2 UPtargetPos = UPcurrentPos - new Vector2(0, UPheight);
        UpPanel.DOAnchorPos(UPtargetPos, 1.0f).SetEase(Ease.OutQuad);


        Vector2 DowncurrentPos = DownPanel.anchoredPosition;
        float Downheight = DownPanel.rect.height;
        Vector2 DowntargetPos = DowncurrentPos + new Vector2(0, Downheight);
        DownPanel.DOAnchorPos(DowntargetPos, 1.0f).SetEase(Ease.OutQuad);


        Vector2 pausebuttonPos = PauseButton.anchoredPosition;
        Vector2 pausetargetPos = pausebuttonPos - new Vector2(0, UPheight);
        PauseButton.DOAnchorPos(pausetargetPos, 1.0f).SetEase(Ease.OutQuad);

        SetJoystick(true);
    }

    public void SetJoystick(bool active)
    {
        joystick.gameObject.SetActive(active);
    }



    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        fpsText.text = $"FPS: {Mathf.Ceil(fps)}";
    }

    public void CallBack_CreateShapeWidget(ShapeEnum shapetype , int count)
    {
        //Debug.Log("Create : " + shapetype); 
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
        // 1. 월드 좌표 → 스크린 좌표 변환
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        // 2. 스크린 좌표 → 캔버스 로컬 좌표 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out Vector2 canvasPos);

        Sprite icon = ShapeManager.Instance.GetShapeSprite(shape);

        // 3. UI 이미지 프리팹 인스턴스 생성 및 초기 위치 설정
        Image spawnedImage = Instantiate(uiImagePrefab, canvasRect);
        spawnedImage.sprite = icon;
        spawnedImage.rectTransform.anchoredPosition = canvasPos;


       
        // 이미지 초기 크기를 작게 설정
        spawnedImage.rectTransform.localScale = Vector3.one * 4.0f;
        
      
        //RectTransform targetUIAnchor = AllBonusWidget[shape].gameObject.GetComponent<RectTransform>();
        RectTransform targetUIAnchor = AllBonusWidget[shape].ShapeImage.GetComponent<RectTransform>();
        Vector3 endPos = targetUIAnchor.position;

        Sequence seq = DOTween.Sequence()
        .Append(spawnedImage.rectTransform.DOScale(Vector3.one, 1.0f).SetEase(Ease.OutBack))
        .Join(spawnedImage.rectTransform.DOMove(endPos, 1.0f).SetEase(Ease.OutCubic))
        .OnComplete(() => OnIconArrived(spawnedImage, shape));

    }

    public void OnIconArrived(Image icon, ShapeEnum shapetype)
    {
        Destroy(icon.gameObject);
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


            //Debug.Log("보너스 위젯 초기화");
            FOnAllBounusAnimEnded?.Invoke(bIsFroceRenewal);
            bIsFroceRenewal = false;
        }

    }


    public void OnForceRenewalBounusWidget()
    {
        bIsFroceRenewal = true;

        Debug.Log("강제 위젯 초기화");
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
                Sprite skillicon = null;
                if (SkillIconManager.Instance != null)
                    skillicon = SkillIconManager.Instance.GetSkillIconSprite(skillData.skilltype);

                skillwidget.SetSkillWidget(skillmanager,skillicon, skillData.SkillCount, skillData.SkillDuration, AllSkillWidgets.Count);
                AllSkillWidgets.Add(skillwidget);
            }

        }
    }

    public void ActiveSkill(int num ,int skillcount)
    {

        AllSkillWidgets[num].ActivateSkillWidget(skillcount);
    }

    public void OnPauseSkillWidget(bool active)
    {
        foreach(var skiilwidget in AllSkillWidgets)
        {
            if(active)
            {
                skiilwidget.PauseSkillCooltime();

            }
            else
            {
                skiilwidget.ResumeSkillCooltime();
            }
        }
    }


    public void SetTimeWidget(float totaltime,GameState gamestate)
    {
       
        timerWidget.SetTime(totaltime);
        //바인딩ㄴ
        gamestate.FOnTotalTimeChanged += timerWidget.CallBack_UpdateTimeText;
    }
    public void SetScoreText(int score)
    {
        CurrentScoreText.text = score.ToString();
    }

    public void CallBack_SetFuelTankWidget()
    {
        if(fuelTankWidget != null)
        {
            fuelTankWidget.SetThermometerData();
        }
    }
    public void CallBack_AddEXPFuelTankWidget(float addgauge)
    {
       
        if (fuelTankWidget != null)
        {
            //Debug.Log(addgauge);
            fuelTankWidget.AddEXPGauge(addgauge);
        }
    }
    public void CallBack_InitFuelTankWidget()
    {
        if (fuelTankWidget != null)
        {
            fuelTankWidget.InitFuelTank();
        }
    }

    public void ChangeStarCntText(int cnt)
    {
        StarCntText.text = cnt.ToString();
    }

    public void ChangeGenerationNameText(string generationname)
    {
        currentGenerationText.text = generationname;
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





    public void UpdateGameState(int totaltime, int totalscore,int starcnt)
    {

        gameStateWidget.SetGameState(totaltime, totalscore, starcnt);
       
    }

   

    public void GoMainScreen()
    {
        DOTween.KillAll();
        //GameManager.Instance.soundManager.PlayBgm(SoundEnum.BGM, 0.2f);
        TransitionManager.Instance().Transition("MainScene", transition, 0.0f);
    }

    public void RestartGame()
    {
        DOTween.KillAll();
        TransitionManager.Instance().Transition("GameScene", transition, 0.0f);
    }

   
}

