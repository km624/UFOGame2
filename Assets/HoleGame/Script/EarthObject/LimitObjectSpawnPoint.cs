using System.Collections.Generic;

using UnityEngine;
using System.Collections;

public class LimitObjectSpawnPoint : ObjectSpawnPoint
{
    [Header("개별 스폰 제한")]
    [SerializeField] int maxSpawnCount = 3;

    private List<FallingObject> mySpawnedList = new();

    private IEnumerator LimitSpawnRoutine()
    {
        while (true)
        {
            if (bIsStopSpawn)
            {
                yield return null;
                continue;
            }

            // 이 스포너가 생성한 오브젝트가 max 이상이면 대기
            if (mySpawnedList.Count >= maxSpawnCount)
            {
                yield return WaitForSecondsWithPause(SpawnTime * 1.0f);
                CleanupDeadObjects(); // 혹시 null이 된 오브젝트 정리
                continue;
            }

            if (currentGeneration != null)
            {
                int prefabnum = SelectPrefabByWeight(currentGeneration, objectManager.ObjectStatList);
                if (prefabnum < 0)
                {
                    Debug.Log("데이터 없음");
                    break;
                }

                FallingObject prefabToSpawn = currentGeneration.objects[prefabnum];
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

                        
                        mySpawnedList.Add(falling);

                        
                        falling.onSwallowed.AddListener(OnMyObjectRemoved);
                    }
                }
            }

            float delay = SpawnTime + Random.Range(-variation, variation);
            yield return WaitForSecondsWithPause(delay);
        }
    }

    private void OnMyObjectRemoved(FallingObject obj)
    {
        mySpawnedList.Remove(obj);
        objectManager.RemoveSpawnedObject(obj);
    }

    private void CleanupDeadObjects()
    {
        mySpawnedList.RemoveAll(item => item == null);
    }

    public override void SpawnPrefab() // 상속 시 메서드 재정의
    {
        if (SpawnCoroutine != null) return;
        SpawnCoroutine = StartCoroutine(LimitSpawnRoutine());
    }
}
