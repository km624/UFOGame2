using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UFOColorData
{
    public Color32 IConColor =  new Color32(255, 255, 255, 255);
    public List<Material> Materials = new(); 
    public int ColorPrice = 0;
}