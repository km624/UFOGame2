using DG.Tweening;
using UnityEngine;

public class MapObject : MonoBehaviour
{

    private Vector3 InitScale;

    private void Awake()
    {
        InitScale = transform.localScale;
    }

    public void MapObjectSpawn()
    {
        

        Vector3 startScale = new Vector3(0.01f, InitScale.y, 0.01f);
        transform.localScale = startScale;

        /*  Vector3 center = transform.position;
          Vector3 halfExtents = InitScale * 0.5f;*/
        BoxCollider box = GetComponent<BoxCollider>();
        if (box != null)
        {
    
            // 1. 월드 크기 = box.size  ×  lossyScale   (한 번만!)
            Vector3 worldSize = Vector3.Scale(box.size, transform.lossyScale) *30f;
            Vector3 halfExtents = worldSize * 0.5f;

            // 2. 월드 중심 = TransformPoint(box.center)
            Vector3 center = transform.TransformPoint(box.center);

            // 3. 실제 검사
           

            // 4. 디버그 시각화
            //DebugBox(center, worldSize, transform.rotation, Color.red, 100f);
            int layerMask = LayerMask.GetMask("Normal", "LiftUp");


            Collider[] overlapping = Physics.OverlapBox(center, halfExtents, transform.rotation, layerMask);

            foreach (Collider col in overlapping)
            {
                if (col.attachedRigidbody != null)
                {
                  

                    Vector3 pushDir = (col.transform.position - center).normalized;
                    if (pushDir == Vector3.zero) pushDir = Random.insideUnitSphere;
                    pushDir.y = 0f;
                    pushDir = pushDir.normalized; // 다시 정규화 (길이 보정)

                    // 힘을 줘서 밀어내거나, 위치를 순간이동시킴
                    col.attachedRigidbody.MovePosition(col.transform.position + pushDir * 3f);
                }
                else
                {
                    // 리지드바디 없을 경우 그냥 위치 이동
                    Vector3 pushDir = (col.transform.position - center).normalized;
                    if (pushDir == Vector3.zero) pushDir = Random.insideUnitSphere;
                    col.transform.position += pushDir * 3f;
                }
                Debug.Log("부딫힘" + col.name);
            }
        }

        // 애니메이션
        transform.DOScale(InitScale, 1f)
            .SetEase(Ease.OutBack);

    }

    public void DebugBox(Vector3 center, Vector3 size, Quaternion rotation, Color color, float duration = 0)
    {
        Vector3 halfSize = size * 0.5f;

        // 8개 꼭짓점 계산 (회전 반영)
        Vector3[] points = new Vector3[8]
        {
        center + rotation * new Vector3(-halfSize.x, -halfSize.y, -halfSize.z),
        center + rotation * new Vector3(halfSize.x, -halfSize.y, -halfSize.z),
        center + rotation * new Vector3(halfSize.x, -halfSize.y, halfSize.z),
        center + rotation * new Vector3(-halfSize.x, -halfSize.y, halfSize.z),
        center + rotation * new Vector3(-halfSize.x, halfSize.y, -halfSize.z),
        center + rotation * new Vector3(halfSize.x, halfSize.y, -halfSize.z),
        center + rotation * new Vector3(halfSize.x, halfSize.y, halfSize.z),
        center + rotation * new Vector3(-halfSize.x, halfSize.y, halfSize.z)
        };

        // Draw lines
        Debug.DrawLine(points[0], points[1], color, duration);
        Debug.DrawLine(points[1], points[2], color, duration);
        Debug.DrawLine(points[2], points[3], color, duration);
        Debug.DrawLine(points[3], points[0], color, duration);

        Debug.DrawLine(points[4], points[5], color, duration);
        Debug.DrawLine(points[5], points[6], color, duration);
        Debug.DrawLine(points[6], points[7], color, duration);
        Debug.DrawLine(points[7], points[4], color, duration);

        Debug.DrawLine(points[0], points[4], color, duration);
        Debug.DrawLine(points[1], points[5], color, duration);
        Debug.DrawLine(points[2], points[6], color, duration);
        Debug.DrawLine(points[3], points[7], color, duration);
    }

    public void DestroyMapObject()
    {
        // 사라지는 애니메이션
        transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 0.5f)
                 .SetEase(Ease.InBack)
                 .OnComplete(() =>
                 {
                     Destroy(gameObject);
                 });
    }


}
