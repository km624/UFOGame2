using DG.Tweening;
using UnityEngine;

public class UFOHeightController : MonoBehaviour
{
    
    private Tween moveTween;

    [SerializeField] private float heightMoveDuration = 0.3f;
    [SerializeField] private UFOMotion uFOMotion;
    [SerializeField] private UFOMotion2 uFOMotion2;
    [SerializeField] private UFOPlayer uFOPlayer;
    [SerializeField] private GameObject UfoModel;
    [SerializeField] Canvas possibleWidget;

    private float addheight = 0.0f;


    public void MoveToHeight(float targetHeight)
    {
        if (moveTween != null) moveTween.Kill();
        addheight = targetHeight;
        
        uFOMotion.SetMotionStart(false);
       
        Debug.Log("UFO 높이");

        // 2. 해당 높이까지 이동
        moveTween = DOTween.Sequence()
         .Append(UfoModel.transform.DOLocalMoveY(targetHeight, heightMoveDuration)
             .SetEase(Ease.OutQuad))
         .Join(possibleWidget.transform.DOLocalMoveY(targetHeight, heightMoveDuration)
             .SetEase(Ease.OutQuad))
         .OnComplete(() =>
         {
             uFOMotion.ChangeBaseY(addheight);
             uFOMotion.SetMotionStart(true);
         });

    }

    public void ResetHeight()
    {
        if (moveTween != null) moveTween.Kill();


        Debug.Log("UFO 리셋");
        uFOMotion.SetMotionStart(false);
       

        moveTween = DOTween.Sequence().Append(UfoModel.transform.DOLocalMoveY(0, heightMoveDuration*0.5f)
             .SetEase(Ease.OutQuad))
           .Join(possibleWidget.transform.DOLocalMoveY(0, heightMoveDuration)
             .SetEase(Ease.OutQuad))
         .OnComplete(() =>
         {
             uFOMotion.ChangeBaseY(0);
             uFOMotion.SetMotionStart(true);
             uFOPlayer.PassProgress(false);
         });

    }
}
