
using System;

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

  

    private ExelBombStatData ExelBombstat = null;
   
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
    [SerializeField]
    private Renderer SafeSpawnRange;

   
    [Header("오브젝트 스폰시간")]
    [SerializeField]
    private float SpawnTime = 5.0f;
    [SerializeField]
    private float variation = 1.0f;

    public event Action<FallingObject> FOnObjectSwallowed;
    public event Action<Vector3 /*아이콘 생성 위치*/, ShapeEnum> FOnBonusSwallowed;
    public event Action<float /*minustime*/> FOnBomnbSwallowed;
    
    private Dictionary<float/*objectsmass*/, int> BonusObjectsOrgin = new Dictionary<float, int>();
    private Dictionary<ShapeEnum, int> BonusObjectsCnt = new Dictionary<ShapeEnum, int>();
    public event Action< ShapeEnum, int /*count*/> FOnBounsWidgetCreated;
    public event Action<Dictionary<float,int>/*BonusObjectsOrgin*/> FOnBounusCompleted;


    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private LayerMask IgnoreMask;

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

    [Header("랜덤스포너")]
    [SerializeField] RandomSpawner bombSpawner;
    [SerializeField] RandomSpawner bossSpawner;
    [SerializeField] RandomSpawner moneySpawner;
    [SerializeField] RandomSpawner bonusSpawner;




    [Header("맵")]
    [SerializeField]
    private MapChange mapChange;

    public void InitObjectManager(GameState state)
    {
        gameState = state;

        LoadStatList();

        CollectSpawnPoints();

        InitRandomSpawner();

        SetUpGeneration(CurrentGenration);

        StatRenewalGeneration(CurrentGenration);

        CreateBonusObjects();

        InitSpawnPoints();
        StartSpawnObjects();

        bombSpawner.OnRandomSpawn(true);
        bossSpawner.OnRandomSpawn(true);
        
        for(int i=0;i<20;i++)
        {
            bonusSpawner.OnRandomSpawn(true);
        }
    }

    private void InitRandomSpawner()
    {
        bombSpawner.Initialize(this, SpawnRange,SafeSpawnRange);

        bossSpawner.Initialize(this, SpawnRange, SafeSpawnRange);

        bonusSpawner.Initialize(this, SpawnRange, SafeSpawnRange);

        moneySpawner.Initialize(this, SpawnRange, SafeSpawnRange);
        moneySpawner.SetSpawnStatData(starobjectprefab, null);
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

    private void InitSpawnPoints()
    {
        foreach (var spawnPoint in objectspawnPoints)
        {
            if (CurrentGenerationData != null)
            {
                spawnPoint.SetGeneration(CurrentGenerationData, this, SpawnTime, variation);
               
            }

        }
    }

    //CSV 오브젝트 ,보스 스텟 파일 로드
    public void LoadStatList()
    {

       
        ExelEarthObjectStatList = CsvLoader.LoadCSV<ExelEarthObjectData>("StatData/CSVEarthObjectList");

        ObjecstStatTime = CsvLoader.LoadSingleCSV<ExelEarthObjectData>("StatData/CSVEarthObjectTime");
       

        ExelBombstat = CsvLoader.LoadSingleCSV<ExelBombStatData>("StatData/CSVBombStat");

        BombStatTime = CsvLoader.LoadSingleCSV<ExelBombStatData>("StatData/CSVBombStatTime");


    }


    
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
   
        if(GameManager.Instance!=null)
        {
            GameManager.Instance.soundManager.PlayBgm(CurrentGenerationData.GenerationBGM,1.0f);
        }

        if (GameManager.Instance.userData != null)
        {
            //( 업적 ) 시대별 방분 
            string GenerationMoveId = $"Behavior_Generation_{currentgeneration}"; ;
            AchievementManager.Instance.ReportProgress(AchieveEnum.Behavior, GenerationMoveId, 1);

        }
    }

    public void ChangeGeneration()
    {
      
        CurrentGenration += 1;
        
       

        //스폰 포인트에 현재 세대 세팅
        SetUpGeneration(CurrentGenration);
        
        //세대 스텟 배율 대로 새로 세팅
        StatRenewalGeneration(CurrentGenration);

        InitSpawnPoints();
        StartSpawnObjects();

        //SpawnBossAtRandomGridPosition();
        bossSpawner.OnRandomSpawn(true);

        for (int i = 0; i < 15; i++)
        {
            bonusSpawner.OnRandomSpawn(true);
        }

        FOnGenerationChanged?.Invoke(CurrentGenration);

        Debug.Log(CurrentGenration + " : 현재 세대");

        if (GameManager.Instance.userData != null)
        {
            //( 업적 )시대 이동 누적 카운트 
            string GenerationMovecntId = $"Behavior_Generation_Cnt"; 
            AchievementManager.Instance.ReportProgress(AchieveEnum.Behavior, GenerationMovecntId, 1);

        }

    }

    private void StatRenewalGeneration(int currentgeneration)
    {
        objectStatList.Clear();

        //오브젝트 세팅
        for (int i = 0; i < ExelEarthObjectStatList.Count; i++)
        {
            EarthObjectStatData earthojectdata = ScriptableObject.CreateInstance<EarthObjectStatData>();

            if(i!= 0)
                earthojectdata.bMovement = true;
            else
                earthojectdata.bMovement = false;   

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
                //보스 스텟 세팅
                //bossStat = earthojectdata;
                bossSpawner.SetSpawnStatData(CurrentGenerationData.Boss, earthojectdata);
            }
            else if(i == 0)
            {
                bonusSpawner.SetSpawnStatData(CurrentGenerationData.objects[i], earthojectdata);
                objectStatList.Add(earthojectdata);
            }
            else
            {
                objectStatList.Add(earthojectdata);
               //Debug.Log("스텟 세팅");
            }
               
        }
        

        //폭탄 세팅
        EarthObjectStatData bombdata = ScriptableObject.CreateInstance<EarthObjectStatData>();
        bombdata.bMovement = false;
        bombdata.mass = ExelBombstat.Mass + (currentgeneration * BombStatTime.Mass);
        bombdata.jumpDistance = 0;
        bombdata.Score = 0;
        bombdata.TimeCnt = ExelBombstat.MinusTime + (currentgeneration * BombStatTime.MinusTime);
        bombdata.EXPCnt = 0;
        bombdata.SpawnWeight = ExelBombstat.SpawnInterval + (currentgeneration * BombStatTime.SpawnInterval);

       // BombSpawnTime = ExelBombstat.SpawnInterval + (currentgeneration * BombStatTime.SpawnInterval);
        //BombStatData = bombdata;

        //폭탄 스텟 세팅
        bombSpawner.SetSpawnStatData(CurrentGenerationData.bomb, bombdata);

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
    public void CallBack_RemoveSpawnedObject(FallingObject destroyoBj)
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
               
                    position = destroyoBj.gameObject.transform.position;

                    FOnBonusSwallowed?.Invoke(position, destroyoBj.GetShapeEnum());
                }


               
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
        GenerationObjects currentGen = CurrentGenerationData;

        if (currentGen.objects == null || currentGen.objects.Count == 0)
        {
            Debug.LogWarning("No objects found in the current GenerationObjects!");
            return;
        }
        Dictionary<ShapeEnum, float> shapeToActualMass = new();

        for (int i = 0; i < currentGen.objects.Count-1; i++)
        {
            ShapeEnum shape = currentGen.objects[i].GetShapeEnum();
            //Debug.Log( shape.ToString() + " mass세팅");
            float mass = objectStatList[i].mass;

            if (!shapeToActualMass.ContainsKey(shape))
                shapeToActualMass.Add(shape, mass);
            else
                Debug.LogWarning($"Duplicate ShapeEnum in stat list: {shape}");
        }


        // 세대당 4레벨씩 루프 돌게 구성
        int localLevel = gameState.GetPlayerCurrentLevel() - (CurrentGenration * 4);
        int usableCount = Mathf.Clamp(localLevel + 1, 1, currentGen.objects.Count);

        List<FallingObject> limitedList = new();
        
        for (int i = 1; i < usableCount; i++) // 0번째 인덱스 제외하고 1부터 시작
        {
            if (i < currentGen.objects.Count)
                limitedList.Add(currentGen.objects[i]);
        }
      
        //  셔플
        for (int i = 0; i < limitedList.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, limitedList.Count);
            (limitedList[i], limitedList[randomIndex]) = (limitedList[randomIndex], limitedList[i]);
        }
       
        int countToSelect = UnityEngine.Random.Range(1, limitedList.Count + 1);

        Debug.Log("countToSelect " + countToSelect);
        for (int i = 0; i < countToSelect; i++)
        {
            int randomCount = UnityEngine.Random.Range(bonusMinCount, bonusMaxCount + 1);
            
  
            //Debug.Log($"[{i}] ObjectMass: {shapeToActualMass[limitedList[i].GetShapeEnum()]}, Shape: {limitedList[i].GetShapeEnum()}");
           

            BonusObjectsOrgin.Add(shapeToActualMass[limitedList[i].GetShapeEnum()], randomCount);
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
        //SpawnStarAtRandomGridPosition();
       moneySpawner.OnRandomSpawn(true);

        //보너스 목표 재생성
        CreateBonusObjects();
    }

    #endregion

    #region 폭탄

    public void OnBombSpawn(bool active)
    {
        bombSpawner.OnRandomSpawn(active);
    }
    /*
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

       
        Vector3 SpawnLocation = GetValidSpawnPosition(false);


      
        GameObject bombobj = Instantiate(spawnbomb.gameObject, SpawnLocation, Quaternion.identity);
      
       
        FallingObject bombfall = bombobj.GetComponent<FallingObject>();
        if (bombfall != null)
        {
            bombfall.SetStatData(BombStatData);
            bombfall.SetBomb();
            bombfall.InitObject();
            bombfall.onSwallowed.AddListener(CallBack_RemoveSpawnedObject);
        }

    }
*/
  /*  private Vector2 RandomSpawnRange(bool boss)
    {
       if(!boss)
            RangeBounds = SpawnRange.bounds;
       else
            RangeBounds = SafeSpawnRange.bounds;
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


    private Vector3 GetValidSpawnPosition(bool boss)
    {
        int raycastMask = ~IgnoreMask; // ignoreMask에 해당하는 레이어만 제외하고 나머지는 전부 감지
       
        for (int i = 0; i < 50; i++)
        {
            Vector2 randXZ = RandomSpawnRange(boss);
            Vector3 origin = new Vector3(randXZ.x, 100f, randXZ.y);
            Vector3 direction = Vector3.down;
            float rayLength = 200f;

            Debug.DrawRay(origin, direction * rayLength, Color.red, 1.0f);

            if (Physics.Raycast(origin, direction, out RaycastHit hit, rayLength, raycastMask))
            {
                GameObject hitObj = hit.collider.gameObject;

                if (((1 << hitObj.layer) & wallMask) != 0)
                {
                    //Debug.Log($"[Spawn] Ray hit WALL: {hitObj.name}");
                    continue;
                }

                if (((1 << hitObj.layer) & groundMask) != 0)
                {
                  // Debug.Log($"[Spawn] Ray hit valid GROUND: {hitObj.name}");
                    return hit.point + Vector3.up * 0.5f;
                }

               // Debug.Log($"[Spawn] Ray hit something else: {hitObj.name} (Layer: {LayerMask.LayerToName(hitObj.layer)})");
            }
            else
            {
                Vector2 safearea = RandomSpawnRange(true);
                Vector3 newSafeArea = new Vector3(safearea.x, 0.5f, safearea.y);
                Debug.Log("[Spawn] Ray missed everything. safe " + safearea);
                return newSafeArea;
            }
        }

        //Debug.LogWarning("[Spawn] Failed to find valid spawn point. Returning fallback position.");
        return transform.position;
    }
*/
    #endregion

    #region 보스

    public void Callback_BossSpawn(BossObject bossobject)
    {
        
        currentBoss = bossobject;
        gameState.CallBack_BossSpawned(bossobject);
    }

   /* void SpawnBossAtRandomGridPosition()
    {
        //BossObject bossprefab = generationList[CurrentGenration].Boss;
        BossObject bossprefab = CurrentGenerationData.Boss;
        if (bossprefab == null) return;

       

        Vector3 spawnPosition = GetValidSpawnPosition(true);
        GameObject bossobj = Instantiate(bossprefab.gameObject, spawnPosition, Quaternion.identity);
      

        BossObject boss = bossobj.GetComponent<BossObject>();
        if (boss != null)
        {
            boss.SetStatData(bossStat);
            boss.InitObject();
            boss.FOnBossSwallowed += CallBacK_BossSwallow;
            currentBoss = boss;
            gameState.CallBack_BossSpawned(boss);


        }
    }
*/
    public void CallBacK_BossSwallow(BossObject bossObject)
    {
        if(bossObject!=null)
        {

            //보스 시간 , 점수 추가
            FOnObjectSwallowed(bossObject);
            if (GameManager.Instance.userData!=null)
            {
               if(GameManager.Instance.userData.userSettingData.bIsDirection)
                {
                    gameState.WarpDirectionStart();
                }
                else
                {
                    ChangeGeneration();
                }
            }
            else
                ChangeGeneration();


            Destroy(bossObject.gameObject);
        }
    }

    #endregion

    #region 별
    /* void SpawnStarAtRandomGridPosition()
     {

         if (starobjectprefab == null) return;


         Vector3 spawnPosition = GetValidSpawnPosition(false) + Vector3.up * 5.0f; 
         GameObject starobj = Instantiate(starobjectprefab.gameObject, spawnPosition, Quaternion.identity);

         StarObject star = starobj.GetComponent<StarObject>();
         if (star != null)
         {
             star.InitObject();
             star.FOnStarSwallowed += gameState.CallBack_StartSwallowed;

             //star spawn 이벤트 발생

             FOnStartSpawned?.Invoke(star);
         }
     }*/

    public void Callback_StarSpawned(StarObject starobject) 
    {
        FOnStartSpawned?.Invoke(starobject);
    }

    public void CallBack_StartSwallowed(StarObject starobject)
    {
        gameState.CallBack_StartSwallowed(starobject);
      
    }
    #endregion

  /*  private void SpawnBounsAtRandomGridPosition()
    {
        //FallingObject spawnbomb = generationList[CurrentGenration].bomb;
        FallingObject spawnbonus = CurrentGenerationData.objects[0];
        if (spawnbonus == null) return;

        
        Vector3 SpawnLocation = GetValidSpawnPosition(false);

        GameObject bonusobj = Instantiate(spawnbonus.gameObject, SpawnLocation, Quaternion.identity);
   
        FallingObject bonusfall = bonusobj.GetComponent<FallingObject>();
        if (bonusfall != null)
        {
            bonusfall.SetStatData(ObjectStatList[0]);

            bonusfall.InitObject();
            bonusfall.onSwallowed.AddListener(CallBack_RemoveSpawnedObject);
        }

    }*/


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

