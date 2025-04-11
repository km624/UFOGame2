using UnityEngine;
using UnityEngine.UI;

public class ThermometerWidget : MonoBehaviour
{
    private float CurrentExpGauge;
    [Header("초기 경험치량")]
    [SerializeField]
    private float MaxExpGauge = 500.0f;

    private float fillSpeed = 2.0f;

    private float TargetfillPercent = 0.0f;
    [SerializeField]
    private Image EXPGaugeBar;

    [SerializeField]
    private Image Level1;
    [SerializeField]
    private Image Level2;
    [SerializeField]
    private Image Level3;
    [SerializeField]
    private Image Level4;
    [SerializeField]
    private Image Boss;


    public void SetThermometerData(GenerationObjects generationdata)
    {
        EXPGaugeBar.fillAmount = 0.0f;
        //Debug.Log("Generation Set");
    }

    void FixedUpdate()
    {

        UpdateSizeGaugeBar();

    }

    private void UpdateSizeGaugeBar()
    {

        EXPGaugeBar.fillAmount = Mathf.Lerp(EXPGaugeBar.fillAmount, TargetfillPercent, Time.deltaTime * fillSpeed);
    }

    public void AddEXPGauge(float gauge)
    {

        CurrentExpGauge += gauge;
    
        TargetfillPercent = Mathf.Clamp01(CurrentExpGauge / MaxExpGauge);

    }

}
