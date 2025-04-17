using UnityEngine;
using UnityEngine.InputSystem;


public class UFOMovement : MonoBehaviour
{
    [Header("이동속도")]
    [SerializeField]
    private float HoleSpeed  = 10.0f;
    
    public float GetHoleSpeed() { return HoleSpeed; }

    [Header("조이스틱")]
    public FloatingJoystick joystick;

    public UFOMotion motion;
    public HoleScript Beam;

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

        //transform.position += new Vector3(moveX, 0, moveZ);

        Vector3 moveVector = new Vector3(moveX, 0, moveZ);
       
       

        if (moveVector.sqrMagnitude > 0.0001f)
        {
            rb.MovePosition(rb.position + moveVector);
        }

        //rb.MovePosition(transform.position + moveVector);

        if (motion != null)
      {
            Vector3 movementDirection = new Vector3(moveX, 0, moveZ);

            if (movementDirection.sqrMagnitude > 0.0001f)
            {
                motion.StartWobble(movementDirection);
                //Beam.StartWobble(movementDirection);


            }
            else
            {
                motion.ResetRotation();
               // Beam.ResetRotation();
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
