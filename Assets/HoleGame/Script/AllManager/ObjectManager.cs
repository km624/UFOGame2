
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

public class ObjectManager : MonoBehaviour
{
    private GameState gameState;


    [SerializeField]
    private GameObject AllObjectSpawnPoints=null;
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

    [Header("전체 오브젝트 수")]
    [SerializeField]
    private int MaxObjectCnt  = 50;

    [Header("맵 범위")]
    [SerializeField]
    private Renderer SpawnRange;

    Bounds RangeBounds;
    
    [Header("폭탄")]
    [SerializeField]
    private float BombSpawnHeight=1.0f;
    [SerializeField] 
    private int BombPostionInterval = 1; 
    //[SerializeField]
    float BombSpawnTime = 5.0f;

    [Header("스폰시간")]
    [SerializeField]
    private float SpawnTime = 5.0f;
    [SerializeField]
    private float variation = 1.0f;

    private Coroutine BombCoroutine;

 
 

    public event Action<FallingObject> FOnObjectSwallowed;
    public event Action<Vector3 /*아이콘 생성 위치*/, ShapeEnum> FOnBonusSwallowed;
    public event Action<float /*minustime*/> FOnBomnbSwallowed;
    

    //private Dictionary<ShapeEnum, int> BonusObjectsOrgin = new Dictionary<ShapeEnum, int>();
    private Dictionary<float/*objectsmass*/, int> BonusObjectsOrgin = new Dictionary<float, int>();
    private Dictionary<ShapeEnum, int> BonusObjectsCnt = new Dictionary<ShapeEnum, int>();
    public event Action< ShapeEnum, int /*count*/> FOnBounsWidgetCreated;
    public event Action<Dictionary<float,int>/*BonusObjectsOrgin*/> FOnBounusCompleted;

    
    public int GetMaxObjectCnt() { return MaxObjectCnt; }

    public string getCurrentGenerationName() 
    {
        return CurrentGenerationData.GenerationName;
    }
    
    [Header("보너스")]
  

    [SerializeField]
    private int bonusMinCount = 1;
    [SerializeField]
    private int bonusMaxCount = 5;

    
    [Header("재화 오브젝트")]
    [SerializeField]
    private StarObject starobjectprefab;

    public event Action<IDetctable> FOnStartSpawned;

    [Header("맵")]
    [SerializeField]
    private MapChange mapChange;

    public void InitObjectManager(GameState state)
    {
        gameState = state;

        LoadStatList();

        CollectSpawnPoints();
        
        SetUpGeneration(CurrentGenration);

        CreateBonusObjects();
        StartSpawnObjects();
        StartBombSapwn();
        SpawnBossAtRandomGridPosition();
    }

    private void CollectSpawnPoints()
    {
        objectspawnPoints.Clear();
        // IncludeInactive=true 로 비활성화된 자식도 수집
        var points = AllObjectSpawnPoints.GetComponentsInChildren<ObjectSpawnPoint>(includeInactive: true);
        foreach (var p in points)
            // 자기 자신에 붙어 있으면 제외하고, 자식만
            if (p.transform != this.transform)
                objectspawnPoints.Add(p);
    }

    //CSV 오브젝트 ,보스 스텟 파일 로드
    public void LoadStatList()
    {

       
        ExelEarthObjectStatList = CsvLoader.LoadCSV<ExelEarthObjectData>("StatData/CSVEarthObjectList");

        ObjecstStatTime = CsvLoader.LoadSingleCSV<ExelEarthObjectData>("StatData/CSVEarthObjectTime");
       

        ExelBombstat = CsvLoader.LoadSingleCSV<ExelBombStatData>("StatData/CSVBombStat");

        BombStatTime = CsvLoader.LoadSingleCSV<ExelBombStatData>("StatData/CSVBombStatTime");


        StatRenewalGeneration(CurrentGenration);

    }


    //무조건 바꿔야함
    public int GetBounusScoreData(float objectmass)
    {
       
        int index = ((int)objectmass - 1) % (objectStatList.Count - 1);
        //Debug.Log( "보너스 달성 이름: " + CurrentGenerationData.objects[index].name);
        //Debug.Log(CurrentGenration + " 번째 시대의 " + index + " 번째 스텟의 Score : " + objectStatList[index].Score);

        return objectStatList[index].Score;
    }

    #region 오브젝트 생성 , 관리
    public void SetUpGeneration(int currentgeneration)
    {
        // CurrentGenerationData = (generationList.Count > 0) ? generationList[currentgeneration] : null;
       
        // 기본 세대 설정 (정상 범위)
        if (generationList.Count > currentgeneration)
        {
            CurrentGenerationData = generationList[currentgeneration];
        }
        else
        {
            //범위 초과 시 랜덤으로 하나 고르되, 중복 방지
            if (generationList.Count > 1)
            {
                // 임시 리스트에서 현재 세대 제외
                List<GenerationObjects> pool = new List<GenerationObjects>(generationList);
                pool.Remove(CurrentGenerationData);

                CurrentGenerationData = pool[Random.Range(0, pool.Count)];
                Debug.Log("시대 반복 : " + CurrentGenerationData.GenerationName);
            }
           
        }
        mapChange.ChangeMap(CurrentGenerationData);
       
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
       /* if (generationList.Count <= CurrentGenration + 1)
        {
            Debug.Log("다음 세대 없음");
            return; 
        }*/

        CurrentGenration += 1;
        
        //세대 스텟 배율 대로 새로 세팅
        StatRenewalGeneration(CurrentGenration);

        //스폰 포인트에 현재 세대 세팅
        SetUpGeneration(CurrentGenration);

        SpawnBossAtRandomGridPosition();

        FOnGenerationChanged?.Invoke(CurrentGenration);
        Debug.Log(CurrentGenration + " : 현재 세대");


    }

    private void StatRenewalGeneration(int currentgeneration)
    {
        objectStatList.Clear();

        //오브젝트 세팅
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

        //폭탄 세팅
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

        Debug.Log(currentgeneration + " 세대 스텟 세팅 완료");
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

    //아예 멈추기 , 다시 시작 못함
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

    #region 보너스 오브젝트 생성 , 관리

    
    private void CreateBonusObjects()
    {

        // generationList 체크: 비어있거나 CurrentGenration 인덱스 초과시 경고 후 null 반환
      /*  if (generationList == null || generationList.Count <= CurrentGenration)
        {
            Debug.LogWarning("Generation list is empty or CurrentGenration index is out of range!");
            return;
        }*/

        // 현재 GenerationObjects 참조
        //GenerationObjects currentGen = generationList[CurrentGenration];
        GenerationObjects currentGen = CurrentGenerationData;

        // objects 리스트가 비었는지 확인
        if (currentGen.objects == null || currentGen.objects.Count == 0)
        {
            Debug.LogWarning("No objects found in the current GenerationObjects!");
            return;
        }

        int levelOffset = CurrentGenration * 4 + 1;

        int usableCount = Mathf.Clamp(gameState.GetPlayerCurrentLevel() - levelOffset + 1, 1, currentGen.objects.Count);

        List<FallingObject> limitedList = currentGen.objects.GetRange(0, usableCount);

        // 셔플

        for (int i = 0; i < limitedList.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, limitedList.Count);
            (limitedList[i], limitedList[randomIndex]) = (limitedList[randomIndex], limitedList[i]);
        }

        int countToSelect = UnityEngine.Random.Range(1, limitedList.Count+1);

        for (int i = 0; i < countToSelect; i++)
        {
            int randomCount = UnityEngine.Random.Range(bonusMinCount, bonusMaxCount + 1); // 2부터 5까지 (6 미포함)


            BonusObjectsOrgin.Add(limitedList[i].ObjectMass, randomCount);
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
            //전에 있던 보너스 클리어 점수 추가
            FOnBounusCompleted?.Invoke(BonusObjectsOrgin);
        }
        else
        {
            Debug.Log("강제 위젯 초기화 해서 보너스 클리어 점수 안들어감");
        }

        //초기화
        BonusObjectsOrgin.Clear();
        BonusObjectsCnt.Clear();

        //별(재화) 생성
        SpawnStarAtRandomGridPosition();

        //보너스 목표 재생성
        CreateBonusObjects();
    }

    #endregion

    #region 폭탄

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
        //FallingObject spawnbomb = generationList[CurrentGenration].bomb;
        FallingObject spawnbomb = CurrentGenerationData.bomb;
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
        // Plane의 Bounds에서 x, z 좌표의 최소/최대값을 정수 단위로 구합니다.
        int minX = Mathf.CeilToInt(RangeBounds.min.x);
        int maxX = Mathf.FloorToInt(RangeBounds.max.x);
        int minZ = Mathf.CeilToInt(RangeBounds.min.z);
        int maxZ = Mathf.FloorToInt(RangeBounds.max.z);

        // 랜덤하게 정수 좌표 생성 (max는 포함시키기 위해 +1)
        int randomX = Random.Range(minX, maxX + BombPostionInterval);
        int randomZ = Random.Range(minZ, maxZ + BombPostionInterval);

        return new Vector2(randomX, randomZ);   
    }
    #endregion

    #region 보스
    
    void SpawnBossAtRandomGridPosition()
    {
        //BossObject bossprefab = generationList[CurrentGenration].Boss;
        BossObject bossprefab = CurrentGenerationData.Boss;
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

            //보스 시간 , 점수 추가
            FOnObjectSwallowed(bossObject);
            //임시 소리
            //gameState.OnUfoSwallowSound();
           
            ChangeGeneration();

            Destroy(bossObject.gameObject);
        }
    }

    #endregion

    #region 별
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

            //star spawn 이벤트 발생
            FOnStartSpawned?.Invoke(star);
        }
    }

    #endregion

    public void AllObjectStopActive(bool active)
    {
        //Debug.Log(active);
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

