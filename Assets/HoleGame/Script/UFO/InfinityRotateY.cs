using DG.Tweening;
using UnityEngine;

public class InfinityRotateY : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 5.0f;

    void Start()
    {
        transform.DOLocalRotate(
            new Vector3(0, 360, 0),        // ��ǥ ���� ȸ��
            rotateSpeed,                   // �ɸ��� �ð�
            RotateMode.FastBeyond360        // 360�� �̻� ȸ�� ���
        )
        .SetEase(Ease.Linear)               // ���� �ӵ�
        .SetLoops(-1, LoopType.Restart);     // ���� �ݺ�
    }
}
