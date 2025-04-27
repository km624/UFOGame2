using UnityEngine;

public class SkillGodMode : SkillBase
{
    public Material Godmodematerial;
    public override void Activate()
    {
        GameState.Instance.Skill_SetIgnoreBomb(true);
        UFOplayer.Skill_UFOspeedUp(true);
        UFOplayer.ChangeUFOMaterial(Godmodematerial);
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.soundManager.PlayBGMOneShot(SoundEnum.Skill_God,0.2f);
        }
    }


    public override void Deactivate()
    {
        GameState.Instance.Skill_SetIgnoreBomb(false);
        UFOplayer.Skill_UFOspeedUp(false);
        UFOplayer.ChangeUFOMaterial(UFOplayer.defaultMaterial);
        if (GameManager.Instance != null)
        {
            GameManager.Instance.soundManager.ResumeBgm();
        }
    }
}
