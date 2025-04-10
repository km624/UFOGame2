using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Xml.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class ObjectManager : MonoBehaviour
{
    [SerializeField]
    private List<ObjectSpawnPoint> objectspawnPoints = new List<ObjectSpawnPoint>();

    [SerializeField]
    private List<GenerationObjects> generationList = new List<GenerationObjects>();

    //public int TotalObjectCnt { get; private set; } = 0;

    public int CurrentGenration { get; private set; } = 0;
    public List<FallingObject> AllSpawnObjects { get; private set; } = new List<FallingObject>();

    public TMP_Text GenereationText;
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
    [SerializeField]
    private int bonusObjectsMinCount = 2;
    [SerializeField]
    private int bonusObjectsMaxCount = 4;

    [SerializeField]
    private int bonusMinCount = 1;
    [SerializeField]
    private int bonusMaxCount = 5;
    

   

    

    public void InitObjectManager()
    {
        CreateBonusObjects();
        SetUpSpawnObjects(CurrentGenration);
        StartSpawnObjects();
        StartBombSapwn();
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
        foreach (var spawnPoint in objectspawnPoints)
        {
            if (gen != null)
            {
                spawnPoint.SetGeneration(gen, this);
                spawnPoint.SpawnPrefab();
            }
            
        }
    }

    public bool ChangeGeneration()
    {
        if (generationList.Count <= CurrentGenration + 1)
        {
            //Debug.Log("���� ���� ����");
            return false;
        }

        CurrentGenration += 1;
        GenereationText.text= CurrentGenration.ToString();  
        SetUpSpawnObjects(CurrentGenration);

       // Debug.Log(CurrentGenration + " : ���� ����");

        return true;

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
            SpawnAtRandomGridPosition();
            yield return new WaitForSeconds(BombSpawnTime);
        }
    }

    private void SpawnAtRandomGridPosition()
    {
        FallingObject spawnbomb = generationList[CurrentGenration].bomb;
    

        Vector2 SpawnLocation = RandomSpawnRange();

       
        Vector3 spawnPosition = new Vector3(SpawnLocation.x, BombSpawnHeight, SpawnLocation.y);
        GameObject bombobj = Instantiate(spawnbomb.gameObject, spawnPosition, Quaternion.identity);
       
        FallingObject falling = bombobj.GetComponent<FallingObject>();
        if (falling != null)
        {
           
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

