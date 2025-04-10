using UnityEngine;

public class FallingTrigger : MonoBehaviour
{

    private UFOPlayer player;
    
    void Start()
    {
        player = GetComponentInParent<UFOPlayer>();
    }

    
    private void OnTriggerEnter(Collider other)
    {
        FallingObject fallingobject = other.GetComponent<FallingObject>();
        if (fallingobject != null)
        {
            
            fallingobject.OnSwallow();
           

        }

    }

   
   
}
