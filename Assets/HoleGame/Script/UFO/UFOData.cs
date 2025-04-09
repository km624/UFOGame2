using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UFOData", menuName = "Scriptable Objects/UFOData")]
public class UFOData : ScriptableObject
{
    public string UFOName;
    public Mesh UFOMesh;
    public List<Material> UFOMaterials = new List<Material>();
    public int MoveSpeed;
    public float LiftSpeed;

    //º¸·ù
    //public float BeamRange;
   
}
