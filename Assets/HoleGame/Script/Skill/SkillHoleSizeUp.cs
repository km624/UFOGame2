using UnityEngine;

public class SkillHoleSizeUp : SkillBase
{
    public override void Activate()
    {
        UFOplayer.ChangeLevel(true);
    }


    public override void Deactivate()
    {
        UFOplayer.ChangeLevel(false);
    }
}
