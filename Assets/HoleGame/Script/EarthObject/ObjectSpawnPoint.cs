using UnityEngine;
using System.Collections;
using System.Data.SqlTypes;
public class ObjectSpawnPoint : MonoBehaviour
{
    private Vector3 CurrentSpawnPoint;

    // 기본 스폰 대기 시간
    private float SpawnTime = 5.0f;
    // 스폰 대기시간의 ±variation 범위
    private float variation = 1.0f;

    private GenerationObjects currentGeneration = null;
    private ObjectManager objectManager = null;

    private Coroutine SpawnCoroutine;

    // 이 변수가 true이면 대기시간 진행을 잠시 멈춥니다.
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

    // SpawnPrefab이 호출되면, 이미 실행 중이 아니면 스폰 루틴 코루틴을 시작합니다.
    public void SpawnPrefab()
    {
        // 이미 코루틴이 실행중이면 중복 실행하지 않음
        if (SpawnCoroutine != null) return;

        // 먼저 즉시 한 번 스폰 (최대 스폰 개수 조건도 체크)
        if (objectManager.AllSpawnObjects.Count < objectManager.GetMaxObjectCnt())
        {
            if (currentGeneration != null)
            {
                // 가중치 기반으로 FallingObject 프리팹 선택
                FallingObject prefabToSpawn = SelectPrefabByWeight(currentGeneration);
                if (prefabToSpawn != null)
                {
                    GameObject spawnObject = Instantiate(prefabToSpawn.gameObject, transform.position, transform.rotation);
                    FallingObject falling = spawnObject.GetComponent<FallingObject>();
                    if (falling != null)
                    {
                        objectManager.RegisterSpawnedObject(falling);
                        // onDestroyed 이벤트가 실행되면 ObjectManager의 RemoveSpawnedObject가 호출되도록 등록
                        falling.onSwallowed.AddListener(objectManager.RemoveSpawnedObject);
                    }
                }
            }
        }

        // 이후 기존 스폰 루틴 코루틴을 시작하여 주기적으로 스폰
        SpawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // 만약 최대 스폰 개수에 도달했다면, SpawnTime의 절반 만큼 대기 (일시정지 기능이 적용됨)
            if (objectManager.AllSpawnObjects.Count >= objectManager.GetMaxObjectCnt())
            {
                yield return WaitForSecondsWithPause(SpawnTime * 0.5f);
                continue;
            }

            // SpawnTime ± variation 범위 내의 랜덤 지연시간을 계산
            float delay = SpawnTime + Random.Range(-variation, variation);
            yield return WaitForSecondsWithPause(delay);

            if (currentGeneration != null)
            {
                // 가중치 기반으로 FallingObject 프리팹 선택
                FallingObject prefabToSpawn = SelectPrefabByWeight(currentGeneration);
                if (prefabToSpawn != null)
                {
                    GameObject spawnObject = Instantiate(prefabToSpawn.gameObject, transform.position, transform.rotation);
                    FallingObject falling = spawnObject.GetComponent<FallingObject>();
                    if (falling != null)
                    {
                        objectManager.RegisterSpawnedObject(falling);
                        // onDestroyed 이벤트가 실행되면 ObjectManager의 RemoveSpawnedObject가 호출되도록 등록
                        falling.onSwallowed.AddListener(objectManager.RemoveSpawnedObject);
                    }
                }
            }
        }
    }

    // WaitForSecondsWithPause : 지정한 시간(waitTime) 동안 대기하되, bIsStopSpawn가 true이면 남은 시간을 소모하지 않습니다.
    private IEnumerator WaitForSecondsWithPause(float waitTime)
    {
        float remainingTime = waitTime;
        while (remainingTime > 0f)
        {
            // 일시정지 상태가 아니라면 남은 시간 감소
            if (!bIsStopSpawn)
            {
                remainingTime -= Time.deltaTime;
            }
            yield return null;
        }
    }

    // 가중치 기반으로 currentGeneration 내의 FallingObject 중 하나를 랜덤으로 선택
    private FallingObject SelectPrefabByWeight(GenerationObjects generation)
    {
        if (generation.objects.Count == 0 || generation.spawnWeights == null || generation.spawnWeights.Length != generation.objects.Count)
        {
            Debug.LogWarning("Spawn weights count does not match objects count or objects list is empty.");
            return null;
        }

        // 총 가중치 합산
        float totalWeight = 0f;
        for (int i = 0; i < generation.spawnWeights.Length; i++)
        {
            totalWeight += generation.spawnWeights[i];
        }

        // 0부터 totalWeight 사이의 랜덤 값 선택
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

        // 혹시 이상 상황이 발생하면 fallback
        return generation.objects[generation.objects.Count - 1];
    }

    // 외부에서 스폰을 중지(일시정지) 또는 재개할 수 있도록 bIsStopSpawn의 상태를 설정하는 메서드
    public void SetPauseState(bool isPaused)
    {
        bIsStopSpawn = isPaused;
    }

    // 스폰 코루틴을 완전히 정지시키고 싶을 때 호출하는 메서드
    public void StopSpawn()
    {
        if (SpawnCoroutine != null)
        {
            StopCoroutine(SpawnCoroutine);
            SpawnCoroutine = null;
        }
    }
}