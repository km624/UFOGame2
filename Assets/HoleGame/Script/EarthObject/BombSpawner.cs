using UnityEngine;
using System.Collections;
public class BombSpawner : RandomSpawner
{

    private Coroutine BombCoroutine;

    protected override void StartSpawn()
    {
        StartBombSapwn();
    }

    protected override void StopSpawn()
    {
        StopBombSpawn();
    }

    protected override void SpawnAtRandomGridPosition()
    {
        //FallingObject spawnbomb = generationList[CurrentGenration].bomb;
        FallingObject spawnbomb = SpawnObject;
        if (spawnbomb == null) return;

        // Vector2 SpawnLocation = RandomSpawnRange();
        Vector3 SpawnLocation = GetValidSpawnPosition(false);


       
        GameObject bombobj = Instantiate(spawnbomb.gameObject, SpawnLocation, Quaternion.identity);
      
        FallingObject bombfall = bombobj.GetComponent<FallingObject>();
        if (bombfall != null)
        {
            bombfall.SetStatData(SpawnStatData);
            bombfall.SetBomb();
            bombfall.InitObject();
            bombfall.onSwallowed.AddListener(objectmanager.CallBack_RemoveSpawnedObject);
        }

    }

    private void StartBombSapwn()
    {

        BombCoroutine = StartCoroutine(SpawnRoutine());
    }

    private void StopBombSpawn()
    {
        StopCoroutine(BombCoroutine);
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            SpawnAtRandomGridPosition();
            yield return new WaitForSeconds(SpawnStatData.SpawnWeight);
        }
    }

 
}
