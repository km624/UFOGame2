
using UnityEngine;
using System.Collections.Generic;

public class MapChange : MonoBehaviour
{
    [SerializeField]
    private List<MeshRenderer> mRenderers = new List<MeshRenderer>();

    [SerializeField]
    private GameObject Map;
   
    private List<MapObject> SpawnMapObject =new List<MapObject>();
    
    public void ChangeMap(GenerationObjects currentgenerationdata)
    {
        foreach(var spawnobj in SpawnMapObject)
        {
            spawnobj.DestroyMapObject();
        }
        SpawnMapObject.Clear();

        foreach (var renderer in mRenderers)
        {
            renderer.material = currentgenerationdata.MapMaterial;
        }

        foreach (var mapobject in currentgenerationdata.MapObjects)
        {
            MapObject mapobj = Instantiate(mapobject, Map.transform);
            mapobj.MapObjectSpawn();
            SpawnMapObject.Add(mapobj);
        }
    }

   
}
