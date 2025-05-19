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
        // ���� �ִϸ��̼� ������ ����
        currentSeq?.Kill();

        LevelUI.gameObject.SetActive(true);

        LevelUI.localScale = transform.localScale;

        float visbletime = visibleTime;
      

        // ���� �ִϸ��̼�
        currentSeq = DOTween.Sequence();
        currentSeq.Append(LevelUI.DOScale(1.0f, showDuration).SetEase(Ease.OutBack));
                


        // ��� �� �����
        currentSeq.AppendInterval(visbletime);

        // ������� �ִϸ��̼�
        currentSeq.Append(LevelUI.DOScale(0.6f, hideDuration * 0.5f).SetEase(Ease.InBack))
                  .OnComplete(() => LevelUI.gameObject.SetActive(false));


    }

  
}
