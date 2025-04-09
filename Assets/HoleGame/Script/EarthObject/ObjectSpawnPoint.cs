using UnityEngine;
using System.Collections;
using System.Data.SqlTypes;
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

    public void SetGeneration(GenerationObjects generation, ObjectManager objectm)
    {
        currentGeneration = generation;
        objectManager = objectm;
    }

    // SpawnPrefab�� ȣ��Ǹ�, �̹� ���� ���� �ƴϸ� ���� ��ƾ �ڷ�ƾ�� �����մϴ�.
    public void SpawnPrefab()
    {
        // �̹� �ڷ�ƾ�� �������̸� �ߺ� �������� ����
        if (SpawnCoroutine != null) return;

        // ���� ��� �� �� ���� (�ִ� ���� ���� ���ǵ� üũ)
        if (objectManager.AllSpawnObjects.Count < objectManager.GetMaxObjectCnt())
        {
            if (currentGeneration != null)
            {
                // ����ġ ������� FallingObject ������ ����
                FallingObject prefabToSpawn = SelectPrefabByWeight(currentGeneration);
                if (prefabToSpawn != null)
                {
                    GameObject spawnObject = Instantiate(prefabToSpawn.gameObject, transform.position, transform.rotation);
                    FallingObject falling = spawnObject.GetComponent<FallingObject>();
                    if (falling != null)
                    {
                        objectManager.RegisterSpawnedObject(falling);
                        // onDestroyed �̺�Ʈ�� ����Ǹ� ObjectManager�� RemoveSpawnedObject�� ȣ��ǵ��� ���
                        falling.onSwallowed.AddListener(objectManager.RemoveSpawnedObject);
                    }
                }
            }
        }

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

            // SpawnTime �� variation ���� ���� ���� �����ð��� ���
            float delay = SpawnTime + Random.Range(-variation, variation);
            yield return WaitForSecondsWithPause(delay);

            if (currentGeneration != null)
            {
                // ����ġ ������� FallingObject ������ ����
                FallingObject prefabToSpawn = SelectPrefabByWeight(currentGeneration);
                if (prefabToSpawn != null)
                {
                    GameObject spawnObject = Instantiate(prefabToSpawn.gameObject, transform.position, transform.rotation);
                    FallingObject falling = spawnObject.GetComponent<FallingObject>();
                    if (falling != null)
                    {
                        objectManager.RegisterSpawnedObject(falling);
                        // onDestroyed �̺�Ʈ�� ����Ǹ� ObjectManager�� RemoveSpawnedObject�� ȣ��ǵ��� ���
                        falling.onSwallowed.AddListener(objectManager.RemoveSpawnedObject);
                    }
                }
            }
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
    private FallingObject SelectPrefabByWeight(GenerationObjects generation)
    {
        if (generation.objects.Count == 0 || generation.spawnWeights == null || generation.spawnWeights.Length != generation.objects.Count)
        {
            Debug.LogWarning("Spawn weights count does not match objects count or objects list is empty.");
            return null;
        }

        // �� ����ġ �ջ�
        float totalWeight = 0f;
        for (int i = 0; i < generation.spawnWeights.Length; i++)
        {
            totalWeight += generation.spawnWeights[i];
        }

        // 0���� totalWeight ������ ���� �� ����
        float randomValue = Random.Range(0f, totalWeight);
        float cumulativeWeight = 0f;

        for (int i = 0; i < generation.objects.Count; i++)
        {
            cumulativeWeight += generation.spawnWeights[i];
            if (randomValue <= cumulativeWeight)
            {
                return generation.objects[i];
            }
        }

        // Ȥ�� �̻� ��Ȳ�� �߻��ϸ� fallback
        return generation.objects[generation.objects.Count - 1];
    }

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