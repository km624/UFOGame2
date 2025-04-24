using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonEffect : MonoBehaviour,IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private VibrationEnum vibetype = VibrationEnum.ButtonClick;

   
     private Image targetImage;
     private float pressedScale = 0.85f;
     private float pressedAlpha = 1f;
     private float duration = 0.1f;

    private Vector3 originalScale;
    private float originalAlpha;

    [SerializeField] private bool bIskAnimation = true;

    private void Start()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();
        originalScale = transform.localScale;
        originalAlpha = targetImage.color.a;

        Button btn = GetComponent<Button>(); 
        if (btn != null)
        {
            if (GameManager.Instance != null)
            {
                btn.onClick.AddListener(() =>
                {
                    GameManager.Instance.vibrationManager.Play(vibetype);
                });
            }

        }
        else
        {
            Debug.LogWarning($"[AutoVibrationBinder] Button ������Ʈ�� {gameObject.name}�� ����!");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!bIskAnimation) return;
        // ������ ���̰� ���� �ø���
        transform.DOScale(originalScale * pressedScale, duration).SetEase(Ease.OutQuad);
        targetImage.DOFade(pressedAlpha, duration);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!bIskAnimation) return;
        // ������� ����
        transform.DOScale(originalScale, duration).SetEase(Ease.OutBack);
        targetImage.DOFade(originalAlpha, duration);
    }
}
