using DG.Tweening;

using UnityEngine;
using UnityEngine.Timeline;

public class LiftAbsorption : MonoBehaviour
{

    private FallingObject swallowobject;

   /* [Header("Tween 시간 설정")]
    [SerializeField]*/
    private float ScaleTime;
    //private float OriginScaleTime = 2.0f;
    private Vector3 defaultScale;
    private Vector3 CurrentScale;

    [Header("줄어들 스케일")]
    //[SerializeField]
    private float TargetScaleValue = 0.25f;
    private Vector3 targetScale;
    private Tween scaleTween;

    Rigidbody rb;
  
    private float objectmass;

    
    void Start()
    {
        targetScale = new Vector3(TargetScaleValue, TargetScaleValue, TargetScaleValue);
        swallowobject = GetComponent<FallingObject>();
        defaultScale = transform.localScale;
        CurrentScale = defaultScale;
        rb = GetComponent<Rigidbody>();
        objectmass = (rb != null) ? rb.mass : 1f;

    }

    public void StartAbsorp(float liftSpeed)
    {

        // ScaleTime = (liftSpeed * 0.2f) / (objectmass * objectmass);
        ScaleTime = 0.005f * Mathf.Pow(liftSpeed / objectmass, 2.32f);
       
        scaleTween?.Kill();
        swallowobject.ActivateBounce(false);
    }


    public void ApplyAbsorptionScale()
    {
        //Debug.Log("빨리는중");
        
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime* 1.5f);
    }


        /// <summary>
        /// 오브젝트의 스케일을 기본 스케일로 부드럽게 복귀시킵니다.
        /// </summary>
    public void ResetScale()
    {
       
        scaleTween?.Kill();
        scaleTween = transform.DOScale(defaultScale, 0.5f).SetEase(Ease.OutQuad)
            .OnComplete(() => swallowobject.ActivateBounce(true));
    }
}
