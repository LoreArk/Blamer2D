using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBullet : MonoBehaviour, I_Shootable
{
    Rigidbody2D rb;
    [SerializeField] public DamageSettings damageSettings;
    NavMeshAgent agent;
    PlayerStateManager player;
    [HideInInspector] public EnemyState enemy;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateUpAxis = false;
        agent.updateRotation = false;
        player = PlayerStateManager.Instance;
        InvokeRepeating("FollowPlayer", .2f, 3);
    }


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
            //Destroy(gameObject);
        }

        if (shootable == null)
            return;


        shootable.DoDamage(damageSettings);

        //Destroy(gameObject);
    }

    public void DoDamage(DamageSettings dmg)
    {
        enemy.createdBullets--;
        Destroy(gameObject);
    }

    void FollowPlayer()
    {
        Vector2 destination = new Vector2(player.transform.position.x, player.transform.position.y+1);
        agent.SetDestination(player.transform.position);
    }
}
