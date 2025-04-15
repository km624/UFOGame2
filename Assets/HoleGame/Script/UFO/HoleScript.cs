
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

   

    [Header("°³¹ßÀÚ")]
    private int LiftUpLayer =10;
    private int NormalLayer = 9;

    private BossObject bossobj = null;


    private Dictionary<Collider, LiftAbsorption> absorptionCache = new();

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


   /* private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == NormalLayer && other.attachedRigidbody != null)
        {
           
            //objectsInTrigger.Add(other.attachedRigidbody);
            //other.gameObject.layer = LiftUpLayer;
           
            *//*LiftAbsorption absorption = other.GetComponent<LiftAbsorption>();
            if (absorption)
            {
                absorption.StartAbsorp(LiftSpeed);
            }*//*

        }

    }*/

    private void OnTriggerExit(Collider other)
    {
      

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
         
            //other.gameObject.layer = NormalLayer;
           
        }

    }

    private void OnTriggerStay(Collider other)
    {
       
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
           

            if (!absorptionCache.TryGetValue(other, out var absorption))
            {
                absorption = other.GetComponent<LiftAbsorption>();
                if (absorption != null)
                    absorptionCache[other] = absorption;
                absorption.StartAbsorp(CurrentLevel);
            }



            absorption?.ApplyAbsorptionScale();

            Vector3 directionToShip = (UFOtransform.position - rb.position).normalized;

            float newLifSpeed = CalculateLiftSpeed(CurrentLevel, absorption.GetObjectMass());
            
            //Vector3 liftForce = Vector3.up * LiftSpeed;
            Vector3 liftForce = Vector3.up * newLifSpeed;
           
            float distance = Vector3.Distance(rb.position, UFOtransform.position);
            float attractionStrength = Mathf.Clamp(distance * 2f, 10f, 50f);
          

            Vector3 attractionForce = directionToShip * attractionStrength;

            rb.AddForce(liftForce + attractionForce, ForceMode.Force);

            rb.linearDamping = Mathf.Lerp(rb.linearDamping, 2f, Time.deltaTime * 2f);

           


        }

    }

    public float CalculateLiftSpeed(int ufoLevel, float objectLevel)
    {
        float delta = objectLevel - ufoLevel;

        float rawSpeed = Mathf.Pow(LiftSpeed, 1f - delta); // °¨¼è ¼ö½Ä
        float clampedSpeed = Mathf.Clamp(rawSpeed, 0.1f, LiftSpeed*3); // ¹üÀ§ Á¦ÇÑ

        return clampedSpeed;
    }


}
