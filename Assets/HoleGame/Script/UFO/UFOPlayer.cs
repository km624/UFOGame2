using DamageNumbersPro;
using DG.Tweening;
using System;
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

    float baseExpPerMass = 8.0f;
    //float baseExpPerMass = 50.0f;
   

    public event Action<float> FOnExpAdded;
    public event Action<int/*curentLevel*/> FOnLevelUped;

    //private float fillSpeed = 3.0f;
    private float TargetfillPercent = 0.0f;

    private ExelUFOStatData UFOBaseStatList;
    private ExelUFOStatData UFOStatTimeList;


    [Header("UFO UI")]
    
    [SerializeField]
    private Image TimeGaugeBar; 
    [SerializeField]
    private Image TimeGaugeBar2;
   
    private int TimeMax = 30;
    private float fullTransitionDuration = 1.0f;
    private Tween _bar1Tween, _bar2Tween;
    private int _prevClamped = -1;
    [SerializeField]
    BossDetectWidget bossdetectwidget;

    [SerializeField]
    private TMP_Text LevelText;
    [SerializeField]
    private DamageNumber numberPrefab;
    //[SerializeField]
    //private DamageNumber LevelUpPrefab;
    [SerializeField]
    private LevelUpMotion levelupEffect;

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
    [SerializeField]
    private BeamColorChangeGradation BeamColor;
    [SerializeField]
    private UFODirecting ufodirection;

    public event Action FOnDirectionEnd;
    public event Action FonGenerationMoved;
    public event Action FOnWarpDirectionEnd;
   

    private float DefaultCameraDistance = 0.0f;

    public Material defaultMaterial { get; private set; } // 기본 머티리얼

    public Vector3 WorldPosition => transform.position;

    [SerializeField] private PossibleWidget possibleWidget;

    [SerializeField] private GameObject UFOWidget;

    [SerializeField] private UFOHeightController heightController;


 
    public void InitUFO(float gametime , UserUFOData userufodata , UFOData selectUFOdata , bool bisdirect)
    {
        LoadPlayerStatList();

       
      
        TimeGaugeBar.fillAmount = gametime / TimeMax;
        float Time2 = gametime - TimeMax;
        
        TimeGaugeBar2.fillAmount = Time2 / TimeMax;

        
        if(userufodata!=null && selectUFOdata!=null)
            SetUFOData(selectUFOdata, userufodata);

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
        {
            UFOBeam.InitBeam(this);
            UFOBeam.SetSwallowLevelSet(CurrentLevel);
        }


        if (trigger != null)
            trigger.SetCurrentLevel(CurrentLevel);

        if (possibleTrigger != null)
            possibleTrigger.SetLevel(CurrentLevel);

        if(BeamColor != null)
        {
            BeamColor.SetMaxLevel(MaxLevel);
            BeamColor.UpdateLevelSettings(CurrentLevel);
           
        }

       // EXPGaugeBar.fillAmount = 0.0f;

        //레벨 표시
        UpdateSizeText(CurrentLevel);
       
        //연출 있으면 시작
        if(bisdirect)
            StartUFODirecting();
        else
            FOnLevelUped?.Invoke(CurrentLevel);
    }

   
   private void LoadPlayerStatList()
   {

        UFOBaseStatList = CsvLoader.LoadSingleCSV <ExelUFOStatData>("StatData/CSVUFOBaseStat");
        UFOStatTimeList = CsvLoader.LoadSingleCSV<ExelUFOStatData>("StatData/CSVUFOStatTime");

   }


    public void StartUFODirecting()
    {
        if (ufodirection != null)
        {
            ufodirection.OnStartUFODirecct();

        }
        CameraDirection(true);
        DirectionProgress(true);
    }

    public void GenerationMoved()
    {
        FonGenerationMoved?.Invoke();
    }

    public void EndUFODirecting()
    {
        CameraDirection(false);
        DirectionProgress(false);
        FOnDirectionEnd?.Invoke();
        FOnLevelUped?.Invoke(CurrentLevel);

    }

    public void StartWarpDirecting()
    {
        if (ufodirection != null)
        {
            ufodirection.OnWarpStart();
        }
        CameraDirection(true);
        DirectionProgress(true);
    }

    public void EndWarpDirecting()
    {
        CameraDirection(false);
        DirectionProgress(false);
        FOnWarpDirectionEnd?.Invoke();
        FOnLevelUped?.Invoke(CurrentLevel);
    }

    private void DirectionProgress(bool bprogress)
    {
       
        UFOBeam.SetSwallow(!bprogress);
       
        UFOWidget.gameObject.SetActive(!bprogress);
        LevelText.gameObject.SetActive(!bprogress);

    }

    private void CameraDirection(bool bprogress)
    {
        if (bprogress)
        {
            ChangeCameraDistance(6.0f);

        }
        else
        {
            ResetCameraDistance();
        }
    }

   
  
    private void OnDisable()
    {
        
        UFORenderer.material = defaultMaterial;
       
    }
    
 
    public void CallBack_SetRemainTime(int remainTime)
    {
        // 1) 목표 fill 비율 계산
        int clamped = Mathf.Clamp(remainTime, 0, TimeMax * 2);
        float target1, target2;
        
        bool above = clamped > TimeMax;
        if (above)
        {
            target1 = 1f;
            target2 = (clamped - TimeMax) / (float)TimeMax;
        }
        else
        {
            target1 = clamped / (float)TimeMax;
            target2 = 0f;
        }

        // 2) 구간 전환 — 이전 구간과 달라질 때만 시퀀스 처리
        if (_prevClamped < 0 || (clamped > TimeMax) != (_prevClamped > TimeMax))
        {
           
            if (above)
            {
                StartSequence(
                    TimeGaugeBar, 1f,
                    TimeGaugeBar2, target2
                );
            }
            else
            {
                StartSequence(
                    TimeGaugeBar2, 0f,
                    TimeGaugeBar, target1
                );
            }
        }
        else
        {
            if (above)
            {
                KillTween(ref _bar2Tween);
                _bar1Tween?.Kill(); TimeGaugeBar.fillAmount = 1f;
                _bar2Tween = TimeGaugeBar2
                    .DOFillAmount(target2, fullTransitionDuration)
                    .SetEase(Ease.OutQuad);
            }
            else
            {
                KillTween(ref _bar1Tween);
                _bar2Tween?.Kill(); TimeGaugeBar2.fillAmount = 0f;
                _bar1Tween = TimeGaugeBar
                    .DOFillAmount(target1, fullTransitionDuration)
                    .SetEase(Ease.OutQuad);
            }
        }

        _prevClamped = clamped;
    }

   
    private void StartSequence(Image firstBar, float firstTarget, Image secondBar, float secondTarget)
    {
        
        KillTween(ref _bar1Tween);
        KillTween(ref _bar2Tween);

        _bar1Tween = firstBar
            .DOFillAmount(firstTarget, fullTransitionDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() =>
            {
                _bar2Tween = secondBar
                    .DOFillAmount(secondTarget, fullTransitionDuration)
                    .SetEase(Ease.OutQuad);
            });
    }

    private void KillTween(ref Tween t)
    {
        if (t != null && t.IsActive())
        {
            t.Kill();
            t = null;
        }
    }

   

    public void SetUFOData(UFOData ufodata,UserUFOData userUFOdata )
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
               Texture basemap = ufodata.UFOColorDataList[selectcolor].Materials[0].GetTexture("_BaseMap");
                
                //추가 오브젝트 세팅
                CreateAddCObject(ufodata,basemap);
                
                //올스텟 오브젝트 세팅
                if(userUFOdata.AllStat())
                {
                    CreateFullStatObject(ufodata,basemap);
                }

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
                    Vector3 pos = Vector3.zero;
                    pos.y += newoffset;
                    LevelText.transform.localPosition = pos;
                }

            }
        }
    }

    private void CreateAddCObject(UFOData currentUFOdata, Texture currentBaseMap)
    {

        // 새 인스턴스 생성
        foreach (var addobject in currentUFOdata.AddUFObject)
        {
            GameObject addobjectInstance = Instantiate(addobject, ufoModel.transform);

            if (addobjectInstance != null)
            {
                MeshRenderer renderer = addobjectInstance.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    Material[] originalMaterials = renderer.sharedMaterials;
                    Material[] newMaterials = new Material[originalMaterials.Length];

                    for (int i = 0; i < originalMaterials.Length; i++)
                    {
                        // 원본 머터리얼 복사
                        newMaterials[i] = new Material(originalMaterials[i]);

                        if (newMaterials[i].HasProperty("_BaseMap"))
                        {
                            newMaterials[i].SetTexture("_BaseMap", currentBaseMap);


                        }
                        // 새 머터리얼 배열 적용
                        renderer.materials = newMaterials;
                    }

                }
            }
        }
    }

    private void CreateFullStatObject(UFOData currentUFOdata, Texture BaseMap)
    {

        // 새 인스턴스 생성
        foreach (var addobject in currentUFOdata.AddFullStatObject)
        {
            GameObject addobjectInstance = Instantiate(addobject, ufoModel.transform);

            if (addobjectInstance != null)
            {
                MeshRenderer renderer = addobjectInstance.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    Material[] originalMaterials = renderer.sharedMaterials;
                    Material[] newMaterials = new Material[originalMaterials.Length];

                    for (int i = 0; i < originalMaterials.Length; i++)
                    {
                        // 원본 머터리얼 복사
                        newMaterials[i] = new Material(originalMaterials[i]);

                        if (newMaterials[i].HasProperty("_BaseMap"))
                        {
                            newMaterials[i].SetTexture("_BaseMap", BaseMap);
                        }

                    }

                    // 새 머터리얼 배열 적용
                    renderer.materials = newMaterials;
                }

            }
        }

    }

    public void CallBack_StopMovement(bool active)
    {
        ufoMovement.SetMoveActive(!active);
        UFOBeam.SetSwallow(!active);
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

  

    public void AddEXPGauge(float gauge , float mass)
    {
       
        SpawnGagueEffect(gauge);

        if (CurrentLevel >= MaxLevel) return;

        float newGague = CalculateExpGain(CurrentLevel, mass);

       
        CurrentExpGauge += newGague;
       
        FOnExpAdded?.Invoke(newGague);

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

            ChangeLevel(true);

          
        }
        TargetfillPercent = Mathf.Clamp01(CurrentExpGauge / MaxExpGauge);

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
        GameManager.Instance.soundManager.PlaySfx(SoundEnum.Swallowed);
       
        string gaugestring = "+" + string.Format("{0:N1}", gauge);
        numberPrefab.Spawn(transform.position, gauge);
    }

    public void SwallowSound()
    {
        GameManager.Instance.soundManager.PlaySfx(SoundEnum.Swallowed);
    }

    public void ChangeLevel(bool bLevelUp)
    {

        CurrentLevel += bLevelUp ? 1 : -1;

        FOnLevelUped?.Invoke(CurrentLevel);

        if (MaxLevel == CurrentLevel)
            bossdetectwidget.ActiveBossDetect(true);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.vibrationManager.Play(VibrationEnum.LevelUp);
            
            GameManager.Instance.soundManager.PlaySfx(SoundEnum.LevelUp, 0.3f);
        }

        /*Vector3 newpos = transform.position + Vector3.up*1;
        LevelUpPrefab.Spawn(newpos, "Level Up");*/

        levelupEffect.PlayPop();

        UFOBeam.SetSwallowLevelSet(CurrentLevel);
        trigger.SetCurrentLevel(CurrentLevel);
        possibleTrigger.SetLevel(CurrentLevel);

        BeamColor.UpdateLevelSettings(CurrentLevel);

        UpdateSizeText(CurrentLevel);
    }

    public void MaxLevelLimitUp()
    {
        MaxLevel  += 4;
        BeamColor.SetMaxLevel(MaxLevel);
        BeamColor.UpdateLevelSettings(CurrentLevel);

        if (GameManager.Instance.userData != null)
        {
            if (!GameManager.Instance.userData.userSettingData.bIsDirection)
                FOnLevelUped?.Invoke(CurrentLevel);
        }
    }

    public void SetCurrentBoss(BossObject boss)
    {

        bossdetectwidget.SetBosstransform(boss.transform);
        bossdetectwidget.ActiveBossDetect(false);

        UFOBeam.SetCurrentBoss(boss);
    }

    private void UpdateSizeText(int currentsizelevel)
    {
    
       LevelText.text = "레벨: " + currentsizelevel.ToString();
        
    }

    public void ChangeCameraDistance(float cameradistance)
    {
        //Debug.Log("Changecamera");
        CamerapositionComposer.CameraDistance = cameradistance;
    }

    public void TestCameraChangeDistance()
    {
        float current =CamerapositionComposer.CameraDistance;
        float target = 3.0f;

        // 거리 줄이기 (Lerp로 부드럽게)
        float newDistance = Mathf.Lerp(current, target, Time.deltaTime * 20.0f);
        ChangeCameraDistance(newDistance);
    }


    public void ResetCameraDistance()
    {
       
        CamerapositionComposer.CameraDistance = DefaultCameraDistance;
    }

    public void OnPossibeWidget(ShapeEnum shape)
    {
        possibleWidget.SetPossibleIcon(shape);
    }

    public void ForcePopPossibleWidget()
    {
        possibleWidget.ForcePop();
    }

    public void CallBack_PassMapObject(bool bpass,float targetheight)
    {
        if(bpass)
        {
            heightController.MoveToHeight(targetheight);
            PassProgress(bpass);
        }
        else
        {
            heightController.ResetHeight();
        }  
       
    }

    public void PassProgress(bool bpass)
    {
        BeamColor.HideMaterial(bpass);
        DirectionProgress(bpass);
    }
}


