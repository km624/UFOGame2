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
    private float MaxExpGauge = 100.0f;

    float baseExpPerMass = 100.0f;

   //[Header("레벨에 따른 최대 경험치 추가량")]
    //[SerializeField]
   // private float AddMaxExp = 25.0f;

    private float fillSpeed = 3.0f;
    private float TargetfillPercent = 0.0f;

   
    //private List<ExelPlayerData> PlayerStatList = new List<ExelPlayerData>();

    //private List<ExelUFOStatData> UFOBaseStatList = new List<ExelUFOStatData>();
    private ExelUFOStatData UFOBaseStatList;

    //private List<ExelUFOStatData> UFOStatTimeList = new List<ExelUFOStatData>();
    private ExelUFOStatData UFOStatTimeList;


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
   

   
   

    private float DefaultCameraDistance = 0.0f;

    public Material defaultMaterial { get; private set; } // 기본 머티리얼

    public Vector3 WorldPosition => transform.position;

    /*//최소 스텟
    private float MinMoveSpeed = 5f;
    private float MinLiftSpeed = 10f;
    private float MinBeamRange = 0.7f;
    private float MinTextOffset = -1.9f;

    
    //배율
    private float MoveSpeedTimes = 2.5f;
    private float LiftSpeedTimes = 10.0f;
    private float BeamRangeTimes = 0.15f;
    private float TextOffsetTimes = -0.3f;
*/



    void Start()
    {
       LoadPlayerStatList();
        
        InitUFO();

    }

    private void InitUFO()
    {
        if (GameManager.Instance != null)
        {
            int selectUFOIndex = GameManager.Instance.userData.CurrentUFO;
            UFOData selectUFOdata = UFOLoadManager.Instance.LoadedUFODataList[selectUFOIndex];
            if (selectUFOdata != null)
            {
                UserUFOData userufodata = GameManager.Instance.userData.serialUFOList.Get(selectUFOdata.UFOName);
                SetUFOData(selectUFOdata , userufodata);
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
       // PlayerStatList = CsvLoader.LoadCSV<ExelPlayerData>("StatData/CSVPlayerEXPList");

        UFOBaseStatList = CsvLoader.LoadSingleCSV <ExelUFOStatData>("StatData/CSVUFOBaseStat");
        UFOStatTimeList = CsvLoader.LoadSingleCSV <ExelUFOStatData>("StatData/CSVUFOStatTime");

        //MaxLevel = PlayerStatList.Count;

        //SetUFOLevelData(0);

    }

   /* private void SetUFOLevelData(int listnum)
    {
        if (listnum >= PlayerStatList.Count)
        {
            Debug.Log("데이터 없음");
            return;
        }
        //MaxExpGauge = PlayerStatList[listnum].MaxExp;
        //CurrentLevel = PlayerStatList[listnum].Level;
    }*/


    void FixedUpdate()
    {

        UpdateSizeGaugeBar();

    }

    private void OnDisable()
    {
        UFORenderer.material = defaultMaterial;
    }

    public void SetUFOData(UFOData ufodata,UserUFOData userUFOdata)
    {

        MeshFilter meshFilter = ufoModel.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            meshFilter.mesh = ufodata.UFOMesh;
        }
        MeshRenderer meshRenderer = ufoModel.GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            if (ufodata.UFOColorDataList != null && ufodata.UFOColorDataList.Count > 0)
            {
                int selectcolor = userUFOdata.CurrentColorIndex;
                meshRenderer.materials = ufodata.UFOColorDataList[selectcolor].Materials.ToArray();
            }
        }
        if (UFOBaseStatList!=null && UFOStatTimeList!=null)
        {
            if (ufoMovement != null)
            {
                int movevalue = userUFOdata.GetReinforceValue(UFOStatEnum.MoveSpeed) - 1;
                float newspeed = UFOBaseStatList.Movespeed +  movevalue * UFOStatTimeList.Movespeed;

                Debug.Log("UFOSpeed : " + newspeed);
                ufoMovement.SetSpeed(newspeed);
            }
            if (UFOBeam != null)
            {
                int liftvalue = userUFOdata.GetReinforceValue(UFOStatEnum.LiftSpeed) -1;
                float newlift = UFOBaseStatList.LiftSpeed + liftvalue * UFOStatTimeList.LiftSpeed;
                Debug.Log("UFOLiftspeed : " + newlift);
                UFOBeam.SetInitLiftSpeed(newlift);

                int rangevalue = userUFOdata.GetReinforceValue(UFOStatEnum.BeamRange) - 1;
                float newrange = UFOBaseStatList.BeamRange + rangevalue * UFOStatTimeList.BeamRange;
                Debug.Log("Beamrange : " + newrange);
                UFOBeam.SetBeamRange(newrange);

                if(LevelText != null)
                {
                    int textoffsetvalue = userUFOdata.GetReinforceValue(UFOStatEnum.BeamRange) - 1;
                    float newoffset = UFOBaseStatList.TextOffset + rangevalue * UFOStatTimeList.TextOffset;
                    Vector3 pos = LevelText.transform.position;
                    pos.y += newoffset;
                    LevelText.transform.position = pos;
                }

            }
        }

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
        //Debug.Log("Taime : " + gauge);
        SpawnGagueEffect(gauge);

        if (CurrentLevel >= MaxLevel) return;

        //if (CurrentLevel >= PlayerStatList.Count) return;

        //CurrentExpGauge += gauge;
        Debug.Log(" 내 레벨 : " + CurrentLevel + " 질량 : "+ mass);
        float newGague = CalculateExpGain(CurrentLevel, mass);

        Debug.Log("겅혐치 : " + newGague);
        CurrentExpGauge += newGague;
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

        if (absorbedMass == MaxLevel)
            return 0f;

        float levelDiff = level - absorbedMass;

        // 지수 감소: 2차이
        float expMultiplier = Mathf.Pow(0.5f, levelDiff);

        // 2n-n = 증가 효과도 포함됨
        float exp = baseExpPerMass * expMultiplier;

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
        
        
        //CurrentLevel = Math.Clamp(CurrentLevel, 0, PlayerStatList.Count);


       // SetUFOLevelData(CurrentLevel - 1);  

        UFOBeam.SetSwallowLevelSet(CurrentLevel);
        trigger.SetCurrentLevel(CurrentLevel);
        possibleTrigger.SetLevel(CurrentLevel);

        UpdateSizeText(CurrentLevel);
    }

    public void MaxLevelLimitUp()
    {
        MaxLevel  += 4;
    }

    public void SetCurrentBoss(BossObject boss)
    {
        Debug.Log("보스 세팅 : " +  boss.name);
        UFOBeam.SetCurrentBoss(boss);
    }

    private void UpdateSizeText(int currentsizelevel)
    {
      /* if(currentsizelevel == PlayerStatList.Count)
            LevelText.text = "레벨: " + "MAX";
       else*/
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
