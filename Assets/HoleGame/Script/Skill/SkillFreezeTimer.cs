using UnityEngine;

public class SkillFreezeTimer : SkillBase
{
    
    public override void Activate()
    {
        GameState.Instance.SetStopGameTimer(bProgressSkill);
        AllObjectFreezeActive(bProgressSkill);
        PostEffectController.Instance.ActiveIceEffect(bProgressSkill);
    }


    public override void Deactivate()
    {
        GameState.Instance.SetStopGameTimer(bProgressSkill);
       // AllObjectFreezeActive(bProgressSkill);
        PostEffectController.Instance.ActiveIceEffect(bProgressSkill);
        AllObjectFreezeActive(bProgressSkill);
    }

    public void AllObjectFreezeActive(bool active)
    {
        //bool AllFall = true;

        foreach (var fallingobject in GameState.Instance.FallingObjects)
        {
            if (fallingobject.Value.Count != 0)
            {
                foreach (var fallingobjectscript in fallingobject.Value)
                {
                   
                    fallingobjectscript.ActiveIce(active);
                    fallingobjectscript.ActivateBounce(!active);


                }
            }
        }
    }
}
