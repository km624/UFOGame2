using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class PostEffectController : MonoBehaviour
{
    public static PostEffectController Instance { get; private set; }
    private Vignette vignette;
    private Volume volume;

    private Color HitColor = new Color(253 / 255f, 46 / 255f, 51 / 255f);
    private Color IceColor = new Color(0 / 255f, 90 / 255f, 195 / 255f);
    
    private void Awake()
    {
        Instance = this;
      
        volume = GetComponent<Volume>();
        if (volume.profile.TryGet<Vignette>(out vignette))
        {
            vignette.intensity.value = 0f;
           
        }
    }

    private void EnableVignette()
    {
        if (vignette != null)
        {
            vignette.intensity.value = 0.35f;
        }
    }

    private void DisableVignette()
    {
        if (vignette != null)
        {
            vignette.intensity.value = 0f;
            
        }
    }
    private void ActiveDamageEffect(bool active)
    {
        if (active)
        {
            EnableVignette();
        }
        else
        {
            DisableVignette();
        }
    }

    public void ActiveHitEffect(bool active)
    {
        if (vignette != null)
        {
            vignette.color.value = HitColor;
            
            ActiveDamageEffect(active);
        }
    }

    public void ActiveIceEffect(bool active)
    {
        if (vignette != null)
        {
            vignette.color.value = IceColor;
            ActiveDamageEffect(active);
        }
    }
}

