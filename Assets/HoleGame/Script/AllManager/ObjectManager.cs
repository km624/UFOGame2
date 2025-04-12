using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Xml.Linq;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class ObjectManager : MonoBehaviour
{
    private GameState gameState;

    [SerializeField]
    private List<ObjectSpawnPoint> objectspawnPoints = new List<ObjectSpawnPoint>();

    [SerializeField]
    private List<GenerationObjects> generationList = new List<GenerationObjects>();

    //public int TotalObjectCnt { get; private set; } = 0;

    public int CurrentGenration { get; private set; } = 0;

    public event Action<GenerationObjects> FOnGenerationDataSeted;
    public event Action<int /*currentgeneraetion*/> FOnGenerationChanged;
    public List<FallingObject> AllSpawnObjects { get; private set; } = new List<FallingObject>();

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
    [SerializeField]
    float BombSpawnTime = 5.0f;

    [Header("�����ð�")]
    [SerializeField]
    private float SpawnTime = 5.0f;
    [SerializeField]
    private float variation = 1.0f;

    private Coroutine BombCoroutine;

    public event Action<FallingObject,int /*currentgeneration*/ > FOnObjectSwallowed;
    public event Action<Vector3 /*������ ���� ��ġ*/, ShapeEnum> FOnBonusSwallowed;
    public event Action FOnBomnbSwallowed;
    

    private Dictionary<ShapeEnum, int> BonusObjectsOrgin = new Dictionary<ShapeEnum, int>();
    private Dictionary<ShapeEnum, int> BonusObjectsCnt = new Dictionary<ShapeEnum, int>();
    public event Action< ShapeEnum, int /*count*/> FOnBounsWidgetCreated;
    public event Action<Dictionary<ShapeEnum,int>/*BonusObjectsOrgin*/, int/*CurrentGenration*/> FOnBounusCompleted;

    
    public int GetMaxObjectCnt() { return MaxObjectCnt; }
    
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
        //CreateBonusObjects();
        CreateBonusObjects();
        SetUpSpawnObjects(CurrentGenration);
        StartSpawnObjects();
        StartBombSapwn();
        SpawnBossAtRandomGridPosition();
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


    public void SetUpSpawnObjects(int currentgeneration)
    {
        GenerationObjects gen = (generationList.Count > 0) ? generationList[currentgeneration] : null;
        FOnGenerationDataSeted?.Invoke(gen);
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
            //Debug.Log("���� ���� ����");
            return; 
        }

        CurrentGenration += 1;

        SetUpSpawnObjects(CurrentGenration);

        FOnGenerationChanged?.Invoke(CurrentGenration);
        Debug.Log(CurrentGenration + " : ���� ����");

       

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
                FOnBomnbSwallowed?.Invoke();
            }
            else
            {
                if(BonusObjectsCnt.ContainsKey(destroyShape))
                {
                    CheckBonusCleared(destroyShape);
                    Vector3 position = Vector3.zero;
                    if (destroyoBj.bIsAttacked)
                    {
                        position = GameState.Instance.GetPlayerPostion();
                    }
                    else
                    {
                        position = destroyoBj.gameObject.transform.position;
                    }

                    FOnBonusSwallowed?.Invoke(position, destroyoBj.GetShapeEnum());
                }


                FOnObjectSwallowed?.Invoke(destroyoBj, CurrentGenration);
            }
         
        }
        if(AllSpawnObjects.Contains(destroyoBj))
            AllSpawnObjects.Remove(destroyoBj);

       
        Destroy(destroyoBj.gameObject);

    }

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

    private void StartBombSapwn()
    {
        
        BombCoroutine = StartCoroutine(SpawnRoutine());
    }

    public void StopBombSpawn()
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

    void SpawnBossAtRandomGridPosition()
    {
        BossObject bossprefab = generationList[CurrentGenration].Boss;
        if (bossprefab == null) return;

        Vector2 SpawnLocation = RandomSpawnRange();


        Vector3 spawnPosition = new Vector3(SpawnLocation.x, 0.0f, SpawnLocation.y);
        GameObject bossobj = Instantiate(bossprefab.gameObject, spawnPosition, Quaternion.identity);

        BossObject boss = bossobj.GetComponent<BossObject>();
        if (boss != null)
        {
            boss.AddGenerationMass(CurrentGenration);
            boss.FOnBossSwallowed += CallBacK_BossSwallow;
           
        }
    }

    void CallBacK_BossSwallow(BossObject bossObject)
    {
        if(bossObject!=null)
        {
            ChangeGeneration();
            Destroy(bossObject.gameObject);
        }
    }

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



    public void AllObjectStopActive(bool active)
    {

        foreach (var fallingobject in AllSpawnObjects)
        {

            fallingobject.ActivateBounce(!active);

        }
    }

    public void IceActive(bool active)
    {
        foreach (var fallingobject in AllSpawnObjects)
        {

            fallingobject.ActiveIce(active);

        }
    }


}

