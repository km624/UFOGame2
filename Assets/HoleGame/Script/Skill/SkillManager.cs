using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public List<SkillBase> AllSkillPrefabs  = new List<SkillBase>();
    private List<SkillBase> AllSkills = new List<SkillBase>();
    public IReadOnlyList<SkillBase> ReadAllSkills => AllSkills;
    private UFOPlayer UFOPlayer;

    public event Action<int/*skillnum*/, int/*count*/> FOnSkillActivated;
   
   

    public void SetSkill(UFOPlayer ufoPlayer)
    {
        UFOPlayer = ufoPlayer;
        int index = 0;
        foreach (SkillBase  skillprefab in  AllSkillPrefabs)
        {
            GameObject skillObject = Instantiate(skillprefab.gameObject, transform);
            SkillBase skill = skillObject.GetComponent<SkillBase>();
            if (skill != null)
            {
                int skillCnt = 2;
                //유저 데이터 토대로 세팅
                if (GameManager.Instance != null)
                    skillCnt = GameManager.Instance.userData.SkillCnt[index];

                skill.InitializedData(UFOPlayer,this, skillCnt);
                AllSkills.Add(skill);
            }

            index++;
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

  
}
