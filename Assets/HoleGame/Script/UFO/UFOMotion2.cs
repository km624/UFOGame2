using UnityEngine;

public class UFOMotion2 : MonoBehaviour
{
    [Header("References")]
    public Transform model; // 기울기 전용 자식 오브젝트
    [Header("Speeds & Angles")]
    public float turnSpeedDeg = 180f;  // Yaw 회전 속도 (도/초)
    public float pitchAngle = 15f;   // 전진 시 아래로 기울기
    public float rollAngle = 30f;   // 좌/우 기울기
    public float tiltSmooth = 5f;    // 기울기 보간 속도

    private Vector3 lastDir = Vector3.forward;
    private float curPitch = 0f;
    private float curRoll = 0f;

    [Header("위아래 이동 설정")]
    [Tooltip("위아래 이동의 높이"), Range(0.0f, 1.0f)]
    public float verticalLength = 1f;
    [Tooltip("위아래 반복 속도"), Range(0.5f, 2)]
    public float verticalSpeed = 1f;
    private float baseY;
    private bool bIsMotion = true;

    private float timeOffset; // 개별 위상 차이 유지용

    private void Start()
    {
        baseY = transform.localPosition.y;
    }

    private void Update()
    {
        if (!bIsMotion) return;
        float newY = baseY + verticalLength * Mathf.Sin(Time.time * verticalSpeed * 2 * Mathf.PI);
        transform.localPosition = new Vector3(transform.localPosition.x, newY, transform.localPosition.x);
    }

    public void ChangeBaseY(float addy)
    {
        baseY += addy;
    }

    public void SetMotionStart(bool motion)
    {
        bIsMotion = motion;
    }

    public void UpdateMotion(Vector3 inputDir)
    {
        // 1) 회전: Yaw만 회전하도록 y=0으로 고정
        if (inputDir.sqrMagnitude > 0.0001f)
            lastDir = inputDir.normalized;

        Vector3 horizontalDir = new Vector3(lastDir.x, 0f, lastDir.z);
        if (horizontalDir.sqrMagnitude < 0.001f)
            horizontalDir = transform.forward;

        Quaternion targetRot = Quaternion.LookRotation(horizontalDir, Vector3.up);
        float step = turnSpeedDeg * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, step);

        // 2) 기울기
        float targetPitch = 0f;
        float targetRoll = 0f;

        if (inputDir.sqrMagnitude > 0.0001f)
        {
            Vector3 localDir = transform.InverseTransformDirection(inputDir.normalized);
            targetPitch = localDir.z * pitchAngle;
            targetRoll = -localDir.x * rollAngle;
        }

        curPitch = Mathf.LerpAngle(curPitch, targetPitch, Time.deltaTime * tiltSmooth);
        curRoll = Mathf.LerpAngle(curRoll, targetRoll, Time.deltaTime * tiltSmooth);

        // 기존 Yaw 유지하고 XZ만 기울이기
        Vector3 angles = model.localEulerAngles;
        angles.x = curPitch;
        angles.z = curRoll;
        model.localEulerAngles = angles;
    }
}


