using DG.Tweening;
using UnityEngine;

public class UFOMotion : MonoBehaviour
{
    [SerializeField]
    private BounceShape bounceShape;

    [Header("���Ʒ� �̵� ����")]
    [Tooltip("���Ʒ� �̵��� ����"),Range(0.0f, 1.0f)]
    public float verticalLength = 1f;
    [Tooltip("���Ʒ� �ݺ� �ӵ�"),Range(1, 2)]
    public float verticalSpeed = 1f;  

    [Header("�¿� ȸ�� ���� ")]
    [Tooltip("ȸ�� �ӵ�"),Range(0, 4)]
    public float rotationSpeed = 2.0f;  

    private float baseY;

    [SerializeField]
    private float wobbleAmount = 5f; 
    private float wobbleDuration = 1f;

    private Tween rotationTween;
    private Tween spinTween;

    private bool bIsMove = false;
   
    void Start()
    {
        
        baseY = transform.localPosition.y;
        //StartSpin();
    }

    void Update()
    {
      
        float newY = baseY + verticalLength * Mathf.Sin(Time.time * verticalSpeed * 2 * Mathf.PI);
        transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.x);


       
    }

  /* private void StartSpin()
    {

        //Debug.Log("��ŸƮ ����");
        spinTween = transform.DORotate(new Vector3(0, 360, 0), 60.0f/rotationSpeed, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }
*/
    public void StartWobble(Vector3 direction)
    {
        spinTween?.Kill();
        rotationTween?.Kill();
        direction *= 100;

        //Debug.Log("������~");
        bIsMove = true;
         Vector3 targetRotation = new Vector3(direction.z * wobbleAmount, 0, -direction.x * wobbleAmount);
       
        rotationTween = transform.DORotate(targetRotation, wobbleDuration)
            .SetEase(Ease.OutQuad);
    }

    public void ResetRotation()
    {
        if (!bIsMove) return;
        rotationTween?.Kill();
       // Debug.Log("���ڸ���");
        bIsMove = false;
        rotationTween = transform.DORotate(Vector3.zero, wobbleDuration).SetEase(Ease.OutQuad);
            //.OnComplete(() => StartSpin());
    }

    public void OnLevelUpMotion()
    {
        //��� ��� �ʱ�ȭ
        spinTween?.Kill();
        rotationTween?.Kill();
        bounceShape.enabled = false;

    }

    
}
