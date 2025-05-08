
using System;
using System.Collections;
using System.Collections.Generic;
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

    // �� ���� �ð�(��), float���� ����
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
        
        //���ʽ� ��ǥ ���� ���� 
        objectManager.FOnBounsWidgetCreated += PlayerHud.CallBack_CreateShapeWidget;

        //���ʽ� ��ǥ ���� ��� , �ı� �� ������ ����
        objectManager.FOnBonusSwallowed += PlayerHud.CallBack_SpawnUIImageAtWorldPos;

        //������Ʈ ���Ƶ鿴���� ���ε�
        objectManager.FOnObjectSwallowed += CallBack_ObjectSwallow;
        
        //��ֹ� ���Ƶ鿴����
        objectManager.FOnBomnbSwallowed += CallBack_BombSwallow;

        //���ʽ� ��ǥ Ŭ���� ������ ���ε�
        objectManager.FOnBounusCompleted += CallBack_BonusClear;

        //���ʽ� ��ǥ �ô� ����Ǽ� ���� ���� ���ε�
        objectManager.FOnGenerationChanged += CallBack_ChangeGeneration;

        //���ʽ� ���� Ŭ���� �ִϸ��̼� ��������
        PlayerHud.FOnAllBounusAnimEnded += objectManager.CallBack_RenewalBonusObject;

        //��ȭ ���� �̺�Ʈ ���ε�
        objectManager.FOnStartSpawned += CallBack_StarSpawned;

        //������ ���� ���ε�
        // objectManager.FOnGenerationDataSeted += PlayerHud.CallBack_SetThermometerWidget;
        ufoplayer.FOnExpAdded += PlayerHud.CallBack_AddEXPFuelTankWidget;

        //UFO�� �ð� ���ε�
        FOnTotalTimeChanged += ufoplayer.CallBack_SetRemainTime;

        //���� ���� ���ε�
        ufoplayer.FOnDirectionEnd += PlayerHud.OnUIDirection;
        ufoplayer.FOnDirectionEnd += DirectionEnd;

        //���� ���� ���ε�
        ufoplayer.FOnWarpDirectionEnd += WarpDirectionEnd;
        ufoplayer.FonGenerationMoved += objectManager.ChangeGeneration;
        
        //������ �ҷ��ͼ� ����
        UserUFOData userufodata = null;
        UFOData selectUFOdata = null;
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.userData != null)
            {
 
              
                string selectUFOname = GameManager.Instance.userData.SelectUFOName;
                selectUFOdata = UFOLoadManager.Instance.ReadLoadedUFODataDic[selectUFOname];
                if (selectUFOdata != null)
                {
                   userufodata = GameManager.Instance.userData.serialUFOList.Get(selectUFOdata.UFOName);
                }
            }
            
        }

        //������ �ʱⰪ ����
        PlayerHud.CallBack_SetFuelTankWidget();

        //��ų ����
        AllSkillManager.SetSkill(ufoplayer, userufodata, selectUFOdata);
        
        //��ų ǥ��
        PlayerHud.CreateSkillWidget(AllSkillManager.CurrentSkill, AllSkillManager); 

        //��ų ���ε�
        AllSkillManager.FOnSkillActivated += PlayerHud.ActiveSkill;

        //��Ÿ ī��Ʈ �ʱ�ȭ 0 ����
        PlayerHud.ChangeStarCntText(StarCnt);

        //���� ���� �÷��̾� ����
        PlayerHud.SetDetectStandardTarget(ufoplayer);

        //�ð� ����
        PlayerHud.SetTimeWidget(TotalTime,this);
        PlayerHud.SetScoreText(TotalScore);

        objectManager.InitObjectManager(this);

        //���� �ô� �̸� ����
        PlayerHud.ChangeGenerationNameText(objectManager.getCurrentGenerationName());

        //�÷��̾� �ʱ�ȭ
        bool bdirecting = false;
        if (GameManager.Instance.userData != null)
            bdirecting = GameManager.Instance.userData.userSettingData.bIsDirection;
        ufoplayer.InitUFO(TotalTime, userufodata, selectUFOdata, bdirecting);
        
        if (bdirecting)
            PlayerHud.InitDirectionPostion();

        if(!bdirecting)
        {
            StartPlayTimer();
            StartGameTimer();
        }

        if(bdirecting)
        {
            objectManager.PauseSpawnObjects(true);
            objectManager.OnBombSpawn(false);
        }

        if(GameManager.Instance.userData == null)
            TestLoadDAta();
    }

    private async void TestLoadDAta()
    {
        await GameManager.Instance.InitData();
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

        ufoplayer.FOnDirectionEnd -= PlayerHud.OnUIDirection;
        ufoplayer.FOnDirectionEnd -= DirectionEnd;

        ufoplayer.FOnWarpDirectionEnd -= WarpDirectionEnd;
        ufoplayer.FonGenerationMoved -= objectManager.ChangeGeneration;


    }

    private void DirectionEnd()
    {

         StartPlayTimer();
         StartGameTimer();
 
         objectManager.PauseSpawnObjects(false);
         objectManager.OnBombSpawn(true);
       
    }

    public void WarpDirectionStart()
    {
        GamePause(true);
        PlayerHud.SetJoystick(false);
        ufoplayer.StartWarpDirecting();

    }
    private void WarpDirectionEnd()
    {
        GamePause(false);
        PlayerHud.SetJoystick(true);
    }

    private void CallBack_ObjectSwallow(FallingObject fallingobject)
    {
        Debug.Log("��� : "  + fallingobject.gameObject.name);
        PlayerHud.SetScoreText(TotalScore);
        
        ufoplayer.AddEXPGauge(fallingobject.Score, fallingobject.ObjectMass);

        //Debug.Log("�����");
        TotalScore += fallingobject.Score;

        float calculatetime = CalculateTimeGain(ufoplayer.CurrentLevel, fallingobject.ObjectMass);
        BossObject bossobject = fallingobject as BossObject;
        if (bossobject != null)
        {
            calculatetime += 10.0f;

            ufoplayer.ResetCameraDistance();
        }
        TotalTime += calculatetime;

 
        PlayerHud.SetScoreText(TotalScore);

        if(GameManager.Instance.userData!=null)
        {
            //( ���� )Ÿ�Ժ� ī��Ʈ 
            string shapeId = $"Swallow_{fallingobject.GetShapeEnum()}_Cnt";
            AchievementManager.Instance.ReportProgress(AchieveEnum.Swallow, shapeId, 1);

            //( ���� ) ���� ��� 
            string AllId = $"Swallow_All_Cnt";
            AchievementManager.Instance.ReportProgress(AchieveEnum.Swallow, AllId, 1);

        }
       
    }

    private float CalculateTimeGain(int level, float absorbedMass)
    {

        float levelDiff = level - absorbedMass;

        // ���� ����: 2����
        float expMultiplier = Mathf.Pow(0.5f, levelDiff);

        // 2n-n = ���� ȿ���� ���Ե�
        float exp = 1.0f * expMultiplier;

        float clampexp = Mathf.Clamp(exp, 0.1f, 2.0f);

        return exp;
    }

    private void CallBack_BonusClear(Dictionary<float, int> allbouns)
    {
        //Debug.Log("���ʽ� ���� �߰� ");
        foreach (var item in allbouns)
        {
           
            int score = objectManager.GetBounusScoreData(item.Key);
            score *= item.Value;
            TotalScore += score;
            //Debug.Log("���� �߰� : " + score);
        }

        if (GameManager.Instance.userData != null)
        {
            //( ���� )���ʽ� Ŭ���� ī��Ʈ 
            string AchieveBonusId = $"Behavior_Bouns_Cnt";
            AchievementManager.Instance.ReportProgress(AchieveEnum.Behavior, AchieveBonusId, 1);


        }

    }
    private void CallBack_BombSwallow(float minustime)
    {
       
        TotalTime -= minustime;
        GameManager.Instance.soundManager.PlaySfx(SoundEnum.Bomb,0.5f);
        Camerashake.HitShakeCamera();
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

    // ���� ���� Ÿ�̸�: �� ������ Time.deltaTime ��ŭ ����
    IEnumerator GameTimerCount()
    {
        // �ڷ�ƾ ���� �� �ʱ� �� �� ���� (Floor)
        int lastSecond = Mathf.CeilToInt(TotalTime);
       

        while (TotalTime > 0f)
        {
            yield return null;

           
            // ���� �ð� ����
            TotalTime -= Time.deltaTime;
            if (TotalTime < 0f)
            {
                TotalTime = 0f;
            }

            // ���� ���� �� (����) ���
            int currentSecond = Mathf.CeilToInt(TotalTime);
            // ���� ���� �����Ӱ� �ʰ� �޶����ٸ� �̺�Ʈ ȣ��
            if (currentSecond != lastSecond)
            {
                
                lastSecond = currentSecond;
                FOnTotalTimeChanged?.Invoke(lastSecond);
            }
           
        }

        // ���� Ÿ�̸� ���� �� ó��
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
        //���� ���� ������ 
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

        //�ӽ� �Ҹ� 
        if (GameManager.Instance != null)
            GameManager.Instance.soundManager.PlaySfx(SoundEnum.AddMoney);

        PlayerHud.CallBack_RemoveDetectTarget(star);
      
        PlayerHud.ChangeStarCntText(StarCnt);
        Destroy(star.gameObject);
    }

    public void OnUfoSwallowSound()
    {
        //�ӽ� �Ҹ� 
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
                Debug.Log("�̹� ���� ����");
                return;

            }
        }
        objectManager.AllObjectStopActive(active);
       
       
      
    }

    public void GamePause(bool active)
    {
        //OnSoundPause(active);
        ufoplayer.CallBack_StopMovement(active);
        GameManager.Instance.vibrationManager.OnStopVibration();
        //GameManager.Instance.vibrationManager.OnPauseVibration(active);
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

        GamePause(true);

        GameManager.Instance.soundManager.PlayBgm(SoundEnum.BGM_GameEnd,0.5f);
        bIsGameEnd = true;
        int converttime = Mathf.RoundToInt(TotalPlayTime);
        PlayerHud.UpdateGameState(converttime, TotalScore, StarCnt);

        SaveUserData();
    }

    private void CheckHardCoreAchivemet()
    {
        if (GameManager.Instance.userData != null)
        {
            //( ���� )�ô� �̵� ���� ī��Ʈ 
            if(AllSkillManager.Skillcount == 0)
            {
                string Skillcnt0Id = $"HardCore_SkiiCnt0";
                AchievementManager.Instance.ReportProgress(AchieveEnum.HardCore, Skillcnt0Id, 1);
            }
          

        }

    }

    private void SaveUserData()
    {
        GameManager.Instance.userData.AddStarCnt(StarCnt);
        GameManager.Instance.userData.UpdateBestScore(TotalScore);

        GameManager.Instance.SaveUserData();
    }


}
