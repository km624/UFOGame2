using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
public class SkillIconManager : MonoBehaviour
{
    [Header("������Ʈ ������")]
    [SerializeField]
    public List<SkillIconData> SkillImageList = new List<SkillIconData>();

    public Dictionary<SkillEnum, Sprite> SkillImageMap = new Dictionary<SkillEnum, Sprite>();

    public Sprite DefaultIcon;

    public static SkillIconManager Instance { get; private set; }

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
        SkillImageMap.Clear();
        foreach (var pair in SkillImageList)
        {
            Sprite icon = pair.Skillicon != null ? pair.Skillicon : DefaultIcon;
            if (!SkillImageMap.ContainsKey(pair.Skilltype))
            {
                SkillImageMap.Add(pair.Skilltype, icon);
            }
        }
    }

    public Sprite GetSkillIconSprite(SkillEnum skillenum)
    {
        return SkillImageMap.TryGetValue(skillenum, out Sprite sprite) ? sprite : DefaultIcon;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        var enumValues = System.Enum.GetValues(typeof(SkillEnum)).Cast<SkillEnum>().ToList();

        foreach (var enumValue in enumValues)
        {
            if (!SkillImageList.Any(d => d.Skilltype == enumValue))
            {
                SkillImageList.Add(new SkillIconData
                {
                    Skilltype = enumValue,
                    Skillicon = DefaultIcon
                });
            }
        }

        // �ߺ� ���� (����ũ�ϰ� ����)
        SkillImageList = SkillImageList
            .GroupBy(d => d.Skilltype)
            .Select(g => g.First())
            .ToList();

        EditorUtility.SetDirty(this); // �ν����� ����
    }
#endif
}
