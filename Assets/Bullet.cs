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

    private void Awake()
    {
        trail = GetComponentInChildren<TrailRenderer>();
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        
        
        trail.startWidth = gameObject.transform.localScale.x;
    }

    public void LaunchBullet()
    {
        launched = true;
        direction = aimTarget.position - transform.position;

        //rb.gravityScale = 1;
        rb.AddForce(direction * force);
        InvokeRepeating("GetMagnitude", 0.5f, 0.2f);

        Destroy(gameObject, 4);
    }

    void GetMagnitude()
    {
        //Debug.Log("BULLET VELOCITY: " + rb.velocity.magnitude);
        if (rb.velocity.magnitude < 20)
        {
            Destroy(gameObject, 0.01f);
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

            trail.endWidth = gameObject.transform.localScale.x;
            trail.time = 1;
        }
        damageSettings.damage = (int) damage;
        Debug.Log("Damage: " +  damage);    
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!launched)
            return;
        if (collision.gameObject)
        {
            if (collision.gameObject.GetComponent<I_Shootable>() != null)
            {
                
                collision.gameObject.GetComponent<I_Shootable>().DoDamage(damageSettings);
                Destroy(gameObject, 0.01f);
            }
        }
    }
}
