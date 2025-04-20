using Lean.Gui;
using Lean.Transition.Method;
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
    private bool bIceActive = false;
    
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

        //�µ��� ���� ���ε�
       // objectManager.FOnGenerationDataSeted += PlayerHud.CallBack_SetThermometerWidget;
        //ufoplayer.FOnExpGagueAdded += PlayerHud.CallBack_AddEXPThermometerWidget;



        //��ų ����
        AllSkillManager.SetSkill(ufoplayer);
        //��ų ǥ��
        foreach (SkillBase skilldata in AllSkillManager.ReadAllSkills)
        {

            PlayerHud.CreateSkillWidget(skilldata, AllSkillManager);
        }

        //��ų ���ε�
        AllSkillManager.FOnSkillActivated += PlayerHud.ActiveSkill;

        //��Ÿ ī��Ʈ �ʱ�ȭ 0 ����
        PlayerHud.ChangeStarCntText(StarCnt);

        //���� ���� �÷��̾� ����
        PlayerHud.SetDetectStandardTarget(ufoplayer);

        //���� �ô� �̸� ����
        PlayerHud.ChangeGenerationNameText(objectManager.getCurrentGenerationName());   

        PlayerHud.SetTimeWidget(TotalTime,this);
        StartPlayTimer();
        StartGameTimer();
        PlayerHud.SetScoreText(TotalScore);

        objectManager.InitObjectManager(this);

       

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
        //ufoplayer.FOnExpGagueAdded -= PlayerHud.CallBack_AddEXPThermometerWidget;
    }

    private void CallBack_ObjectSwallow(FallingObject fallingobject)
    {

        //Debug.Log("�����");
        TotalScore += fallingobject.Score;
        TotalTime += fallingobject.TimeCnt;

        PlayerHud.SetScoreText(TotalScore);

        ufoplayer.AddEXPGauge(fallingobject.TimeCnt, fallingobject.ObjectMass);
      
    }

    private void CallBack_BonusClear(Dictionary<ShapeEnum, int> allbouns , int currentgeneration)
    {
        //Debug.Log("���ʽ� ���� �߰� ");
        foreach (var item in allbouns)
        {
            for(int i = 0; i < item.Value;i++)
            {
                float timecnt = objectManager.SearchShapeData(item.Key, currentgeneration);

                TotalScore += (int)(timecnt * 100.0f) * (1 + currentgeneration) * 2;
               
            }
         
        }
    }
    private void CallBack_BombSwallow(float minustime)
    {
       
        TotalTime -= minustime;
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
        OnUfoSwallowSound();

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
        bIceActive = active;

        objectManager.PauseSpawnObjects(active);
        objectManager.OnBombSpawn(active);

        StopGameTimer(active); 
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
       
        if (bIceActive)
            objectManager.IceActive(active);
      
    }

    public void GamePause(bool active)
    {
        objectManager.PauseSpawnObjects(active);
        AllObjectStopActive(active);
        StopGameTimer(  active );
        StopPlayTimer(  active );
        objectManager.OnBombSpawn(active);
    }
    

    public void GameEnd()
    {
       
        AllObjectStopActive(true);
        objectManager.StopSpawnObjects();
        objectManager.OnBombSpawn(false);
        bIsGameEnd = true;

        ufoplayer.CallBack_StopMovement();
        int converttime = Mathf.RoundToInt(TotalPlayTime);
        PlayerHud.UpdateGameState(converttime, TotalScore, StarCnt);

        SaveUserData();
    }

    private void SaveUserData()
    {
        GameManager.Instance.userData.AddStarCnt(StarCnt);
        GameManager.Instance.userData.UpdateBestScore(TotalScore);

        //��ų ī��Ʈ ������Ʈ
        for(int i=0;i<4;i++)
        {
            int cnt = AllSkillManager.ReadAllSkills[i].SkillCount;
            GameManager.Instance.userData.UpdateSkillCnt(i, cnt);
        }
       
        
        GameManager.Instance.SaveUserData();
    }

}
