using UnityEngine;
using UnityEngine.UI;

public class BossDetectWidget : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform radarCircleRect;      // ���� UI (RectTransform)
    [SerializeField] private RectTransform arrowRect;            // ȭ��ǥ UI (RectTransform)
    private Transform bossTransform;            // ���� ���� ������Ʈ
    [SerializeField] private Camera mainCamera;                  // ���� ī�޶�

    [Header("Settings")]
    [SerializeField]private float arrowOffset = 0f;             // �� �ѷ����� �ణ ���� ���� ���

    private bool bIsBossDetected = false;

    public void SetBosstransform(Transform bosstransform)
    {
        //Debug.Log(bosstransform);
        bossTransform = bosstransform;
    }
    public void ActiveBossDetect(bool actvie)
    {
        bIsBossDetected = actvie;
        //Debug.Log(bIsBossDetected);
        if (!bIsBossDetected)
            arrowRect.gameObject.SetActive(bIsBossDetected);
    }

    private void Update()
    {
        UpdateBossArrow();
    }

    private void UpdateBossArrow()
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
    }

    private bool IsBossVisible(Camera cam, Transform boss)
    {
        Vector3 viewportPos = cam.WorldToViewportPoint(boss.position);
        return viewportPos.z > 0 &&
               viewportPos.x > 0 && viewportPos.x < 1 &&
               viewportPos.y > 0 && viewportPos.y < 1;
    }

    private Vector3 GetWorldPositionOnCircle(RectTransform circleRect, Vector3 bossWorldPos)
    {
        Vector3 center = circleRect.position;
        Vector3 flatBoss = new Vector3(bossWorldPos.x, center.y, bossWorldPos.z); // ���� ����

        Vector3 direction = (flatBoss - center).normalized;
        float radius = (circleRect.rect.width * 0.5f) * circleRect.lossyScale.x + arrowOffset;

        return center + direction * radius;
    }
}

