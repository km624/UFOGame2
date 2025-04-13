using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
public class ShapeManager : MonoBehaviour
{
   
    [Header("오브젝트 아이콘")]
    [SerializeField]
    public List<ShapeIconData> shapeImageList = new List<ShapeIconData>();

    public Dictionary<ShapeEnum, Sprite> shapeImageMap = new Dictionary<ShapeEnum, Sprite>();

    public Sprite DefaultIcon;

    public static ShapeManager Instance { get; private set; }

    void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        UpdateShapeMap();
    }

    private void UpdateShapeMap()
    {
        shapeImageMap.Clear();
        foreach (var pair in shapeImageList)
        {
            Sprite icon = pair.icon != null ? pair.icon : DefaultIcon;
            if (!shapeImageMap.ContainsKey(pair.shape))
            {
                shapeImageMap.Add(pair.shape, icon);
            }
        }
    }

    public Sprite GetShapeSprite(ShapeEnum shape)
    {
        return shapeImageMap.TryGetValue(shape, out Sprite sprite) ? sprite : DefaultIcon;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        var enumValues = System.Enum.GetValues(typeof(ShapeEnum)).Cast<ShapeEnum>().ToList();

        foreach (var enumValue in enumValues)
        {
            if (!shapeImageList.Any(d => d.shape == enumValue))
            {
                shapeImageList.Add(new ShapeIconData
                {
                    shape = enumValue,
                    icon = DefaultIcon
                });
            }
        }

        // 중복 제거 (유니크하게 유지)
        shapeImageList = shapeImageList
            .GroupBy(d => d.shape)
            .Select(g => g.First())
            .ToList();

        EditorUtility.SetDirty(this); // 인스펙터 갱신
    }
#endif
}
