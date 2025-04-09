using UnityEngine;
using DG.Tweening;
public class BounceShape : MonoBehaviour
{
    [SerializeField]
    [Range(0, 0.6f)]
    public float squishAmount = 0.2f;
    [SerializeField]
    [Range(0, 0.6f)]
    public float squishSpeed = 0.5f;

    Vector3 defaultScale; 

    private Tween squishTween = null;

    void Start()
    {
       defaultScale = transform.localScale;
        StartSquishEffect();
    }

    void StartSquishEffect()
    {
        // �⺻ ũ��

        // ���Ʒ��� ���ȴٰ� �ٽ� ���� ũ��� �����ϴ� �ִϸ��̼�
        squishTween = transform.DOScale(new Vector3(defaultScale.x , defaultScale.y * (1 - squishAmount), defaultScale.z), squishSpeed)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutQuad);
    }

    
    private void OnEnable()
    {
        squishTween?.Restart();
    }
    void OnDisable()
    {
        squishTween?.Pause();
    }

    

    public void ForceSetBounce(float amount,float speed)
    {
        squishAmount=amount;
        squishSpeed=  0.5f * squishAmount + 0.2f;
    }
}
