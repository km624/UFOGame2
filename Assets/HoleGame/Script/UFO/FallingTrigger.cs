using UnityEngine;

public class FallingTrigger : MonoBehaviour
{
    private int currentLevel;
    public void SetCurrentLevel(int  level)
    {
        currentLevel = level;
    }

   
    private void OnTriggerStay(Collider other)
    {
        //Debug.Log(other.name);
        FallingObject fallingobject = other.GetComponent<FallingObject>();
        if (fallingobject != null&& fallingobject.gameObject.layer==10)
        {
            
            fallingobject.OnSwallow();

            BossObject boss = other.GetComponent<BossObject>();
            
            if(boss)
            {
                boss.BossSwallow();
            }

            StarObject star = other.GetComponent<StarObject>();
            if(star)
            {
                star.StarSwallow();
            }

        }
    }
 
}
