using DG.Tweening;

using UnityEngine;

public class UFOMotion : MonoBehaviour
{
    [SerializeField]
    private BounceShape bounceShape;

    [Header("위아래 이동 설정")]
    [Tooltip("위아래 이동의 높이"),Range(0.0f, 1.0f)]
    public float verticalLength = 1f;
    [Tooltip("위아래 반복 속도"),Range(1, 2)]
    public float verticalSpeed = 1f;  

    [Header("좌우 회전 설정 ")]
    [Tooltip("회전 속도"),Range(0, 4)]
    public float rotationSpeed = 2.0f;  

    private float baseY;

    [SerializeField]
    private float wobbleAmount = 5f; 
    private float wobbleDuration = 1f;

    private Tween rotationTween;
    private Tween spinTween;

   
    private bool bIsMove = false;
    private bool bIsMotion = true;

   

    void Start()
    {
        baseY = transform.localPosition.y; // 초기 진동 중심
      
    }

    public void Update()
    {
        if (!bIsMotion) return;
        float newY = baseY + verticalLength * Mathf.Sin(Time.time * verticalSpeed * 2 * Mathf.PI);
        transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.x);
    }
   

    public void ChangeBaseY(float newy)
    {
       
        baseY = newy;

    }


    public void SetMotionStart(bool motion)
    {
       bIsMotion = motion;
    }

 
    public void StartWobble(Vector3 direction)
    {
        spinTween?.Kill();
        rotationTween?.Kill();
        direction *= 100;

        //Debug.Log("움직임~");
        bIsMove = true;
         Vector3 targetRotation = new Vector3(direction.z * wobbleAmount, 0, -direction.x * wobbleAmount);
       
        rotationTween = transform.DORotate(targetRotation, wobbleDuration)
            .SetEase(Ease.OutQuad);
    }

    public void ResetRotation()
    {
        if (!bIsMove) return;
        rotationTween?.Kill();
       // Debug.Log("제자리로");
        bIsMove = false;
        rotationTween = transform.DORotate(Vector3.zero, wobbleDuration).SetEase(Ease.OutQuad);
           
    }

   
}
