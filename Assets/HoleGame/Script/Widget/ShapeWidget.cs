using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShapeWidget : MonoBehaviour
{

    public TMP_Text CountText;
    public Image ShapeImage;

   
    void Start()
    {
        
    }

    void Update()
    {
     
    }

    public void SetShapeWidget(int count,Sprite shpaeImage)
    {
        CountText.text = count.ToString();
        if(shpaeImage != null) 
            ShapeImage.sprite = shpaeImage;
    }

    public void UpdateShapeCount(int count)
    {
       
        CountText.text = count.ToString();
    }
}
