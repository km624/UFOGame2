using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using DG.Tweening;


public class SkillWidget : MonoBehaviour
{

    private SkillManager skillmanager;
    public Image SkillIcon;
    public TMP_Text SkillCountText;
    public Image SkillCoolTimeImage;
    public UIButtomAnimation SkillCoolTimeAnimation;
    [SerializeField] private GameObject ButtonObject;
    
    protected int SkillNum;

    private float SkillCoolTime;

    //public AudioSource audiosource;

    Coroutine skillcooltime;
    private float remainingCoolTime;

    private float DefaultPostionY = 0.0f; 
    [SerializeField] private float animationTime = 0.5f;

    public void SetSkillWidget(SkillManager manager,  Sprite skillIcon, int skillcount, float cooltime,int skillnum)
    {
        skillmanager = manager;
        SkillIcon.sprite = skillIcon;
        SkillCountText.text = skillcount.ToString();
        SkillCoolTime = cooltime;
        SkillNum = skillnum;
        
        DefaultPostionY = ButtonObject.transform.localPosition.y;
    }
   

    public void SkillClick()
    {

        skillmanager.ActivateSkill(SkillNum);
        
    }

    public void ActivateSkillWidget(int skillcount)
    {
        //audiosource.Play();
        SkillCountText.text = skillcount.ToString();
        remainingCoolTime = SkillCoolTime;

        // 버튼 아래로 이동 (y -20), 0.5초
        ButtonObject.transform.DOLocalMoveY(-5.0f, animationTime).SetEase(Ease.InBack);

        SkillCoolTimeAnimation.SetAnimationDuration(SkillCoolTime);
        SkillCoolTimeAnimation.StartAnimation();
        SkillCoolTimeAnimation.SetAnimationStopState(true);

        if (skillcooltime != null)
            StopCoroutine(skillcooltime);
        skillcooltime = StartCoroutine(SkillStartCoolTime());
    }

    // 스킬 위젯 일시정지
    public void PauseSkillCooltime()
    {
        if (skillcooltime != null)
        {
            StopCoroutine(skillcooltime);
            skillcooltime = null;
        }
        // audiosource.Pause(); // 오디오도 중지
        SkillCoolTimeAnimation.PauseAnimation();
    }

    // 스킬 위젯 다시 재개
    public void ResumeSkillCooltime()
    {
        if (skillcooltime == null && remainingCoolTime > 0f)
        {
            skillcooltime = StartCoroutine(SkillStartCoolTime());
            //audiosource.UnPause(); // 오디오 재개
            SkillCoolTimeAnimation.ResumeAnimation();
        }
    }

    // 실제 타이머 Coroutine (Pause/Resume 대응)
    IEnumerator SkillStartCoolTime()
    {
        SkillCoolTimeImage.fillAmount = remainingCoolTime / SkillCoolTime;

        while (remainingCoolTime > 0f)
        {
            remainingCoolTime -= Time.unscaledDeltaTime; // 일시정지와 무관
            SkillCoolTimeImage.fillAmount = remainingCoolTime / SkillCoolTime;
            yield return null;
        }

        //audiosource.Stop();
        SkillCoolTimeImage.fillAmount = 0f;
        SkillCoolTimeAnimation.SetAnimationStopState(false);

        // 버튼 원위치 (y +20), 0.5초
        ButtonObject.transform.DOLocalMoveY(DefaultPostionY, animationTime).SetEase(Ease.InBack);
    }

}
    

