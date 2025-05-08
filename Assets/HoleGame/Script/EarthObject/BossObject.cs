using System;
using UnityEngine;

public class BossObject : FallingObject
{

    public event Action<BossObject> FOnBossSwallowed;
  

    public override void OnSwallow()
    {
        FOnBossSwallowed?.Invoke(this);
    }

   /* public void BossSwallow()
    {
        FOnBossSwallowed?.Invoke(this); 
    }*/
}
