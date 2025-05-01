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
   
    private Rigidbody rb;
    public void SetJupmpDistance(float distance)
    {
        jumpDistance = distance;
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>(); 
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
        squishAmount = 0.5f; 
        //squishAmount = 0.6f - 0.2f * D;
       //squishAmount = Mathf.Clamp(squishAmount, 0.2f, 0.6f);
        // squishDuration은 0.1f로 고정

        // JumpDelayTime 계산
        JumpDelayTime = 2.5f - 0.4f * D;
        //JumpDelayTime = 3.0f - 0.4f * D;

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


    /*private void PerformRandomJump2()
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

      
        Vector3 targetPosition = transform.position + horizontalDir * jumpDistance;
        Vector3 SquishedScale = new Vector3(
                     defaultScale.x * (1 + squishAmount),
                     defaultScale.y * (1 - squishAmount),
                     defaultScale.z * (1 + squishAmount));
 
       
         jumpSeq = DOTween.Sequence();

        jumpSeq.Append(transform.DORotate(Quaternion.LookRotation(horizontalDir).eulerAngles, 0.5f, RotateMode.Fast)
                  .SetEase(Ease.OutQuad)).SetUpdate(UpdateType.Fixed);

        jumpSeq.Append(transform.DOScale(SquishedScale, squishDuration).SetEase(Ease.OutQuad))
               .Append(transform.DOScale(defaultScale, squishDuration).SetEase(Ease.InQuad)).SetUpdate(UpdateType.Fixed);


        //jumpSeq.Append(transform.DOJump(targetPosition, jumpPower, 1, jumpDuration).SetEase(Ease.OutQuad)).SetUpdate(UpdateType.Fixed);
        jumpSeq.Append(rb
            .DOJump(
                transform.position + horizontalDir * jumpDistance,
                jumpPower,      // jump height
                1,              // num of jumps
                jumpDuration   // duration
            )
            .SetEase(Ease.OutQuad)
            .SetUpdate(UpdateType.Fixed));



    }*/

    /*private void PerformRandomJump2()
    {
       
        Vector3 horizontalDir;
        if (ForceMoveObject == null)
        {
            Vector2 randomDir2D = Random.insideUnitCircle.normalized;
            horizontalDir = new Vector3(randomDir2D.x, 0f, randomDir2D.y);
        }
        else
        {
            Vector3 forceJumpDirection = CalculateUfoDirection(ForceMoveObject.transform);
            horizontalDir = new Vector3(forceJumpDirection.x, 0f, forceJumpDirection.z) * 2f;
        }

      
        if (squishDuration * 2 > jumpDuration)
            squishDuration = jumpDuration / 2f;

        
        Vector3 targetPosition = transform.position + horizontalDir * jumpDistance;
        Vector3 squishedScale = new Vector3(
            defaultScale.x * (1 + squishAmount),
            defaultScale.y * (1 - squishAmount),
            defaultScale.z * (1 + squishAmount)
        );

       
        if (jumpSeq != null && jumpSeq.IsActive())
            jumpSeq.Kill();

      
        jumpSeq = DOTween.Sequence()
           
            .Append(transform
                .DORotate(Quaternion.LookRotation(horizontalDir).eulerAngles, 0.5f, RotateMode.Fast)
                .SetEase(Ease.OutQuad)
            )
            
            .Append(transform
                .DOScale(squishedScale, squishDuration)
                .SetEase(Ease.OutQuad)
            )
            .Append(transform
                .DOScale(defaultScale, squishDuration)
                .SetEase(Ease.InQuad)
            )
            
            .AppendCallback(() =>
            {
                rb.useGravity = false;
                rb.linearVelocity = Vector3.zero;
            })
            
            .AppendCallback(() =>
            {
                rb
                  .DOJump(targetPosition, jumpPower, 1, jumpDuration)
                  .SetEase(Ease.OutQuad)
                  .SetUpdate(UpdateType.Fixed)
                  .OnComplete(() =>
                  {
                     
                      rb.useGravity = true;
                  });
            })
           
            .SetUpdate(UpdateType.Fixed);
    }*/

    private void PerformRandomJump2()
    {
        //수평 방향 계산 (기존 로직)
        Vector3 horizontalDir;
        if (ForceMoveObject == null)
        {
            Vector2 rnd = Random.insideUnitCircle.normalized;
            horizontalDir = new Vector3(rnd.x, 0f, rnd.y);
        }
        else
        {
            Vector3 d = CalculateUfoDirection(ForceMoveObject.transform);
            horizontalDir = new Vector3(d.x, 0f, d.z) * 2f;
        }

        //squash 타이밍 보정
        if (squishDuration * 2 > jumpDuration)
            squishDuration = jumpDuration / 2f;

        // 목표 수평 속도 계산: 거리 / 시간
        float horizontalSpeed = jumpDistance / jumpDuration;

        // squash 스케일 계산
        Vector3 squishedScale = new Vector3(
            defaultScale.x * (1 + squishAmount),
            defaultScale.y * (1 - squishAmount),
            defaultScale.z * (1 + squishAmount)
        );

        //이전 시퀀스 정리
        if (jumpSeq != null && jumpSeq.IsActive())
            jumpSeq.Kill();

        // 새 시퀀스 생성
        jumpSeq = DOTween.Sequence()
            //  바라보기 회전
            .Append(transform
                .DORotate(Quaternion.LookRotation(horizontalDir).eulerAngles, 0.5f, RotateMode.Fast)
                .SetEase(Ease.OutQuad)
            )
            //  squash
            .Append(transform
                .DOScale(squishedScale, squishDuration)
                .SetEase(Ease.OutQuad)
            )
            //  unsquash
            .Append(transform
                .DOScale(defaultScale, squishDuration)
                .SetEase(Ease.InQuad)
            )
            // 점프 직전: 중력 켜고 속도 리셋
            .AppendCallback(() =>
            {
                //rb.useGravity = true;
                rb.linearVelocity = Vector3.zero;
            })
            //  점프용 속도 부여
            .AppendCallback(() =>
            {
             /*   rb.AddForce(horizontalDir * horizontalSpeed*2.0f
           + Vector3.up * jumpPower*20f,
             ForceMode.Impulse);*/
                rb.AddForce(horizontalDir * horizontalSpeed 
          + Vector3.up * jumpPower * 5f,
            ForceMode.VelocityChange);
            })
            //  FixedUpdate 타이밍에 맞춤
            .SetUpdate(UpdateType.Fixed);
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
