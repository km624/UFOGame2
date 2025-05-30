using DG.Tweening;

using UnityEngine;


public class LiftAbsorption : MonoBehaviour
{

    private FallingObject swallowobject;

   /* [Header("Tween 시간 설정")]
    [SerializeField]*/
    private float ScaleTime;
    //private float OriginScaleTime = 2.0f;
    private Vector3 defaultScale;
   

    [Header("줄어들 스케일")]
    //[SerializeField]
    private float TargetScaleValue = 0.25f;
    private Vector3 targetScale;
    private Sequence scaleseq;

    Rigidbody rb;
  
    private float objectmass;

    
    public void InitiaLiftAbsorption(Vector3 orginsclale,float mass)
    {
        targetScale = new Vector3(TargetScaleValue, TargetScaleValue, TargetScaleValue);
        swallowobject = GetComponent<FallingObject>();
        defaultScale = orginsclale;
        objectmass = mass;

    }

   
    public void StartAbsorp(int swallowlevel)
    {

       

        if((swallowlevel - objectmass) >=0)
        {
            ScaleTime = 7.0f;
        }
        else
        {
            ScaleTime = 0.5f;
        }

       // bHasLanded = false;

        scaleseq?.Kill();
        swallowobject.ActivateBounce(false);
    }


    public void ApplyAbsorptionScale()
    {
        //Debug.Log("빨리는중");
        
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime* ScaleTime);
    }
    public float GetObjectMass()
    {
        return objectmass;
    }
    


        /// <summary>
        /// 오브젝트의 스케일을 기본 스케일로 부드럽게 복귀시킵니다.
        /// </summary>
    public void ResetScale()
    {
       
        scaleseq = DOTween.Sequence();
        scaleseq.Append(transform.DOScale(defaultScale, 0.5f).SetEase(Ease.OutQuad));  // 0.5초 스케일 tween
        scaleseq.AppendInterval(1.5f);  // tween 완료 후 1초 지연
        scaleseq.OnComplete(() => swallowobject.ActivateBounce(true));  // 지연 후 콜백 실행*/

    }


   /* private void OnCollisionEnter(Collision collision)
    {
        // 이미 처리된 경우 무시
        if (bHasLanded) return;

      
        if (collision.gameObject.layer == groundlayer)
        {
            bHasLanded = true; // 한 번만 실행되게 막음
            swallowobject.ActivateBounce(true);
            Debug.Log(collision.gameObject.name  + " : 땅에 닿음");
        }
    }*/
}
