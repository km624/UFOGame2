using UnityEngine;

public class SkillGodMode : SkillBase
{
    public Material Godmodematerial;
    public override void Activate()
    {
        GameState.Instance.Skill_SetIgnoreBomb(bProgressSkill);
        UFOplayer.Skill_UFOspeedUp(bProgressSkill);
        UFOplayer.ChangeUFOMaterial(Godmodematerial);
    }


    public override void Deactivate()
    {
        GameState.Instance.Skill_SetIgnoreBomb(bProgressSkill);
        UFOplayer.Skill_UFOspeedUp(bProgressSkill);
        UFOplayer.ChangeUFOMaterial(UFOplayer.defaultMaterial);
    }
}
