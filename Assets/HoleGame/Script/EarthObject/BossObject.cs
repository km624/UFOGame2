using System;
using UnityEngine;

public class BossObject : FallingObject
{

    public event Action<BossObject> FOnBossSwallowed;
    public override void AddGenerationMass(int generation)
    {
        generation++;
        base.AddGenerationMass(generation);

        Debug.Log("Boss "+ ObjectMass);
    }

    public void BossSwallow()
    {
        FOnBossSwallowed?.Invoke(this); 
    }
}
