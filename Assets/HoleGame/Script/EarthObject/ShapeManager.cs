using System.Collections.Generic;
using UnityEngine;

public class ShapeManager : MonoBehaviour
{
   
    [Header("������Ʈ ������")]
    [SerializeField]
    public List<ShapeIconData> shapeImageList = new List<ShapeIconData>();

    public Dictionary<ShapeEnum, Sprite> shapeImageMap = new Dictionary<ShapeEnum, Sprite>();

    public Sprite DefaultIcon;

    public static ShapeManager Instance { get; private set; }

    void Awake()
    {
        // �̱��� �ν��Ͻ� ����
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
