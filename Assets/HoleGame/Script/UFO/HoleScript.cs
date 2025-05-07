
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class HoleScript : MonoBehaviour
{
   

    
    [Header("UFO ²ø¾î´ç±â´Â Èû")]
    [SerializeField]
    private float LiftSpeed = 10f;  // ²ø¾î´ç±â´Â Èû
                                    // [Header("Ãß°¡µÇ´Â ²ø¾î´ç±â´Â Èû")]
                                    //[SerializeField]
                                    //private float addLiftSpeed = 5f;

    private int CurrentLevel = 0;
   
    public Transform UFOtransform;

    private UFOPlayer ufoplayer;

    [Header("°³¹ßÀÚ")]
    private int LiftUpLayer =10;
    private int NormalLayer = 9;

    private BossObject bossobj = null;

    private Dictionary<Collider, LiftAbsorption> absorptionCache = new();

    private bool bIsSwalow = true;
    public void InitBeam(UFOPlayer layer)
    {
        ufoplayer = layer;  
    }

    public void SetSwallow(bool swallow)
    {
        bIsSwalow = swallow;
    }

    public void SetCurrentBoss (BossObject boss)
    {
        this.bossobj = boss; 
    }

    public void SetSwallowLevelSet(int swallowlevel)
    {
        CurrentLevel = swallowlevel;
    }

    public void SetInitLiftSpeed(float liftSpeed)
    {
        LiftSpeed = liftSpeed;
    }


    public void SetBeamRange(float range)
    {
        Vector3 scale = transform.localScale;
        scale.x = range;
        scale.z = range;
        transform.localScale = scale;
    }


    private void OnTriggerExit(Collider other)
    {
        if (!bIsSwalow) return;

        if (other.gameObject.layer == LiftUpLayer && other.attachedRigidbody != null)
        {
            //other.gameObject.layer = NormalLayer;
            if (!absorptionCache.TryGetValue(other, out var absorption))
            {
                absorption = other.GetComponent<LiftAbsorption>();
                if (absorption != null)
                    absorptionCache[other] = absorption;
            }
            absorption?.ResetScale();
            absorptionCache.Remove(other);
         
             if(GameManager.Instance!=null)
              {
                    GameManager.Instance.vibrationManager.StopLiftLoopVibration();  
              }

             if(other.gameObject == bossobj.gameObject)
                ufoplayer.ResetCameraDistance();

        }

    }

     private void OnTriggerStay(Collider other)
     {
        if (!bIsSwalow) return;

         Rigidbody rb = other.attachedRigidbody;
         if (rb != null && (other.gameObject.layer == NormalLayer || other.gameObject.layer == LiftUpLayer))
         {
             if (bossobj != null && bossobj.gameObject == other.gameObject)
             {
                 if(bossobj.ObjectMass > CurrentLevel) return;
             }

             if(other.gameObject.layer == NormalLayer)
             {
                 other.gameObject.layer = LiftUpLayer;
             }

             if (GameManager.Instance != null)
             {
                 GameManager.Instance.vibrationManager.StartLiftLoopVibration();
             }

             if (!absorptionCache.TryGetValue(other, out var absorption))
             {
                 absorption = other.GetComponent<LiftAbsorption>();
                 if (absorption != null)
                     absorptionCache[other] = absorption;
                 absorption.StartAbsorp(CurrentLevel);
             }



             absorption?.ApplyAbsorptionScale();

 
            Vector3 targetPos = UFOtransform.position + Vector3.up * 1.5f;
            Vector3 pullDir = (targetPos - rb.position).normalized;
            float absorbStrength = CalculateAbsorbStrength(CurrentLevel, absorption.GetObjectMass(),LiftSpeed);

            if (bossobj.gameObject == other.gameObject)
            {
                ufoplayer.TestCameraChangeDistance();
                absorbStrength = 4.5f;
            }
               
            rb.AddForce(pullDir * absorbStrength, ForceMode.Acceleration);
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, pullDir * absorbStrength, Time.deltaTime * 3f);

        }

     }


  

    public float CalculateAbsorbStrength(int ufoLevel, float objectMass, float baseStrength)
    {
        float delta = objectMass - ufoLevel;
        float powerBase = baseStrength == 20f ? 5f : 3.333f;

        float strength = baseStrength * Mathf.Pow(powerBase, -delta);

        if (delta <= 0)
        {
            strength *= 1.5f;
        }

        return Mathf.Clamp(strength, 2.5f, 50f);
    }
}
