using UnityEngine;

[CreateAssetMenu(fileName = "ShapeData", menuName = "Scriptable Objects/ShapeData")]
public class ShapeData : ScriptableObject
{
    public ShapeEnum shapeType;
    public Sprite shapeSprite;
    
}
