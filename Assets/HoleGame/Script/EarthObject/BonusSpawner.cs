using UnityEngine;

public class BonusSpawner : RandomSpawner
{
    protected override void StartSpawn()
    {
        SpawnAtRandomGridPosition();
    }

    protected override void StopSpawn()
    {
        Debug.Log("���ʽ� ���� X");
    }

    protected override void SpawnAtRandomGridPosition()
    {
       
        FallingObject spawnbonus = SpawnObject;
        if (spawnbonus == null) return;


        Vector3 SpawnLocation = GetValidSpawnPosition(false);

        GameObject bonusobj = Instantiate(spawnbonus.gameObject, SpawnLocation, Quaternion.identity);
      
        FallingObject bonusfall = bonusobj.GetComponent<FallingObject>();
        if (bonusfall != null)
        {
            bonusfall.SetStatData(SpawnStatData);

            bonusfall.InitObject();
            bonusfall.onSwallowed.AddListener(objectmanager.CallBack_RemoveSpawnedObject);
        }
    }





  

}
