
using UnityEngine;
using System.Collections.Generic;

public class MapChange : MonoBehaviour
{
    [SerializeField]
    private List<MeshRenderer> mRenderers = new List<MeshRenderer>();

    [SerializeField]
    private GameObject Map;
   
    private List<GameObject> SpawnMapObject =new List<GameObject>();
    
    public void ChangeMap(GenerationObjects currentgenerationdata)
    {
        foreach(var spawnobj in SpawnMapObject)
        {
            Destroy(spawnobj);  
        }

        foreach (var renderer in mRenderers)
        {
            renderer.material = currentgenerationdata.MapMaterial;
        }

        foreach (var mapobject in currentgenerationdata.MapObjects)
        {
            GameObject mapobj = Instantiate(mapobject, Map.transform);
        }
    }

   
}
