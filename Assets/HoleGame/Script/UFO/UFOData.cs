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

    public int BaseMoveSpeed = 1;
    public int BaseLiftSpeed = 1;
    public int BaseBeamRange = 1;

    public int MaxMoveSpeed = 1;
    public int MaxLiftSpeed = 1;
    public int MaxBeamRange = 1;
   
}
