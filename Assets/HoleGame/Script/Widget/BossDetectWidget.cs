using UnityEngine;
using UnityEngine.UI;

public class BossDetectWidget : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform radarCircleRect;      // ���� UI (RectTransform)
    [SerializeField] private RectTransform arrowRect;            // ȭ��ǥ UI (RectTransform)
    [SerializeField] private RectTransform IconRect;            // ȭ��ǥ UI (RectTransform)
    private Transform bossTransform;            // ���� ���� ������Ʈ
    [SerializeField] private Camera mainCamera;                  // ���� ī�޶�

    [Header("Settings")]
               
    [SerializeField]private float arrowOffset = 50f;             // �� �ѷ����� �ణ ���� ���� ���
    [SerializeField]private float cornorradius = 10f;             // �� �ѷ����� �ణ ���� ���� ���

    private bool bIsBossDetected = false;

    public void SetBosstransform(Transform bosstransform)
    {
        Debug.Log(bosstransform.name + " ���� ��ġ ����");
        bossTransform = bosstransform;
    }
    public void ActiveBossDetect(bool actvie)
    {
        bIsBossDetected = actvie;
        //Debug.Log(bIsBossDetected);
        if (!bIsBossDetected)
        {
            arrowRect.gameObject.SetActive(bIsBossDetected);
            IconRect.gameObject.SetActive(bIsBossDetected);
        }
           
    }

    private void Update()
    {
        UpdateBossArrow();

       
    }

   /* private void UpdateBossArrow()
    {
        if (!bIsBossDetected|| bossTransform == null) return;
        bool isBossVisible = IsBossVisible(mainCamera, bossTransform);
        //Debug.Log("UpdataeBoss");
        arrowRect.gameObject.SetActive(!isBossVisible);
     
        if (!isBossVisible)
        {
            Vector3 arrowWorldPos = GetWorldPositionOnCircle(radarCircleRect, bossTransform.position);
            arrowRect.position = arrowWorldPos;

            // ��� ���� ���� ȸ�� (XZ ����)
            Vector3 dir = (bossTransform.position - radarCircleRect.position);
            dir.y = 0f;

            float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
            arrowRect.localEulerAngles = new Vector3(0, 0, angle - 90); // �Ǵ� +9
        }
    }*/
    private void UpdateBossArrow()
    {
       if (!bIsBossDetected || bossTransform == null) return;
       //if ( bossTransform == null) return;
        bool isBossVisible = IsBossVisible(mainCamera, bossTransform);
       
        arrowRect.gameObject.SetActive(!isBossVisible);
        IconRect.gameObject.SetActive(!isBossVisible);

        if (!isBossVisible)
        {
            Vector2 arrowWorldPos = GetScreenPositionOnRoundedRect(radarCircleRect, bossTransform,mainCamera, cornorradius, arrowOffset);
            //Vector2 iconWorldPos = GetScreenPositionOnRoundedRect(radarCircleRect, bossTransform,mainCamera, cornorradius, iconOffset + arrowOffset);
            arrowRect.position = arrowWorldPos;
            //IconRect.position = iconWorldPos;

            // 3) ��ũ�� ���� ���� ���
            Vector2 center = radarCircleRect.position;
            //Vector2 dir = (iconWorldPos - center).normalized;
            Vector2 dir = (arrowWorldPos - center).normalized;

            // 4) ��ũ�� ���� ȸ��
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
           
            arrowRect.localEulerAngles = new Vector3(0, 0, angle - 90); // -90�� ȭ��ǥ �⺻ ���� ����
            IconRect.rotation = Quaternion.identity;
        }
    }

    private Vector3 GetScreenPositionOnRoundedRect(RectTransform rectTransform, Transform bossTransform, Camera cam, float cornerRadius, float insideOffset)
    {
        Vector2 center = rectTransform.position;
       
        //Vector2 bossScreenPos = cam.WorldToScreenPoint(bossTransform.position);
        //Vector2 center = RectTransformUtility.WorldToScreenPoint(cam, rectTransform.position);
        Vector3 bossScreen3D = cam.WorldToScreenPoint(bossTransform.position);
        if (bossScreen3D.z < 0f)
        {
            bossScreen3D.x = -bossScreen3D.x;
            bossScreen3D.y = -bossScreen3D.y;
        }

        Vector2 bossScreenPos = new Vector2(bossScreen3D.x, bossScreen3D.y);
        Vector2 direction = bossScreenPos - center;

        Vector2 rectSize = new Vector2(
            rectTransform.rect.width * rectTransform.lossyScale.x,
            rectTransform.rect.height * rectTransform.lossyScale.y
        );

        Vector2 localPos = direction;

        float halfWidth = rectSize.x / 2f - cornerRadius;
        float halfHeight = rectSize.y / 2f - cornerRadius;

        // �⺻ Clamp
        localPos.x = Mathf.Clamp(localPos.x, -halfWidth, halfWidth);
        localPos.y = Mathf.Clamp(localPos.y, -halfHeight, halfHeight);

        // �ڳ� ����
        if (Mathf.Abs(direction.x) > halfWidth && Mathf.Abs(direction.y) > halfHeight)
        {
            Vector2 cornerCenter = new Vector2(
                Mathf.Sign(direction.x) * halfWidth,
                Mathf.Sign(direction.y) * halfHeight
            );

            Vector2 fromCorner = direction - cornerCenter;
            fromCorner = fromCorner.normalized * cornerRadius;
            localPos = cornerCenter + fromCorner;
        }

        // �߰�: �������� �б�
        if (localPos != Vector2.zero)
        {
            localPos -= localPos.normalized * insideOffset;
        }

        return center + localPos;
    }

    private bool IsBossVisible(Camera cam, Transform boss)
    {
        Vector3 viewportPos = cam.WorldToViewportPoint(boss.position);
        return viewportPos.z > 0 &&
               viewportPos.x > 0 && viewportPos.x < 1 &&
               viewportPos.y > 0 && viewportPos.y < 1;
    }

  
}

