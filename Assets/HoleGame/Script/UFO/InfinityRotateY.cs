using DG.Tweening;
using UnityEngine;

public class InfinityRotateY : MonoBehaviour
{
    [SerializeField] private float rotateSpeed = 5.0f;

    void Start()
    {
        transform.DOLocalRotate(
            new Vector3(0, 360, 0),        // 목표 로컬 회전
            rotateSpeed,                   // 걸리는 시간
            RotateMode.FastBeyond360        // 360도 이상 회전 허용
        )
        .SetEase(Ease.Linear)               // 일정 속도
        .SetLoops(-1, LoopType.Restart);     // 무한 반복
    }
}
