
using DG.Tweening;
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
    [SerializeField] private float Duration = 0.5f;

   

    public void SetShapeWidget(int count,Sprite shpaeImage, PlayerHudWidget hud)
    {
        Shapecount = count;
        CountText.text = Shapecount.ToString();
        if(shpaeImage != null) 
            ShapeImage.sprite = shpaeImage;

        playerhud = hud;

        // 초기 스케일 설정 (X=0에서 시작)
        transform.localScale = new Vector3(0f, 1f, 1f);
        transform.DOScaleX(1.2f, Duration*0.7f).SetEase(Ease.OutBack).onComplete = () =>
        {
            transform.DOScaleX(1f, Duration * 0.3f).SetEase(Ease.OutSine);
        };
       
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
            OnShpaeWidgetAnimation();

        } 


    }

    public bool CheckDisable() { return Shapecount <= 0; }

   
    public void OnShpaeWidgetAnimation()
    {

        transform.DOScaleX(1.2f, Duration * 0.3f).SetEase(Ease.OutBack).onComplete = () =>
        {
            transform.DOScaleX(0f, Duration * 0.7f)
                     .SetEase(Ease.InCubic)
                     .OnComplete(() =>
                     {
                         DisableShapeWidget();
                     });
        };



    }

    private void DisableShapeWidget()
    {


        CountText.enabled = false;
        ShapeImage.enabled = false;
        DoubleText.enabled = false;

        playerhud.CallBack_WidgetAnimationEnd();
    }

}
