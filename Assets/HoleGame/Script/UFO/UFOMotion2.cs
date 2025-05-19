using UnityEngine;

public class UFOMotion2 : MonoBehaviour
{
    [Header("References")]
    public Transform model; // ���� ���� �ڽ� ������Ʈ
    [Header("Speeds & Angles")]
    public float turnSpeedDeg = 180f;  // Yaw ȸ�� �ӵ� (��/��)
    public float pitchAngle = 15f;   // ���� �� �Ʒ��� ����
    public float rollAngle = 30f;   // ��/�� ����
    public float tiltSmooth = 5f;    // ���� ���� �ӵ�

    private Vector3 lastDir = Vector3.forward;
    private float curPitch = 0f;
    private float curRoll = 0f;

    [Header("���Ʒ� �̵� ����")]
    [Tooltip("���Ʒ� �̵��� ����"), Range(0.0f, 1.0f)]
    public float verticalLength = 1f;
    [Tooltip("���Ʒ� �ݺ� �ӵ�"), Range(0.5f, 2)]
    public float verticalSpeed = 1f;
    private float baseY;
    private bool bIsMotion = true;

    private float timeOffset; // ���� ���� ���� ������

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
        // 1) ȸ��: Yaw�� ȸ���ϵ��� y=0���� ����
        if (inputDir.sqrMagnitude > 0.0001f)
            lastDir = inputDir.normalized;

        Vector3 horizontalDir = new Vector3(lastDir.x, 0f, lastDir.z);
        if (horizontalDir.sqrMagnitude < 0.001f)
            horizontalDir = transform.forward;

        Quaternion targetRot = Quaternion.LookRotation(horizontalDir, Vector3.up);
        float step = turnSpeedDeg * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, step);

        // 2) ����
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

        // ���� Yaw �����ϰ� XZ�� ����̱�
        Vector3 angles = model.localEulerAngles;
        angles.x = curPitch;
        angles.z = curRoll;
        model.localEulerAngles = angles;
    }
}


