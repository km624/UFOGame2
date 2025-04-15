
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

   
    private List<EarthObjectStatData> objectStatList = new List<EarthObjectStatData>();
    public IReadOnlyList<EarthObjectStatData> ObjectStatList => objectStatList;

    private List<EarthObjectStatData> bossStatList  = new List<EarthObjectStatData>();
    public IReadOnlyList<EarthObjectStatData> BossStatList => bossStatList;



    public int CurrentGenration { get; private set; } = 0;

  
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
    [SerializeField]
    float BombSpawnTime = 5.0f;

    [Header("스폰시간")]
    [SerializeField]
    private float SpawnTime = 5.0f;
    [SerializeField]
    private float variation = 1.0f;

    private Coroutine BombCoroutine;

    public event Action<FallingObject> FOnObjectSwallowed;
    public event Action<Vector3 /*아이콘 생성 위치*/, ShapeEnum> FOnBonusSwallowed;
    public event Action FOnBomnbSwallowed;
    

    private Dictionary<ShapeEnum, int> BonusObjectsOrgin = new Dictionary<ShapeEnum, int>();
    private Dictionary<ShapeEnum, int> BonusObjectsCnt = new Dictionary<ShapeEnum, int>();
    public event Action< ShapeEnum, int /*count*/> FOnBounsWidgetCreated;
    public event Action<Dictionary<ShapeEnum,int>/*BonusObjectsOrgin*/, int/*CurrentGenration*/> FOnBounusCompleted;

    
    public int GetMaxObjectCnt() { return MaxObjectCnt; }

    public string getCurrentGenerationName() 
    {
        return generationList[CurrentGenration].GenerationName;
    }
    
    [Header("보너스")]
   /* [SerializeField]
    private int bonusObjectsMinCount = 2;
    [SerializeField]
    private int bonusObjectsMaxCount = 4;*/

    [SerializeField]
    private int bonusMinCount = 1;
    [SerializeField]
    private int bonusMaxCount = 5;

    
    [Header("재화 오브젝트")]
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

    //CSV 오브젝트 ,보스 스텟 파일 로드
    public void LoadStatList()
    {
       
        List<ExelBossData> ExelBossStatList = new List<ExelBossData>();
        List<ExelEarthObjectData> ExelEarthObjectStatList = new List<ExelEarthObjectData>();

        ExelBossStatList = CsvLoader.LoadCSV<ExelBossData>("StatData/BossStatList");
        ExelEarthObjectStatList = CsvLoader.LoadCSV<ExelEarthObjectData>("StatData/EarthObjectList");


        EarthObjectStatData earthobjectdata= ScriptableObject.CreateInstance<EarthObjectStatData>();

        int generation = 0;
       foreach (var BossStat in ExelBossStatList)
       {
            EarthObjectStatData bossobjectdata = ScriptableObject.CreateInstance<EarthObjectStatData>();

            bossobjectdata.bMovement = true;
            bossobjectdata.mass = 5.0f + generation * 4.0f;
            bossobjectdata.jumpDistance = BossStat.MoveDistance;
            bossobjectdata.Score = BossStat.AddScore;
            bossobjectdata.TimeCnt = BossStat.Addtime;

            bossStatList.Add(bossobjectdata);
            
            generation++;   
        }

        foreach (var objectStat in ExelEarthObjectStatList)
        {
            EarthObjectStatData earthojectdata = ScriptableObject.CreateInstance<EarthObjectStatData>();

            earthojectdata.bMovement = true;
            earthojectdata.mass = objectStat.Mass;
            earthojectdata.jumpDistance = objectStat.MoveDistance;
            earthojectdata.Score = objectStat.AddScore;
            earthojectdata.TimeCnt = objectStat.AddTime;
            earthojectdata.EXPCnt = objectStat.AddExp;

            objectStatList.Add(earthojectdata);
  
        }

    }


    //무조건 바꿔야함
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

    #region 오브젝트 생성 , 관리
    public void SetUpSpawnObjects(int currentgeneration)
    {
        GenerationObjects gen = (generationList.Count > 0) ? generationList[currentgeneration] : null;
        //FOnGenerationDataSeted?.Invoke(gen);
        foreach (var spawnPoint in objectspawnPoints)
        {
            if (gen != null)
            {
                spawnPoint.SetGeneration(gen, this,SpawnTime,variation);
                spawnPoint.SpawnPrefab();
            }
            
        }
    }

    public void ChangeGeneration()
    {
        if (generationList.Count <= CurrentGenration + 1)
        {
            //Debug.Log("다음 세대 없음");
            return; 
        }

        CurrentGenration += 1;

        SetUpSpawnObjects(CurrentGenration);

        FOnGenerationChanged?.Invoke(CurrentGenration);
        Debug.Log(CurrentGenration + " : 현재 세대");


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
                FOnBomnbSwallowed?.Invoke();
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

    /*private void CreateBonusObjects()
    {
        // generationList 체크: 비어있거나 CurrentGenration 인덱스 초과시 경고 후 null 반환
        if (generationList == null || generationList.Count <= CurrentGenration)
        {
            Debug.LogWarning("Generation list is empty or CurrentGenration index is out of range!");
            return;
        }

        // 현재 GenerationObjects 참조
        GenerationObjects currentGen = generationList[CurrentGenration];

        // objects 리스트가 비었는지 확인
        if (currentGen.objects == null || currentGen.objects.Count == 0)
        {
            Debug.LogWarning("No objects found in the current GenerationObjects!");
            return;
        }

        // 최대 선택 가능한 개수: 5와 리스트 개수 중 작은 값
        int maxSelectable = Mathf.Clamp(bonusObjectsMaxCount, 1, currentGen.objects.Count);                                                                  
        int minSelectable = Mathf.Clamp(bonusObjectsMinCount, 1, maxSelectable);
        // 선택할 오브젝트의 개수: 최소 2개부터 maxSelectable 개 사이 (maxSelectable + 1은 Random.Range의 상한 미포함 특성 때문)
        int countToSelect = UnityEngine.Random.Range(minSelectable, maxSelectable + 1);

        //Debug.Log("랜덤 : " + countToSelect);
        // objects 리스트를 복사한 후 무작위 순서로 셔플
        List<FallingObject> tempList = new List<FallingObject>(currentGen.objects);
        for (int i = 0; i < tempList.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, tempList.Count);
            FallingObject temp = tempList[i];
            tempList[i] = tempList[randomIndex];
            tempList[randomIndex] = temp;
        }

        // 섞인 리스트에서 앞쪽부터 countToSelect 개체를 선택
       
        for (int i = 0; i < countToSelect; i++)
        {
            int randomCount = UnityEngine.Random.Range(bonusMinCount, bonusMaxCount+1); // 2부터 5까지 (6 미포함)


            BonusObjectsOrgin.Add(tempList[i].GetShapeEnum(), randomCount);
            BonusObjectsCnt.Add(tempList[i].GetShapeEnum(), randomCount);
            FOnBounsWidgetCreated?.Invoke(tempList[i].GetShapeEnum(), randomCount);
        }

    }*/
    private void CreateBonusObjects()
    {

        // generationList 체크: 비어있거나 CurrentGenration 인덱스 초과시 경고 후 null 반환
        if (generationList == null || generationList.Count <= CurrentGenration)
        {
            Debug.LogWarning("Generation list is empty or CurrentGenration index is out of range!");
            return;
        }

        // 현재 GenerationObjects 참조
        GenerationObjects currentGen = generationList[CurrentGenration];

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
            //전에 있던 보너스 클리어 점수 추가
            FOnBounusCompleted?.Invoke(BonusObjectsOrgin, CurrentGenration);
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
        FallingObject spawnbomb = generationList[CurrentGenration].bomb;
        if (spawnbomb == null) return;

        Vector2 SpawnLocation = RandomSpawnRange();

       
        Vector3 spawnPosition = new Vector3(SpawnLocation.x, BombSpawnHeight, SpawnLocation.y);
        GameObject bombobj = Instantiate(spawnbomb.gameObject, spawnPosition, Quaternion.identity);
       
        FallingObject falling = bombobj.GetComponent<FallingObject>();
        if (falling != null)
        {
            falling.AddGenerationMass(CurrentGenration);
            falling.onSwallowed.AddListener(RemoveSpawnedObject);
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
        BossObject bossprefab = generationList[CurrentGenration].Boss;
        if (bossprefab == null) return;

        Vector2 SpawnLocation = RandomSpawnRange();


        Vector3 spawnPosition = new Vector3(SpawnLocation.x, 0.0f, SpawnLocation.y);
        GameObject bossobj = Instantiate(bossprefab.gameObject, spawnPosition, Quaternion.identity);
        EarthObjectStatData bossstatdata = null;
        if (CurrentGenration < BossStatList.Count)
            bossstatdata = BossStatList[CurrentGenration];
        BossObject boss = bossobj.GetComponent<BossObject>();
        if (boss != null)
        {
            boss.SetStatData(bossstatdata);
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
            ChangeGeneration();

            //보스 시간 , 점수 추가
            FOnObjectSwallowed(bossObject);
            //임시 소리
            gameState.OnUfoSwallowSound();

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

