using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMovement : MonoBehaviour
{
    [SerializeField] Transform groundCheck = null;
    [SerializeField] LayerMask groundMask = 0;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] float groundDownwardForce = -5.0f;

    [SerializeField] float isJumpingTimer = 1.5f;

    private CharacterController charController = null;
    private float xMovement;
    private float zMovement;
    private float currentSpeed;
    private Vector3 movementVector;
    private Vector3 velocity;

    private bool isSprinting = false;
    private bool isGrounded = false;
    private bool isJumping = false;

    private void Awake()
    {
        charController = GetComponent<CharacterController>();
    }

    private void Start()
    {
        currentSpeed = PlayerSettings.walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        Inputs();
        GroundMovementController();
    }

    private void Inputs()
    {
        // General Movement
        xMovement = Input.GetAxis("Horizontal");
        zMovement = Input.GetAxis("Vertical");
        movementVector = transform.right * xMovement + transform.forward * zMovement;

        // Sprinting
        if (Input.GetKey(KeyCode.LeftShift))
            isSprinting = true;
        else
            isSprinting = false;

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
            StartCoroutine(JumpControl());
    }

    private void GroundMovementController()
    {
        GroundCheck();
        if (isGrounded && velocity.y < groundDownwardForce)
        {
            ResetDownwardVelocity();
        }

        if (isSprinting && isGrounded)
        {
            currentSpeed = Mathf.Lerp(currentSpeed, PlayerSettings.runSpeed, Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, PlayerSettings.walkSpeed, Time.deltaTime);
        }

        velocity.y += PlayerSettings.gravity * Time.deltaTime;

        charController.Move(movementVector * currentSpeed * Time.deltaTime);
        charController.Move(velocity * Time.deltaTime);
    }


    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    }

    IEnumerator JumpControl()
    {
        isJumping = true;
        velocity.y = Mathf.Sqrt(PlayerSettings.jumpHeight * -2.0f * PlayerSettings.gravity);
        yield return new WaitForSeconds(isJumpingTimer);
        isJumping = false;
    }

    private void ResetDownwardVelocity() { velocity.y = groundDownwardForce; }
}
