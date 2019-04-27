using UnityEngine;

public class BirdMovement : MonoBehaviour
{
    Rigidbody rb;
    Collider collider;

    //Movement
    public float speed;
    public float rotateSpeed;
    private Transform birdTransform;
    private Vector3 directionToMove;

    //Jump
    private int jumpCount;
    private bool playerJumped;
    private float jumpSpeed;
    private bool canJump;
    float distanceToGround;

    //Glide
    private bool glideHeld = false;
    private Vector3 glideGravity;
    private Vector3 jumpGravity;
    public float glideWeight;
    private float defaultWeight;
    private bool isGliding = false;

    //Animation
    private Animator animator;

    void Start()
    {
        Physics.gravity = new Vector3(0, -9.81f, 0);
        rb = GetComponent<Rigidbody>();
        birdTransform = GetComponent<Transform>();
        jumpSpeed = -Physics.gravity.y * 3;
        glideGravity = Physics.gravity / 4;
        jumpGravity = Physics.gravity * 3;
        Physics.gravity = jumpGravity;
        collider = GetComponent<Collider>();
        distanceToGround = 1f;
        animator = GetComponentInChildren<Animator>();

        defaultWeight = rb.mass;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(collider.transform.position, Vector3.down, distanceToGround);
    }


    private bool CanJump()
    {
        bool isGrounded = IsGrounded();
        if (isGrounded)
        {
            jumpCount = 0;
        }

        return isGrounded || jumpCount < 2;
    }

    private bool CanGlide()
    {
        if (IsGrounded())
        {
            isGliding = false;
            return false;
        }

        float downwardVelocity = -rb.velocity.y;
        return downwardVelocity > 0 && glideHeld && jumpCount == 2;
    }
   

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            playerJumped = true;
        }

        glideHeld = Input.GetButton("Jump");

        float moveVertical = Input.GetAxis("Vertical");
        float moveHorizontal = Input.GetAxis("Horizontal");

        Vector3 prevPosition = birdTransform.position;
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);

        animator.SetBool("isGrounded", IsGrounded());
        animator.SetFloat("verticalVel", rb.velocity.y);


        if (playerJumped && CanJump() && !isGliding)
        {
            Vector3 jumpForce = new Vector3(0, jumpSpeed, 0);
            rb.AddForce(jumpForce, ForceMode.Impulse);
            jumpCount++;
            animator.SetTrigger("jump");
            playerJumped = false;
        }

        if (CanGlide())
        {
            isGliding = true;
            Physics.gravity = glideGravity;
            rb.mass = glideWeight;
            animator.SetBool("isGliding", true);
        }
        else
        {
            Physics.gravity = jumpGravity;
            rb.mass = defaultWeight;
            animator.SetBool("isGliding", false);
        }

        if (movement == Vector3.zero)
        {
            animator.SetBool("isWalking", false);
            return;
        }

        animator.SetBool("isWalking", true);

        //Normalize to prevent diagonal acceleration
        Vector3 targetDirection = new Vector3(moveHorizontal, 0f, moveVertical).normalized;
        targetDirection = Camera.main.transform.TransformDirection(targetDirection);
        targetDirection.y = 0.0f;

        birdTransform.position += targetDirection * Time.deltaTime * speed;


        //Get Direction of movement
        Vector3 movementDir = birdTransform.position - prevPosition;
        movementDir.y = 0;

        if (movementDir == Vector3.zero) return;
        //Rotate towards movement direction
        var newRotation = Quaternion.LookRotation(movementDir);
        birdTransform.rotation =
            Quaternion.Slerp(birdTransform.rotation, newRotation, Time.deltaTime * rotateSpeed);
    }
}