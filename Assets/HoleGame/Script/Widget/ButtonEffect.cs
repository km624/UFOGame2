using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonEffect : MonoBehaviour,IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private VibrationEnum vibetype = VibrationEnum.ButtonClick;
    [SerializeField] private SoundEnum sound = SoundEnum.ButtonClick;
 
     private Image targetImage;
    [SerializeField] private float pressedScale = 0.85f;
     private float pressedAlpha = 1f;
     private float duration = 0.1f;

    private Vector3 originalScale;
    private float originalAlpha;

    [SerializeField] private bool bDisableAlpha = false;
    [SerializeField] private bool bIsAnimation = true;
    [SerializeField] private bool revrseAnimation = false;
    [SerializeField] private bool bSoundMute = false;
    [SerializeField] private bool bVibrateMute = false;
    [SerializeField] private bool ForceBinding = false;
    [SerializeField] private float SoundVolume = 0.3f;

    private void Start()
    {
        if (targetImage == null)
            targetImage = GetComponent<Image>();
        originalScale = transform.localScale;
        originalAlpha = targetImage.color.a;
        if (ForceBinding) return;
        Button btn = GetComponent<Button>(); 
        if (btn != null)
        {
           
            if (GameManager.Instance != null)
            {
                btn.onClick.AddListener(() =>
                {
                    if (!bSoundMute) 
                        GameManager.Instance.vibrationManager.Play(vibetype);
                    if(!bVibrateMute)
                        GameManager.Instance.soundManager.PlaySfx(sound, SoundVolume);
                });
            }

        }
        else
        {
            Debug.LogWarning($"[AutoVibrationBinder] Button 컴포넌트가 {gameObject.name}에 없음!");
        }
    }

    public void OnForceButtonClick()
    {
        if (!bSoundMute)
            GameManager.Instance.vibrationManager.Play(vibetype);
        if (!bVibrateMute)
            GameManager.Instance.soundManager.PlaySfx(sound, SoundVolume);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!bIsAnimation) return;
        // 스케일 줄이고 알파 올리기
        float targetscale = pressedScale;
        if (revrseAnimation)
            targetscale = 1 + (1 - pressedScale);

        transform.DOScale(originalScale * targetscale, duration).SetEase(Ease.OutQuad);
        if(!bDisableAlpha)
            targetImage.DOFade(pressedAlpha, duration);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!bIsAnimation) return;
        // 원래대로 복원
        transform.DOScale(originalScale, duration).SetEase(Ease.OutBack);
        if (!bDisableAlpha)
            targetImage.DOFade(originalAlpha, duration);
    }
}
