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
    public int LiftUpLayer;
    public int NormalLayer;

    [SerializeField]
    private float wobbleAmount = 2.5f;
    private float wobbleDuration = 1f;

    private Tween rotationTween;
    private Tween spinTween;

    private bool bIsMove = false;
    /* public void InitGameState(GameState gamestate)
     {
         this.gameState = gamestate;

     }

     void Start()
     {
         UpdateSizeText(CurrentSizeLevel);

         initialHoleSize = transform.localScale.x;
         targetSize = initialHoleSize;
         targetHeight = transform.position.y;


     }


     void FixedUpdate()
     {
         float moveX;
         float moveZ;


         moveX = joystick.Horizontal * 0.01f * HoleSpeed;
         moveZ = joystick.Vertical * 0.01f * HoleSpeed;

         transform.position += new Vector3(moveX, 0, moveZ);


         UpdateSizeGaugeBar();

         UpdateHoleSize();
     }

     private void UpdateSizeGaugeBar()
     {

         SizeGaugeBar.fillAmount =  Mathf.Lerp(SizeGaugeBar.fillAmount, TargetfillPercent, Time.deltaTime * fillSpeed);
     }

     public void AddSizeGauge(float gauge , FallingObject fallingobj)
     {

         CurrentSizeGauge += gauge;

         if(CurrentSizeGauge>= MaxSizeGauge)
         {
             CurrentSizeGauge -= MaxSizeGauge;
             SizeGaugeBar.fillAmount = 0.0f;
             ChangeSize(true);
         }
         TargetfillPercent = Mathf.Clamp01(CurrentSizeGauge/ MaxSizeGauge);

         SpawnGagueEffect(gauge);

         gameState.RemoveFallingObject(fallingobj);
     }

     void SpawnGagueEffect(float gauge)
     {
         AddGaugeSound.Play();
         DamageNumber damageNumber = numberPrefab.Spawn(transform.position, gauge);
     }

     public void ChangeSize(bool bSizeUp)
     {
         if (bSizeUp)
         {
             CurrentSizeLevel++;
             Vector3 CurrentScale = transform.localScale;

             targetSize = CurrentScale.x + AddSizeUp;
             targetHeight = transform.position.y + AddSizeUp;

         }
         else
         {
             CurrentSizeLevel--;
             Vector3 CurrentScale = transform.localScale;
             targetSize = CurrentScale.x - AddSizeUp;
             targetHeight = transform.position.y - AddSizeUp;
         }
         float SizePercent = targetSize / initialHoleSize;


         CameraScript.CallBack_CameraDistancedUp(SizePercent);
         UpdateSizeText(CurrentSizeLevel);
     }


     private void UpdateHoleSize()
     {

         //사이즈 업데이트
         float NewSizeupSclale = Mathf.SmoothDamp(transform.localScale.x, targetSize, ref Holevelocity, smoothTime);
         transform.localScale = new Vector3(NewSizeupSclale, transform.localScale.y, NewSizeupSclale);

         //ufo 높이
         float newY = Mathf.SmoothDamp(transform.position.y, targetHeight, ref Heightvelocity, smoothTime);

         transform.position = new Vector3(transform.position.x, newY, transform.position.z);


     }

     private void UpdateSizeText(int currentsizelevel)
     {
         SizeText.text = "크기: " + currentsizelevel;
     }*/

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
