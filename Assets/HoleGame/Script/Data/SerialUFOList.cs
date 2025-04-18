using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerialUFOList
{
    [SerializeField] private List<UserUFOData> ufoList = new(); // 직렬화 + 저장용
    private Dictionary<string, UserUFOData> ufoMap = new();     // 런타임용 빠른 조회

    public IReadOnlyList<UserUFOData> UFOList => ufoList;
    public IReadOnlyDictionary<string, UserUFOData> UFOMap => ufoMap;

    public void InitializeFromUFOList()
    {
        ufoMap.Clear();
        foreach (var ufo in ufoList)
        {
            if (!string.IsNullOrEmpty(ufo.UFOName))
                ufoMap[ufo.UFOName] = ufo;
        }
    }

    public void AddUFO(UserUFOData newUFO)
    {
        if (!ufoMap.ContainsKey(newUFO.UFOName))
        {
            ufoList.Add(newUFO);
            ufoMap[newUFO.UFOName] = newUFO;
            Debug.Log(newUFO.UFOName + " 추가");
        }
    }

    public UserUFOData Get(string ufoName)
    {
        return ufoMap.TryGetValue(ufoName, out var data) ? data : null;
    }

    public bool Contains(string ufoName)
    {
        return ufoMap.ContainsKey(ufoName);
    }

    public void Remove(string ufoName)
    {
        if (ufoMap.TryGetValue(ufoName, out var data))
        {
            ufoMap.Remove(ufoName);
            ufoList.Remove(data);
        }
    }
}
