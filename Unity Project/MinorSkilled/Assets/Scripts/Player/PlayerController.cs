using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float walkSpeed = 2;
    [SerializeField] private float jumpStrength = 5;
    [SerializeField, Range(0, 1)] private float pullingSlow = 0.3f;
    [SerializeField,Range(0,0.9999f)] private float drag = 0.5f;
    [SerializeField] private Vector3 gravity = new Vector3(0,-20,0);
    [SerializeField] public bool useGravity = true;

    [Tooltip("The time the player has to be sprinting before he can slide (stops players from insta sliding)")]
    [SerializeField] private float slideWaitTime = 0.5f;

    [SerializeField] private float runModifier = 2;
    [SerializeField] private float sneakModifier = 0.5f;
    [SerializeField] private bool canSneakWhileRunning = true;

    [Header("Debug")]
    [ReadOnly] public float verticalInput;
    [ReadOnly] public float horizontalInput;
    [ReadOnly] public bool canMove = true;

    [ReadOnly,SerializeField] private bool canJump = true;
    [HideInInspector] public bool allowJump = true;
        
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public bool grabInput;
    [HideInInspector] public bool pulling;
    public bool grounded { get { return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 1.2f); } }

    [ReadOnly] public bool jumpInput;
    [ReadOnly] public bool sprintInput;
    [ReadOnly] public bool crouchInput;
    [ReadOnly] public bool sneakInput;
    [ReadOnly] public bool slideInput;

    private bool isMoving;

    private float sprintStartTime;

    private Vector3 forward;
    private Camera playerCamera;

    private void Start ()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        if (Camera.main != null) playerCamera = Camera.main;
        else Debug.LogError("Main scene camera is missing! Please make sure that there is a camera tagged Main Camera!");
    }

    private void Update ()
	{
	    GetInput();

	    if (Input.GetKeyDown(KeyCode.Escape))
	    {
	        Cursor.visible = true;
	        Cursor.lockState = CursorLockMode.None;
	    }
	    else if (Input.GetMouseButton(0))
	    {
	        Cursor.visible = false;
	        Cursor.lockState = CursorLockMode.Locked;
	    }

	    //If the player is locked he should not be able to turn
	    if (!canMove) return;
	    Turn();

	    //Check if can jump
	    if (rb.velocity.y <= 0 && grounded) canJump = true;
    }

    private void GetInput()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        isMoving = Mathf.Abs(verticalInput) > 0.05f || Mathf.Abs(horizontalInput) < 0.05f;

        jumpInput = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Joystick1Button0);
        grabInput = Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.Joystick1Button3);

        //Sprinting
        bool oldSprint = sprintInput;
        sprintInput = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Joystick1Button2);
        if (sprintInput && !oldSprint) //Wasn't sprinting before, save time
            sprintStartTime = Time.timeSinceLevelLoad;
        else if (!sprintInput)
            sprintStartTime = Time.timeSinceLevelLoad; //Just reset time 

        //Been sprinting for long enough?
        if (Time.timeSinceLevelLoad - sprintStartTime > slideWaitTime)
            slideInput = Input.GetKeyDown(KeyCode.C) || Input.GetAxis("XboxAxis9") > 0.2f;

        //If we are not moving we can crouch
        if (!isMoving && !sprintInput)
        {
            sneakInput = false;

            if (Input.GetKeyDown(KeyCode.C) || Input.GetAxis("XboxAxis9") > 0.2f)
                crouchInput = !crouchInput;
        }
        //If we are moving then we can sneak
        else if (isMoving)
        {
            crouchInput = false;

            if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.Joystick1Button9))
                sneakInput = !sneakInput;
        }

        //Can't sneak while sprinting? Are we sprinting? disable sneaking
        if (!canSneakWhileRunning && sprintInput)
            sneakInput = false;
    }

    private void Turn()
    {
        //Don't turn if we're not moving
        //if (!isMoving)
        //    return;

        //Set forward direction
        if (playerCamera != null)
        {
            Vector3 horizontalMovement = horizontalInput * playerCamera.transform.right;
            Vector3 verticalMovement = verticalInput * playerCamera.transform.forward;
            forward = horizontalMovement + verticalMovement;
            forward = new Vector3(forward.x, 0, forward.z);
        }
        else forward = new Vector3(verticalInput, 0, -horizontalInput).normalized;

        //Turn towards forward direction
        if (forward.magnitude > 0.001f && isMoving) transform.rotation = Quaternion.LookRotation(forward);
    }

    private void FixedUpdate()
    {
        float speed = walkSpeed;
        if (sprintInput) speed *= runModifier;
        if (sneakInput) speed *= sneakModifier;

        //Apply gravity
        if (useGravity)
            rb.AddForce(gravity, ForceMode.Acceleration);
        if (!canMove) return;
        //Run
        float slow = 1;
        slow *= pulling ? pullingSlow : 1;
        rb.AddForce(forward * speed * slow, ForceMode.Impulse);

        //Jump
        if (jumpInput && canJump && allowJump)
        {
            rb.AddForce(0, jumpStrength * 100, 0);
            canJump = false;
        }

        //Apply drag
        rb.velocity = new Vector3(rb.velocity.x * (1 - drag), rb.velocity.y, rb.velocity.z * (1 - drag));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + Vector3.up * 0.1f, transform.position + Vector3.up * 0.1f + Vector3.down*1.2f);
        Gizmos.color = Color.white;
    }
}
