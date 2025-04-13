using System;
using UnityEngine;

public class StarObject : FallingObject,IDetctable
{

    public Vector3 WorldPosition => transform.position;

    public event Action<StarObject> FOnStarSwallowed;
    public override void AddGenerationMass(int generation)
    {
        generation = 0;
        base.AddGenerationMass(generation);
        
        //Debug.Log("Boss "+ ObjectMass);
    }

    public void StarSwallow()
    {
        FOnStarSwallowed?.Invoke(this);
    }
}
