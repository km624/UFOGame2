using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;

public class MapChange : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer mRenderer;

    [SerializeField]
    private GameObject SpawnMapObject;
    
    public void ChangeMap(GenerationObjects currentgenerationdata)
    {
        mRenderer.material  = currentgenerationdata.MapMaterial;
    }

    private void CreateMapObject()
    {

    } 
}
