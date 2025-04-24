using DG.Tweening;
using UnityEngine;
using System.Collections;
using System.ComponentModel;
public class ObjectMovement : MonoBehaviour
{
    [SerializeField]
    private FallingObject fallingObject;

    [Header("Squish Settings")]
    [Tooltip("������ �� �̵��� �Ÿ� ( ����)")]
    [SerializeField]
    private float jumpDistance = 0.5f;
   

    [Tooltip("���� �ִϸ��̼� ���� �ð�")]
    [SerializeField]
    private float jumpDuration = 1.0f;
    [Tooltip("���� �� squish ���� (�ϴ� Y�� ���� ����)")]
    [SerializeField]
    private float squishAmount = 0.2f;
    [Tooltip("Squish �ִϸ��̼� �ð�")]
    [SerializeField]
    private float squishDuration = 0.1f;

    [Tooltip("���� ���")]
    [SerializeField]
    private float JumpDelayTime = 1.0f;

    [Tooltip("���� �Ŀ� (����2)")]
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
        // jumpDistance (D) ��� ���
        float D = jumpDistance;

        // jumpDuration ���
        if (D >= 1.0f)
            jumpDuration = 0.4f;
        else
            jumpDuration = 0.6f;

        // squishAmount ���
        squishAmount = 0.6f - 0.2f * D;
        squishAmount = Mathf.Clamp(squishAmount, 0.2f, 0.6f);
        // squishDuration�� 0.1f�� ����

        // JumpDelayTime ���
        JumpDelayTime = 1.8f - 0.4f * D;

        // jumpPower ��� (���ǿ� ����)
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

        // ��ǥ ��ġ (���� �̵� �Ÿ��� ����)
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
        // UFO�� ��ġ
        Vector3 ForcePosition = objecttransform.position;

        // ������Ʈ�� ��ġ
        Vector3 objectPosition = transform.position;

        // Y ���� �����ϰ� ���� (���̸� �����ϱ� ����)
        ForcePosition.y = 0;
        objectPosition.y = 0;

        // UFO ���� ���� ��� (���� ����)
        Vector3 directionToUFO = (ForcePosition - objectPosition).normalized;

        return directionToUFO;
        /*  // �����: �� �信�� ���� ǥ��
          Debug.DrawLine(objectPosition, objectPosition + directionToUFO * 5f, Color.red);*/


    }
}
