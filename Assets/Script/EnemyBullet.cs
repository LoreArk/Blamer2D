using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBullet : MonoBehaviour, I_Shootable
{
    Rigidbody2D rb;
    [SerializeField] public DamageSettings damageSettings;
    [SerializeField] private GameObject bloodParticle;
    NavMeshAgent agent;
    PlayerStateManager player;
    [HideInInspector] public EnemyState enemy;
    public GameObject sprite;
    bool isFacingRight = true;
    bool isFacingDown = true;
    [SerializeField] private LayerMask damageLayer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        agent.updateRotation = false;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        player = PlayerStateManager.Instance;
        
        InvokeRepeating("FollowPlayer", .2f, 3);

        damageSettings = new DamageSettings();
        damageSettings.damage = 1;

        Physics2D.IgnoreCollision(player.bodyCollider, GetComponent<CircleCollider2D>());
    }


    private void Update()
    {
        Vector2 moveDirection = rb.velocity;
        if (moveDirection != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveDirection.y, moveDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }

        DamageTrigger();

        HandleFlip();
    }

    void HandleFlip()
    {
        //Debug.Log(agent.velocity.x);

        if (agent.velocity.y > 2 && isFacingDown || agent.velocity.y < -2 && !isFacingDown)
        {
            Debug.Log("LOOK UP");

            isFacingDown = !isFacingDown;
            Vector3 localScale = transform.localScale;
            localScale.y *= -1f;
            transform.localScale = localScale;
        }

        if (agent.velocity.x > 2 && !isFacingRight || agent.velocity.x < -2 && isFacingRight)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }

    }

    void DamageTrigger()
    {
        Collider2D[] overlapping = Physics2D.OverlapCircleAll(transform.position, 0.5f, damageLayer);

        foreach(Collider2D col in overlapping)
        {
            I_Shootable shootable = col.gameObject.GetComponent<I_Shootable>();
            if (shootable == null)
                continue;

            shootable.DoDamage(damageSettings);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log(collision.gameObject.layer);
        
        
        //Destroy(gameObject);
    }

    public void DoDamage(DamageSettings dmg)
    {
        enemy.createdBullets--;
        Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), player.bodyCollider);
        GameObject instance = Instantiate(bloodParticle, transform.position, Quaternion.Euler(0, 0, 0));
        instance.transform.localScale = new Vector3(.6f, .6f, .6f);

        Destroy(gameObject, 0.1f);
    }

    void FollowPlayer()
    {
        Vector2 destination = new Vector2(player.transform.position.x, player.transform.position.y+1);
        agent.SetDestination(player.transform.position);
    }
}
