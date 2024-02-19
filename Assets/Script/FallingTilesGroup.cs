using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingTilesGroup : MonoBehaviour
{
    [SerializeField] private FallingTile[] tiles;
    Rigidbody2D rb;
    bool activated;

    bool doDamage;
    private List<I_Shootable> shootables = new List<I_Shootable>();
    bool checkForDeactivation;

    PlayerStateManager playerState;
    BoxCollider2D boxCollider;
    private AudioSource audioSource;

    [Header("Particles")]
    [SerializeField] private GameObject particlePrefab;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        tiles = GetComponentsInChildren<FallingTile>();
        boxCollider = GetComponent<BoxCollider2D>();
        foreach (FallingTile tile in tiles)
        {
            tile.group = this;
        }
        rb.simulated = false;

        playerState = PlayerStateManager.Instance;
        Physics2D.IgnoreCollision(playerState.bodyCollider, boxCollider, true);
        audioSource = GetComponentInChildren<AudioSource>();
    }

    private void FixedUpdate()
    {
        if (!activated)
            return;

        //Debug.Log("TILE UPDATE");
        if (rb.velocity.magnitude > 0.2f)
            doDamage = true;
        else
            doDamage = false;

        if (checkForDeactivation)
        {
            if(rb.velocity.magnitude == 0)
            {
                Deactivate();
            }
        }
    }

    void ActivateGroup()
    {
        activated = true;

        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
        
        foreach (FallingTile tile in tiles)
        {
            tile.group = null;
            tile.rb.simulated = false;
        }

        rb.simulated = true;
        StartCoroutine(WaitBeforeCheckForDeactivation());
    }

    public void TileActivated()
    {
        bool shouldActivateGroup = true;
        foreach (FallingTile tile in tiles)
        {
            if (tile.activated == false)
                shouldActivateGroup = false;
        }

        if (shouldActivateGroup == true)
            ActivateGroup();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!activated)
            return;

        if (!doDamage)
            return;

        if (collision)
        {

            AudioManager.instance.FallingGroupLanded(audioSource);
            
            Instantiate(particlePrefab, collision.ClosestPoint(collision.gameObject.transform.position), Quaternion.Euler(0, 0, 0));

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

    void Deactivate()
    {
        Debug.Log("DEACTIVATE");
        foreach(FallingTile tile in tiles)
        {
            tile.rb.simulated = true;
            tile.rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        BoxCollider2D[] boxes = GetComponents<BoxCollider2D>();
        foreach(BoxCollider2D box in boxes)
        {
            box.enabled = false;
        }
    }

    IEnumerator WaitBeforeCheckForDeactivation()
    {
        yield return new WaitForSeconds(1);
        checkForDeactivation = true;
    }
}
