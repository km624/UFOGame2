using System.Collections.Generic;

using UnityEngine;
using System.Collections;

public class LimitObjectSpawnPoint : ObjectSpawnPoint
{
    [Header("���� ���� ����")]
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

            // �� �����ʰ� ������ ������Ʈ�� max �̻��̸� ���
            if (mySpawnedList.Count >= maxSpawnCount)
            {
                yield return WaitForSecondsWithPause(SpawnTime * 1.0f);
                CleanupDeadObjects(); // Ȥ�� null�� �� ������Ʈ ����
                continue;
            }

            if (currentGeneration != null)
            {
                int prefabnum = SelectPrefabByWeight(currentGeneration, objectManager.ObjectStatList);
                if (prefabnum < 0)
                {
                    Debug.Log("������ ����");
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

    public override void SpawnPrefab() // ��� �� �޼��� ������
    {
        if (SpawnCoroutine != null) return;
        SpawnCoroutine = StartCoroutine(LimitSpawnRoutine());
    }
}
