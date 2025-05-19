
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class UFOModelAround : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
   
    public Transform UFOMesh;              // 회전시킬 UFO 모델
    public float RotationSpeed = 0.4f;     // 회전 감도
    public Canvas Canvas;
    private bool isDragging = false;

    private bool bStartDrag = false;
    [Header("Tuning")]
    public float dragSpeed = 0.3f;      // 감도
    public float lerpSpeed = 10f;       // 부드러움
    public float maxTiltDeg = 80f;       // (옵션) 위아래 한계

    /*  내부 상태  */
   
    private Quaternion targetRot;

    private Quaternion initialRotation;
    private Quaternion deltaRot = Quaternion.identity; // 드래그로 누적된 변화분
    void Awake()
    {
        initialRotation = UFOMesh.rotation;
        targetRot = initialRotation;
    }
  

    public void SetModelDrag(bool dragstart)
    {
        bStartDrag = dragstart;
    }

    public void ResetRotation()
    {
        targetRot = initialRotation;
        deltaRot = Quaternion.identity;
       
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!bStartDrag) return;
      
 
        isDragging = true;
      
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDragging) return;
        isDragging = false;
        ResetRotation();
    }

  
    public void OnDrag(PointerEventData e)
    {
        if (!bStartDrag) return;
        if (!isDragging) return;


        // 1. 드래그 양
        float dx = e.delta.x * dragSpeed;
        float dy = e.delta.y * dragSpeed;

        // 2. 카메라 기준 축
        Vector3 upAxis = Vector3.up;
        Vector3 rightAxis = Canvas.worldCamera.transform.right;

        // 3. 변화 쿼터니언 (증분)
        Quaternion qYaw = Quaternion.AngleAxis(-dx, upAxis);
        Quaternion qPitch = Quaternion.AngleAxis(dy, rightAxis);

        // 4. 누적 변화 업데이트
        deltaRot = qYaw * qPitch * deltaRot;

        // 5. 변화분의 Euler(상대각) 추출 & 클램프
        Vector3 relEuler = deltaRot.eulerAngles;
        relEuler.x = ClampAngleSigned(relEuler.x, -maxTiltDeg, maxTiltDeg);
        relEuler.z = ClampAngleSigned(relEuler.z, -maxTiltDeg, maxTiltDeg);
        deltaRot = Quaternion.Euler(relEuler);
    }

    /*────────────────────────────*/
    void Update()
    {
        // 초기 자세 ⨉ 변화분  →  최종 자세
        Quaternion target = initialRotation * deltaRot;

        UFOMesh.rotation = Quaternion.Slerp(
            UFOMesh.rotation, target, Time.deltaTime * lerpSpeed);
    }


    /*Utility*/
    private static float ClampAngleSigned(float angle, float min, float max)
    {
        angle = Mathf.DeltaAngle(0, angle);          // −180‥180 로 변환
        return Mathf.Clamp(angle, min, max);
    }
}
