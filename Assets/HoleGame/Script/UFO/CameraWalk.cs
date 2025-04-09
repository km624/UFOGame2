using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;



public class CameraWalk: MonoBehaviour
{
    [Header("ī�޶� Ÿ��")]
    [SerializeField]
    private Transform target;

    [Header("ī�޶� �̵� ������")]
    [SerializeField, Range(0.0f, 1f)]
    private float Delay = 0.3f;
   

    [Header("ī�޶� �Ÿ�")]
    [SerializeField,Range(-20f, 0f)]
    private float offsetZ = 0f;
    [SerializeField,Range(0f, 20f)]
    private float offsetY = 0f;


    private float initialCameraDistance;
    private Vector3 cameraDirection;
   
    private float targetCameraDistance;
   
    Vector3 velocity = Vector3.zero;

    void Start()
     {
         if (target != null)
         {
            Vector3 currentPos = target.position;
            transform.position = new Vector3(currentPos.x, currentPos.y + offsetY, currentPos.z + offsetZ);

            initialCameraDistance = Vector3.Distance(transform.position, target.position);
            cameraDirection = (transform.position - target.position).normalized;
            targetCameraDistance = initialCameraDistance;

        
         }
      
     }
   

    private void FixedUpdate()
    {
        if (target != null) {

           
            float CurrentCameraDistance = Vector3.Distance(transform.position, target.position);
            
            transform.position = Vector3.SmoothDamp(transform.position, target.position+cameraDirection * targetCameraDistance, ref velocity, Delay);
           
           


        }
    }

    public void CallBack_CameraDistancedUp(float targetsizePercent)
    {
        if (target != null)
        {
            targetCameraDistance = initialCameraDistance * targetsizePercent;
        }
    }

   
}
