using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "UFOAllData", menuName = "Scriptable Objects/UFOAllData")]
public class UFOAllData : ScriptableObject
{
    public List<AssetReference> UFOAllDataList;
}
