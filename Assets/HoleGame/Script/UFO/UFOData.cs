using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UFOData", menuName = "Scriptable Objects/UFOData")]
public class UFOData : ScriptableObject
{
    public string UFOName;
    public Mesh UFOMesh;
    public List<UFOColorData> UFOColorDataList = new List<UFOColorData>();

    public Mesh AllStatUFOMesh;
    public Material AllStatUFOMaterial;

    public int UFOPrice;

    public List<UFOStatData> StatList = new();

}
