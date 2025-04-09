using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;
using UnityEngine.Audio;

public class SkillWidget : MonoBehaviour
{

    private SkillManager skillmanager;
    public Image SkillIcon;
    public TMP_Text SkillCountText;
    public Image SkillCoolTimeImage;
    public UIButtomAnimation SkillCoolTimeAnimation;
    
    protected int SkillNum;

    private float SkillCoolTime;

    public AudioSource audiosource;

    Coroutine skillcooltime;
    public void SetSkillWidget(SkillManager manager,  Sprite skillIcon, int skillcount, float cooltime,int skillnum)
    {
        skillmanager = manager;
        SkillIcon.sprite = skillIcon;
        SkillCountText.text = skillcount.ToString();
        SkillCoolTime = cooltime;
        SkillNum = skillnum;
    }
    void Start()
    {
        
    }

   
    void Update()
    {
        
    }

    public void SkillClick()
    {

        skillmanager.ActivateSkill(SkillNum);
        
    }

    public void ActivateSkillWidget(int skillcount)
    {
        audiosource.Play();
        SkillCountText.text = skillcount.ToString();   
        skillcooltime =  StartCoroutine(SkillStartCoolTime());
        
        SkillCoolTimeAnimation.SetAnimationDuration(SkillCoolTime);
        SkillCoolTimeAnimation.StartAnimation();
        SkillCoolTimeAnimation.SetAnimationStopState(true);
    }

    public void StopSkillCooltime()
    {
        StopCoroutine(skillcooltime);
    }

    IEnumerator SkillStartCoolTime()
    {
        float elapsedTime = 0f;
        SkillCoolTimeImage.fillAmount = 1f; 

        while (elapsedTime < SkillCoolTime)
        {
            elapsedTime += Time.deltaTime;
            SkillCoolTimeImage.fillAmount = 1 - (elapsedTime / SkillCoolTime); 
            yield return null;
        }

        audiosource.Stop();
        SkillCoolTimeImage.fillAmount = 0f;
        SkillCoolTimeAnimation.SetAnimationStopState(false);
        //Debug.Log(DateTime.Now.Minute +  "½ºÅ³ ³¡ À§Á¬");
    }

   
}
    

