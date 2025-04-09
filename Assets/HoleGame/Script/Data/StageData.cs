using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "StageData", menuName = "Scriptable Objects/StageData")]
public class StageData : ScriptableObject, ISerializationCallbackReceiver
{
   

    public int TimeMin = 1;
   
    public int TimeSecond = 30;

    public int star1 = 30;
    public int star2 = 40;
    public int star3 = 50;

    public Dictionary<ShapeEnum, int> RequiredShapeCnt =new Dictionary<ShapeEnum, int>();
  
    public AssetReferenceGameObject stagePrefab;

    [SerializeField]
    private List<ShapeEnum> _keys = new List<ShapeEnum>();

    [SerializeField]
    private List<int> _values = new List<int>();
   
    /// <summary>
    /// "에디터 / 런타임"에서 ScriptableObject를 파일로 저장할 때
    /// (Dictionary → List) 변환하여 직렬화
    /// </summary>
    public void OnBeforeSerialize()
    {
        _keys.Clear();
        _values.Clear();

        foreach (var kvp in RequiredShapeCnt)
        {
            _keys.Add(kvp.Key);
            _values.Add(kvp.Value);
        }
    }

    /// <summary>
    /// 파일에서 ScriptableObject를 읽어올 때
    /// (List → Dictionary) 변환하여 실제 런타임 Dictionary 복원
    /// </summary>
    public void OnAfterDeserialize()
    {
        RequiredShapeCnt.Clear();

        for (int i = 0; i < _keys.Count; i++)
        {
            if (!RequiredShapeCnt.ContainsKey(_keys[i]))
            {
                RequiredShapeCnt.Add(_keys[i], _values[i]);
            }
        }
    }

}
