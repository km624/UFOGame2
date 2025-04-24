using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIButtomAnimation : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector3 originalScale;

    [Header("DOTween Settings")]
    public float duration = 0.5f;       // ��ü �ִϸ��̼� �ð�
    public float strength = 0.25f;      // ������ ȿ�� ���� (Ŭ���� ���� Ʀ)
    public int vibrato = 2;            // ���� Ƚ�� (Ŭ���� �� ��鸲)
    public float elasticity = 0.6f;       // ź�� (0 = �����ϰ�, 1 = ź�� ����)
    private bool StopAnimation = false;

    Sequence skillsequence;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;

        // ��ư�� Ŭ�� �̺�Ʈ ����
        //GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    public void SetAnimationDuration(float dura)
    {
        duration = dura;
    }
   public void SetAnimationStopState(bool stop)
    {
        StopAnimation = stop;
    }

    public void PauseAnimation()
    {
        skillsequence?.Pause();
    }

    // �ִϸ��̼� �簳
    public void ResumeAnimation()
    {
        skillsequence?.Play();
    }
    public void StartAnimation()
    {
        if (StopAnimation) return;
        // DOTween �ִϸ��̼� ������ ����
        skillsequence = DOTween.Sequence();

        // 1. Ŭ�� �� ��ư�� �۾����ٰ� Ŀ���� �ִϸ��̼� 
        skillsequence.Append(rectTransform.DOScale(originalScale * 0.7f, 0.1f).SetEase(Ease.OutQuad)); // ��¦ �پ��
        skillsequence.Append(rectTransform.DOScale(originalScale * 1.4f, 0.1f).SetEase(Ease.OutBack));  // Ȯ Ŀ��

        // 2. ��鸮�鼭 ���� ũ��� ���ư���
        skillsequence.Append(rectTransform.DOShakeScale(duration, strength, vibrato, elasticity, true))
                .Append(rectTransform.DOScale(originalScale, 0.3f).SetEase(Ease.OutElastic));
    }
}
