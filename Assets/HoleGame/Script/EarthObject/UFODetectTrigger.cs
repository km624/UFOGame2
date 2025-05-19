using UnityEngine;

public class UFODetectTrigger : MonoBehaviour
{

    [SerializeField] private float ufoTargetHeight = 2f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("UFO"))
        {
            UFOPlayer controller = other.GetComponent<UFOPlayer>();
            if (controller != null)
            {
              
                controller.CallBack_PassMapObject(true, ufoTargetHeight);

                
            } 
        }
        
    }

  

  
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("UFO"))
        {
            UFOPlayer controller = other.GetComponent<UFOPlayer>();
            if (controller != null)
            {
                controller.CallBack_PassMapObject(false ,0);
               
            }
                
        }
    }
}
