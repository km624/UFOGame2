using UnityEngine;
using UnityEngine.InputSystem;


public class UFOMovement : MonoBehaviour
{
    [Header("이동속도")]
    [SerializeField]
    private float HoleSpeed  = 10.0f;
    
    public float GetHoleSpeed() { return HoleSpeed; }

    [Header("조이스틱")]
    public Joystick joystick;

    public UFOMotion motion;
    

    private bool MoveActive = true;
    private Rigidbody rb;

  
    void Start()
    {
        rb = GetComponent<Rigidbody>();
       
    }


    void FixedUpdate()
    {
        if (!MoveActive) return;
        float moveX;
        float moveZ;


        moveX = joystick.Horizontal* 0.01f* HoleSpeed;
        moveZ = joystick.Vertical * 0.01f* HoleSpeed;

 
        Vector3 moveVector = new Vector3(moveX, 0, moveZ);
       
       

        if (moveVector.sqrMagnitude > 0.0001f)
        {
            rb.MovePosition(rb.position + moveVector);
        }

    
        if (motion != null)
      {
            Vector3 movementDirection = new Vector3(moveX, 0, moveZ);

            if (movementDirection.sqrMagnitude > 0.0001f)
            {
                motion.StartWobble(movementDirection);
                
            }
            else
            {
                motion.ResetRotation();
              
            }
      }
       
       
    }

    public void SetMoveActive(bool active)
    {
        MoveActive = active;
    }

    public void SetSpeed(float speed)
    {
        HoleSpeed = speed;
    }
}
