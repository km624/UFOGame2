using NUnit.Framework;
using System;
using UnityEngine;
using System.Collections.Generic;

public class MapManager : MonoBehaviour
{
    public List<string> Maps  = new List<string>();
    
   

   
    void Update()
    {
        Debug.Log("������Ʈ");
    }
    public string GetMapsName(int level)
    {
        return Maps[level];
    }

 
}
