using DG.Tweening;
using UnityEngine;

public class LevelUpMotion : MonoBehaviour
{
    [SerializeField] private RectTransform LevelUI;
    [SerializeField] private float showDuration = 0.4f;
    [SerializeField] private float hideDuration = 0.3f;
    [SerializeField] private float visibleTime = 5.0f;
    

    private Sequence currentSeq;

    

    private void Start()
    {
        LevelUI.gameObject.SetActive(false);
    }

 
    public void PlayPop()
    {
        // 이전 애니메이션 있으면 정리
        currentSeq?.Kill();

        LevelUI.gameObject.SetActive(true);

        LevelUI.localScale = transform.localScale;

        float visbletime = visibleTime;
      

        // 등장 애니메이션
        currentSeq = DOTween.Sequence();
        currentSeq.Append(LevelUI.DOScale(1.0f, showDuration).SetEase(Ease.OutBack));
                


        // 대기 후 사라짐
        currentSeq.AppendInterval(visbletime);

        // 사라지는 애니메이션
        currentSeq.Append(LevelUI.DOScale(0.6f, hideDuration * 0.5f).SetEase(Ease.InBack))
                  .OnComplete(() => LevelUI.gameObject.SetActive(false));


    }

  
}
