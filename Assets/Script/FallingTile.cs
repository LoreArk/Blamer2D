using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FallingTile : MonoBehaviour, I_Shootable, I_JumpableNoCollision
{

    PlayerStateManager playerState;
    [SerializeField] Sprite activeTileSprite;
    [HideInInspector]public Rigidbody2D rb;
    [SerializeField] public bool activated;
    bool doDamage;
    [HideInInspector]public List<I_Shootable> shootables = new List<I_Shootable>();
    BoxCollider2D boxCollider;
    public BoxCollider2D ignoreCharCol;
    public LayerMask scenarioLayer;
    public bool grounded;
    Collider2D playerCollider;
    public bool isSolid;
    public bool isHorizontalSolid;
    public Transform topSurfaceReference;
    Collider2D[] nearbyJumpable;
    public bool nearbyTileSwitched;
    public bool isWalkableSurface;
    public float checkIfSurfaceRadius = 0.3f;
    Color isSurfaceColor;
    [HideInInspector] public FallingTilesGroup group;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        isSurfaceColor = Color.yellow;
    }

    void Start()
    {
        playerState = PlayerStateManager.Instance;
        playerCollider = playerState.bodyCollider;
        IgnoreCollision(playerCollider, true);
    }

    void Update()
    {
        if (isSolid)
        {
            CheckPlayerYPosition();
        }
        if (isHorizontalSolid)
        {
            CheckPlayerXPosition();
        }

        isWalkableSurface = IsWalkableSurface();

        if (isWalkableSurface)
        {
            isSurfaceColor = Color.green;
        }
        else
            isSurfaceColor = Color.yellow;

    }

    public bool IsWalkableSurface()
    {
        Vector2 pos = new Vector2(topSurfaceReference.position.x, topSurfaceReference.position.y + .5f);
        Collider2D topCol = Physics2D.OverlapCircle(pos, checkIfSurfaceRadius, scenarioLayer);


        if (topCol == null)
            return true;
        else
            return false;
            
    }

    void FixedUpdate()
    {
        if (!activated)
            return;
        if (group != null)
            return;

       // Debug.Log("TILE UPDATE");
        if (rb.velocity.magnitude > 0.2f)
            doDamage = true;
        else
            doDamage = false;

        grounded = IsGrounded();
        
    }


    public void MakeSolidOtherTiles()
    {
        Debug.Log("activateTiles");
        nearbyTileSwitched = true;
        Vector2 origin = new Vector2(transform.position.x, transform.position.y + .5f);
        Vector2 horizontalLeft = new Vector2(origin.x - 2, origin.y);
        Vector2 horizontalRight = new Vector2(origin.x + 2, origin.y);
        Vector2 verticalUp = new Vector2(origin.x, origin.y + 2);
        Vector2 verticalDown = new Vector2(origin.x, origin.y - 2);
        Debug.DrawLine(horizontalRight, horizontalLeft, Color.green, 6);
        Debug.DrawLine(verticalDown, verticalUp, Color.green, 6);
        RaycastHit2D vHit;
        RaycastHit2D hHit;
        hHit = Physics2D.Linecast(horizontalLeft, horizontalRight);
        vHit = Physics2D.Linecast(verticalDown, verticalUp);

        if (hHit.collider.GetComponent<I_JumpableNoCollision>() != null)
        {
            Debug.Log("Horizontal Hit");
            hHit.collider.GetComponent<I_JumpableNoCollision>().IgnoreCollision(playerCollider, false);
        }
        if(vHit.collider.GetComponent<I_JumpableNoCollision>() != null)
        {
            Debug.Log("Vertical Hit");

            vHit.collider.GetComponent<I_JumpableNoCollision>().IgnoreCollision(playerCollider, false);
        }
    }

    public void CheckPlayerYPosition()
    {
        float playerY = playerState.movement.feetPosition.position.y;
        float tileTopY = topSurfaceReference.position.y;

        if (playerY >= tileTopY)
            return;

        //Debug.Log(playerY + " is < " + topSurfaceReference.position.y);
        IgnoreCollision(playerCollider, true);
    }
    public void CheckPlayerXPosition()
    {
        Vector2 origin = playerState.movement.groundCheck.position;
        float distance = Vector2.Distance(transform.position, origin);
        if (distance > 1.25f)
        {
            Debug.Log("IGNORE BY DISTANCE");
            isHorizontalSolid = false;
            IgnoreCollision(playerCollider, true);
        }
    }

    public void DoDamage(DamageSettings newDamage)
    {
        if (activated)
            return;

        activated = true;
        GetComponentInChildren<SpriteRenderer>().sprite = activeTileSprite;

        if (group != null)
        {
            group.TileActivated();
            return;
        }

        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
    }



    bool IsGrounded()
    {
        if (grounded)
            return true;
        Vector2 origin = new Vector2(transform.position.x, transform.position.y);
        Debug.DrawLine(transform.position, origin, Color.white);
        return Physics2D.OverlapCircle(origin, 0.5f, scenarioLayer);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!doDamage)
            return;
        //Debug.Log(collision.gameObject.name);


        if (collision)
        {
            //Debug.Log(collision.gameObject.name);
            I_Shootable shootable = collision.gameObject.GetComponent<I_Shootable>();
            if (shootable == null)
                return;

            if (shootables.Contains(shootable))
                return;
            Debug.Log("Falling tile damage");
            DamageSettings newDamage = new DamageSettings();
            newDamage.damage = 100;
            shootable.DoDamage(newDamage);
            shootables.Add(shootable);
        }
    }

    public void IgnoreCollision(Collider2D other, bool ignore)
    {
        //nearbyTileSwitched = false;
        isSolid = !ignore;
        Physics2D.IgnoreCollision(boxCollider, other, ignore);
    }

    public void MakeSolidByAxis(bool vertical, bool horizontal)
    {
        if (!isWalkableSurface && vertical)
            return;

        if (vertical)
        {
            if (isSolid)
                return;
            IgnoreCollision(playerCollider, false);
        }
        if (horizontal)
        {
            if (isHorizontalSolid)
            return;

            MakeSolidHorizontal();
        }
    }

    void MakeSolidHorizontal()
    {
        isHorizontalSolid = true;
        Physics2D.IgnoreCollision(boxCollider, playerCollider, false);
    }

    public bool IsSolid()
    {
        return isSolid || isHorizontalSolid;
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = isSurfaceColor;
        Vector2 pos = new Vector2(topSurfaceReference.position.x, topSurfaceReference.position.y + .5f);
        Gizmos.DrawWireSphere(pos, checkIfSurfaceRadius);

        if (isSolid)
            Gizmos.color = Color.black;
        else
            Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(new Vector2(transform.position.x, transform.position.y+.5f), checkIfSurfaceRadius);
    }
}
