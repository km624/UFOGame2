using DG.Tweening;

using UnityEngine;


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
    private Sequence scaleseq;

    Rigidbody rb;
  
    private float objectmass;

    //private bool activelift = false;

    
    public void InitiaLiftAbsorption(Vector3 orginsclale,float mass)
    {
        targetScale = new Vector3(TargetScaleValue, TargetScaleValue, TargetScaleValue);
        swallowobject = GetComponent<FallingObject>();
        defaultScale = orginsclale;
        objectmass = mass;

    }

   

    public void StartAbsorp(int swallowlevel)
    {

        /* float referenceSpeed = 10f;
         float referenceMass = 1f;
         float baseScaleTime = 1.5f;

         float liftRatio = (liftSpeed / referenceSpeed) / (objectmass / referenceMass);
         ScaleTime = baseScaleTime * liftRatio;
 */
        /* float power = -1f;
         float baseFactor = 15f;

         ScaleTime = baseFactor * Mathf.Pow(objectmass / liftSpeed, power);*/
       
       

        if((swallowlevel - objectmass) >=0)
        {
            ScaleTime = 3.0f;
        }
        else
        {
            ScaleTime = 0.5f;
        }
       // Debug.Log(gameObject.name + " : " + objectmass);


        scaleseq?.Kill();
        swallowobject.ActivateBounce(false);
    }


    public void ApplyAbsorptionScale()
    {
        //Debug.Log("��������");
        
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime* ScaleTime);
    }
    public float GetObjectMass()
    {
        return objectmass;
    }
    


        /// <summary>
        /// ������Ʈ�� �������� �⺻ �����Ϸ� �ε巴�� ���ͽ�ŵ�ϴ�.
        /// </summary>
    public void ResetScale()
    {
       
        scaleseq = DOTween.Sequence();
        scaleseq.Append(transform.DOScale(defaultScale, 0.5f).SetEase(Ease.OutQuad));  // 0.5�� ������ tween
        scaleseq.AppendInterval(1.0f);  // tween �Ϸ� �� 1�� ����
        scaleseq.OnComplete(() => swallowobject.ActivateBounce(true));  // ���� �� �ݹ� ����


       
    }
}
