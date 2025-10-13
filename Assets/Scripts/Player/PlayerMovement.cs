using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    GameManager gm;
    // Get only instance of player script 
    public static PlayerMovement player;

    [Header("Movement")]
    [SerializeField] float moveSpeed;
    [Space(5)]
    [SerializeField] float jumpForce;
    [SerializeField] float jumpCooldown;
    [SerializeField] float airMult;
    bool canJump = true;

    [Header("Ground Check")]
    [SerializeField] LayerMask isGround;
    [SerializeField] bool grounded = true;
    [SerializeField] float groundDrag;

    [Header("Slope Handling")]
    [SerializeField] float maxSlopeAngle;
    RaycastHit slopeHit;

    [Space(15)]
    public Transform camOrientation; // Grab orientation for rotation 

    // Not sure if we need this yet

    public PlayerState state;
    public enum PlayerState
    {
        moving,
        pushing,
        light,
    }

    Rigidbody rb;
    Vector3 moveDirection;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        player = this;
    }
    void Start()
    {
        gm = GameManager.instance;
        rb = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0, -27f, 0);
    }

    void PlayerInput()
    {
        // Get keyboard input
        Transform orientation = camOrientation;
        orientation.localEulerAngles = new Vector3(0f, orientation.localEulerAngles.y, 0f);
        moveDirection = orientation.forward * Input.GetAxisRaw("Vertical") + orientation.right * Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && canJump && grounded)
        {
            canJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown); // Wait before resetting jump
        }
    }

    void FixedUpdate()
    {
        Movement();
    }

    void Update()
    {
        PlayerInput();
        GroundCheck();
    }

    void Movement()
    {
        Vector3 move = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);

        if (OnSlope()) // Adjust speed while on slope
            rb.linearVelocity = GetSlopeMoveDirection() * moveSpeed * 10.0f;

        // Limit movement in air
        rb.linearVelocity = (grounded) ? move : new Vector3(move.x * airMult, rb.linearVelocity.y, move.z * airMult);

        // Turn off gravity on slope
        rb.useGravity = !OnSlope();
    }

    void GroundCheck()
    {
        // Ground check 
        grounded = Physics.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector3.down, 1.2f, isGround);
        //Debug.DrawLine(new Vector3(transform.position.x, transform.position.y, transform.position.z), new Vector3(transform.position.x, transform.position.y - 1.2f, transform.position.z), Color.magenta);

        // Handle drag
        rb.linearDamping = (grounded) ? groundDrag : 0;
    }

    void Jump()
    {
        // Always start with Y Vel at 0
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.linearVelocity += Vector3.up * jumpForce;
    }

    void ResetJump()
    {
        canJump = true;
    }

    bool OnSlope()
    {
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), Vector3.down, out slopeHit, 0.8f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal); // Calculate slope steepness
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Exit")
        {
            gm.ResetScene();
        }
    }
}
