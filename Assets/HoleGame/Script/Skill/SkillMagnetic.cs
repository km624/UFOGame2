using UnityEngine;

public class SkillMagnetic : SkillBase
{
    public GameObject Magnetic;

    protected GameObject InstantMagnetic;

    public Sprite mindcontrolIcon;

    [Header("마인드 컨트롤 최소 시간")]
    public float MinMindControlTime = 10.0f;

    public override void Activate()
    {
        
        InstantMagnetic = Instantiate(Magnetic, UFOplayer.gameObject.transform);
        MagneticTrigger trigger = InstantMagnetic.GetComponent<MagneticTrigger>();
        if (trigger != null)
        {
            trigger.SetMindcontrolData(UFOplayer.transform, MinMindControlTime, mindcontrolIcon);
        }
        InstantMagnetic.transform.localPosition = new Vector3(0, -3, 0);
        UFOplayer.ChangeCameraDistance(15.0f);
    }


    public override void Deactivate() 
    {
        DestroyImmediate(InstantMagnetic, true);
        UFOplayer.ResetCameraDistance();
    }
}
