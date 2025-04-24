using DG.Tweening;
using UnityEngine;
using System.Collections;
using System.ComponentModel;
public class ObjectMovement : MonoBehaviour
{
    [SerializeField]
    private FallingObject fallingObject;

    [Header("Squish Settings")]
    [Tooltip("점프할 때 이동할 거리 ( 빗변)")]
    [SerializeField]
    private float jumpDistance = 0.5f;
   

    [Tooltip("점프 애니메이션 지속 시간")]
    [SerializeField]
    private float jumpDuration = 1.0f;
    [Tooltip("점프 시 squish 정도 (일단 Y축 감소 비율)")]
    [SerializeField]
    private float squishAmount = 0.2f;
    [Tooltip("Squish 애니메이션 시간")]
    [SerializeField]
    private float squishDuration = 0.1f;

    [Tooltip("점프 대기")]
    [SerializeField]
    private float JumpDelayTime = 1.0f;

    [Tooltip("점프 파워 (점프2)")]
    [SerializeField]
    private float jumpPower = 1.0f;

  

    private Vector3 defaultScale = Vector3.zero;
    
    private Sequence jumpSeq;
    Coroutine jumpRoutine;

    //private Vector3 ForceJumpDirection = Vector3.zero;
    private Coroutine ForceDirectionCoroutine;

    private Transform ForceMoveObject = null;

    public void SetJupmpDistance(float distance)
    {
        jumpDistance = distance;
    }

    void Start()
    {
        JumpNormalized();
        fallingObject = GetComponent<FallingObject>();
        defaultScale = transform.localScale;
        JumpActivate(true);
    }

    private void JumpNormalized()
    {
        // jumpDistance (D) 기반 계산
        float D = jumpDistance;

        // jumpDuration 계산
        if (D >= 1.0f)
            jumpDuration = 0.4f;
        else
            jumpDuration = 0.6f;

        // squishAmount 계산
        squishAmount = 0.6f - 0.2f * D;
        squishAmount = Mathf.Clamp(squishAmount, 0.2f, 0.6f);
        // squishDuration은 0.1f로 고정

        // JumpDelayTime 계산
        JumpDelayTime = 1.8f - 0.4f * D;

        // jumpPower 계산 (조건에 따라)
        if (D >= 1.0f)
            jumpPower = 0.4f * D + 0.2f;
        else
            jumpPower = 0.2f * D + 0.4f;
    }
    private void OnEnable()
    {
        //Debug.Log("JimpOnenable");
        if(defaultScale!=Vector3.zero)
            JumpActivate(true);


    }
    void OnDisable()
    {
       
        JumpActivate(false);
    }


    public void JumpActivate(bool bActiveJump)
    {
        if (!bActiveJump)
        {
            //Debug.Log("SToJumpp");
            if(jumpRoutine!=null)
                StopCoroutine(jumpRoutine);
            jumpSeq?.Kill();
            jumpRoutine = null;

        }
        else
        {
           // Debug.Log("StartJump");
            jumpRoutine = StartCoroutine(JumpLoop());
        }

    }

    IEnumerator JumpLoop()
    {
        while (true) 
        {
           // PerformRandomJump(); 

            PerformRandomJump2();

            float delay = 0.5f + squishDuration * 2.0f + jumpDuration+JumpDelayTime;
            yield return new WaitForSeconds(delay); 
        }
    }

    public void ForceSetMoveOBject(Transform forceObject, float progresstime)
    {
        if (ForceDirectionCoroutine != null)
            StopCoroutine(ForceDirectionCoroutine);
        ForceDirectionCoroutine = StartCoroutine(ResetForceJumpDirection(progresstime));
        ForceMoveObject= forceObject;
        fallingObject.ActiveStateIconWidget(true);
    }

    private IEnumerator ResetForceJumpDirection(float progresstime)
    {
        yield return new WaitForSeconds(progresstime);
        //ActiveStateIconWidget()
        ForceMoveObject = null;
        fallingObject.ActiveStateIconWidget(false);
    }


    private void PerformRandomJump2()
    {

        Vector3 horizontalDir = Vector3.zero;
        
        if (ForceMoveObject == null)
        {
            Vector2 randomDir2D = Random.insideUnitCircle.normalized;
            horizontalDir = new Vector3(randomDir2D.x, 0f, randomDir2D.y);
        }
        else
        {
           
            Vector3 forceJumpDirection = CalculateUfoDirection(ForceMoveObject.transform);
            horizontalDir = new Vector3(forceJumpDirection.x, 0f, forceJumpDirection.z);
            horizontalDir *= 2.0f;
        }
       

        if (squishDuration*2 > jumpDuration)
        {
            squishDuration = jumpDuration/2;
        }

        // 목표 위치 (수평 이동 거리만 적용)
        Vector3 targetPosition = transform.position + horizontalDir * jumpDistance;
        Vector3 SquishedScale = new Vector3(
                     defaultScale.x * (1 + squishAmount),
                     defaultScale.y * (1 - squishAmount),
                     defaultScale.z * (1 + squishAmount));
 
       
         jumpSeq = DOTween.Sequence();
       
        jumpSeq.Append(transform.DORotate(Quaternion.LookRotation(horizontalDir).eulerAngles, 0.5f, RotateMode.Fast)
                  .SetEase(Ease.OutQuad))
                .Append(transform.DOScale(SquishedScale, squishDuration).SetEase(Ease.OutQuad))
               .Append(transform.DOScale(defaultScale, squishDuration).SetEase(Ease.InQuad));
               
        jumpSeq.Append(transform.DOJump(targetPosition, jumpPower, 1, jumpDuration).SetEase(Ease.OutQuad));
       
      
    }

    Vector3 CalculateUfoDirection(Transform objecttransform)
    {
        // UFO의 위치
        Vector3 ForcePosition = objecttransform.position;

        // 오브젝트의 위치
        Vector3 objectPosition = transform.position;

        // Y 값을 동일하게 설정 (높이를 무시하기 위해)
        ForcePosition.y = 0;
        objectPosition.y = 0;

        // UFO 방향 벡터 계산 (높이 무시)
        Vector3 directionToUFO = (ForcePosition - objectPosition).normalized;

        return directionToUFO;
        /*  // 디버그: 씬 뷰에서 방향 표시
          Debug.DrawLine(objectPosition, objectPosition + directionToUFO * 5f, Color.red);*/


    }
}
