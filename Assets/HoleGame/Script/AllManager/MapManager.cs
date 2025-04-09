using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    public List<string> Maps  = new List<string>();
    
   

   
    void Update()
    {
        Debug.Log("업데이트");
    }
    public string GetMapsName(int level)
    {
        return Maps[level];
    }

 
}
