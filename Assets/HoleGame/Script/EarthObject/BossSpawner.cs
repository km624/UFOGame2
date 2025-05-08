using UnityEngine;

public class BossSpawner : RandomSpawner
{
    protected override void StartSpawn()
    {
        SpawnAtRandomGridPosition();
    }

    protected override void StopSpawn()
    {
        Debug.Log("보스 스폰 중단 기능 구현 X");
    }

    protected override void SpawnAtRandomGridPosition()
    {
        //BossObject bossprefab = generationList[CurrentGenration].Boss;
        FallingObject bossprefab = SpawnObject;
        if (bossprefab == null) return;

        Vector3 spawnPosition = GetValidSpawnPosition(true);
        GameObject bossobj = Instantiate(bossprefab.gameObject, spawnPosition, Quaternion.identity);


        BossObject boss = bossobj.GetComponent<BossObject>();
        if (boss != null)
        {
            boss.SetStatData(SpawnStatData);
            boss.InitObject();
            boss.FOnBossSwallowed += objectmanager.CallBacK_BossSwallow;
            objectmanager.Callback_BossSpawn(boss);

        }
    }

   
}
