
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Random = UnityEngine.Random;

public class ObjectManager : MonoBehaviour
{
    private GameState gameState;

    [SerializeField]
    private List<ObjectSpawnPoint> objectspawnPoints = new List<ObjectSpawnPoint>();
   
    [SerializeField]
    private List<GenerationObjects> generationList = new List<GenerationObjects>();

    private GenerationObjects CurrentGenerationData = null;
    public int CurrentGenration { get; private set; } = 0;


    private List<ExelEarthObjectData> ExelEarthObjectStatList = new List<ExelEarthObjectData>();
    private List<EarthObjectStatData> objectStatList = new List<EarthObjectStatData>();
    public IReadOnlyList<EarthObjectStatData> ObjectStatList => objectStatList;

    public ExelEarthObjectData ObjecstStatTime { get; private set; } = null;

    private EarthObjectStatData bossStat = null;

    private ExelBombStatData ExelBombstat = null;
    public EarthObjectStatData BombStatData { get; private set; } = null;
    public ExelBombStatData BombStatTime { get; private set; } = null;



  
    public event Action<int /*currentgeneraetion*/> FOnGenerationChanged;
    public List<FallingObject> AllSpawnObjects { get; private set; } = new List<FallingObject>();

    private BossObject currentBoss = null;

    [Header("��ü ������Ʈ ��")]
    [SerializeField]
    private int MaxObjectCnt  = 50;

    [Header("�� ����")]
    [SerializeField]
    private Renderer SpawnRange;

    Bounds RangeBounds;
    
    [Header("��ź")]
    [SerializeField]
    private float BombSpawnHeight=1.0f;
    [SerializeField] 
    private int BombPostionInterval = 1; 
    //[SerializeField]
    float BombSpawnTime = 5.0f;

    [Header("�����ð�")]
    [SerializeField]
    private float SpawnTime = 5.0f;
    [SerializeField]
    private float variation = 1.0f;

    private Coroutine BombCoroutine;

    public event Action<FallingObject> FOnObjectSwallowed;
    public event Action<Vector3 /*������ ���� ��ġ*/, ShapeEnum> FOnBonusSwallowed;
    public event Action<float /*minustime*/> FOnBomnbSwallowed;
    

    private Dictionary<ShapeEnum, int> BonusObjectsOrgin = new Dictionary<ShapeEnum, int>();
    private Dictionary<ShapeEnum, int> BonusObjectsCnt = new Dictionary<ShapeEnum, int>();
    public event Action< ShapeEnum, int /*count*/> FOnBounsWidgetCreated;
    public event Action<Dictionary<ShapeEnum,int>/*BonusObjectsOrgin*/, int/*CurrentGenration*/> FOnBounusCompleted;

    
    public int GetMaxObjectCnt() { return MaxObjectCnt; }

    public string getCurrentGenerationName() 
    {
        return generationList[CurrentGenration].GenerationName;
    }
    
    [Header("���ʽ�")]
   /* [SerializeField]
    private int bonusObjectsMinCount = 2;
    [SerializeField]
    private int bonusObjectsMaxCount = 4;*/

    [SerializeField]
    private int bonusMinCount = 1;
    [SerializeField]
    private int bonusMaxCount = 5;

    
    [Header("��ȭ ������Ʈ")]
    [SerializeField]
    private StarObject starobjectprefab;

    public event Action<IDetctable> FOnStartSpawned;



    public void InitObjectManager(GameState state)
    {
        gameState = state;

        LoadStatList();


        CreateBonusObjects();
        SetUpSpawnObjects(CurrentGenration);
        StartSpawnObjects();
        StartBombSapwn();
        SpawnBossAtRandomGridPosition();
    }

    //CSV ������Ʈ ,���� ���� ���� �ε�
    public void LoadStatList()
    {

       
        ExelEarthObjectStatList = CsvLoader.LoadCSV<ExelEarthObjectData>("StatData/CSVEarthObjectList");

        ObjecstStatTime = CsvLoader.LoadSingleCSV<ExelEarthObjectData>("StatData/CSVEarthObjectTime");
       

        ExelBombstat = CsvLoader.LoadSingleCSV<ExelBombStatData>("StatData/CSVBombStat");

        BombStatTime = CsvLoader.LoadSingleCSV<ExelBombStatData>("StatData/CSVBombStatTime");


        StatRenewalGeneration(CurrentGenration);

    }


    //������ �ٲ����
    public float SearchShapeData(ShapeEnum shape,int generation)
    {
        if (generationList.Count <= generation) return 0.0f;
       
        foreach(var objectdata in generationList[generation].objects)
        {
            if(objectdata.GetShapeEnum()== shape)
            {
                return objectdata.GetForceData().TimeCnt;
            }
        }

        return 0.0f;
    }

    #region ������Ʈ ���� , ����
    public void SetUpSpawnObjects(int currentgeneration)
    {
        CurrentGenerationData = (generationList.Count > 0) ? generationList[currentgeneration] : null;
        //FOnGenerationDataSeted?.Invoke(gen);
        foreach (var spawnPoint in objectspawnPoints)
        {
            if (CurrentGenerationData != null)
            {
                spawnPoint.SetGeneration(CurrentGenerationData, this,SpawnTime,variation);
                spawnPoint.SpawnPrefab();
            }
            
        }
    }

    public void ChangeGeneration()
    {
        if (generationList.Count <= CurrentGenration + 1)
        {
            Debug.Log("���� ���� ����");
            return; 
        }

        CurrentGenration += 1;
        
        //���� ���� ���� ��� ���� ����
        StatRenewalGeneration(CurrentGenration);

        //���� ����Ʈ�� ���� ���� ����
        SetUpSpawnObjects(CurrentGenration);

        SpawnBossAtRandomGridPosition();

        FOnGenerationChanged?.Invoke(CurrentGenration);
        Debug.Log(CurrentGenration + " : ���� ����");


    }

    private void StatRenewalGeneration(int currentgeneration)
    {
        objectStatList.Clear();

        //������Ʈ ����
        for (int i = 0; i < ExelEarthObjectStatList.Count; i++)
        {
            EarthObjectStatData earthojectdata = ScriptableObject.CreateInstance<EarthObjectStatData>();

            earthojectdata.bMovement = true;
            earthojectdata.mass = ExelEarthObjectStatList[i].Mass + (currentgeneration * ObjecstStatTime.Mass);
            earthojectdata.jumpDistance = ExelEarthObjectStatList[i].MoveDistance + (currentgeneration * ObjecstStatTime.MoveDistance);
            
            float baseScore = ExelEarthObjectStatList[i].AddScore;
            float scoreRate = ObjecstStatTime.AddScore;

            float baseTime = ExelEarthObjectStatList[i].AddTime;
            float timeRate = ObjecstStatTime.AddTime;

            earthojectdata.Score = Mathf.RoundToInt(baseScore * Mathf.Pow(scoreRate, currentgeneration));
            earthojectdata.TimeCnt = baseTime * Mathf.Pow(timeRate, currentgeneration);

            earthojectdata.EXPCnt = ExelEarthObjectStatList[i].AddExp + (currentgeneration * ObjecstStatTime.AddExp);
            earthojectdata.SpawnWeight = ExelEarthObjectStatList[i].SpawnWeight + (currentgeneration * ObjecstStatTime.SpawnWeight);
            if (i == ExelEarthObjectStatList.Count - 1)
            {
                earthojectdata.EXPCnt = 0;
               

                bossStat = earthojectdata;
            }  
            else
                objectStatList.Add(earthojectdata);
        }

        //��ź ����
        EarthObjectStatData bombdata = ScriptableObject.CreateInstance<EarthObjectStatData>();
        bombdata.bMovement = false;
        bombdata.mass = ExelBombstat.Mass + (currentgeneration * BombStatTime.Mass);
        bombdata.jumpDistance = 0;
        bombdata.Score = 0;
        bombdata.TimeCnt = ExelBombstat.MinusTime + (currentgeneration * BombStatTime.MinusTime);
        bombdata.EXPCnt = 0;
        bombdata.SpawnWeight = 0;
        
        BombSpawnTime = ExelBombstat.SpawnInterval + (currentgeneration * BombStatTime.SpawnInterval);

        BombStatData = bombdata;

        Debug.Log(currentgeneration + " ���� ���� ���� �Ϸ�");
    }
    public void StartSpawnObjects()
    {
        
        foreach (var spawnPoint in objectspawnPoints)
        {
          
           spawnPoint.SpawnPrefab();

        }
    }


    public void PauseSpawnObjects(bool bpause)
    {
        
        foreach (var spawnPoint in objectspawnPoints)
        { 
           
           spawnPoint.SetPauseState(bpause);
 
        }
    }

    //�ƿ� ���߱� , �ٽ� ���� ����
    public void StopSpawnObjects()
    {
        foreach (var spawnPoint in objectspawnPoints)
        {

            spawnPoint.StopSpawn();

        }
    }

    public void RegisterSpawnedObject(FallingObject spawnedObj)
    {
        // TotalObjectCnt++;
        AllSpawnObjects.Add(spawnedObj);

    }
    public void RemoveSpawnedObject(FallingObject destroyoBj)
    {
        if (GameState.Instance != null)
        {
            ShapeEnum destroyShape = destroyoBj.GetShapeEnum();
            //TotalObjectCnt--;
            if (destroyShape == ShapeEnum.boomb)
            {
                FOnBomnbSwallowed?.Invoke(destroyoBj.TimeCnt);
            }
            else
            {
                if(BonusObjectsCnt.ContainsKey(destroyShape))
                {
                    CheckBonusCleared(destroyShape);
                    Vector3 position = Vector3.zero;
                  /*  if (destroyoBj.bIsAttacked)
                    {
                        position = GameState.Instance.GetPlayerPostion();
                    }
                    else
                    {
                        position = destroyoBj.gameObject.transform.position;
                    }*/
                    position = destroyoBj.gameObject.transform.position;

                    FOnBonusSwallowed?.Invoke(position, destroyoBj.GetShapeEnum());
                }


                //FOnObjectSwallowed?.Invoke(destroyoBj, CurrentGenration);
                FOnObjectSwallowed?.Invoke(destroyoBj);
            }
         
        }
        if(AllSpawnObjects.Contains(destroyoBj))
            AllSpawnObjects.Remove(destroyoBj);

       
        Destroy(destroyoBj.gameObject);

    }

    #endregion

    #region ���ʽ� ������Ʈ ���� , ����

    /*private void CreateBonusObjects()
    {
        // generationList üũ: ����ְų� CurrentGenration �ε��� �ʰ��� ��� �� null ��ȯ
        if (generationList == null || generationList.Count <= CurrentGenration)
        {
            Debug.LogWarning("Generation list is empty or CurrentGenration index is out of range!");
            return;
        }

        // ���� GenerationObjects ����
        GenerationObjects currentGen = generationList[CurrentGenration];

        // objects ����Ʈ�� ������� Ȯ��
        if (currentGen.objects == null || currentGen.objects.Count == 0)
        {
            Debug.LogWarning("No objects found in the current GenerationObjects!");
            return;
        }

        // �ִ� ���� ������ ����: 5�� ����Ʈ ���� �� ���� ��
        int maxSelectable = Mathf.Clamp(bonusObjectsMaxCount, 1, currentGen.objects.Count);                                                                  
        int minSelectable = Mathf.Clamp(bonusObjectsMinCount, 1, maxSelectable);
        // ������ ������Ʈ�� ����: �ּ� 2������ maxSelectable �� ���� (maxSelectable + 1�� Random.Range�� ���� ������ Ư�� ����)
        int countToSelect = UnityEngine.Random.Range(minSelectable, maxSelectable + 1);

        //Debug.Log("���� : " + countToSelect);
        // objects ����Ʈ�� ������ �� ������ ������ ����
        List<FallingObject> tempList = new List<FallingObject>(currentGen.objects);
        for (int i = 0; i < tempList.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, tempList.Count);
            FallingObject temp = tempList[i];
            tempList[i] = tempList[randomIndex];
            tempList[randomIndex] = temp;
        }

        // ���� ����Ʈ���� ���ʺ��� countToSelect ��ü�� ����
       
        for (int i = 0; i < countToSelect; i++)
        {
            int randomCount = UnityEngine.Random.Range(bonusMinCount, bonusMaxCount+1); // 2���� 5���� (6 ������)


            BonusObjectsOrgin.Add(tempList[i].GetShapeEnum(), randomCount);
            BonusObjectsCnt.Add(tempList[i].GetShapeEnum(), randomCount);
            FOnBounsWidgetCreated?.Invoke(tempList[i].GetShapeEnum(), randomCount);
        }

    }*/
    private void CreateBonusObjects()
    {

        // generationList üũ: ����ְų� CurrentGenration �ε��� �ʰ��� ��� �� null ��ȯ
        if (generationList == null || generationList.Count <= CurrentGenration)
        {
            Debug.LogWarning("Generation list is empty or CurrentGenration index is out of range!");
            return;
        }

        // ���� GenerationObjects ����
        GenerationObjects currentGen = generationList[CurrentGenration];

        // objects ����Ʈ�� ������� Ȯ��
        if (currentGen.objects == null || currentGen.objects.Count == 0)
        {
            Debug.LogWarning("No objects found in the current GenerationObjects!");
            return;
        }

        int levelOffset = CurrentGenration * 4 + 1;

        int usableCount = Mathf.Clamp(gameState.GetPlayerCurrentLevel() - levelOffset + 1, 1, currentGen.objects.Count);

        List<FallingObject> limitedList = currentGen.objects.GetRange(0, usableCount);

        // ����

        for (int i = 0; i < limitedList.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, limitedList.Count);
            (limitedList[i], limitedList[randomIndex]) = (limitedList[randomIndex], limitedList[i]);
        }

        int countToSelect = UnityEngine.Random.Range(1, limitedList.Count+1);

        for (int i = 0; i < countToSelect; i++)
        {
            int randomCount = UnityEngine.Random.Range(bonusMinCount, bonusMaxCount + 1); // 2���� 5���� (6 ������)


            BonusObjectsOrgin.Add(limitedList[i].GetShapeEnum(), randomCount);
            BonusObjectsCnt.Add(limitedList[i].GetShapeEnum(), randomCount);
            FOnBounsWidgetCreated?.Invoke(limitedList[i].GetShapeEnum(), randomCount);
        }

      
    }

    private void CheckBonusCleared(ShapeEnum shape)
    {
        if (BonusObjectsCnt.ContainsKey(shape))
        {
            BonusObjectsCnt[shape]--;

            if (BonusObjectsCnt[shape] <= 0)
            {
                BonusObjectsCnt.Remove(shape);

            }
        }
    }


    public void CallBack_RenewalBonusObject(bool bonusclear)
    {
        if (!bonusclear)
        {
            //���� �ִ� ���ʽ� Ŭ���� ���� �߰�
            FOnBounusCompleted?.Invoke(BonusObjectsOrgin, CurrentGenration);
        }
        else
        {
            Debug.Log("���� ���� �ʱ�ȭ �ؼ� ���ʽ� Ŭ���� ���� �ȵ�");
        }

        //�ʱ�ȭ
        BonusObjectsOrgin.Clear();
        BonusObjectsCnt.Clear();

        //��(��ȭ) ����
        SpawnStarAtRandomGridPosition();

        //���ʽ� ��ǥ �����
        CreateBonusObjects();
    }

    #endregion

    #region ��ź

    public void OnBombSpawn(bool active)
    {
        if(active)
        {
            StartBombSapwn();
        }
        else
        {
            StopBombSpawn();
        }
    }

    private void StartBombSapwn()
    {
        
        BombCoroutine = StartCoroutine(SpawnRoutine());
    }


    
    private void StopBombSpawn()
    {
        StopCoroutine(BombCoroutine);
    }
    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnBombAtRandomGridPosition();
            yield return new WaitForSeconds(BombSpawnTime);
        }
    }

    private void SpawnBombAtRandomGridPosition()
    {
        FallingObject spawnbomb = generationList[CurrentGenration].bomb;
        if (spawnbomb == null) return;

        Vector2 SpawnLocation = RandomSpawnRange();

       
        Vector3 spawnPosition = new Vector3(SpawnLocation.x, BombSpawnHeight, SpawnLocation.y);
        GameObject bombobj = Instantiate(spawnbomb.gameObject, spawnPosition, Quaternion.identity);
       
        FallingObject bombfall = bombobj.GetComponent<FallingObject>();
        if (bombfall != null)
        {
            bombfall.SetStatData(BombStatData);
            bombfall.SetBomb();
            bombfall.AddGenerationMass(CurrentGenration);
            bombfall.onSwallowed.AddListener(RemoveSpawnedObject);
        }

    }

    private Vector2 RandomSpawnRange()
    {
       
       RangeBounds = SpawnRange.bounds;
        // Plane�� Bounds���� x, z ��ǥ�� �ּ�/�ִ밪�� ���� ������ ���մϴ�.
        int minX = Mathf.CeilToInt(RangeBounds.min.x);
        int maxX = Mathf.FloorToInt(RangeBounds.max.x);
        int minZ = Mathf.CeilToInt(RangeBounds.min.z);
        int maxZ = Mathf.FloorToInt(RangeBounds.max.z);

        // �����ϰ� ���� ��ǥ ���� (max�� ���Խ�Ű�� ���� +1)
        int randomX = Random.Range(minX, maxX + BombPostionInterval);
        int randomZ = Random.Range(minZ, maxZ + BombPostionInterval);

        return new Vector2(randomX, randomZ);   
    }
    #endregion

    #region ����
    
    void SpawnBossAtRandomGridPosition()
    {
        BossObject bossprefab = generationList[CurrentGenration].Boss;
        if (bossprefab == null) return;

        Vector2 SpawnLocation = RandomSpawnRange();


        Vector3 spawnPosition = new Vector3(SpawnLocation.x, 0.0f, SpawnLocation.y);
        GameObject bossobj = Instantiate(bossprefab.gameObject, spawnPosition, Quaternion.identity);
        //EarthObjectStatData bossstatdata = null;
        //if (CurrentGenration < BossStatList.Count)
         //bossstatdata = bossstatdata;

        BossObject boss = bossobj.GetComponent<BossObject>();
        if (boss != null)
        {
            boss.SetStatData(bossStat);
            boss.AddGenerationMass(CurrentGenration);
            boss.FOnBossSwallowed += CallBacK_BossSwallow;
            currentBoss = boss;
            gameState.CallBack_BossSpawned(boss);


        }
    }

    void CallBacK_BossSwallow(BossObject bossObject)
    {
        if(bossObject!=null)
        {

            //���� �ð� , ���� �߰�
            FOnObjectSwallowed(bossObject);
            //�ӽ� �Ҹ�
            //gameState.OnUfoSwallowSound();
           
            ChangeGeneration();

            Destroy(bossObject.gameObject);
        }
    }

    #endregion

    #region ��
    void SpawnStarAtRandomGridPosition()
    {
       
        if (starobjectprefab == null) return;

        Vector2 SpawnLocation = RandomSpawnRange();


        Vector3 spawnPosition = new Vector3(SpawnLocation.x, BombSpawnHeight +5.0f, SpawnLocation.y);
        GameObject starobj = Instantiate(starobjectprefab.gameObject, spawnPosition, Quaternion.identity);

        StarObject star = starobj.GetComponent<StarObject>();
        if (star != null)
        {
            star.AddGenerationMass(0);
            star.FOnStarSwallowed += gameState.CallBack_StartSwallowed;

            //star spawn �̺�Ʈ �߻�
            FOnStartSpawned?.Invoke(star);
        }
    }

    #endregion

    public void AllObjectStopActive(bool active)
    {

        foreach (var fallingobject in AllSpawnObjects)
        {

            fallingobject.ActivateBounce(!active);

        }
        currentBoss.ActivateBounce(!active);
    }

    public void IceActive(bool active)
    {
        foreach (var fallingobject in AllSpawnObjects)
        {

            fallingobject.ActiveIce(active);

        }
        currentBoss.ActiveIce(active);
    }


}

