using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class ObjectManager : MonoBehaviour
{
    [SerializeField]
    private List<ObjectSpawnPoint> objectspawnPoints = new List<ObjectSpawnPoint>();

    [SerializeField]
    private List<GenerationObjects> generationList = new List<GenerationObjects>();

    //public int TotalObjectCnt { get; private set; } = 0;

    public int CurrentGenration { get; private set; } = 0;
    public List<FallingObject> AllSpawnObjects { get; private set; } = new List<FallingObject>();

    [SerializeField]
    private int MaxObjectCnt  = 50;

   
    public event Action<FallingObject,int /*currentgeneration*/ > FOnObjectSwallowed;
    public event Action<Vector3 /*아이콘 생성 위치*/, ShapeEnum> FOnBonusSwallowed;
    public event Action FOnBomnbSwallowed;
    

    private Dictionary<ShapeEnum, int> BonusObjectsOrgin = new Dictionary<ShapeEnum, int>();
    private Dictionary<ShapeEnum, int> BonusObjectsCnt = new Dictionary<ShapeEnum, int>();
    public event Action< ShapeEnum, int /*count*/> FOnBounsWidgetCreated;
    public event Action<Dictionary<ShapeEnum,int>/*BonusObjectsOrgin*/, int/*CurrentGenration*/> FOnBounusCompleted;
   

    public int GetMaxObjectCnt() { return MaxObjectCnt; }

    [SerializeField]
    private int bonusMaxCount = 5;
    [SerializeField]
    private int bonusMinCount = 1;

    [SerializeField]
    private int bonusObjectsMinCount =2;

    

    public void InitObjectManager()
    {
        CreateBonusObjects();
        SetUpSpawnObjects(CurrentGenration);
        StartSpawnObjects();
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
            Debug.Log("다음 세대 없음");
            return false;
        }

        CurrentGenration += 1;
        SetUpSpawnObjects(CurrentGenration);

        Debug.Log(CurrentGenration + " : 현재 세대");

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
        int maxSelectable = currentGen.objects.Count;
        int minSelectable = Mathf.Clamp(bonusObjectsMinCount, 1, maxSelectable);
        // 선택할 오브젝트의 개수: 최소 2개부터 maxSelectable 개 사이 (maxSelectable + 1은 Random.Range의 상한 미포함 특성 때문)
        int countToSelect = UnityEngine.Random.Range(minSelectable, maxSelectable + 1);

        Debug.Log("랜덤 : " + countToSelect);
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

        //보너스 목표 재생성
        CreateBonusObjects();
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

