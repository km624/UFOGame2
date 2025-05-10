using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PossibleWidget : MonoBehaviour
{
    [SerializeField] private RectTransform targetUI;
    [SerializeField] private float showDuration = 0.4f;
    [SerializeField] private float hideDuration = 0.3f;
    [SerializeField] private float visibleTime = 5.0f;
    [SerializeField] private float bossvisibleTime = 1.5f;

    private Sequence currentSeq;

    [SerializeField] private Image ObjectImage;

    private void Start()
    {
        targetUI.gameObject.SetActive(false);
    }

    public void SetPossibleIcon(ShapeEnum shapetype)
    {
     

        if (ShapeManager.Instance != null)
        {
            Sprite shapeicon = ShapeManager.Instance.GetShapeSprite(shapetype);
            if (shapeicon != null)
            {
                ObjectImage.sprite = shapeicon;
            }
        }

        PlayPop(shapetype);
    }

    


    public void PlayPop(ShapeEnum shapetype)
    {
        // ���� �ִϸ��̼� ������ ����
        currentSeq?.Kill();

        //Debug.Log(shapetype.ToString());
        targetUI.gameObject.SetActive(true);
        
        targetUI.localScale = transform.localScale;

        float visbletime = visibleTime;
        if (shapetype == ShapeEnum.boss)
            visbletime = bossvisibleTime;

        // ���� �ִϸ��̼�
        currentSeq = DOTween.Sequence();
        currentSeq.Append(targetUI.DOScale(1.2f, showDuration * 0.6f).SetEase(Ease.OutBack))
                  .Append(targetUI.DOScale(1f, showDuration * 0.4f).SetEase(Ease.OutElastic));

        // ��� �� �����
        currentSeq.AppendInterval(visbletime);

        // ������� �ִϸ��̼�
        currentSeq.Append(targetUI.DOScale(0.6f, hideDuration * 0.5f).SetEase(Ease.InBack))
                  .Append(targetUI.DOScale(0f, hideDuration * 0.5f).SetEase(Ease.InSine))
                  .OnComplete(() => targetUI.gameObject.SetActive(false));
    }

  
}
