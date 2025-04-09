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
    public event Action<Vector3 /*������ ���� ��ġ*/, ShapeEnum> FOnBonusSwallowed;
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
            Debug.Log("���� ���� ����");
            return false;
        }

        CurrentGenration += 1;
        SetUpSpawnObjects(CurrentGenration);

        Debug.Log(CurrentGenration + " : ���� ����");

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
        int maxSelectable = currentGen.objects.Count;
        int minSelectable = Mathf.Clamp(bonusObjectsMinCount, 1, maxSelectable);
        // ������ ������Ʈ�� ����: �ּ� 2������ maxSelectable �� ���� (maxSelectable + 1�� Random.Range�� ���� ������ Ư�� ����)
        int countToSelect = UnityEngine.Random.Range(minSelectable, maxSelectable + 1);

        Debug.Log("���� : " + countToSelect);
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

