using System;
using UnityEngine;

public class StarObject : FallingObject,IDetctable
{

    public Vector3 WorldPosition => transform.position;

    public event Action<StarObject> FOnStarSwallowed;
  
    public override void OnSwallow()
    {
        FOnStarSwallowed?.Invoke(this);
    }

    public void Update()
    {
        if(transform.position.y<-10.0f)
        {
            Destroy(gameObject);
        }
    }
   /* public void StarSwallow()
    {
        FOnStarSwallowed?.Invoke(this);
    }*/
}
