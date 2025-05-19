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
    
            // 1. ���� ũ�� = box.size  ��  lossyScale   (�� ����!)
            Vector3 worldSize = Vector3.Scale(box.size, transform.lossyScale) *30f;
            Vector3 halfExtents = worldSize * 0.5f;

            // 2. ���� �߽� = TransformPoint(box.center)
            Vector3 center = transform.TransformPoint(box.center);

            // 3. ���� �˻�
           

            // 4. ����� �ð�ȭ
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
                    pushDir = pushDir.normalized; // �ٽ� ����ȭ (���� ����)

                    // ���� �༭ �о�ų�, ��ġ�� �����̵���Ŵ
                    col.attachedRigidbody.MovePosition(col.transform.position + pushDir * 3f);
                }
                else
                {
                    // ������ٵ� ���� ��� �׳� ��ġ �̵�
                    Vector3 pushDir = (col.transform.position - center).normalized;
                    if (pushDir == Vector3.zero) pushDir = Random.insideUnitSphere;
                    col.transform.position += pushDir * 3f;
                }
                Debug.Log("�΋H��" + col.name);
            }
        }

        // �ִϸ��̼�
        transform.DOScale(InitScale, 1f)
            .SetEase(Ease.OutBack);

    }

    public void DebugBox(Vector3 center, Vector3 size, Quaternion rotation, Color color, float duration = 0)
    {
        Vector3 halfSize = size * 0.5f;

        // 8�� ������ ��� (ȸ�� �ݿ�)
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
        // ������� �ִϸ��̼�
        transform.DOScale(new Vector3(0.01f, 0.01f, 0.01f), 0.5f)
                 .SetEase(Ease.InBack)
                 .OnComplete(() =>
                 {
                     Destroy(gameObject);
                 });
    }


}
