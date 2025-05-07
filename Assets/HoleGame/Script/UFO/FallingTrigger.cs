using UnityEngine;

public class FallingTrigger : MonoBehaviour
{
    private int currentLevel;

    float BosstargetScale = 0.3f;
    public void SetCurrentLevel(int  level)
    {
        currentLevel = level;
    }

    private void OnTriggerEnter(Collider other)
    {
        FallingObject fallingobject = other.GetComponent<FallingObject>();
        if (fallingobject != null && fallingobject.gameObject.layer == 10)
        {
            BossObject boss = fallingobject as BossObject;

            if (boss != null) return;

            fallingobject.OnSwallow();
   

            if(GameManager.Instance!=null)
            {
                
                GameManager.Instance.vibrationManager.Play(VibrationEnum.AbsorbObject);
               
            }

        }
    }

   private void OnTriggerStay(Collider other)
    {
       
        BossObject bossgobject = other.GetComponent<BossObject>();
        if (bossgobject != null&& bossgobject.gameObject.layer==10 && bossgobject.transform.localScale.y <= BosstargetScale)
        {

            bossgobject.OnSwallow();


        }
    }
 
}
