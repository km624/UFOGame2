using UnityEngine;

public class SkillSpeedUp : SkillBase
{

    public override void Activate()
    {
        UFOplayer.SkillUFOspeedUp(bProgressSkill);

    }


    public override void Deactivate()
    {
        UFOplayer.SkillUFOspeedUp(bProgressSkill);
    }
}
