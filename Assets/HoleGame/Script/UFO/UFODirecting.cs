using DG.Tweening;
using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

public class UFODirecting : MonoBehaviour
{
   [SerializeField]private UFOPlayer owner;

    [SerializeField] private GameObject UFOModel;
    [SerializeField] private Animator UFOAnimator;

    [SerializeField] private ParticleSystem hitgroundpartice;
    [SerializeField] private ParticleSystem smokeparticle;
    [SerializeField] private ParticleSystem hitparticle;
    [SerializeField] private ParticleSystem Warpparticle;
    [SerializeField] private ParticleSystem Disappearparticle;
    [SerializeField] private ParticleSystem appearparticle;
    [SerializeField] private Material BeamMaterial;
    [SerializeField] private CameraShake camerashake;
   

    private List<GameObject> Directgameobjects = new List<GameObject>();
    private GameObject WarpEffectInstance = null;

    public void OnStartUFODirecct()
    {
        UFOAnimator.enabled = true;
        UFOAnimator.Rebind();
        UFOAnimator.SetTrigger("StartFall");
    }

    public void UFOFAlling(float duration)
    {
        Debug.Log("Shakle");
        camerashake.ShakeCamera(duration);
        OnSmokeEffect();
    }

    public void OnGroundFit()
    {
        GameObject groundhit = Instantiate(hitgroundpartice.gameObject, UFOModel.transform);
        groundhit.gameObject.transform.localPosition += Vector3.up*1.0f;
    }

    public void OnSmokeEffect()
    {
        Debug.Log("Smoke");
        GameObject smoke1 = Instantiate(smokeparticle.gameObject, UFOModel.transform);
        smoke1.transform.localPosition = new Vector3(-0.5f,0.5f,0f);

        Directgameobjects.Add(smoke1);
       
    }

    public void OnUFOBreak()
    {
        Instantiate(hitparticle.gameObject,UFOModel.transform);
        hitparticle.gameObject.transform.localPosition = new Vector3(-0.5f,1.5f,0.0f);
    }

    public void EndUFOSmokeEffect()
    {
        foreach (GameObject obj in Directgameobjects)
        {
          
                // 파티클 본체를 스케일 줄이기
                obj.transform.DOScale(Vector3.zero, 0.5f)
                    .SetEase(Ease.InQuad)
                    .OnComplete(() =>
                    {
                        Directgameobjects.Remove(obj);
                        Destroy(obj);
                    });
            
        }

        Directgameobjects.Clear();

    }

    public void AnimatorEnd()
    {
       
        owner.EndUFODirecting();
        UFOAnimator.enabled = false;
    }

    public void OnWarpStart()
    {
        
        UFOAnimator.enabled = true;
        UFOAnimator.Rebind();
        UFOAnimator.SetTrigger("UFOWarp");
        WarpEffectInstance = Instantiate(Warpparticle.gameObject, owner.transform);
        WarpEffectInstance.transform.localPosition = new Vector3(0, 0, 0);

    }

    public void DisapperEffect()
    {
        Destroy(WarpEffectInstance);
        GameObject disappear =  Instantiate(Disappearparticle.gameObject, owner.transform);
        disappear.transform.localPosition = new Vector3(0,0,6);
       
        owner.GenerationMoved();
    }

    public void AppearEffect()
    {
        GameObject appear = Instantiate(appearparticle.gameObject, owner.transform);
        appear.transform.position = UFOModel.transform.position;
    }

    public void WarpEffectEnd()
    {
        Debug.Log("WarpEnd");
        owner.EndWarpDirecting();
        UFOAnimator.enabled = false;
    }
}


