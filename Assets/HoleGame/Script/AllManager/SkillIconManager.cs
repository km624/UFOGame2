using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
public class SkillIconManager : MonoBehaviour
{
    [Header("오브젝트 아이콘")]
    [SerializeField]
    public List<SkillIconData> SkillImageList = new List<SkillIconData>();

    public Dictionary<SkillEnum, Sprite> SkillImageMap = new Dictionary<SkillEnum, Sprite>();

    public Sprite DefaultIcon;

    public static SkillIconManager Instance { get; private set; }

    void Awake()
    {
        // 싱글톤 인스턴스 설정
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

        // 중복 제거 (유니크하게 유지)
        SkillImageList = SkillImageList
            .GroupBy(d => d.Skilltype)
            .Select(g => g.First())
            .ToList();

        EditorUtility.SetDirty(this); // 인스펙터 갱신
    }
#endif
}
