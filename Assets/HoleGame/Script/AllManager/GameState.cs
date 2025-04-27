
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle.Manifest;
using UnityEngine;




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
 
    public float TotalPlayTime { get; private set; } = 0.0f;

 
    private int TotalScore = 0;
   
    private bool bIgnoredbomb = false;
    //private bool bIceActive = false;
    
    private bool bIsGameEnd = false; 


    public Vector3 GetPlayerPostion() { return ufoplayer.transform.position; }

    public int GetPlayerCurrentLevel() { return ufoplayer.CurrentLevel; }

    public int StarCnt { get; private set; } = 0;


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
        
        //보너스 목표 위젯 생성 
        objectManager.FOnBounsWidgetCreated += PlayerHud.CallBack_CreateShapeWidget;

        //보너스 목표 위젯 흡수 , 파괴 시 아이콘 생성
        objectManager.FOnBonusSwallowed += PlayerHud.CallBack_SpawnUIImageAtWorldPos;

        //오브젝트 빨아들였을때 바인딩
        objectManager.FOnObjectSwallowed += CallBack_ObjectSwallow;
        
        //장애물 빨아들였을때
        objectManager.FOnBomnbSwallowed += CallBack_BombSwallow;

        //보너스 목표 클리어 했을때 바인딩
        objectManager.FOnBounusCompleted += CallBack_BonusClear;

        //보너스 목표 시대 변경되서 강제 갱신 바인딩
        objectManager.FOnGenerationChanged += CallBack_ChangeGeneration;

        //보너스 위젯 클리어 애니메이션 끝났을때
        PlayerHud.FOnAllBounusAnimEnded += objectManager.CallBack_RenewalBonusObject;

        //재화 스폰 이벤트 바인딩
        objectManager.FOnStartSpawned += CallBack_StarSpawned;

        //연료통 위젯 바인딩
        // objectManager.FOnGenerationDataSeted += PlayerHud.CallBack_SetThermometerWidget;
        PlayerHud.CallBack_SetFuelTankWidget();
        ufoplayer.FOnExpAdded += PlayerHud.CallBack_AddEXPFuelTankWidget;

        //UFO에 시간 바인딩
        FOnTotalTimeChanged += ufoplayer.CallBack_SetRemainTime;
        //스킬 세팅

        //데이터 불러와서 세팅
        UserUFOData userufodata = null;
        UFOData selectUFOdata = null;
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.userData != null)
            {

                int selectUFOIndex = GameManager.Instance.userData.CurrentUFO;
               selectUFOdata = UFOLoadManager.Instance.LoadedUFODataList[selectUFOIndex];
                if (selectUFOdata != null)
                {
                   userufodata = GameManager.Instance.userData.serialUFOList.Get(selectUFOdata.UFOName);
                }
            }
        }

            
        AllSkillManager.SetSkill(ufoplayer, userufodata, selectUFOdata);
        
        //스킬 표시
        PlayerHud.CreateSkillWidget(AllSkillManager.CurrentSkill, AllSkillManager); 

        //스킬 바인딩
        AllSkillManager.FOnSkillActivated += PlayerHud.ActiveSkill;

        //스타 카운트 초기화 0 으로
        PlayerHud.ChangeStarCntText(StarCnt);

        //감지 위젯 플레이어 세팅
        PlayerHud.SetDetectStandardTarget(ufoplayer);

        //시간 세팅
        PlayerHud.SetTimeWidget(TotalTime,this);
        StartPlayTimer();
        StartGameTimer();
        PlayerHud.SetScoreText(TotalScore);

        objectManager.InitObjectManager(this);

        //현재 시대 이름 세팅
        PlayerHud.ChangeGenerationNameText(objectManager.getCurrentGenerationName());

        //플레이어 초기화
        ufoplayer.InitUFO(TotalTime, userufodata, selectUFOdata);

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
        ufoplayer.FOnExpAdded -= PlayerHud.CallBack_AddEXPFuelTankWidget;

        //ufoplayer.FOnExpGagueAdded -= PlayerHud.CallBack_AddEXPThermometerWidget;
    }

    private void CallBack_ObjectSwallow(FallingObject fallingobject)
    {

 
        PlayerHud.SetScoreText(TotalScore);
        
        ufoplayer.AddEXPGauge(fallingobject.Score, fallingobject.ObjectMass);
        //Debug.Log("흡수함");
        TotalScore += fallingobject.Score;

        float calculatetime = CalculateTimeGain(ufoplayer.CurrentLevel, fallingobject.ObjectMass);
        BossObject bossobject = fallingobject as BossObject;
        if (bossobject != null)
        {
            calculatetime += 10.0f;
        }

        TotalTime += calculatetime;

       // Debug.Log(calculatetime);

        PlayerHud.SetScoreText(TotalScore);
    }

    private float CalculateTimeGain(int level, float absorbedMass)
    {

        float levelDiff = level - absorbedMass;

        // 지수 감소: 2차이
        float expMultiplier = Mathf.Pow(0.3f, levelDiff);

        // 2n-n = 증가 효과도 포함됨
        float exp = 1.0f * expMultiplier;

        return exp;
    }

    private void CallBack_BonusClear(Dictionary<float, int> allbouns)
    {
        //Debug.Log("보너스 점수 추가 ");
        foreach (var item in allbouns)
        {
           
            int score = objectManager.GetBounusScoreData(item.Key);
            score *= item.Value;
            TotalScore += score;
            //Debug.Log("점수 추가 : " + score);

        }
    }
    private void CallBack_BombSwallow(float minustime)
    {
       
        TotalTime -= minustime;
        GameManager.Instance.soundManager.PlaySfx(SoundEnum.Bomb,0.5f);
        Camerashake.ShakeCamera();
    }

    public void CallBack_BossSpawned(BossObject boss)
    {
        if(ufoplayer!=null)
        {
            ufoplayer.SetCurrentBoss(boss);
        }
    }

    public void CallBacK_BossSwallow(BossObject bossObject)
    {
        CallBack_ObjectSwallow(bossObject);
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


   
    public void CallBack_ChangeGeneration(int currentgeneration)
    {
        //세대 변경 됬을때 
        string generationname = objectManager.getCurrentGenerationName();
        PlayerHud.ChangeGenerationNameText(generationname);
        PlayerHud.OnForceRenewalBounusWidget();
        
        PlayerHud.CallBack_InitFuelTankWidget();
       
        ufoplayer.MaxLevelLimitUp();
    }

    public void CallBack_StarSpawned(IDetctable targetstar)
    {
        PlayerHud.CallBack_AddDetectTarget(targetstar);
    }

    public void CallBack_StartSwallowed(StarObject star)
    {
        StarCnt++;

        //임시 소리 
        OnUfoSwallowSound();

        PlayerHud.CallBack_RemoveDetectTarget(star);
      
        PlayerHud.ChangeStarCntText(StarCnt);
        Destroy(star.gameObject);
    }

    public void OnUfoSwallowSound()
    {
        //임시 소리 
        ufoplayer.SwallowSound();
    }

    public void Skill_SetIgnoreBomb(bool active)
    {
        bIgnoredbomb = active;
    }
    public void Skill_SetIceActive(bool active)
    {
        //bIceActive = active;

        objectManager.PauseSpawnObjects(active);
        objectManager.OnBombSpawn(active);

        StopGameTimer(active);
        
        
       objectManager.IceActive(active);
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
       
       
      
    }

    public void GamePause(bool active)
    {
        //OnSoundPause(active);
        ufoplayer.CallBack_StopMovement(active);
        GameManager.Instance.vibrationManager.OnPauseVibration(active);
        AllSkillManager.PauseSkillActive(active);
        objectManager.PauseSpawnObjects(active);
        AllObjectStopActive(active);
        StopGameTimer( active );
        StopPlayTimer( active );
        objectManager.OnBombSpawn(!active);
        PlayerHud.OnPauseSkillWidget(active);
       
    }
    
    public void OnSoundPause(bool active)
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.soundManager.OnSoundPauseActive(active);
        }
    }

    public void GameEnd()
    {
        /*  GameManager.Instance.vibrationManager.OnPauseVibration(true);
          AllSkillManager.PauseSkillActive(true);
          AllObjectStopActive(true);
          objectManager.StopSpawnObjects();
          objectManager.OnBombSpawn(false);*/

        //ufoplayer.CallBack_StopMovement(true);

        GamePause(true);
        GameManager.Instance.soundManager.PlayBgm(SoundEnum.BGM_GameEnd,0.5f);
        bIsGameEnd = true;
        int converttime = Mathf.RoundToInt(TotalPlayTime);
        PlayerHud.UpdateGameState(converttime, TotalScore, StarCnt);

        SaveUserData();
    }

    private void SaveUserData()
    {
        GameManager.Instance.userData.AddStarCnt(StarCnt);
        GameManager.Instance.userData.UpdateBestScore(TotalScore);

       
      /*  for(int i=0;i<4;i++)
        {
            int cnt = AllSkillManager.ReadAllSkills[i].SkillCount;
            GameManager.Instance.userData.UpdateSkillCnt(i, cnt);
        }
       */
        
        GameManager.Instance.SaveUserData();
    }

}
