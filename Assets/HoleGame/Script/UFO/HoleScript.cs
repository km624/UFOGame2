using DamageNumbersPro;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;


public class HoleScript : MonoBehaviour
{
    private GameState gameState;

    
    [Header("UFO 끌어당기는 힘")]
    [SerializeField]
    private float LiftSpeed = 10f;  // 끌어당기는 힘
    [Header("추가되는 끌어당기는 힘")]
    [SerializeField]
    private float addLiftSpeed = 5f;

    private HashSet<Rigidbody> objectsInTrigger = new HashSet<Rigidbody>();
    public Transform UFOtransform;

   

    [Header("개발자")]
    private int LiftUpLayer =10;
    private int NormalLayer = 9;

    [SerializeField]
    private float wobbleAmount = 2.5f;
    private float wobbleDuration = 1f;

    private Tween rotationTween;
    private Tween spinTween;

    private bool bIsMove = false;
   
    public void StartWobble(Vector3 direction)
    {
        spinTween?.Kill();
        rotationTween?.Kill();
        direction *= 100;

        //Debug.Log("움직임~");
        bIsMove = true;
        Vector3 targetRotation = new Vector3(direction.z * wobbleAmount, 0, -direction.x * wobbleAmount);

        rotationTween = transform.DORotate(targetRotation, wobbleDuration)
            .SetEase(Ease.OutQuad);
    }

    public void ResetRotation()
    {
        if (!bIsMove) return;
        rotationTween?.Kill();
        // Debug.Log("제자리로");
        bIsMove = false;
        rotationTween = transform.DORotate(Vector3.zero, wobbleDuration).SetEase(Ease.OutQuad);
           
    }

    public void SetInitLiftSpeed(float liftSpeed)
    {
        LiftSpeed = liftSpeed;
    }

    public void ChangeLiftSpeed(bool bLevelUp)
    {

        LiftSpeed += bLevelUp ? addLiftSpeed : -addLiftSpeed;
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.layer == NormalLayer && other.attachedRigidbody != null)
        {
           
            objectsInTrigger.Add(other.attachedRigidbody);
            other.gameObject.layer = LiftUpLayer;
           
            LiftAbsorption absorption = other.GetComponent<LiftAbsorption>();
            if (absorption)
            {
                absorption.StartAbsorp(LiftSpeed);
            }

        }

    }

    private void OnTriggerExit(Collider other)
    {
      

        if (other.gameObject.layer == LiftUpLayer && other.attachedRigidbody != null)
        {
            
            LiftAbsorption absorption = other.GetComponent<LiftAbsorption>();
            if (absorption)
            {
                absorption.ResetScale();
            }
            other.gameObject.layer = NormalLayer;
            objectsInTrigger.Remove(other.attachedRigidbody);
        }

    }

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && objectsInTrigger.Contains(rb))
        {

            Vector3 directionToShip = (UFOtransform.position - rb.position).normalized;
   
            Vector3 liftForce = Vector3.up * LiftSpeed;

            float distance = Vector3.Distance(rb.position, UFOtransform.position); 
            float attractionStrength = Mathf.Clamp(distance * 2f, 10f, 50f); 

            Vector3 attractionForce = directionToShip * attractionStrength; 
  
            rb.AddForce(liftForce + attractionForce, ForceMode.Force);
         
            rb.linearDamping = Mathf.Lerp(rb.linearDamping, 2f, Time.deltaTime * 2f);

            LiftAbsorption absorption = other.GetComponent<LiftAbsorption>();
            if (absorption)
            {
                absorption.ApplyAbsorptionScale();
            }


        }

    }

    
}
