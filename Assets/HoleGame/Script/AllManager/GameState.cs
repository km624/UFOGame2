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

    // �� ���� �ð�(��), float���� ����
    [SerializeField]
    private float TotalTime = 30.0f;
    //private bool bIsTimeStop = false;
    Coroutine GameTimeCoroutine;
    Coroutine PlayTimeCoroutine;
    public event Action<int /*TotalSecound*/> FOnTotalTimeChanged;
   
    // �÷��� �ð� ����, float���� ���� (���Ѵٸ� int�� ��ȯ�ؼ� ǥ�� ����)
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

     //���ʽ� ��ǥ ���� ���� or ����
        objectManager.FOnBounsWidgetCreated += PlayerHud.CallBack_CreateShapeWidget;

        //���ʽ� ��ǥ ���� ��� , �ı� �� ������ ������
        objectManager.FOnBonusSwallowed += PlayerHud.CallBack_SpawnUIImageAtWorldPos;

        objectManager.FOnBounusCompleted += CallBack_BonusClear;
        //������Ʈ ���Ƶ鿴���� ���ε�
        objectManager.FOnObjectSwallowed += CallBack_ObjectSwallow;
        
        //��ֹ� ���Ƶ鿴����
        objectManager.FOnBomnbSwallowed += CallBack_BombSwallow;


        PlayerHud.FOnAllBounusAnimEnded += objectManager.CallBack_RenewalBonusObject;



        //��ų ����
        AllSkillManager.SetSkill(ufoplayer);
        //��ų ǥ��
        foreach (SkillBase skilldata in AllSkillManager.AllSkillPrefabs)
        {

            PlayerHud.CreateSkillWidget(skilldata, AllSkillManager);
        }
        //��ų ���ε�
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

        //Debug.Log("�����");
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
                Debug.Log("���ʽ� ���� �߰� : " + TotalScore);
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
                Debug.Log("�̹� ���� ����");
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
