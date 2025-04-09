using UnityEngine;

public class SkillFreezeTimer : SkillBase
{
    
    public override void Activate()
    {
        GameState.Instance.Skill_SetIceActive(bProgressSkill);
        GameState.Instance.AllObjectStopActive(true);
        PostEffectController.Instance.ActiveIceEffect(bProgressSkill);
    }


    public override void Deactivate()
    {
        GameState.Instance.AllObjectStopActive(false);
        GameState.Instance.Skill_SetIceActive(bProgressSkill);
        PostEffectController.Instance.ActiveIceEffect(bProgressSkill);
     
    }

   
}
