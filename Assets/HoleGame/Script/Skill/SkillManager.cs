
using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{

    public SkillBase CurrentSkill { get; private set; } = null;
    
    private UFOPlayer UFOPlayer;

    public event Action<int/*skillnum*/, int/*count*/> FOnSkillActivated;

    [SerializeField] private List<SkillData> skillEntries;

    private Dictionary<SkillEnum, SkillBase> skillDict;

    public int Skillcount { get; private set; } = 0;

    private void Awake()
    {
        
        skillDict = new Dictionary<SkillEnum, SkillBase>();
        foreach (var entry in skillEntries)
        {
            if (!skillDict.ContainsKey(entry.Skilltype))
            {
                skillDict.Add(entry.Skilltype, entry.Skillbase);
            }
        }

    }



    public void SetSkill(UFOPlayer ufoPlayer, UserUFOData ufouserdata , UFOData ufodata)
    {
        UFOPlayer = ufoPlayer;
        //int index = 0;
        SkillBase ufoskill = skillDict[SkillEnum.MindControl];
        if (ufodata != null)  
            ufoskill = skillDict[ufodata.Skilltype];
        GameObject skillObject = Instantiate(ufoskill.gameObject, transform);
        SkillBase skill = skillObject.GetComponent<SkillBase>();
        if (skill != null)
        {
            int skillCnt = 2;
            if (ufodata != null)
                skillCnt = ufouserdata.GetReinforceValue(UFOStatEnum.SkillCount);

            skill.Initialize(UFOPlayer, this, skillCnt);
            //AllSkills.Add(skill);
            CurrentSkill = skill;
        }

    }


    public void ActivateSkill(int num)
    {
        if (CurrentSkill != null)
        {
           int CurrentSkillCount = CurrentSkill.UseSkill();
            if (CurrentSkillCount >= 0)
            {
                FOnSkillActivated?.Invoke(num, CurrentSkillCount);
            }

            Skillcount++;
        }
    }
    
    public void PauseSkillActive(bool active)
    {
        if(active)
        {
            PauseAllSkills();
        }
        else
        {
            ResumeAllSkills();
        }
    }

    private void PauseAllSkills()
    {
        CurrentSkill.PauseSkill();
    }

    private void ResumeAllSkills()
    {
        CurrentSkill.ResumeSkill();
    }


}
