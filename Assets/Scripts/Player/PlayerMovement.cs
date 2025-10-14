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
    bool exitingSlope;

    [Space(15)]
    public Transform camOrientation; // Grab orientation for rotation
    public DetectPlayer moveableObj = null;
    bool moveObj;

    // Not sure if we need this yet

    public PlayerState state;
    public enum PlayerState
    {
        walking,
        grabbing,
        light,
    }

    Rigidbody rb;
    Vector3 moveDirection;

    void Awake()
    {
        player = this;
    }
    void Start()
    {
        Physics.gravity = new Vector3(0, -27f, 0);
        Cursor.lockState = CursorLockMode.Locked;

        gm = GameManager.instance;
        rb = GetComponent<Rigidbody>();
    }

    void PlayerInput()
    {
        // Get forward position from camera Y rotation
        Transform orientation = camOrientation;
        orientation.localEulerAngles = new Vector3(0f, orientation.localEulerAngles.y, 0f);

        // Get keyboard input
        if (state == PlayerState.grabbing)
            // Prevent left or right movement while grabbing
            moveDirection = transform.forward * Input.GetAxisRaw("Vertical") + (orientation.right * Input.GetAxisRaw("Horizontal") * 0.2f);
        else
            // Move in direction of camera
            moveDirection = orientation.forward * Input.GetAxisRaw("Vertical") + orientation.right * Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && canJump && grounded && state != PlayerState.grabbing)
        {
            canJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown); // Wait before resetting jump
        }


        // Check if player is facing moveable object
        if (moveableObj != null)
        {
            moveObj = Physics.Raycast(transform.position, transform.forward, 0.55f, isGround);
            //Debug.DrawLine(transform.position, new Vector3(transform.position.x - 0.55f, transform.position.y, transform.position.z), Color.magenta);
        }
        else
        {
            if (moveObj) moveObj = false;
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
        StateHandler();
    }

    void StateHandler()
    {
        if (Input.GetMouseButton(0) && moveObj)
        {
            state = PlayerState.grabbing;
            moveSpeed = 2.5f; // Limit player speed while grabbing
            // Set player as the Move Transform object parent
            if (moveableObj != null) moveableObj.transform.SetParent(this.transform);
        }
        else if (!Input.GetMouseButton(0) || moveableObj == null)
        {
            state = PlayerState.walking;
            // Unparent player
            if (moveableObj != null) moveableObj.transform.SetParent(null);
            moveSpeed = 5.0f; // Reset speed
        }
    }

    void Movement()
    {
        Vector3 move = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);

        if (OnSlope() && !exitingSlope)
        {
            // Adjust speed while on slope
            rb.linearVelocity = GetSlopeMoveDirection() * moveSpeed;

            // Prevent bump effect when running upward
            if (rb.linearVelocity.y > 0f) rb.AddForce(Vector3.down * 80.0f, ForceMode.Force);
        }
        else
        {
            // Limit movement in air
            rb.linearVelocity = (grounded) ? move : new Vector3(move.x * airMult, rb.linearVelocity.y, move.z * airMult);
        }

        // Turn off gravity on slope
        rb.useGravity = !OnSlope();

        // Handle rotation
        if (moveDirection != Vector3.zero && state != PlayerState.grabbing)
        {
            float angleDiff = Vector3.SignedAngle(transform.forward, moveDirection, Vector3.up);
            rb.angularVelocity = new Vector3(rb.angularVelocity.x, angleDiff * 0.2f, rb.angularVelocity.z);
        }
        else
        {
            rb.angularVelocity = Vector3.zero;
        }
    }

    void GroundCheck()
    {
        // Ground check 
        grounded = Physics.Raycast(transform.position, Vector3.down, 1.2f, isGround);
        //Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - 1.2f, transform.position.z), Color.magenta);

        // Handle drag
        rb.linearDamping = (grounded) ? groundDrag : 0;
    }

    void Jump()
    {
        exitingSlope = true;
        // Always start with Y Vel at 0
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.linearVelocity += Vector3.up * jumpForce;
    }

    void ResetJump()
    {
        canJump = true;
        exitingSlope = false;
    }

    bool OnSlope()
    {
        if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector3.down, out slopeHit, 1.2f))
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
