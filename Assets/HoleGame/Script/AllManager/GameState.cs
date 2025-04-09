using Lean.Gui;
using Lean.Transition.Method;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;



public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    private StageData stagedata;
    [SerializeField]
    private UFOPlayer ufoplayer;
    [SerializeField]
    private CameraShake Camerashake;

    [SerializeField]
    private PlayerHudWidget PlayerHud;
    //[SerializeField]
   // private ShapeManager AllShapeImage;
    [SerializeField]
    private SkillManager AllSkillManager;
    public Dictionary<ShapeEnum, List<FallingObject>> FallingObjects { get; private set; } = new Dictionary<ShapeEnum, List<FallingObject>>();

    delegate void FUpdateShapeCount(ShapeEnum shape, int count);
    FUpdateShapeCount ShapeCountChanged;

    public event Action<Vector3 /*아이콘 생성 위치*/, Sprite/*아이콘 도형*/ , ShapeEnum> FOnSwallowShaped;
    public event Action FOnBombSwallowed;

    [SerializeField]
    private int TimeMin = 1;
    [SerializeField]
    private int TimeSecond = 30;

    private bool bIgnoreBomb = false;

    //게임 스테이트 (클리어 ,실패 ) 이벤트 생성 예정
    //public event Action<bool /*게임 클리어 , 실패 */> FOnGameStated;
    bool bIsGameClear = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }


        foreach (ShapeEnum shape in System.Enum.GetValues(typeof(ShapeEnum)))
        {
            FallingObjects[shape] = new List<FallingObject>();
        }



      
    }

    void Start()
    {
        

        //오브젝트 위젯 생성
        SetAllFallingObjectWidget();

        //오브젝트 카운트 바인딩
        ShapeCountChanged += PlayerHud.UpdateAllShapeCount;

        //오브젝트 빨아들였을때 바인딩
        FOnSwallowShaped += PlayerHud.SpawnUIImageAtWorldPos;
        //스킬 세팅
        AllSkillManager.SetSkill(ufoplayer);

        //스킬 표시
        foreach (SkillBase skilldata in AllSkillManager.AllSkillPrefabs)
        {

            PlayerHud.CreateSkillWidget(skilldata, AllSkillManager);
        }
        //스킬 바인딩
        AllSkillManager.FOnSkillActivated += PlayerHud.ActiveSkill;

        //타이머 세팅

        if (stagedata != null)
        {
            TimeMin = stagedata.TimeMin;
            TimeSecond = stagedata.TimeSecond;
        }

        PlayerHud.SetTimeWidget(TimeMin, TimeSecond);

        PlayerHud.StartTimer();

        //폭탄 바인딩
        FOnBombSwallowed += PlayerHud.MinusTimer;
        FOnBombSwallowed += Camerashake.ShakeCamera;
    }
   
    /*void OnLoadandSetData(UFOData data)
    {
        if(ufoplayer!=null)
        {
            if(data!=null)
                ufoplayer.SetUFOData(data);
        }
        else
        {
            Debug.Log("UFO Not Set");
        }
    }*/

    public void AbsorptionObject(FallingObject obj)
    { 
        if(obj!=null)
        {
            ShapeEnum objshape = obj.GetShapeEnum();
            if(objshape ==ShapeEnum.boomb)
            {
                if(!bIgnoreBomb)
                    FOnBombSwallowed?.Invoke();
                return;
            }
           

            RemoveFallingObject(obj, ufoplayer.gameObject.transform.position);
        }
    }

    void SetAllFallingObjectWidget()
    {

        if (stagedata != null)
        {
            //StageData stagedata = GameManager.Instance.GetCurrentMapData();
            //Debug.Log("아이콘 생성");
            if(stagedata.RequiredShapeCnt.Count==0)
            {
                Debug.Log("없음");
            }
            foreach (var fallingobject in stagedata.RequiredShapeCnt)
            {
                if (fallingobject.Value == 0)
                {
                   
                    continue;
                }


                    Sprite ShapeImage = ShapeManager.Instance.GetShapeSprite(fallingobject.Key);

                PlayerHud.CreateShapeWidget(fallingobject.Key, fallingobject.Value, ShapeImage);

                if (ShapeImage == null)
                    Debug.Log(fallingobject.Key + " : 아이콘 없음");

            }
        }

      
    }

    public void RegisterFallingObject(FallingObject obj)
    {
        if(!FallingObjects[obj.Shape].Contains(obj))
        {
            FallingObjects[obj.Shape].Add(obj);
            
        }
    }

    public void RemoveFallingObject(FallingObject obj , Vector3 iconpostion)
    {
        if (FallingObjects[obj.Shape].Contains(obj))
        {
            FallingObjects[obj.Shape].Remove(obj);

            ShapeCountChanged?.Invoke(obj.Shape, FallingObjects[obj.Shape].Count);

            FOnSwallowShaped?.Invoke(iconpostion, ShapeManager.Instance.GetShapeSprite(obj.Shape), obj.Shape);

            //ufoplayer.AddEXPGauge(obj.EXPCnt);
        }
        if (ShapeEnum.boomb != obj.Shape)
            ufoplayer.AddEXPGauge(obj.EXPCnt);
        else
            ufoplayer.AddEXPGauge(1);

        if (CheckFallingObjectCount())
        {
            bIsGameClear = true;
            GameEnd();
        }

    }

    bool CheckFallingObjectCount()
    {
        bool AllFall = true;

        foreach(var fallingobject in FallingObjects)
        {
            if(fallingobject.Value.Count!=0)
            { 
                AllFall = false;
                break;
            }
        }

        return AllFall;
    }

    public void SetStopGameTimer(bool bisStop)
    {
        if (bisStop)
        {
            PlayerHud.StopTimer();
        }
        else
        {
            PlayerHud.StartTimer();
        }
    }

  
    public void SetIgnoreBomb(bool active)
    {
        bIgnoreBomb = active;
    }

    public void GameEnd()
    {
        int star = 0;
        int totalsecond = PlayerHud.StopTimer();
        AllObjectStopActive(true);
        if (bIsGameClear)
        {
            switch (totalsecond)
            {
                case int n when (n >= stagedata.star3):
                    star = 3;
                    Debug.Log("star3 " + totalsecond);
                    break;

                case int n when (n >= stagedata.star2):
                    star = 2;
                    Debug.Log("star2 " + totalsecond);
                    break;

                case int n when (n >= stagedata.star1):
                    star = 1;
                    Debug.Log("star1 " + totalsecond);
                    break;

                default:
                    star = 1;

                    break;
            }
        }
        ufoplayer.CallBack_StopMovement();
        PlayerHud.UpdateGameState(bIsGameClear, star);
     
        GameManager.Instance.SaveUserData();
    }
    public void AllObjectStopActive(bool active)
    {
        //bool AllFall = true;

        foreach (var fallingobject in FallingObjects)
        {
            if (fallingobject.Value.Count != 0)
            {
                foreach (var fallingobjectscript in fallingobject.Value)
                {

                    //fallingobjectscript.ActiveIce(active);
                    fallingobjectscript.ActivateBounce(!active);


                }
            }
        }
    }


}
