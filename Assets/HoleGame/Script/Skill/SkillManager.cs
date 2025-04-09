using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public List<SkillBase> AllSkillPrefabs  = new List<SkillBase>();
    private List<SkillBase> AllSkills = new List<SkillBase>();
    private UFOPlayer UFOPlayer;

    public event Action<int/*skillnum*/, int/*count*/> FOnSkillActivated;
   
   

    public void SetSkill(UFOPlayer ufoPlayer)
    {
        UFOPlayer = ufoPlayer;
        foreach (SkillBase  skillprefab in  AllSkillPrefabs)
        {
            GameObject skillObject = Instantiate(skillprefab.gameObject, transform);
            SkillBase skill = skillObject.GetComponent<SkillBase>();
            if (skill != null)
            {
                skill.InitializedData(UFOPlayer,this);
                AllSkills.Add(skill);
            }

        }
    }

    void Start()
    {
    

    }



    void Update()
    {
        
    }

    public void ActivateSkill(int num)
    {
        if (AllSkills[num]!=null)
        {
           int CurrentSkillCount = AllSkills[num].UseSkill();
            if (CurrentSkillCount >= 0)
            {
                FOnSkillActivated?.Invoke(num, CurrentSkillCount);
            }

        }
    }

  /*  public void Skill_Freeze(bool bActive)
    {
        GameState.Instance.SetStopGameTimer(bActive);
    }*/

   
    /*public void Skill_LaserAttack(bool bActive)
    {

    }*/

    /*public void Skill_GodMode(bool bActive)
    {
       GameState.Instance.SetIgnoreBomb(bActive);
        UFOPlayer.SkillUFOspeedUp(bActive);
    }*/

}
