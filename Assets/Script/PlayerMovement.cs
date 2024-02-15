using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    PlayerStateManager stateManager;
    [Header("Physics")]
    [SerializeField] private float jumpPower = 5;
    [SerializeField] private float chargedJumpPower = 5;
    [SerializeField] private float acceleration = 50;
    [SerializeField] private float targetTopSpeed = 12f;
    [SerializeField] private float topSpeed = 12f;
    [SerializeField] private float jumpTopSpeed = 16f;
    [SerializeField] public float linearDrag = 10f;
    [SerializeField] private float slopeLinearDrag = 15f;
    [SerializeField] private float fluidDrag = 15f;
    [SerializeField] private float airLinearDrag = 8f;
    [SerializeField] private float bulletPushLinearDrag = 4f;
    [SerializeField] private float fallMultiplier = 5f;
    [SerializeField] private float lowJumpFallMultiplier = 10f;
    private bool directionChange => (rb.velocity.x > 0f && horizontal < 0f) || (rb.velocity.x < 0f && horizontal > 0f);

    [Header("States")]
    public float horizontal;
    public float jump;
    public bool crouching;
    public bool grounded;
    public bool jumping;
    public bool bulletPush;
    public bool prepareJump;
    public bool performJump;
    public bool crouchPress;
    public bool crouchRelease;
    bool onSlope;
    bool onFluid;
    bool crouchStart;
    bool crouchEnd;
    public bool onNoCollisionTiles;
    public bool inFrontOfNoCollisionWalkable;

    InputManager input;

    [Header("Components")]
    [SerializeField] public Rigidbody2D rb;
    [SerializeField] public Transform groundCheck;
    [SerializeField] public Transform checkJumpableGround;
    [SerializeField] public Transform feetPosition;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask jumpableLayer;
    [SerializeField] private LayerMask fluidLayer;
    public float checkSolidRad = .3f;
    public float checkGroundingRad = .4f;


    private void Awake()
    {
        input = GetComponent<InputManager>();
        stateManager = GetComponent<PlayerStateManager>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        GetInput();
        GetMovementActions();

        jumping = stateManager.anim.GetBool("jumping");
    }

    void GetInput()
    {
        horizontal = input.horizontalMovement.ReadValue<float>();
        jump = input.jump.ReadValue<float>();
        crouchPress = input.down.WasPerformedThisFrame();
        crouchRelease = input.down.WasReleasedThisFrame();
        performJump = input.jump.WasPressedThisFrame();
    }

    void GetMovementActions()
    {
        if (performJump)
        {
            Jump();
        }

        if (crouchPress && grounded)
        {
            crouching = true;
            stateManager.AdjustCollider(crouching);
        }
        if (crouchRelease || !grounded )
        {
            crouching = false;
            stateManager.AdjustCollider(crouching);
        }
    }

    private void FixedUpdate()
    {
        MoveCharacter();



        if (grounded)
        {
            ApplyGroundLinearDrag();
            if (onSlope && !bulletPush)
                ApplyGroundSlopeDrag();
        }
        if (onFluid)
        {
            ApplyFluidLinearDrag();
        }
        if(!grounded)
        {
            ApplyAirLinearDrag();
            FallMultiplier();
        }
        

        CheckSolidTerrainDownwards();

        grounded = IsGrounded();
        onFluid = isOnFluid();
        onSlope = OnSlope();

        if (bulletPush)
        {
            ApplyBulletPushLinearDrag();
            //Debug.Log(rb.velocity.y);
            if (rb.velocity.y < 0)
            {
                bulletPush = false;
            }
        }
        if (onSlope)
        {
            checkGroundingRad = 1;
        }
        else checkGroundingRad = .4f;
    }

    void ApplyBulletPushLinearDrag()
    {
        rb.drag = bulletPushLinearDrag;
    }

    private void Jump()
    {
        if (!grounded)
            return;

        float jumpF = jumpPower;
        if (crouching)
        {
            jumpF = chargedJumpPower;
            crouching = false;
        }

        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.AddForce(Vector2.up * jumpF, ForceMode2D.Impulse);
    }

    private void MoveCharacter()
    {
        if (bulletPush)
            return;
        if (crouching)
        {
            return;
        }

        rb.AddForce(new Vector2(horizontal, 0f) * acceleration);

        targetTopSpeed = topSpeed;
        if (jumping)
            targetTopSpeed = jumpTopSpeed;

        if(Mathf.Abs(rb.velocity.x) > targetTopSpeed)
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * topSpeed, rb.velocity.y);
        }
    }

    bool OnSlope()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up * -1, 2, groundLayer);
        
        if(hit.collider != null)
        {
           // Debug.Log("Object: " + hit.collider.gameObject.name + " normal: " + hit.normal);

            if (hit.normal.x != 0)
                return true;
        }

        return false;
        
    }

    private bool IsGrounded()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, checkGroundingRad, groundLayer);


        if (colliders.Length <= 0)
            return false;

        bool onGround = false;
        I_JumpableNoCollision jumpable = null;
        foreach (Collider2D collider in colliders)
        {
            jumpable = collider.GetComponent<I_JumpableNoCollision>();

            if(jumpable != null)
            {
                if (jumpable.IsSolid())
                {
                    return true;
                }
            }
            

            if (jumpable == null && onGround == false)
                onGround = true;
        }


        if (jumpable != null && onGround == true)
        {
            //Debug.Log("JUMPABLE AND GROUND");
            return true;
        }
        if (jumpable != null && onGround == false)
        {
            //Debug.Log(" NO GROUND");
            return false;
        }
        else
            return true;
        //return Physics2D.OverlapCircle(groundCheck.position, checkGroundingRad, groundLayer) && !inFrontOfNoCollisionWalkable;
    }

    private bool isOnFluid()
    {
        return Physics2D.OverlapCircle(groundCheck.position, checkGroundingRad, fluidLayer) && !inFrontOfNoCollisionWalkable;

    }

    void CheckSolidTerrainDownwards()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(checkJumpableGround.position, checkSolidRad, jumpableLayer);
        // Debug.Log(collider != null);
        if (colliders.Length <= 0)
            return;

        foreach (Collider2D collider in colliders)
        {

            if (collider.gameObject.GetComponent<I_JumpableNoCollision>() == null)
            {
                inFrontOfNoCollisionWalkable = false;
                onNoCollisionTiles = false;
                return;
            }

            //Debug.Log(collider.gameObject.name);
            GameObject g = collider.gameObject;
            I_JumpableNoCollision tile = collider.gameObject.GetComponent<I_JumpableNoCollision>();

            float tileY = g.transform.position.y;
            float playerY = groundCheck.position.y;
            float dif = playerY - tileY;
            //Debug.Log(dif);
            onNoCollisionTiles = tile != null && dif > .5f && tile.IsWalkableSurface();
            inFrontOfNoCollisionWalkable = tile != null && !onNoCollisionTiles;
            if (!onNoCollisionTiles)
              return;
            tile.MakeSolidByAxis(true, false);
            CheckSolidTerrainHorizontal();
        }
        
        
    }

    void CheckSolidTerrainHorizontal()
    {
        Vector2 origin = new Vector2(groundCheck.position.x - 1, groundCheck.position.y + .5f);
        Vector2 right = new Vector2(origin.x + 1, origin.y);
        Debug.DrawLine(origin, right);

        RaycastHit2D hit;
        hit = Physics2D.Linecast(origin, right, groundLayer);

        if (!hit)
            return;

        if (hit.collider.gameObject.GetComponent<I_JumpableNoCollision>() != null)
        {
            hit.collider.gameObject.GetComponent<I_JumpableNoCollision>().MakeSolidByAxis(false, true);
        }
    }
    void ApplyGroundSlopeDrag()
    {
        rb.gravityScale = 5;

        if (Mathf.Abs(horizontal) == 0 || directionChange)
        {

            rb.drag = 2500;
        }
        else
        {
            rb.drag = slopeLinearDrag;
        }
    }
    private void ApplyGroundLinearDrag()
    {
        rb.gravityScale = 1;
        if(Mathf.Abs(horizontal) == 0 || directionChange)
        {
            rb.drag = linearDrag;
        }
        else if(!crouching)
        {
            rb.drag = 0f;
        }
        else
        {
            rb.drag = 1000;
        }
        
    }

    private void ApplyFluidLinearDrag()
    {
        rb.gravityScale = 1;
        rb.drag = fluidDrag;
    }

    private void FallMultiplier()
    {
        if(rb.velocity.y < 0)
        {
            //Debug.Log("fall multiplier");
            rb.gravityScale = fallMultiplier;
        }
        else if(rb.velocity.y > 0 && jump < 1)
        {
           // Debug.Log("low Jump fall mult");
            rb.gravityScale = lowJumpFallMultiplier;
        }
        else
        {
            rb.gravityScale = 1;
        }
    }

    private void ApplyAirLinearDrag()
    {
        rb.drag = airLinearDrag;
    }

    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!bulletPush)
            return;

       // Debug.Log(collision.gameObject);

        
        //bulletPush = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(checkJumpableGround.position, checkSolidRad);
        
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(groundCheck.position, checkGroundingRad);
    }
}
