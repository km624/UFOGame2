using UnityEngine;

public class SkillBeam : SkillBase
{
    public GameObject GunPrefab;
    private GameObject GunInstant;

    public GameObject Beamprefab;
    private GameObject BeamInstant;

    public GameObject BeamRange;
    private GameObject BeamRangeInstant;

    public float ShootInterval = 1.0f;

    public override void Activate()
    {

        GunInstant = Instantiate(GunPrefab, UFOplayer.gameObject.transform);
        GunInstant.transform.localPosition = new Vector3(0, 1, 0);

        BeamInstant = Instantiate(Beamprefab, UFOplayer.transform);
        BeamInstant.SetActive(false);

         BeamRangeInstant = Instantiate(BeamRange, UFOplayer.gameObject.transform);
        BeamRangeInstant.transform.localPosition = new Vector3(0, -3, 0);

        BeamTrigger trigger = BeamRangeInstant.GetComponent<BeamTrigger>();
        if (trigger != null)
        {
            trigger.SetBeamData(GunInstant.transform, ShootInterval, BeamInstant);
        }
       
        UFOplayer.ChangeCameraDistance(15.0f);
    }


    public override void Deactivate()
    {
        DestroyImmediate(GunInstant, true);
        DestroyImmediate(BeamRangeInstant, true);
        DestroyImmediate(BeamInstant, true);
        UFOplayer.ResetCameraDistance();
    }
}
