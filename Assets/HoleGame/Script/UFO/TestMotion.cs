using DG.Tweening;
using UnityEngine;

public class TestMotion : MonoBehaviour
{
    [Header("위로 살짝 올라갈 높이")]
    public float upOffset = 0.7f;

    [Header("올라갈 때 걸리는 시간(초)")]
    public float upDuration = 0.5f;

    [Header("내려올 때 걸리는 시간(초)")]
    public float downDuration = 0.2f;


    public Vector3 defaultscale;

    public bool test;

   
    private void Start()
    {

        defaultscale = transform.localScale;
        if (test)
        {
            MotionStart();
        }
        else { MotionStart2(); }
        
      
    }

    public void MotionStart()
    {

        Sequence seq = DOTween.Sequence();

        // 1) 올라가면서 UFO를 살짝 납작하게 만들기
        //    - y축으로 올라가면서 x는 늘리고 y는 줄이는 식으로 과장
        seq.Append(
            transform.DOMoveY(transform.position.y + upOffset, upDuration)
                     .SetEase(Ease.OutQuad) // 움직임 느낌은 기호에 맞춰 변경
        );
        // squashScaleRatio를 활용해서 x/y에 각각 곱해줌
        seq.Join(
            transform.DOScale(new Vector3(
               defaultscale.x*0.9f,
                defaultscale.y *1.4f,
                defaultscale.z*0.9f),
                upDuration
            ).SetEase(Ease.OutBack)
        );

        // 2) 내려올 때 UFO를 스케일 0.3 -> 0.8로 자연스럽게 키우면서
        //    y축은 endY까지 이동
        seq.Append(
            transform.DOMoveY(transform.position.y, downDuration)
                     .SetEase(Ease.InQuad)
        );
        seq.Join(
            transform.DOScale(new Vector3(
                defaultscale.x * 1.3f,
                defaultscale.y * 0.7f,
                defaultscale.z *1.3f),
                downDuration
            ).SetEase(Ease.OutQuad)
        );

        // 3) 최종적으로 x,y,z가 endScale로 맞춰지도록 보정 (살짝 부풀린 뒤 본래 의도한 사이즈로)
        //    - 필요 없으면 빼도 됨

        seq.Append(
           transform.DOMoveY(transform.position.y, 0.2f)
                    .SetEase(Ease.InQuad)
        );
        seq.Join(
           transform.DOScale(defaultscale, 0.2f)
                    .SetEase(Ease.Linear)
       );

        // 시퀀스 재생
        seq.SetLoops(-1, LoopType.Restart);
    }

    public void MotionStart2()
    {

        Sequence seq = DOTween.Sequence();

        // 1) 올라가면서 UFO를 살짝 납작하게 만들기
        //    - y축으로 올라가면서 x는 늘리고 y는 줄이는 식으로 과장
      
        // squashScaleRatio를 활용해서 x/y에 각각 곱해줌
        seq.Append(
            transform.DOScale(new Vector3(
               defaultscale.x * 0.7f,
                defaultscale.y * 1.4f,
                defaultscale.z * 0.7f),
                upDuration
            ).SetEase(Ease.OutBack)
        );

        // 2) 내려올 때 UFO를 스케일 0.3 -> 0.8로 자연스럽게 키우면서
        //    y축은 endY까지 이동
       
      
        seq.Append(
            transform.DOScale(new Vector3(
                defaultscale.x * 1.2f,
                defaultscale.y * 0.8f,
                defaultscale.z * 1.2f),
                downDuration
            ).SetEase(Ease.OutQuad)
        );

        // 3) 최종적으로 x,y,z가 endScale로 맞춰지도록 보정 (살짝 부풀린 뒤 본래 의도한 사이즈로)
        //    - 필요 없으면 빼도 됨

        seq.Append(
           transform.DOScale(defaultscale, 0.2f)
                    .SetEase(Ease.Linear)
       );

        // 시퀀스 재생
        seq.SetLoops(-1, LoopType.Restart);
    }
}
