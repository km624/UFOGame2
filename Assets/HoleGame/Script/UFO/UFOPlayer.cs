using DamageNumbersPro;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

using UnityEngine.UI;


public class UFOPlayer : MonoBehaviour
{
    //private GameState gameState;

    [Header("레벨 스텟")]
    //private float LiftSpeed = 10f;  // 끌어당기는 힘
    public int CurrentLevel { get; private set; } = 1;
    //private int MaxLevel = 5;

    private float CurrentExpGauge;
    [Header("초기 경험치량")]
    [SerializeField]
   // private float MaxExpGauge = 25.0f;
    private float MaxExpGauge = 100.0f;

    float baseExpPerMass = 10f;

    [Header("레벨에 따른 최대 경험치 추가량")]
    //[SerializeField]
   // private float AddMaxExp = 25.0f;

    private float fillSpeed = 3.0f;
    private float TargetfillPercent = 0.0f;


    [Header("UFO UI")]
    [SerializeField]
    private Image EXPGaugeBar;
    [SerializeField]
    private Text LevelText;
    [SerializeField]
    private DamageNumber numberPrefab;

    [Header("UFO Sound")]
    public AudioSource AddGaugeSound;

    [Header(" 개발자 ")]
    [SerializeField]
    private UFOMovement ufoMovement;
    [SerializeField]
    private HoleScript UFOBeam;
    [SerializeField]
    private GameObject ufoModel;
    [SerializeField]
    private CinemachinePositionComposer CamerapositionComposer;
    [SerializeField]
    private Renderer UFORenderer;
    [SerializeField]
    private FallingTrigger trigger;

    public event Action<float /**/> FOnExpGagueAdded;
   

    private float DefaultCameraDistance = 0.0f;

    public Material defaultMaterial { get; private set; } // 기본 머티리얼
    

    void Start()
    {
        if (GameManager.Instance != null)
        {
            UFOData selectUFOdata = UFOLoadManager.Instance.selectUFOData;
            if (selectUFOdata != null)
            {
                SetUFOData(selectUFOdata);
            }
           
        }
        if (CamerapositionComposer != null)
        {
            DefaultCameraDistance = CamerapositionComposer.CameraDistance;

        }
        if (UFORenderer != null)
        {
            defaultMaterial = UFORenderer.material;
        }

        if (UFOBeam != null)
            UFOBeam.SetSwallowLevelSet(CurrentLevel);

        if(trigger!=null)
            trigger.SetCurrentLevel(CurrentLevel);

        UpdateSizeText(CurrentLevel);
    }


    void FixedUpdate()
    {

        UpdateSizeGaugeBar();

    }

    private void OnDisable()
    {
        UFORenderer.material = defaultMaterial;
    }

    public void SetUFOData(UFOData ufodata)
    {

        MeshFilter meshFilter = ufoModel.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            meshFilter.mesh = ufodata.UFOMesh;
        }
        MeshRenderer meshRenderer = ufoModel.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            if (ufodata.UFOMaterials != null && ufodata.UFOMaterials.Count > 0)
            {

                meshRenderer.materials = ufodata.UFOMaterials.ToArray();
            }
        }

        if (ufoMovement != null)
        {
            ufoMovement.SetSpeed(ufodata.MoveSpeed);
        }
        if (UFOBeam != null)
        {
            UFOBeam.SetInitLiftSpeed(ufodata.LiftSpeed);
        }

    }

    public void CallBack_StopMovement()
    {
        ufoMovement.SetMoveActive(false);
    }

    public void SkillUFOspeedUp(bool bactivate)
    {
        if (ufoMovement != null)
        {
            int newspeed = bactivate ? ufoMovement.HoleSpeed + 5 : ufoMovement.HoleSpeed - 5;
            ufoMovement.SetSpeed(newspeed);

        }
    }

    public void ChangeUFOMaterial(Material newmaterial)
    {
        UFORenderer.material = newmaterial;
    }

    private void UpdateSizeGaugeBar()
    {

        EXPGaugeBar.fillAmount = Mathf.Lerp(EXPGaugeBar.fillAmount, TargetfillPercent, Time.deltaTime * fillSpeed);
    }

    public void AddEXPGauge(float gauge,float mass)
    {
        SpawnGagueEffect(gauge);

        float newGague = CalculateExpGain(CurrentLevel, mass);
        //CurrentExpGauge += gauge;
        CurrentExpGauge += newGague;
        Debug.Log("UFOPLAYEr : " + newGague);
        FOnExpGagueAdded?.Invoke(newGague);

        if (CurrentExpGauge >= MaxExpGauge)
        {
            
            CurrentExpGauge -= MaxExpGauge;

            EXPGaugeBar.fillAmount = 0.0f;

            ChangeLevel(true);

            //MaxExpGauge += AddMaxExp;
        }
        TargetfillPercent = Mathf.Clamp01(CurrentExpGauge / MaxExpGauge);

        //SpawnGagueEffect(gauge);
    }
    public float CalculateExpGain(int level, float absorbedMass)
    {

        float exp = baseExpPerMass * absorbedMass / Mathf.Pow(2f, level - 1);
        return exp;
    }


    void SpawnGagueEffect(float gauge)
    {
        AddGaugeSound.Play();
        DamageNumber damageNumber = numberPrefab.Spawn(transform.position, gauge);
    }

    public void SwallowSound()
    {
        AddGaugeSound.Play();
    }

    public void ChangeLevel(bool bLevelUp)
    {

        CurrentLevel += bLevelUp ? 1 : -1;
      
        UFOBeam.SetSwallowLevelSet(CurrentLevel);
        trigger.SetCurrentLevel(CurrentLevel);
       
        UpdateSizeText(CurrentLevel);
    }

    private void UpdateSizeText(int currentsizelevel)
    {
       
       LevelText.text = "레벨: " + currentsizelevel;
        
    }

    public void ChangeCameraDistance(float cameradistance)
    {
        CamerapositionComposer.CameraDistance = cameradistance;
    }

    public void ResetCameraDistance()
    {
        CamerapositionComposer.CameraDistance = DefaultCameraDistance;
    }


}
