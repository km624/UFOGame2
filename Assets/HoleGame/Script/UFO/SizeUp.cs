using UnityEngine;

public class SizeUp : MonoBehaviour
{
    private float CurrentSizeGauge;
    private float MaxSizeGauge = 100.0f;
    
    private int CurrentSizeLevel = 1;
    public float AddSizeUp = 0.5f;

    private float targetSize;
    private float targetHeight;
    private float Holevelocity = 0.0f;
    private float Heightvelocity = 0.0f;
    private float smoothTime = 0.2f;

    private float initialHoleSize;

    [Header("UFO 개발자란")]
    [SerializeField]
    private CameraWalk CameraScript;
    void Start()
    {
        initialHoleSize = transform.localScale.x;
        targetSize = initialHoleSize;
        targetHeight = transform.position.y;
    }


    void FixedUpdate()
    {


        UpdateHoleSize();
    }

    public void AddSizeGauge(float gauge, FallingObject fallingobj)
    {

        CurrentSizeGauge += gauge;

        if (CurrentSizeGauge >= MaxSizeGauge)
        {
            CurrentSizeGauge -= MaxSizeGauge;
            //SizeGaugeBar.fillAmount = 0.0f;
            ChangeSize(true);
        }
        //TargetfillPercent = Mathf.Clamp01(CurrentSizeGauge / MaxSizeGauge);

        //SpawnGagueEffect(gauge);

        //gameState.RemoveFallingObject(fallingobj);
    }

    public void ChangeSize(bool bSizeUp)
    {
        if (bSizeUp)
        {
            CurrentSizeLevel++;
            Vector3 CurrentScale = transform.localScale;

            targetSize = CurrentScale.x + AddSizeUp;

            targetHeight = transform.position.y + (1 + AddSizeUp);

        }
        else
        {
            CurrentSizeLevel--;
            Vector3 CurrentScale = transform.localScale;
            targetSize = CurrentScale.x - AddSizeUp;
            targetHeight = transform.position.y - (1 + AddSizeUp);
        }
        float SizePercent = targetSize / initialHoleSize;


        CameraScript.CallBack_CameraDistancedUp(SizePercent);
        //UpdateSizeText(CurrentSizeLevel);
    }


    private void UpdateHoleSize()
    {

        //사이즈 업데이트
        float NewSizeupSclale = Mathf.SmoothDamp(transform.localScale.x, targetSize, ref Holevelocity, smoothTime);
        transform.localScale = new Vector3(NewSizeupSclale, NewSizeupSclale, NewSizeupSclale);

        //ufo 높이
        float newY = Mathf.SmoothDamp(transform.position.y, targetHeight, ref Heightvelocity, smoothTime);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);


    }
}
