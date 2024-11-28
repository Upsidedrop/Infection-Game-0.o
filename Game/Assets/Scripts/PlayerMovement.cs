using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 12;
    public float gravity = -18;
    public float groundDistance = 0.4f;
    public float jumpHeight = 3;

    public Transform groundCheck;
    public LayerMask groundMask;
    public CharacterController controller;

    private Vector3 velocity;
    private bool isGrounded;
    private float sprintMeter = 1;
    private bool canSprint = true;
    private float sprintRecharge;
    private Vector3 move;
    private InputAction walk;
    private InputAction sprint;
    InputAction jump;

    private void Start()
    {
        walk = InputSystem.actions.FindAction("Move");
        sprint = InputSystem.actions.FindAction("Sprint");
        jump = InputSystem.actions.FindAction("Jump");

        jump.performed += context =>
        {
            if (isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            }
        };
        
    }

    // Update is called once per frame
    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2;
        }

        Vector2 val = walk.ReadValue<Vector2>();
        move = transform.right * val.x + transform.forward * val.y;

        if (canSprint && sprint.IsPressed() && move.magnitude > 0)
        {
            move *= 2;

            sprintMeter -= Time.deltaTime / 3.5f;

            if (sprintMeter <= 0)
            {
                canSprint = false;
            }

            sprintRecharge = 0;
        }
        else
        {
            if (sprintRecharge > 1 && sprintMeter < 1)
            {
                sprintMeter += Time.deltaTime / 5;
            }
            else
            {
                sprintRecharge += Time.deltaTime;
            }

            if (!canSprint)
            {
                move /= 2.5f;
                if (sprintMeter >= 1)
                {
                    canSprint = true;
                }
            }
        }

        controller.Move(move * speed * Time.deltaTime);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

}
