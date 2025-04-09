using UnityEngine;
using System.Collections;

public abstract  class SkillBase : MonoBehaviour
{
    public int SkillCount;
    public Sprite SkillIcon;
    public float SkillDuration;
    protected bool CanProgressSkill = false;
    public bool bProgressSkill { get; private set; } = false;
    protected UFOPlayer UFOplayer;
    protected SkillManager skillManager;


    public void InitializedData(UFOPlayer ufoplayer, SkillManager manager)
    {
        UFOplayer = ufoplayer;
        skillManager = manager;
    }

    protected virtual void Start()
    {
       
    }


   public void SetSkillCount(int skillCount)
        { this.SkillCount = skillCount; }


    public virtual bool CanUseSkill()
    {
        return SkillCount > 0;
    }
    public int UseSkill()
    {
        if (CanUseSkill()&& !bProgressSkill)
        {
            bProgressSkill =true;
            StartCoroutine(SkillDeactive(SkillDuration));
            Activate();
            SkillCount--;
            return SkillCount;
        }
        else
        {
            
            Debug.Log("스킬 사용 불가");
            return -1;
        }
    }
    IEnumerator SkillDeactive(float duration)
    {
        yield return new WaitForSeconds(duration);
        bProgressSkill = false;
        Deactivate();
        
        //Debug.Log(DateTime.Now.Minute + "스킬 끝 액터");
    }

    public abstract void Activate();
    public abstract void Deactivate();




}
