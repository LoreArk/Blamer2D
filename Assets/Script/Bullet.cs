using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float force;
    Rigidbody2D rb;
    Vector3 direction;
    public Transform aimTarget;
    bool launched;
    TrailRenderer trail;
    DamageSettings damageSettings;
    [SerializeField] private LayerMask shootableLayers;
    bool isCharged;
    List<I_Shootable> shotObjects = new List<I_Shootable>();
    AudioSource audioSource;

    private void Awake()
    {
        trail = GetComponentInChildren<TrailRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        
        
        trail.startWidth = gameObject.transform.localScale.x;

        if (!launched)
            return;

        RaycastShootable();
    }

    public void LaunchBullet()
    {

        launched = true;
        direction = aimTarget.position - transform.position;
        GetComponent<SpriteRenderer>().renderingLayerMask = 0;
        //rb.gravityScale = 1;
        rb.AddForce(direction * force);
        //InvokeRepeating("GetMagnitude", 0.5f, 0.2f);

        Destroy(gameObject, 10);
    }

    void GetMagnitude()
    {
        //Debug.Log("BULLET VELOCITY: " + rb.velocity.magnitude);
        if (rb.velocity.magnitude < 50)
        {
            launched = false;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            
            Destroy(gameObject, 1f);
        }
    }

    public void CalculateDamage(float chargeLevel)
    {
        damageSettings = new DamageSettings();
        float damage = 10;
        if (chargeLevel > 0.5)
        {
            damage = 20;
        }
        if(chargeLevel == 1)
        {
            damage = 30;
            isCharged = true;
            trail.endWidth = gameObject.transform.localScale.x;
            trail.time = 1;
        }
        damageSettings.damage = (int) damage;
        //Debug.Log("Damage: " +  damage);    
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!launched)
            return;

        if(collision != null)
        {
            I_Shootable shootable = collision.transform.GetComponent<I_Shootable>();
            if (shootable != null && !shotObjects.Contains(shootable))
            {
                collision.transform.GetComponent<I_Shootable>().DoDamage(damageSettings);
                shotObjects.Add(shootable);
            }
        }
            
        GetMagnitude();
    }

    void RaycastShootable()
    {
        if (isCharged)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x * 1f, shootableLayers);

            foreach (Collider2D col in colliders)
            {
                I_Shootable shootable = col.GetComponent<I_Shootable>();
                if (shootable != null && !shotObjects.Contains(shootable))
                {
                    Debug.Log(col.gameObject.name);
                    shootable.DoDamage(damageSettings);
                    shotObjects.Add(shootable);
                }
            }
        }
        else
        {
            Collider2D collider = Physics2D.OverlapCircle(transform.position, transform.localScale.x * .9f, shootableLayers);
            if (collider)
            {
                I_Shootable shootable = collider.GetComponent<I_Shootable>();

                if (shootable != null && !shotObjects.Contains(shootable))
                {
                    Debug.Log(collider.gameObject.name);
                    shootable.DoDamage(damageSettings);
                    shotObjects.Add(shootable);

                    launched = false;
                    Destroy(gameObject, .01f);
                }
            }
                
        }

    }

    private void OnDrawGizmos()
    {

        Gizmos.DrawSphere(transform.position, transform.localScale.x * 0.9f);
    }

    
}
