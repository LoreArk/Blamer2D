using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] public DamageSettings damageSettings;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    //public LayerMask destructionLayer;
    private void Update()
    {
        Vector2 moveDirection = rb.velocity;
        if (moveDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.gameObject.layer);
        I_Shootable shootable = collision.gameObject.GetComponent<I_Shootable>();

        damageSettings = new DamageSettings();
        damageSettings.damage = 1;
        
        if(collision.gameObject.layer == 3 || collision.gameObject.layer == 6)
        {
            Destroy(gameObject);
        }

        if (shootable == null)
            return;


        shootable.DoDamage(damageSettings);

        Destroy(gameObject);
    }
}
