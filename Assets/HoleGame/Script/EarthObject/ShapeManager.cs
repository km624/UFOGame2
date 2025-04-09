using System.Collections.Generic;
using UnityEngine;

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

        shapeImageMap.Clear();
        foreach (var pair in shapeImageList)
        {
            if (!shapeImageMap.ContainsKey(pair.shape))
            {
                shapeImageMap.Add(pair.shape, pair.icon);
            }
        }
    }

    public Sprite GetShapeSprite(ShapeEnum shape)
    {
        return shapeImageMap.TryGetValue(shape, out Sprite sprite) ? sprite : DefaultIcon;
    }
}
