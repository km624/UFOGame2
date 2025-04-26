using UnityEngine;
using UnityEngine.UI;

public class BossDetectWidget : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform radarCircleRect;      // 원형 UI (RectTransform)
    [SerializeField] private RectTransform arrowRect;            // 화살표 UI (RectTransform)
    private Transform bossTransform;            // 보스 월드 오브젝트
    [SerializeField] private Camera mainCamera;                  // 메인 카메라

    [Header("Settings")]
    [SerializeField]private float arrowOffset = 0f;             // 원 둘레에서 약간 띄우고 싶을 경우

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

            // 평면 방향 기준 회전 (XZ 기준)
            Vector3 dir = (bossTransform.position - radarCircleRect.position);
            dir.y = 0f;

            float angle = Mathf.Atan2(dir.z, dir.x) * Mathf.Rad2Deg;
            arrowRect.localEulerAngles = new Vector3(0, 0, angle - 90); // 또는 +9
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
        Vector3 flatBoss = new Vector3(bossWorldPos.x, center.y, bossWorldPos.z); // 높이 무시

        Vector3 direction = (flatBoss - center).normalized;
        float radius = (circleRect.rect.width * 0.5f) * circleRect.lossyScale.x + arrowOffset;

        return center + direction * radius;
    }
}

