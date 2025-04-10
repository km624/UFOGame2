using Lean.Gui;
using Lean.Transition.Method;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;



public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

  
    [SerializeField]
    private UFOPlayer ufoplayer;

    [SerializeField]
    private ObjectManager objectManager;

    [SerializeField]
    private CameraShake Camerashake;

    [SerializeField]
    private PlayerHudWidget PlayerHud;
 
    [SerializeField]
    private SkillManager AllSkillManager;

    // 총 남은 시간(초), float으로 관리
    [SerializeField]
    private float TotalTime = 30.0f;
    //private bool bIsTimeStop = false;
    Coroutine GameTimeCoroutine;
    Coroutine PlayTimeCoroutine;
    public event Action<int /*TotalSecound*/> FOnTotalTimeChanged;
   
    // 플레이 시간 누적, float으로 관리 (원한다면 int로 변환해서 표시 가능)
    public float TotalPlayTime { get; private set; } = 0.0f;

    [SerializeField]
    private int AddChangeGenerationScore = 1000;
    private int CurrentChangeGenerationScore = 0; 
    private int TotalScore = 0;
   
    private bool bIgnoredbomb = false;
    private bool bIceActive = false;
    
    private bool bIsGameEnd = false; 


    public Vector3 GetPlayerPostion() { return ufoplayer.transform.position; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

    }

    void Start()
    {
        CurrentChangeGenerationScore = AddChangeGenerationScore;

     //보너스 목표 위젯 생성 or 갱신
        objectManager.FOnBounsWidgetCreated += PlayerHud.CallBack_CreateShapeWidget;

        //보너스 목표 위젯 흡수 , 파괴 시 아이콘 생성ㄴ
        objectManager.FOnBonusSwallowed += PlayerHud.CallBack_SpawnUIImageAtWorldPos;

        objectManager.FOnBounusCompleted += CallBack_BonusClear;
        //오브젝트 빨아들였을때 바인딩
        objectManager.FOnObjectSwallowed += CallBack_ObjectSwallow;
        
        //장애물 빨아들였을때
        objectManager.FOnBomnbSwallowed += CallBack_BombSwallow;


        PlayerHud.FOnAllBounusAnimEnded += objectManager.CallBack_RenewalBonusObject;



        //스킬 세팅
        AllSkillManager.SetSkill(ufoplayer);
        //스킬 표시
        foreach (SkillBase skilldata in AllSkillManager.AllSkillPrefabs)
        {

            PlayerHud.CreateSkillWidget(skilldata, AllSkillManager);
        }
        //스킬 바인딩
        AllSkillManager.FOnSkillActivated += PlayerHud.ActiveSkill;


        PlayerHud.SetTimeWidget(TotalTime,this);
        StartPlayTimer();
        StartGameTimer();
        PlayerHud.SetScoreText(TotalScore);

        objectManager.InitObjectManager();


    }

    private void OnDisable()
    {
        objectManager.FOnBounsWidgetCreated -= PlayerHud.CallBack_CreateShapeWidget;
        objectManager.FOnBonusSwallowed -= PlayerHud.CallBack_SpawnUIImageAtWorldPos;
        objectManager.FOnBounusCompleted -= CallBack_BonusClear;
        objectManager.FOnObjectSwallowed -= CallBack_ObjectSwallow;
        objectManager.FOnBomnbSwallowed -= CallBack_BombSwallow;

        AllSkillManager.FOnSkillActivated -= PlayerHud.ActiveSkill;
        PlayerHud.FOnAllBounusAnimEnded -= objectManager.CallBack_RenewalBonusObject;
    }

    private void CallBack_ObjectSwallow(FallingObject fallingobject,int currentgeneration)
    {

        //Debug.Log("흡수함");
        TotalScore += (int)(fallingobject.TimeCnt * 100.0f)*(1+currentgeneration);
        TotalTime += fallingobject.TimeCnt;

        PlayerHud.SetScoreText(TotalScore);

        ufoplayer.AddEXPGauge(fallingobject.EXPCnt);

        if (TotalScore >= CurrentChangeGenerationScore)
        {
            CurrentChangeGenerationScore += AddChangeGenerationScore;
            bool changed = objectManager.ChangeGeneration();
            if (changed)
            {
                PlayerHud.OnForceRenewalBounusWidget();
            }
           
        }
    }
    private void CallBack_BonusClear(Dictionary<ShapeEnum, int> allbouns , int currentgeneration)
    {
        
        foreach(var item in allbouns)
        {
            for(int i = 0; i < item.Value;i++)
            {
                float timecnt = objectManager.SearchShapeData(item.Key, currentgeneration);

                TotalScore += (int)(timecnt * 100.0f) * (1 + currentgeneration) * 2;
                Debug.Log("보너스 점수 추가 : " + TotalScore);
            }
         
        }
      
    }


    private void CallBack_BombSwallow()
    {
        TotalTime -= 10.0f;
        Camerashake.ShakeCamera();
    }

 
    public void StartGameTimer()
    {
        GameTimeCoroutine = StartCoroutine(GameTimerCount());
        
    }

    public void StartPlayTimer()
    {
        PlayTimeCoroutine = StartCoroutine(PlayTimerCount());
    }

    // 게임 진행 타이머: 매 프레임 Time.deltaTime 만큼 감소
    IEnumerator GameTimerCount()
    {
        // 코루틴 시작 시 초기 초 값 저장 (Floor)
        int lastSecond = Mathf.CeilToInt(TotalTime);

        while (TotalTime > 0f)
        {
            yield return null;

           
            // 남은 시간 감소
            TotalTime -= Time.deltaTime;
            if (TotalTime < 0f)
            {
                TotalTime = 0f;
            }

            // 현재 남은 초 (정수) 계산
            int currentSecond = Mathf.CeilToInt(TotalTime);
            // 만약 이전 프레임과 초가 달라졌다면 이벤트 호출
            if (currentSecond != lastSecond)
            {
                
                lastSecond = currentSecond;
                FOnTotalTimeChanged?.Invoke(lastSecond);
            }
        }

        // 게임 타이머 종료 시 처리
        GameEnd();
    }

    IEnumerator PlayTimerCount()
    {
        while (true)
        {
            yield return null;
            TotalPlayTime += Time.deltaTime;
        }
    }


    private void StopGameTimer(bool bisStop)
    {

        if (bisStop)
        {
            StopCoroutine(GameTimeCoroutine);
        }
        else
        {
            StartGameTimer();
        }
    }

    private void StopPlayTimer(bool bisStop)
    {
        if (bisStop)
        {
            StopCoroutine(PlayTimeCoroutine);
        }
        else
        {
           StartPlayTimer();
        }
    }

    public void Skill_SetIgnoreBomb(bool active)
    {
        bIgnoredbomb = active;
    }
    public void Skill_SetIceActive(bool active)
    {
        bIceActive = active;
        StopGameTimer(active); 
    }

 
    public void AllObjectStopActive(bool active)
    {
        if (!active)
        {
            if (bIsGameEnd)
            {
                Debug.Log("이미 게임 끝남");
                return;

            }
        }
        objectManager.AllObjectStopActive(active);
        if(bIceActive)
            objectManager.IceActive(active);
      
    }

    public void GameEnd()
    {
       
        AllObjectStopActive(true);
        objectManager.StopSpawnObjects();
        bIsGameEnd = true;

        ufoplayer.CallBack_StopMovement();
        int converttime = Mathf.FloorToInt(TotalPlayTime);
        PlayerHud.UpdateGameState(converttime, TotalScore);

        GameManager.Instance.SaveUserData();
    }

}
