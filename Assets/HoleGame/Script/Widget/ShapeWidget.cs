
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShapeWidget : MonoBehaviour
{

    public TMP_Text CountText;
    public TMP_Text DoubleText;
    public Image ShapeImage;
    private int Shapecount = 0;
    private PlayerHudWidget playerhud;

   

    public void SetShapeWidget(int count,Sprite shpaeImage, PlayerHudWidget hud)
    {
        Shapecount = count;
        CountText.text = Shapecount.ToString();
        if(shpaeImage != null) 
            ShapeImage.sprite = shpaeImage;

        playerhud = hud;
    }

    public void UpdateShapeCount(int count)
    {
        Shapecount = count;
        CountText.text = Shapecount.ToString();
    }

    public void MinusShapeCount()
    {
        Shapecount--;
        CountText.text = Shapecount.ToString();
        if (Shapecount == 0)
        {
            DisableShapeWidget();

        } 


    }

    public bool CheckDisable() { return Shapecount <= 0; }

    private void DisableShapeWidget()
    {

        //애니메이션 추가 가능
        CountText.enabled = false;
        ShapeImage.enabled = false;
        DoubleText.enabled = false;

        OnShpaeWidgetAnimation();
    }

    public void OnShpaeWidgetAnimation()
    {
        playerhud.CallBack_WidgetAnimationEnd();
    }
}
