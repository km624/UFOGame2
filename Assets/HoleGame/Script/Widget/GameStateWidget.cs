using UnityEngine;
using TMPro;
using Lean.Gui;
using DG.Tweening;
using AssetKits.ParticleImage;


public class GameStateWidget : MonoBehaviour
{
    public TMP_Text PlayTimeText;
    public TMP_Text ScoreText;
    public TMP_Text CoinCntText;
    public LeanWindow GameStateModal;
    
    [SerializeField] private RectTransform[] panels;
    private Sequence EndSeq;

    [SerializeField] private ParticleImage CoinParticle;
    [SerializeField] private GameObject SkipButton;

    private int Coincnt = 0;
    private int fakecoincnt = 0;

    public void SetGameState(int totalplaytime ,int totalscore ,int coincnt)
    {

        int min = totalplaytime / 60;
        int sec = totalplaytime % 60;
        PlayTimeText.text = min.ToString("D1") + " : " + sec.ToString("D2");
       
        ScoreText.text = totalscore.ToString();

       
        Coincnt = coincnt;
        //Coincnt = 3;
        CoinParticle.rateOverLifetime = Coincnt;

        SkipButton.SetActive(true);
        GameStateModal.TurnOn();
        PlaySequence();

        
    }

   
    public void PlaySequence()
    {
        EndSeq?.Kill(); // 혹시 실행 중이면 정리

        foreach(var panel in panels)
        {
            panel.localScale = Vector3.zero;
        }

        EndSeq = DOTween.Sequence();

        for (int i = 0; i < 3; i++)
        {
            EndSeq.Append(panels[i].DOScale(1.2f, 0.35f).SetEase(Ease.OutBack))
                   .AppendCallback(() =>
                   {
                       GameManager.Instance.soundManager.PlaySfx(SoundEnum.BonusDisappear,0.5f);

                   })
                  .Append(panels[i].DOScale(1f, 0.1f).SetEase(Ease.InSine))
                  .AppendInterval(0.05f);
                  
        }

       
        if (Coincnt > 0)
        {
            EndSeq.AppendCallback(() =>
            {
                CoinParticle.gameObject.SetActive(true);   
                                                                              
            })
            .AppendInterval(1f);                           
        }


        EndSeq.Append(panels[3].DOScale(1.2f, 0.35f).SetEase(Ease.OutBack))
             .AppendCallback(() =>
             {
                 GameManager.Instance.soundManager.PlaySfx(SoundEnum.BonusDisappear, 0.5f);

             })
              .Append(panels[3].DOScale(1f, 0.1f).SetEase(Ease.InSine))
              .AppendCallback(() =>
                {
                    SkipButton.SetActive(false);

                });


    }

    

    public void CoinArrive()
    {
        fakecoincnt++;
        GameManager.Instance.soundManager.PlaySfx(SoundEnum.AddMoney, 0.4f);
        GameManager.Instance.vibrationManager.Play(VibrationEnum.CoinArrive);
        if(fakecoincnt <= Coincnt)
         CoinCntText.text = fakecoincnt.ToString();
    }

    public void SkipSequenceAndShowAll()
    {
        EndSeq?.Kill(); // 애니메이션 즉시 중단

        foreach (var panel in panels)
        {
            panel.localScale = Vector3.one; // 즉시 보여줌
        }
        CoinParticle.gameObject.SetActive(false);

        CoinCntText.text = Coincnt.ToString();

        SkipButton.SetActive(false);
    }

}
