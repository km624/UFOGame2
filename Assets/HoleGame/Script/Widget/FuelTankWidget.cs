using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelTankWidget: MonoBehaviour
{
    private float CurrentExpGauge;
    [Header("초기 경험치량")]
    [SerializeField]
    private float MaxExpGauge = 400.0f;

    private float fillSpeed = 2.0f;

    private float TargetfillPercent = 0.0f;
    [SerializeField]
    private Image EXPGaugeBar;

   
   
    /*[SerializeField]
    private Image Boss;*/


    public void SetThermometerData()
    {
        EXPGaugeBar.fillAmount = 0.0f;
        CurrentExpGauge = 0;
        TargetfillPercent = 0;
        //Debug.Log("Generation Set");

     
        //Boss.sprite = ShapeManager.Instance.GetShapeSprite(generationdata.Boss.GetShapeEnum());*/
    }

    void FixedUpdate()
    {

        UpdateSizeGaugeBar();

    }

    private void UpdateSizeGaugeBar()
    {

        EXPGaugeBar.fillAmount = Mathf.Lerp(EXPGaugeBar.fillAmount, TargetfillPercent, Time.deltaTime * fillSpeed);
    }

    public void InitFuelTank()
    {
        CurrentExpGauge = 0;
        TargetfillPercent = 0.0f;

    }

    public void AddEXPGauge(float gauge)
    {

        CurrentExpGauge += gauge;
    
        TargetfillPercent = Mathf.Clamp01(CurrentExpGauge / MaxExpGauge);

    }

}
