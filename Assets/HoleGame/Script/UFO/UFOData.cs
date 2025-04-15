using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UFOData", menuName = "Scriptable Objects/UFOData")]
public class UFOData : ScriptableObject
{
    public string UFOName;
    public Mesh UFOMesh;
    public List<Material> UFOMaterials = new List<Material>();

    public Mesh AllStatUFOMesh;
    public Material AllStatUFOMaterial;
    public int MoveSpeed = 10;
    public float LiftSpeed = 10.0f;
    public float BeamRange = 1.0f;
   
}
