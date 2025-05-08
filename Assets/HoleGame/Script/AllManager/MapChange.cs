
using UnityEngine;
using System.Collections.Generic;

public class MapChange : MonoBehaviour
{
    [SerializeField]
    private List<MeshRenderer> mRenderers = new List<MeshRenderer>();
    
   
    private List<GameObject> SpawnMapObject;
    
    public void ChangeMap(GenerationObjects currentgenerationdata)
    {
        foreach (var renderer in mRenderers)
        {
            renderer.material = currentgenerationdata.MapMaterial;
        }
    }

    private void CreateMapObject()
    {

    } 
}
