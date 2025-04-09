using DG.Tweening;
using EasyTransition;
using System.Collections.Generic;
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

   /* private Vector2 startPosition = new Vector2(110, -500); 
   private float spacingX = 250f;*/

    public GameObject AllShapeWidget;
    public Dictionary<ShapeEnum, ShapeWidget> AllFallingObjectsWidget { get; private set; } = new Dictionary<ShapeEnum, ShapeWidget>();

    public List<SkillWidget> AllSkillWidgets { get; private set; } = new List<SkillWidget>();
    public GameObject AllSkillWidgetObject;
    

   /* private Vector2 SklillstartPosition = new Vector2(-445, 100);
    private float SkillspacingX = 330f;*/

    public TimerWidget timerWidget;

    public GameStateWidget gameStateWidget;

    public TransitionSettings transition;
    void Start()
    {

    }
   
    void Update()
    {
        
    }

    public void CreateShapeWidget(ShapeEnum shapetype , int count, Sprite shapeImage)
    {
        GameObject newWidgetObj = Instantiate(FallingObjectWidgetPrefab, AllShapeWidget.transform);
       
        if (newWidgetObj != null)
        {
           
           ShapeWidget shapeWidget = newWidgetObj.GetComponent<ShapeWidget>();

            if(shapeWidget!=null)
            { 
                shapeWidget.SetShapeWidget(count, shapeImage);
                AllFallingObjectsWidget.Add(shapetype, shapeWidget);
            }

        }
    }
    public void UpdateAllShapeCount(ShapeEnum shapetype,int count)
    {
        if (AllFallingObjectsWidget.ContainsKey(shapetype))
        {
            AllFallingObjectsWidget[shapetype].UpdateShapeCount(count);
        }
       
    }
    public void SpawnUIImageAtWorldPos(Vector3 worldPos,Sprite icon, ShapeEnum shape)
    {
        // 1. 월드 좌표 → 스크린 좌표 변환
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        // 2. 스크린 좌표 → 캔버스 로컬 좌표 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out Vector2 canvasPos);

      

        // 3. UI 이미지 프리팹 인스턴스 생성 및 초기 위치 설정
        Image spawnedImage = Instantiate(uiImagePrefab, canvasRect);
        spawnedImage.sprite = icon;
        spawnedImage.rectTransform.anchoredPosition = canvasPos;
       

       // 4. DOTween Sequence 생성
       // Sequence seq = DOTween.Sequence();
       
        
        // 이미지 초기 크기를 작게 설정
        spawnedImage.rectTransform.localScale = Vector3.one * 0.5f;
        
        // 축소 후 정상
       // seq.Append(spawnedImage.rectTransform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack));

        // 확대 후 정상 
        /*seq.Append(spawnedImage.rectTransform.DOScale(Vector3.one * 1.4f, 0.4f).SetEase(Ease.OutBack));
        seq.Append(spawnedImage.rectTransform.DOScale(Vector3.one, 0.4f).SetEase(Ease.InOutQuad));*/

        RectTransform targetUIAnchor = AllFallingObjectsWidget[shape].gameObject.GetComponent<RectTransform>();
        Vector3 endPos = targetUIAnchor.position;

        Sequence seq = DOTween.Sequence()
        .Append(spawnedImage.rectTransform.DOScale(Vector3.one, 1.5f).SetEase(Ease.OutBack))
        .Join(spawnedImage.rectTransform.DOMove(endPos, 0.8f).SetEase(Ease.OutCubic))
        .OnComplete(() => Destroy(spawnedImage));




    }


    public void CreateSkillWidget(SkillBase skillData,SkillManager skillmanager)
    {
        GameObject newskillWidgetObj = Instantiate(SkillWidgetPrefab, AllSkillWidgetObject.transform);

        if (newskillWidgetObj != null)
        {

            //Vector2 newPosition = SklillstartPosition + new Vector2(AllSkillWidgets.Count * SkillspacingX, 0);

           /* RectTransform widgetRect = newskillWidgetObj.GetComponent<RectTransform>();
            if (widgetRect != null)
            {
                widgetRect.anchoredPosition = newPosition;
            }*/
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

    public void SetTimeWidget(int min, int second)
    {
       
        timerWidget.SetTime(min, second);
    }

    public void StartTimer()
    {
        timerWidget.StartTimer();
    }

    public void MinusTimer()
    {
        int newTotalSecond = timerWidget.GetTotalSecond() - 10;

        timerWidget.ChangeTimer(newTotalSecond);
    }

    public int StopTimer()
    {
        timerWidget.StopTime();
        return timerWidget.GetTotalSecond();
    }

  

    public void UpdateGameState(bool gameclear,int star)
    {
        timerWidget.StopTime();
        int TotalTime = timerWidget.GetTotalSecond();

        gameStateWidget.SetGameState(TotalTime, gameclear, star);
       
    }

    public void GoMainScreen()
    {
        
        TransitionManager.Instance().Transition("MainScene", transition, 0.0f);
    }

    
}

