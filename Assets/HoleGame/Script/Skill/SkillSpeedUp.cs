using UnityEngine;

public class SkillSpeedUp : SkillBase
{

    public override void Activate()
    {
        UFOplayer.Skill_UFOspeedUp(bProgressSkill);

    }


    public override void Deactivate()
    {
        UFOplayer.Skill_UFOspeedUp(bProgressSkill);
    }
}
