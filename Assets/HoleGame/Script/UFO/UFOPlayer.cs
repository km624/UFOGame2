using DamageNumbersPro;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using Unity.Cinemachine;

using UnityEngine;
using UnityEngine.UI;


public class UFOPlayer : MonoBehaviour, IDetctable
{
    //private GameState gameState;

    [Header("���� ����")]
    //private float LiftSpeed = 10f;  // ������� ��
    public int CurrentLevel { get; private set; } = 1;
    private int MaxLevel = 5;

    private float CurrentExpGauge;
    [Header("�ʱ� ����ġ��")]
    [SerializeField]
   // private float MaxExpGauge = 25.0f;
    private float MaxExpGauge = 100.0f;

    float baseExpPerMass = 50.0f;

   //[Header("������ ���� �ִ� ����ġ �߰���")]
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

    [Header(" ������ ")]
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
    [SerializeField]
    private GameObject AddRangeLevelWidget;

   // public event Action<float /**/> FOnExpGagueAdded;
   

    private float DefaultCameraDistance = 0.0f;

    public Material defaultMaterial { get; private set; } // �⺻ ��Ƽ����

    public Vector3 WorldPosition => transform.position;


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

        if(possibleTrigger!=null)
            possibleTrigger.SetLevel(CurrentLevel);

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

        if (CurrentLevel >= MaxLevel) return;

        float newGague = CalculateExpGain(CurrentLevel, mass);
        //CurrentExpGauge += gauge;
        CurrentExpGauge += newGague;
        //Debug.Log("UFOPLAYEr : " + newGague);
        //FOnExpGagueAdded?.Invoke(newGague);

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
       
       LevelText.text = "����: " + currentsizelevel;
        
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
