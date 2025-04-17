using DamageNumbersPro;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;

using UnityEngine;
using UnityEngine.UI;


public class UFOPlayer : MonoBehaviour, IDetctable
{
    //private GameState gameState;

    [Header("레벨 스텟")]
    //private float LiftSpeed = 10f;  // 끌어당기는 힘
    public int CurrentLevel { get; private set; } = 1;
    
    private int MaxLevel = 5;

    private float CurrentExpGauge;
    [Header("초기 경험치량")]
    [SerializeField]
   // private float MaxExpGauge = 25.0f;
    private float MaxExpGauge = 100.0f;

    float baseExpPerMass = 50.0f;

   //[Header("레벨에 따른 최대 경험치 추가량")]
    //[SerializeField]
   // private float AddMaxExp = 25.0f;

    private float fillSpeed = 3.0f;
    private float TargetfillPercent = 0.0f;

    [SerializeField]
    private List<ExelPlayerData> PlayerStatList = new List<ExelPlayerData>();


    [Header("UFO UI")]
    [SerializeField]
    private Image EXPGaugeBar;
    [SerializeField]
    //private Text LevelText;
    private TMP_Text LevelText;
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
    [SerializeField]
    private PossibleTrigger possibleTrigger;
    //[SerializeField]
    //private GameObject AddRangeLevelWidget;

   
   

    private float DefaultCameraDistance = 0.0f;

    public Material defaultMaterial { get; private set; } // 기본 머티리얼

    public Vector3 WorldPosition => transform.position;

    //최소 스텟
    private float MinMoveSpeed = 5f;
    private float MinLiftSpeed = 10f;
    private float MinBeamRange = 0.7f;
    private float MinTextOffset = -1.9f;

    
    //배율
    private float MoveSpeedTimes = 2.5f;
    private float LiftSpeedTimes = 10.0f;
    private float BeamRangeTimes = 0.15f;
    private float TextOffsetTimes = -0.3f;




    void Start()
    {
        LoadPlayerStatList();
        
        InitUFO();

    }

    private void InitUFO()
    {
        if (GameManager.Instance != null)
        {
            UFOData selectUFOdata = UFOLoadManager.Instance.selectUFOData;
            if (selectUFOdata != null)
            {
                SetUFOData(selectUFOdata);
            }

        }
        //default 값 설정
        if (CamerapositionComposer != null)
        {
            DefaultCameraDistance = CamerapositionComposer.CameraDistance;
        }
        if (UFORenderer != null)
        {
            defaultMaterial = UFORenderer.material;
        }

        //레벨 세팅
        if (UFOBeam != null)
            UFOBeam.SetSwallowLevelSet(CurrentLevel);

        if (trigger != null)
            trigger.SetCurrentLevel(CurrentLevel);

        if (possibleTrigger != null)
            possibleTrigger.SetLevel(CurrentLevel);

        EXPGaugeBar.fillAmount = 0.0f;

        //레벨 표시
        UpdateSizeText(CurrentLevel);
    }

   
    private void LoadPlayerStatList()
    {
        PlayerStatList = CsvLoader.LoadCSV<ExelPlayerData>("StatData/PlayerEXPList");

        //MaxLevel = PlayerStatList.Count;

        SetUFOLevelData(0);

    }

    private void SetUFOLevelData(int listnum)
    {
        if (listnum >= PlayerStatList.Count)
        {
            Debug.Log("데이터 없음");
            return;
        }
        MaxExpGauge = PlayerStatList[listnum].MaxExp;
        CurrentLevel = PlayerStatList[listnum].Level;
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

       /* if (ufoMovement != null)
        {
            ufoMovement.SetSpeed(ufodata.MoveSpeed);
        }
        if (UFOBeam != null)
        {
            UFOBeam.SetInitLiftSpeed(ufodata.LiftSpeed);
        }*/

    }

    public void CallBack_StopMovement()
    {
        ufoMovement.SetMoveActive(false);
    }

    public void Skill_UFOspeedUp(bool bactivate)
    {
        if (ufoMovement != null)
        {
            float newspeed = bactivate ? ufoMovement.GetHoleSpeed() + 5.0f : ufoMovement.GetHoleSpeed() - 5.0f;
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

    public void AddEXPGauge(float gauge , float mass)
    {
        SpawnGagueEffect(gauge);

        if (CurrentLevel >= MaxLevel) return;

        if (CurrentLevel >= PlayerStatList.Count) return;

        CurrentExpGauge += gauge;

        //float newGague = CalculateExpGain(CurrentLevel, mass);
       
        //CurrentExpGauge += newGague;
        //Debug.Log("UFOPLAYEr : " + newGague);
       

        if (CurrentExpGauge >= MaxExpGauge)
        {
            if(CurrentLevel < MaxLevel - 1)
            {
                CurrentExpGauge -= MaxExpGauge;
            }
            else
            {
                CurrentExpGauge = 0.0f;
            }
            

            EXPGaugeBar.fillAmount = 0.0f;

            ChangeLevel(true);

          
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
        
        
        CurrentLevel = Math.Clamp(CurrentLevel, 0, PlayerStatList.Count);


        SetUFOLevelData(CurrentLevel - 1);  

        UFOBeam.SetSwallowLevelSet(CurrentLevel);
        trigger.SetCurrentLevel(CurrentLevel);
        possibleTrigger.SetLevel(CurrentLevel);

        UpdateSizeText(CurrentLevel);
    }

    public void MaxLevelLimitUp()
    {
        MaxLevel += 4;
    }

    public void SetCurrentBoss(BossObject boss)
    {
        UFOBeam.SetCurrentBoss(boss);
    }

    private void UpdateSizeText(int currentsizelevel)
    {
       if(currentsizelevel == PlayerStatList.Count)
            LevelText.text = "레벨: " + "MAX";
       else
            LevelText.text = "레벨: " + currentsizelevel.ToString();
        
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
