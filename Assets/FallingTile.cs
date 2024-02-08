using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FallingTile : MonoBehaviour, I_Shootable, I_JumpableNoCollision
{
    PlayerStateManager playerState;
    Rigidbody2D rb;
    bool activated;
    public List<I_Shootable> shootables = new List<I_Shootable>();
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

    void Start()
    {
        playerState = PlayerStateManager.Instance;
        boxCollider = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        playerCollider = playerState.bodyCollider;
        
        IgnoreCollision(playerCollider, true);

        isSurfaceColor = Color.yellow;
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

        grounded = IsGrounded();
        if (grounded)
        {
            //activated = false;
        }
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
            hHit.collider.GetComponent<I_JumpableNoCollision>().IgnoreCollision(playerCollider, false);
        }
        if(vHit.collider.GetComponent<I_JumpableNoCollision>() != null)
        {
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
            isHorizontalSolid = false;
            IgnoreCollision(playerCollider, true);
        }
    }

    public void DoDamage(DamageSettings newDamage)
    {
        activated = true;

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
        if (!activated)
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
        if (!isWalkableSurface)
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

    private void OnDrawGizmos()
    {
        Gizmos.color = isSurfaceColor;
        Vector2 pos = new Vector2(topSurfaceReference.position.x, topSurfaceReference.position.y + .5f);
        Gizmos.DrawWireSphere(pos, checkIfSurfaceRadius);
    }
}
