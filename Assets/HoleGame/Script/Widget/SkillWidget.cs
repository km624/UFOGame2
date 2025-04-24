using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;


public class SkillWidget : MonoBehaviour
{

    private SkillManager skillmanager;
    public Image SkillIcon;
    public TMP_Text SkillCountText;
    public Image SkillCoolTimeImage;
    public UIButtomAnimation SkillCoolTimeAnimation;
    
    protected int SkillNum;

    private float SkillCoolTime;

    //public AudioSource audiosource;

    Coroutine skillcooltime;
    private float remainingCoolTime;
    public void SetSkillWidget(SkillManager manager,  Sprite skillIcon, int skillcount, float cooltime,int skillnum)
    {
        skillmanager = manager;
        SkillIcon.sprite = skillIcon;
        SkillCountText.text = skillcount.ToString();
        SkillCoolTime = cooltime;
        SkillNum = skillnum;
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

        SkillCoolTimeAnimation.SetAnimationDuration(SkillCoolTime);
        SkillCoolTimeAnimation.StartAnimation();
        SkillCoolTimeAnimation.SetAnimationStopState(true);

        if (skillcooltime != null)
            StopCoroutine(skillcooltime);
        skillcooltime = StartCoroutine(SkillStartCoolTime());
    }

    // ��ų ���� �Ͻ�����
    public void PauseSkillCooltime()
    {
        if (skillcooltime != null)
        {
            StopCoroutine(skillcooltime);
            skillcooltime = null;
        }
        // audiosource.Pause(); // ������� ����
        SkillCoolTimeAnimation.PauseAnimation();
    }

    // ��ų ���� �ٽ� �簳
    public void ResumeSkillCooltime()
    {
        if (skillcooltime == null && remainingCoolTime > 0f)
        {
            skillcooltime = StartCoroutine(SkillStartCoolTime());
            //audiosource.UnPause(); // ����� �簳
            SkillCoolTimeAnimation.ResumeAnimation();
        }
    }

    // ���� Ÿ�̸� Coroutine (Pause/Resume ����)
    IEnumerator SkillStartCoolTime()
    {
        SkillCoolTimeImage.fillAmount = remainingCoolTime / SkillCoolTime;

        while (remainingCoolTime > 0f)
        {
            remainingCoolTime -= Time.unscaledDeltaTime; // �Ͻ������� ����
            SkillCoolTimeImage.fillAmount = remainingCoolTime / SkillCoolTime;
            yield return null;
        }

        //audiosource.Stop();
        SkillCoolTimeImage.fillAmount = 0f;
        SkillCoolTimeAnimation.SetAnimationStopState(false);
    }

}
    

