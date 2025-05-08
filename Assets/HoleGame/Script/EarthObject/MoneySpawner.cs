using UnityEngine;

public class MoneySpawner : RandomSpawner
{
    protected override void StartSpawn()
    {
        SpawnAtRandomGridPosition();
    }

    protected override void StopSpawn()
    {
        Debug.Log("Money ½ºÆù X");
    }

    protected override void SpawnAtRandomGridPosition()
    {
      

        Vector3 spawnPosition = GetValidSpawnPosition(false) + Vector3.up * 5.0f;
        GameObject starobj = Instantiate(SpawnObject.gameObject, spawnPosition, Quaternion.identity);

        StarObject star = starobj.GetComponent<StarObject>();
        if (star != null)
        {
            star.InitObject();
            star.FOnStarSwallowed += objectmanager.CallBack_StartSwallowed;
            objectmanager.Callback_StarSpawned(star);


        }
    }
    


}
