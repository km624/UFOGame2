using UnityEngine;

public class FallingTrigger : MonoBehaviour
{
    private int currentLevel;
    public void SetCurrentLevel(int  level)
    {
        currentLevel = level;
    }
 

    
    private void OnTriggerEnter(Collider other)
    {
        FallingObject fallingobject = other.GetComponent<FallingObject>();
        if (fallingobject != null&& fallingobject.gameObject.layer==10)
        {
            
            fallingobject.OnSwallow();
    
        }

    }

   
   
}
