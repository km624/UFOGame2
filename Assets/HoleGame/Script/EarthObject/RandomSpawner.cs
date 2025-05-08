
using UnityEngine;
using System;
using Random = UnityEngine.Random;



public abstract class RandomSpawner : MonoBehaviour
{

    protected ObjectManager objectmanager;

    protected EarthObjectStatData SpawnStatData;
    protected FallingObject SpawnObject;

    protected Renderer SpawnRange;
   
    protected Renderer SafeSpawnRange;

    Bounds RangeBounds;

    [SerializeField]
    protected int PostionInterval = 1;

    [SerializeField] protected LayerMask groundMask;
    [SerializeField] protected LayerMask wallMask;
    [SerializeField] protected LayerMask IgnoreMask;

    //public event Action<FallingObject> FOnSpawned;
    public void Initialize( ObjectManager manager, Renderer spawnRange, Renderer safeSpawnRange)
    {
        objectmanager = manager;
        SpawnRange =spawnRange;
        SafeSpawnRange=safeSpawnRange;

    }

    public void SetSpawnStatData(FallingObject spawnObject, EarthObjectStatData statdata)
    {
        SpawnStatData = statdata;
        SpawnObject = spawnObject;
    }
    

    private Vector2 RandomSpawnRange(bool bsafearea)
    {
        if (!bsafearea)
            RangeBounds = SpawnRange.bounds;
        else
            RangeBounds = SafeSpawnRange.bounds;
        // Plane의 Bounds에서 x, z 좌표의 최소/최대값을 정수 단위로 구합니다.
        int minX = Mathf.CeilToInt(RangeBounds.min.x);
        int maxX = Mathf.FloorToInt(RangeBounds.max.x);
        int minZ = Mathf.CeilToInt(RangeBounds.min.z);
        int maxZ = Mathf.FloorToInt(RangeBounds.max.z);

        // 랜덤하게 정수 좌표 생성 (max는 포함시키기 위해 +1)
        int randomX = Random.Range(minX, maxX + PostionInterval);
        int randomZ = Random.Range(minZ, maxZ + PostionInterval);

        return new Vector2(randomX, randomZ);
    }


    protected Vector3 GetValidSpawnPosition(bool bsafearea)
    {
        int raycastMask = ~IgnoreMask; // ignoreMask에 해당하는 레이어만 제외하고 나머지는 전부 감지

        for (int i = 0; i < 50; i++)
        {
            Vector2 randXZ = RandomSpawnRange(bsafearea);
            Vector3 origin = new Vector3(randXZ.x, 100f, randXZ.y);
            Vector3 direction = Vector3.down;
            float rayLength = 200f;

            Debug.DrawRay(origin, direction * rayLength, Color.red, 1.0f);

            if (Physics.Raycast(origin, direction, out RaycastHit hit, rayLength, raycastMask))
            {
                GameObject hitObj = hit.collider.gameObject;

                if (((1 << hitObj.layer) & wallMask) != 0)
                {
                    //Debug.Log($"[Spawn] Ray hit WALL: {hitObj.name}");
                    continue;
                }

                if (((1 << hitObj.layer) & groundMask) != 0)
                {
                    // Debug.Log($"[Spawn] Ray hit valid GROUND: {hitObj.name}");
                    return hit.point + Vector3.up * 0.5f;
                }

                // Debug.Log($"[Spawn] Ray hit something else: {hitObj.name} (Layer: {LayerMask.LayerToName(hitObj.layer)})");
            }
            else
            {
                Vector2 safearea = RandomSpawnRange(true);
                Vector3 newSafeArea = new Vector3(safearea.x, 0.5f, safearea.y);
                Debug.Log("[Spawn] Ray missed everything. safe " + safearea);
                return newSafeArea;
            }
        }

        //Debug.LogWarning("[Spawn] Failed to find valid spawn point. Returning fallback position.");
        return transform.position;
    }


    public void OnRandomSpawn(bool active)
    {
        if (active)
        {
            StartSpawn();
        }
        else
        {
            StopSpawn();
        }
    }

    protected abstract void StartSpawn();
    
    protected abstract void StopSpawn();

    protected abstract void SpawnAtRandomGridPosition();


  
}
