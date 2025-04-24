using UnityEngine;

public class SkillFreezeTimer : SkillBase
{
    
    public override void Activate()
    {
        GameState.Instance.Skill_SetIceActive(true);
        GameState.Instance.AllObjectStopActive(true);
        PostEffectController.Instance.ActiveIceEffect(true);
        if(GameManager.Instance !=null)
        {
            GameManager.Instance.soundManager.PlaySfx(SoundEnum.Skill_Ice);
        }
    }


    public override void Deactivate()
    {
        GameState.Instance.Skill_SetIceActive(false);
        GameState.Instance.AllObjectStopActive(false);
       
        PostEffectController.Instance.ActiveIceEffect(bProgressSkill);
     
    }

   
}
