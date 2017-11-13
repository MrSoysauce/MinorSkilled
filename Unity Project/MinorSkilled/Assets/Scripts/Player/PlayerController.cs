using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Physics")]
    [SerializeField,Range(0,0.9999f)] private float drag = 0.5f;
    [SerializeField] private Vector3 gravity = new Vector3(0,-20,0);
    [SerializeField] public bool useGravity = true;

    [Header("Walking")]
    [SerializeField] private float walkSpeed = 2;
    [SerializeField] private float stairsGravityModifier = 0.01f;

    [Header("Sprinting")]
    [SerializeField] private float runModifier = 2;
    [SerializeField] private float sprintDrainSpeed = 0.1f;
    [SerializeField] private float sprintRegainSpeed = 0.2f;

    [Header("Crouching")]
    [SerializeField] private float sneakModifier = 0.5f;
    [SerializeField] private float crouchDetectionDistance = 0.5f;
    [SerializeField] private LayerMask crouchDetectLayer;

    public enum JumpMode { Controlled, MultiJump }
    [Header("Jumping")]
    [SerializeField] private LayerMask groundedLayerMask;
	[SerializeField] private float groundedDetectRange = 1.2f;
    [SerializeField] public JumpMode jumpMode;
    //Controlled jumping
    [SerializeField] private float jumpStrength = 5;
    [SerializeField] private float controlledJumpTime = 0.1f;
    private Coroutine controlledJumpTimer;
    //Multi jumping
    [SerializeField] private int multiJumps = 2;
    [SerializeField] private float groundedJumpStrength = 6f;
    [SerializeField] private float midairJumpStrength = 10f;
    [SerializeField] private float multiJumpDelay = 0.1f;
    private int jumps = 0;
    private bool delayingJumps = false;

    [Header("Mid-Air")]
    [SerializeField] private float midairModifier = 0.5f;
    [SerializeField] private float midairDrag = 0.1f;

    [Header("Dragging")]
    [SerializeField, Range(0, 1)] private float pullingSlow = 0.3f;
    [SerializeField] private float grabModifier = 0.5f;
    [SerializeField] private float grabDistance = 2f;
    [SerializeField] private float throwForce = 5f;

    [Header("Climbing")]
    [SerializeField] private float climbDistance = 1f;
    [SerializeField] private float climbSpeed = 5;
    [SerializeField] private LayerMask climbLayer;

    [Header("Sliding")]
    [Tooltip("The time the player has to be sprinting before he can slide (stops players from insta sliding)")]
    [SerializeField] private float slideTime = 0.2f;

    [Header("Temp feedback")]
    [SerializeField] private GameObject visuals;

    [Header("Debug")]
    [ReadOnly] public float verticalInput;
    [ReadOnly] public float horizontalInput;
    [ReadOnly] public bool canMove = true;

    [ReadOnly] public bool canJump = true;
    [HideInInspector] public bool allowJump = true;
        
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public bool grabInput;
    [HideInInspector] public bool pulling;
    [Header("Input")]
    [ReadOnly] public bool jumpInput;
    [ReadOnly] public bool jumpInputPressed;
    [ReadOnly] public bool sprintInput;
    [ReadOnly] public bool crouchInput;
    [ReadOnly] public bool climbInput;
    [ReadOnly] public bool isMoving;

    [ReadOnly] public float sprintCharge = 100;

	public bool grounded { get { return Physics.Raycast(transform.position + Vector3.up, Vector3.down, groundedDetectRange + 1f, groundedLayerMask); } }
    public bool IsGrounded(float distance) { return Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, distance, groundedLayerMask); }

    private Vector3 forward;
    private Camera playerCamera;

    [Header("Movement variables")]
    [ReadOnly] public bool crouching;
    [ReadOnly] public bool sprinting;
    [ReadOnly] public bool sliding;
    [ReadOnly] public bool climbing;
    [ReadOnly] public bool grabbing;
    [ReadOnly] public bool onStairs;

    [HideInInspector] public float scriptableSpeedModifier = 1;
    [HideInInspector] public bool forceMovement;
    private float oldInputX, oldInputY;

    private Coroutine slidingRoutine = null;
    private Grabable pickedObject = null;
    private Transform pickedObjOldParent = null;
    private Transform ladder;
    [HideInInspector] public bool onlyWalk;

    private PlayerInteractions interactions;

    private void Start ()
    {
        interactions = GetComponent<PlayerInteractions>();

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

	    //If the player is locked he should not be able to turn
        if (!canMove) return;

	    Turn();

        VerifyJumpPossibility();
    }

    private void GetInput()
    {
        verticalInput = Input.GetAxis("Vertical");
        horizontalInput = Input.GetAxis("Horizontal");
        isMoving = Mathf.Abs(verticalInput) > 0.1f || Mathf.Abs(horizontalInput) > 0.1f;

        if (!jumpInput)
            jumpInput = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button0); //Space or A
        jumpInputPressed = Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Joystick1Button0); //Space or A

        climbInput = Input.GetKey(KeyCode.LeftControl) || Input.GetAxis("XboxAxis10") > 0.5f; //Left ctrl or trigger
        grabInput = Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.Joystick1Button3); //J or Y

        sprintInput = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.Joystick1Button2); //Left shift or X
        crouchInput = Input.GetKey(KeyCode.K) || Input.GetAxis("XboxAxis9") > 0.5f; //K or trigger
    }

    private void Turn()
    {
        float v = verticalInput;
        float h = horizontalInput;

        if (forceMovement)
        {
            if (Math.Abs(oldInputX) < float.Epsilon && Math.Abs(oldInputY) < float.Epsilon)
                oldInputY = 1;

            //Round to 1 or -1
            Vector2 movement = new Vector2(h, v);
            movement.Normalize();
            h = movement.x;
            v = movement.y;

            if (Math.Abs(v) < float.Epsilon || Math.Abs(h) < float.Epsilon)
            {
                v = oldInputY;
                h = oldInputX;
            }

            oldInputY = v;
            oldInputX = h;
        }

        //Set forward direction
        if (playerCamera != null)
        {
            Vector3 horizontalMovement = h * playerCamera.transform.right;
            Vector3 verticalMovement = v * playerCamera.transform.forward;
            forward = horizontalMovement + verticalMovement;
            forward = new Vector3(forward.x, 0, forward.z);
        }
        else forward = new Vector3(v, 0, -h).normalized;

        //Don't turn if grabbing obj
        if (pickedObject != null && grabbing && !pickedObject.liftable)
            return;

        //Turn towards forward direction
        if (climbing)
        {
            Vector3 pos = transform.position;
            Vector3 lad = ladder.position;
            pos.y = 0;
            lad.y = 0;

            transform.rotation = Quaternion.LookRotation((lad - pos).normalized, Vector3.up);
        }
        else if (forward.magnitude > 0.001f && isMoving) transform.rotation = Quaternion.LookRotation(forward);
    }

    private void ProcessInput()
    {
        bool wasCrouching = crouching;
        bool wasSprinting = sprinting;

        crouching = crouchInput;
        sprinting = sprintInput;

        //Are crouching, don't sprint
        if (wasCrouching)
        {
            sprinting = false;
        }

        //Are sprinting, crouching means sliding now
        if (wasSprinting && slidingRoutine == null)
        {
            if (crouching && grounded)
            {
                slidingRoutine = StartCoroutine(SlideCountDown());
            }
        }

        if (!sliding)
        {
            //Force crouching if under object
            if (Physics.Raycast(transform.position, transform.up, crouchDetectionDistance, crouchDetectLayer))
            {
                crouching = true;
                sliding = false;
                sprinting = false;
            }
        }

        //Can't crouch while sprinting or sliding
        if (sprinting || sliding)
            crouching = false;

        //Climbing (can't climb while sliding)
        climbing = false;

        bool canClimb = false;
        for (int i = 0; i < interactions.raycastPoints.Length; i++)
        {
            RaycastHit hit;
            if (Physics.Raycast(interactions.raycastPoints[i].position, transform.forward, out hit, climbDistance,
                climbLayer))
            {
                Vector3 climbDirection = -hit.transform.forward;
                Vector3 walkDir = forward;

                climbDirection.Normalize();
                walkDir.Normalize();
                climbDirection.y = 0;
                walkDir.y = 0;

                float climb = Vector3.Dot(walkDir, climbDirection);
                if (climb <= 0)
                    continue;

                ladder = hit.transform;
                canClimb = true;
                break;
            }
        }

        if (climbInput && !sliding && canClimb)
        {
            sprinting = false;
            crouching = false;
            climbing = true;

            allowJump = false;
        }

        //Grabbing
        if (!climbing && !sliding)
        {
            if (grabInput && pickedObject == null)
            {
                sprinting = false;
                climbing = false;
                sliding = false;

                allowJump = false;

                grabbing = true;

                RaycastHit hit;
                if (Physics.Raycast(transform.position - transform.forward/10, transform.forward, out hit, grabDistance))
                {
                    Grabable grab = hit.transform.GetComponent<Grabable>();
                    if (grab)
                    {
                        pickedObject = grab;
                        pickedObjOldParent = hit.transform.parent;

                        if (!grab.liftable)
                        {
                            Quaternion q = Quaternion.LookRotation(grab.transform.position - transform.position, Vector3.up);
                            transform.rotation = Quaternion.Euler(0, q.eulerAngles.y, 0);
                        }

                        grab.Grab();

                        if (grab.liftable)
                        {
                            hit.transform.localPosition = new Vector3(0, 2.5f, 0);
                            hit.transform.SetParent(transform);
                        }
                        else
                        {
                            hit.transform.SetParent(transform, true);
                        }   
                    }
                }
            }
        }

        //Ungrabbing
        if (grabbing && !grabInput)
        {
            if (pickedObject != null)
            {
                pickedObject.UnGrab();

                //Throw liftables
                if (pickedObject.liftable)
                    pickedObject.GetComponent<Rigidbody>().AddForce(rb.velocity * pickedObject.GetComponent<Rigidbody>().mass * throwForce);
                pickedObject.transform.SetParent(pickedObjOldParent);
                pickedObject = null;
            }
            grabbing = false;
        }

        //Limit player if requested
        if (onlyWalk)
        {
            sliding = false;
            sprinting = false;
            allowJump = false;
        }

        //Temporary feedback
        if (crouching)
        {
            visuals.transform.localPosition = new Vector3(0, -0.5f, 0);
            visuals.transform.localScale = new Vector3(1, 0.5f, 1);
        }
        else
        {
            visuals.transform.localPosition = new Vector3(0, 0, 0);
            visuals.transform.localScale = new Vector3(1, 1, 1);
        }

        //Disable jumping when crouching or sliding
        if (crouching || sliding)
            allowJump = false;

        //Sprint charger
        if (sprinting && isMoving)
            sprintCharge -= sprintDrainSpeed * Time.fixedDeltaTime;
        else
            sprintCharge += sprintRegainSpeed * Time.fixedDeltaTime;

        sprintCharge = Mathf.Clamp(sprintCharge, 0, 100);
        if (Mathf.RoundToInt(sprintCharge) == 0)
            sprinting = false;
    }

    private void FixedUpdate()
    {
        bool isGrounded = grounded;

        ProcessInput();

        float speed = walkSpeed;
        if (sprinting && isGrounded) speed *= runModifier;
        if (crouching) speed *= sneakModifier;
        if (grabbing) speed *= grabModifier;
        if (climbing) speed = 0;
        speed *= scriptableSpeedModifier;

        Vector3 grav = gravity;
        if (onStairs)
            grav *= stairsGravityModifier;
        //Apply gravity
        if (useGravity && !climbing)
            rb.AddForce(grav, ForceMode.Acceleration);

        if (!canMove && !sliding) return;

        //Apply jump input
        Jump();

        //Apply movement
        Move(isGrounded, speed);

        //Apply climbing
        Climb();

        //Apply drag
        ApplyDrag(isGrounded);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + Vector3.up * 0.1f, transform.position + Vector3.up * 0.1f + Vector3.down*1.2f);
        Gizmos.color = Color.white;

        if (ladder != null)
        {
            Vector3 climbDirection = -ladder.forward;
            Vector3 walkDir = forward;

            climbDirection.Normalize();
            walkDir.Normalize();
            climbDirection.y = 0;
            walkDir.y = 0;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(Vector3.zero + new Vector3(0, 0, 3), climbDirection + new Vector3(0, 0, 3));

            Gizmos.color = Color.green;
            Gizmos.DrawLine(Vector3.zero + new Vector3(0, 0, 3), walkDir + new Vector3(0, 0, 3));
        }
    }

    private IEnumerator SlideCountDown()
    {
        Vector3 euler = new Vector3(-90, 0, 0);
        visuals.transform.localRotation = Quaternion.Euler(euler);

        canMove = false;
        sliding = true;

        RigidbodyConstraints constraints = rb.constraints;

        rb.constraints = RigidbodyConstraints.FreezeRotation;

        yield return new WaitForSeconds(slideTime);

        canMove = true;
        sliding = false;
        crouching = true;
        sprinting = false;
        euler = Vector3.zero;
        visuals.transform.localRotation = Quaternion.Euler(euler);
        Vector3 pos = transform.localPosition;
        pos.y += 0.5f;
        transform.localPosition = pos;
        slidingRoutine = null;

        rb.constraints = constraints;
    }

    private void VerifyJumpPossibility()
    {
        if (grounded)
        {
            canJump = true;
            jumps = multiJumps;

            if (controlledJumpTimer != null)
            {
                StopCoroutine(controlledJumpTimer);
                controlledJumpTimer = null;
            }
        }
        else
        {
            //Remove one jump
            if (jumps == multiJumps)
                jumps--;
        }
    }

    private void Jump()
    {
        //Jump
        if (jumpInput && allowJump && canJump)
        {
            switch (jumpMode)
            {
                case JumpMode.Controlled:
                    if (controlledJumpTimer == null)
                        controlledJumpTimer = StartCoroutine(CountDownControlledJump());
                    rb.AddForce(0, jumpStrength*100, 0, ForceMode.Acceleration);

                    if (!jumpInputPressed)//|| !IsGrounded(2))
                    {
                        canJump = false;
                        jumpInput = false;
                    }
                    break;
                case JumpMode.MultiJump:
                    if (delayingJumps)
                        break;

                    if (jumps <= 0)
                    {
                        canJump = false;
                        jumps = 0;
                        jumpInput = false;
                        break;
                    }

                    jumpInput = false;
                    if (grounded)
                        rb.velocity = new Vector3(rb.velocity.x, groundedJumpStrength, rb.velocity.z);
                    else
                        rb.velocity = new Vector3(rb.velocity.x, midairJumpStrength, rb.velocity.z);
                    jumps--;

                    delayingJumps = true;
                    StartCoroutine(DelayMultiJump());
                    break;
            }
        }
    }

    private IEnumerator CountDownControlledJump()
    {
        yield return new WaitForSeconds(controlledJumpTime);
        canJump = false;
        jumpInput = false;
        controlledJumpTimer = null;
    }

    private void Move(bool isGrounded, float speed)
    {
        //Move
        float slow = 1;
        slow *= pulling ? pullingSlow : 1;
        if (!isGrounded)
            slow *= midairModifier;
        rb.AddForce(forward * speed * slow, ForceMode.Impulse);
    }

    private void Climb()
    {
        if (!climbing) return;

        Debug.Assert(ladder != null, "We are climbing without a ladder!?");
        Vector3 climbDirection = -ladder.forward;
        Vector3 walkDir = forward;

        climbDirection.Normalize();
        walkDir.Normalize();
        climbDirection.y = 0;
        walkDir.y = 0;

        float climb = Vector3.Dot(walkDir, climbDirection);
        Vector3 forwardModifier = climbDirection * 2 * walkSpeed;
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed * climb, rb.velocity.z) + forwardModifier;
    }

    private void ApplyDrag(bool isGrounded)
    {
        float d;
        if (climbing)
            d = 0;
        else if (!isGrounded)
            d = midairDrag;
        else
            d = drag;

        rb.velocity = new Vector3(rb.velocity.x * (1 - d), rb.velocity.y, rb.velocity.z * (1 - d));
    }

    private IEnumerator DelayMultiJump()
    {
        yield return new WaitForSeconds(multiJumpDelay);
        delayingJumps = false;
    }
}
