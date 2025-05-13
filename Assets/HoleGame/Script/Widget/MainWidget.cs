
using EasyTransition;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Splines;
using System.Collections.Generic;
using AssetKits.ParticleImage;
using System.Collections;



public class MainWidget : MonoBehaviour
{
    //public LeanSwitch leanSwitch;
    public TransitionSettings transition;

    public Button PlayButton;

    public UFOAllWidget ufoAllWidget;

    [SerializeField] private AllAchievementWidget allArchievementWidget;

    public TMP_Text StarCntText;
   
    [SerializeField] private SplineContainer splineContainer;
    [SerializeField] private Transform ufomodel;
    [SerializeField] private float moveDuration = 2.0f;


    [SerializeField] Vector3 UFOTargetScale = new Vector3(0.5f, 0.5f, 0.5f); // 도착 시 크기


    [SerializeField] private RectTransform MoneyTransform;
    [SerializeField] private List<ParticleImage> CoinParticles = new List<ParticleImage>();
    private Queue<ParticleImage> particleQueue = new Queue<ParticleImage>();
    [SerializeField] private float ParticlePlayTime = 1.0f;

    private void Awake()
    {
        foreach (var obj in CoinParticles)
        {
            obj.gameObject.SetActive(false); 
            particleQueue.Enqueue(obj);
        }
    }
    void Start()
    {
       
        if (GameManager.Instance != null)
        {

            GameManager.Instance.soundManager.PlayBgm(SoundEnum.BGM, 0.2f);
            if (GameManager.Instance.userData != null)
            {
                SetStarCntText(GameManager.Instance.userData.StarCnt);

                ufoAllWidget.InitializedUFOAllWidget();

                allArchievementWidget.InitAllArchiveWidget();

                // 업적 포인트 보상 해금 바인딩
                allArchievementWidget.FOnUfoRewarded += ufoAllWidget.CallBack_UFORewardCompleted;
                allArchievementWidget.FOnUfoColorRewarded += ufoAllWidget.CallBack_UFOColorCompleted;
            }
            else
            {
                Debug.Log("Userdata없음");
                
                TestLoadData();  
            }
          
        }
        //UFOPathCalculate();

        int currentLevel = QualitySettings.GetQualityLevel();
        string levelName = QualitySettings.names[currentLevel];
       
        Debug.Log($"Quality Level: {levelName} ({currentLevel})");
    }

    public async void TestLoadData()
    {
        if (GameManager.Instance != null)
        {
            await GameManager.Instance.InitData();

            SetStarCntText(GameManager.Instance.userData.StarCnt);

            ufoAllWidget.InitializedUFOAllWidget();

            allArchievementWidget.InitAllArchiveWidget();

            allArchievementWidget.FOnUfoRewarded += ufoAllWidget.CallBack_UFORewardCompleted;
            allArchievementWidget.FOnUfoColorRewarded += ufoAllWidget.CallBack_UFOColorCompleted;
        }
    }

   
  
    public void SetStarCntText(int cnt)
    {
        StarCntText.text= cnt.ToString();
    }

    public void CallBack_OnPurchased(bool bsucess , int currentcnt)
    {
        if(bsucess)
        {
            // 구매하는 애니메이션 실행

            SetStarCntText(currentcnt);
        }
        else
        {
            //돈 없는 인포 창 띄위기
            Debug.Log("구매 실패");
        }
        
    }

    public void Callback_OnRewarded(int currentcnt , RectTransform buttontransform)
    {
        SetStarCntText(currentcnt);
        PlayParticleAt(buttontransform);
    }

    public void PlayParticleAt(RectTransform startTransform)
    {
        if (particleQueue.Count == 0)
        {
            Debug.LogWarning("No particle available in pool!");
            return;
        }

        var particle = particleQueue.Dequeue();


        var rect = particle.GetComponent<RectTransform>();
        if(startTransform.pivot.x==1.0f)
        {
            Vector3 offset = startTransform.TransformVector(Vector3.left * (startTransform.rect.width / 2f));
            rect.position = startTransform.position + offset;
        } 
        else
            rect.position = startTransform.position;

        particle.attractorTarget = MoneyTransform;

        particle.gameObject.SetActive(true);

        // 일정 시간 후 자동 반환
        StartCoroutine(ReturnToPoolAfterDelay(particle, ParticlePlayTime)); // ← 파티클 지속시간
    }

    private IEnumerator ReturnToPoolAfterDelay(ParticleImage particle, float delay)
    {
        yield return new WaitForSeconds(delay);
        particle.Stop();
        particle.gameObject.SetActive(false);
        particleQueue.Enqueue(particle);
    }

    public void CallBack_CoinArrive()
    {
        GameManager.Instance.vibrationManager.Play(VibrationEnum.CoinArrive);
        Debug.Log(Time.time + " : arrive");
    }


    public void StartUFOPathMove()
    {
        // Spline 포인트 → DOTween 경로로 변환
        var spline = splineContainer.Spline;
        int sampleCount = 30;
        List<Vector3> path = new();

        for (int i = 0; i <= sampleCount; i++)
        {
            float t = i / (float)sampleCount;

            Vector3 localPos = spline.EvaluatePosition(t);
            Vector3 worldPos = splineContainer.transform.TransformPoint(localPos);
            
           

            path.Add(worldPos);
        }

        

        BounceShape bounce = ufomodel.GetComponent<BounceShape>();   

        if(bounce !=null)
        {
            bounce.enabled = false;

        }

        Sequence seq = DOTween.Sequence();
        seq.Append(ufomodel.DOPath(path.ToArray(), moveDuration, PathType.CatmullRom)
              .SetEase(Ease.InOutSine));
             
        seq.Join(ufomodel.DOScale(UFOTargetScale, moveDuration).SetEase(Ease.InOutSine));

        seq.OnComplete(() => PlayGame());

    }


    public void PlayGame()
   {

        DOTween.KillAll();
        TransitionManager.Instance().Transition("GameScene", transition,0);
       


    }
 
   
}
