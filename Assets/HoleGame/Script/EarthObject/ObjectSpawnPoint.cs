using UnityEngine;
using System.Collections;
using System.Data.SqlTypes;
using System.Collections.Generic;
public class ObjectSpawnPoint : MonoBehaviour
{
    private Vector3 CurrentSpawnPoint;

    // �⺻ ���� ��� �ð�
    private float SpawnTime = 5.0f;
    // ���� ���ð��� ��variation ����
    private float variation = 1.0f;

    private GenerationObjects currentGeneration = null;
   
    private ObjectManager objectManager = null;

    private Coroutine SpawnCoroutine;

    // �� ������ true�̸� ���ð� ������ ��� ����ϴ�.
    private bool bIsStopSpawn = false;

    void Start()
    {
        CurrentSpawnPoint = transform.position;
    }

    public void SetGeneration(GenerationObjects generation, ObjectManager objectm ,float time , float vari)
    {
        currentGeneration = generation;

        objectManager = objectm;
        SpawnTime=time;
        if(SpawnTime - variation<=0)
        {
            variation = SpawnTime;
            SpawnTime += 1;
            
        }
        else
        {
            variation = vari;
        }
      
    }

    // SpawnPrefab�� ȣ��Ǹ�, �̹� ���� ���� �ƴϸ� ���� ��ƾ �ڷ�ƾ�� �����մϴ�.
    public void SpawnPrefab()
    {
        // �̹� �ڷ�ƾ�� �������̸� �ߺ� �������� ����
        if (SpawnCoroutine != null) return;

     
        // ���� ���� ���� ��ƾ �ڷ�ƾ�� �����Ͽ� �ֱ������� ����
        SpawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // ���� �ִ� ���� ������ �����ߴٸ�, SpawnTime�� ���� ��ŭ ��� (�Ͻ����� ����� �����)
            if (objectManager.AllSpawnObjects.Count >= objectManager.GetMaxObjectCnt())
            {
                yield return WaitForSecondsWithPause(SpawnTime * 0.5f);
                continue;
            }

 
            if (currentGeneration != null)
            {
               
                // ����ġ ������� FallingObject ������ ����
                int prefabnum = SelectPrefabByWeight(currentGeneration, objectManager.ObjectStatList);
                if (prefabnum < 0)
                {
                    Debug.Log("������ ����");
                    break;
                }
                FallingObject prefabToSpawn = currentGeneration.objects[prefabnum];
               
                //.EarthObjectStatData objectdata = GetEarthObjectStatData(objectManager.CurrentGenration, prefabnum);
                EarthObjectStatData objectdata = objectManager.ObjectStatList[prefabnum];
                if (prefabToSpawn != null)
                {
                    GameObject spawnObject = Instantiate(prefabToSpawn.gameObject, transform.position, transform.rotation);
                    FallingObject falling = spawnObject.GetComponent<FallingObject>();
                    if (falling != null)
                    {
                        objectManager.RegisterSpawnedObject(falling);
                        
                        falling.SetStatData(objectdata);

                        falling.AddGenerationMass(objectManager.CurrentGenration);
                        falling.onSwallowed.AddListener(objectManager.RemoveSpawnedObject);
                    }
                }
            }
            float delay = SpawnTime + Random.Range(-variation, variation);
            yield return WaitForSecondsWithPause(delay);
        }
    }

   

    // WaitForSecondsWithPause : ������ �ð�(waitTime) ���� ����ϵ�, bIsStopSpawn�� true�̸� ���� �ð��� �Ҹ����� �ʽ��ϴ�.
    private IEnumerator WaitForSecondsWithPause(float waitTime)
    {
        float remainingTime = waitTime;
        while (remainingTime > 0f)
        {
            // �Ͻ����� ���°� �ƴ϶�� ���� �ð� ����
            if (!bIsStopSpawn)
            {
                remainingTime -= Time.deltaTime;
            }
            yield return null;
        }
    }

    // ����ġ ������� currentGeneration ���� FallingObject �� �ϳ��� �������� ����
    private int SelectPrefabByWeight(GenerationObjects generation ,IReadOnlyList<EarthObjectStatData> earthObjectDatas)
    {
        //if (generation.objects.Count == 0 || generation.spawnWeights == null || generation.spawnWeights.Length != generation.objects.Count)
        if (generation.objects.Count == 0 || earthObjectDatas.Count -1 == generation.objects.Count)
        {
            Debug.LogWarning("Spawn weights count does not match objects count or objects list is empty.");
            return -1;
        }

        // �� ����ġ �ջ�
        float totalWeight = 0f;
        for (int i = 0; i < earthObjectDatas.Count; i++)
        {
            totalWeight += earthObjectDatas[i].SpawnWeight;
        }
       
        // 0���� totalWeight ������ ���� �� ����
        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;
        //Debug.Log("randomValue : " + randomValue);
        for (int i = 0; i < generation.objects.Count; i++)
        {
            cumulativeWeight += earthObjectDatas[i].SpawnWeight;
            if (randomValue <= cumulativeWeight)
            {
                //return generation.objects[i];
                return i;
            }
        }

        // Ȥ�� �̻� ��Ȳ�� �߻��ϸ� fallback
        //return generation.objects[generation.objects.Count - 1];
        return 0;
    }

   /* private EarthObjectStatData GetEarthObjectStatData(int generation , int num)
    {

       ExelEarthObjectData stattime = objectManager.ObjecstStatTime;
        
        if (stattime == null) return null;
      
        


        return objectManager.ObjectStatList[num];
    }
*/
    // �ܺο��� ������ ����(�Ͻ�����) �Ǵ� �簳�� �� �ֵ��� bIsStopSpawn�� ���¸� �����ϴ� �޼���
    public void SetPauseState(bool isPaused)
    {
        bIsStopSpawn = isPaused;
    }

    // ���� �ڷ�ƾ�� ������ ������Ű�� ���� �� ȣ���ϴ� �޼���
    public void StopSpawn()
    {
        if (SpawnCoroutine != null)
        {
            StopCoroutine(SpawnCoroutine);
            SpawnCoroutine = null;
        }
    }
}