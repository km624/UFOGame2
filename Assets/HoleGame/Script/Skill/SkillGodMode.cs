using UnityEngine;

public class SkillGodMode : SkillBase
{
    public Material Godmodematerial;
    public override void Activate()
    {
        GameState.Instance.SetIgnoreBomb(bProgressSkill);
        UFOplayer.SkillUFOspeedUp(bProgressSkill);
        UFOplayer.ChangeUFOMaterial(Godmodematerial);
    }


    public override void Deactivate()
    {
        GameState.Instance.SetIgnoreBomb(bProgressSkill);
        UFOplayer.SkillUFOspeedUp(bProgressSkill);
        UFOplayer.ChangeUFOMaterial(UFOplayer.defaultMaterial);
    }
}
