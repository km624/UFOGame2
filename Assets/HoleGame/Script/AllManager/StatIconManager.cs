using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
public class StatIconManager : MonoBehaviour
{
    [Header("������Ʈ ������")]
    [SerializeField]
    public List<StatIconData> statImageList = new List<StatIconData>();

    public Dictionary<UFOStatEnum, Sprite> statImageMap = new Dictionary<UFOStatEnum, Sprite>();

    public Sprite DefaultIcon;

    public static StatIconManager Instance { get; private set; }

    void Awake()
    {
        // �̱��� �ν��Ͻ� ����
        if (Instance == null)
        {
            Instance = this;
        }

        UpdateShapeMap();
    }

    private void UpdateShapeMap()
    {
        statImageMap.Clear();
        foreach (var pair in statImageList)
        {
            Sprite icon = pair.staticon != null ? pair.staticon : DefaultIcon;
            if (!statImageMap.ContainsKey(pair.Stattype))
            {
                statImageMap.Add(pair.Stattype, icon);
            }
        }
    }

    public Sprite GetStatSprite(UFOStatEnum statenum)
    {
        return statImageMap.TryGetValue(statenum, out Sprite sprite) ? sprite : DefaultIcon;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        var enumValues = System.Enum.GetValues(typeof(UFOStatEnum)).Cast<UFOStatEnum>().ToList();

        foreach (var enumValue in enumValues)
        {
            if (!statImageList.Any(d => d.Stattype == enumValue))
            {
                statImageList.Add(new StatIconData
                {
                    Stattype = enumValue,
                    staticon = DefaultIcon
                });
            }
        }

        // �ߺ� ���� (����ũ�ϰ� ����)
        statImageList = statImageList
            .GroupBy(d => d.Stattype)
            .Select(g => g.First())
            .ToList();

        EditorUtility.SetDirty(this); // �ν����� ����
    }
#endif
}
