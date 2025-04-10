using DG.Tweening;

using UnityEngine;
using UnityEngine.Timeline;

public class LiftAbsorption : MonoBehaviour
{

    private FallingObject swallowobject;

   /* [Header("Tween �ð� ����")]
    [SerializeField]*/
    private float ScaleTime;
    //private float OriginScaleTime = 2.0f;
    private Vector3 defaultScale;
   

    [Header("�پ�� ������")]
    //[SerializeField]
    private float TargetScaleValue = 0.25f;
    private Vector3 targetScale;
    private Tween scaleTween;

    Rigidbody rb;
  
    private float objectmass;

    private bool activelift = false;

    
    public void InitiaLiftAbsorption(Vector3 orginsclale)
    {
        targetScale = new Vector3(TargetScaleValue, TargetScaleValue, TargetScaleValue);
        swallowobject = GetComponent<FallingObject>();
        defaultScale = orginsclale;
       
        rb = GetComponent<Rigidbody>();
        
        objectmass = (rb != null) ? rb.mass : 1f;
       
    }

   

    public void StartAbsorp(float liftSpeed)
    {

        float referenceSpeed = 10f;
        float referenceMass = 1f;
        float baseScaleTime = 1.5f;

        float liftRatio = (liftSpeed / referenceSpeed) / (objectmass / referenceMass);
        ScaleTime = baseScaleTime * liftRatio;

        /* float power = -1f;
         float baseFactor = 15f;

         ScaleTime = baseFactor * Mathf.Pow(objectmass / liftSpeed, power);*/

        Debug.Log(ScaleTime);
        scaleTween?.Kill();
        swallowobject.ActivateBounce(false);
    }


    public void ApplyAbsorptionScale()
    {
        //Debug.Log("��������");
        
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime* ScaleTime);
    }


        /// <summary>
        /// ������Ʈ�� �������� �⺻ �����Ϸ� �ε巴�� ���ͽ�ŵ�ϴ�.
        /// </summary>
    public void ResetScale()
    {

        Sequence seq = DOTween.Sequence();
        seq.Append(transform.DOScale(defaultScale, 0.5f).SetEase(Ease.OutQuad));  // 0.5�� ������ tween
        seq.AppendInterval(1.0f);  // tween �Ϸ� �� 1�� ����
        seq.OnComplete(() => swallowobject.ActivateBounce(true));  // ���� �� �ݹ� ����

       
        scaleTween = seq;
    }
}
