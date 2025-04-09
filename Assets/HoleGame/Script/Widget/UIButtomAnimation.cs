using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIButtomAnimation : MonoBehaviour
{
    private RectTransform rectTransform;
    private Vector3 originalScale;

    [Header("DOTween Settings")]
    public float duration = 0.5f;       // 전체 애니메이션 시간
    public float strength = 0.25f;      // 스프링 효과 강도 (클수록 많이 튐)
    public int vibrato = 2;            // 진동 횟수 (클수록 더 흔들림)
    public float elasticity = 0.6f;       // 탄성 (0 = 딱딱하게, 1 = 탄성 높게)
    private bool StopAnimation = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = rectTransform.localScale;

        // 버튼에 클릭 이벤트 연결
        //GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }

    public void SetAnimationDuration(float dura)
    {
        duration= dura;
    }
   public void SetAnimationStopState(bool stop)
    {
        StopAnimation = stop;
    }
    public void StartAnimation()
    {
        if (StopAnimation) return;
        // DOTween 애니메이션 시퀀스 생성
        Sequence sequence = DOTween.Sequence();

        // 1. 클릭 시 버튼이 작아졌다가 커지는 애니메이션 
        sequence.Append(rectTransform.DOScale(originalScale * 0.7f, 0.1f).SetEase(Ease.OutQuad)); // 살짝 줄어듦
        sequence.Append(rectTransform.DOScale(originalScale * 1.4f, 0.1f).SetEase(Ease.OutBack));  // 확 커짐

        // 2. 흔들리면서 원래 크기로 돌아가기
        sequence.Append(rectTransform.DOShakeScale(duration, strength, vibrato, elasticity, true))
                .Append(rectTransform.DOScale(originalScale, 0.3f).SetEase(Ease.OutElastic));
    }
}
