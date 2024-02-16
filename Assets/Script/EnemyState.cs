using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemyState : MonoBehaviour, I_Shootable
{
    PlayerStateManager playerState;
    public AI aiState;
    public Transform eyes;
    public LayerMask lookLayer;
    public GameObject bulletPrefab;


    [Header("Stats")]
    public int hp = 50;
    bool dead;

    [Header("Patrol")]
    public float waitOnPoint;
    public float movementPrecision;
    public int patrolIndex;
    public List<Transform> patrolPoints = new List<Transform>();
    public int availableAttackSpotIndex;
    public List<Transform> attackPoints = new List<Transform>();
    public List<Vector2> availableAttackPoints = new List<Vector2>();
    public float speed;
    public float unawareSpeed;
    public float engageSpeed;
    public bool waiting;
    bool isFacingRight = true;
    public Vector3 targetAttackSpot;

    [Header("Senses")]
    public float sightRange;
    public float sightAngle;
    bool lookingRight;

    public float angleFromPlayer;
    public float distanceFromPlayer;

    Rigidbody2D rb;
    
    public Light2D eyeLight;
    bool shouldFlipToPlayer;

    [Header("Shooting")]
    public float minRandomFire =3;
    public float maxRandomFire =6;
    public float bulletSpeed;
    bool triggerAttacking;
    bool attackStateTrigger;
    public GameObject explosionPrefab;
    public int maxBulletSpawn = 5;
    public int createdBullets;

    [Header("Audio")]
    [SerializeField] private AudioSource eyeAudioSource;
    [SerializeField] private AudioSource bodyAudioSource;
    [SerializeField] private AudioSource damageAudioSource;

    void Start()
    {
        playerState = FindObjectOfType<PlayerStateManager>();
        patrolIndex = 0;
        rb = GetComponent<Rigidbody2D>();
        eyeLight = eyes.GetComponentInChildren<Light2D>();
    }

    void Update()
    {
        if (hp <= 0)
        {
            Death();
        }
        if (dead)
            return;

        switch (aiState)
        {
            case AI.unaware:
                speed = unawareSpeed;
                waitOnPoint = 1;
                if(patrolPoints.Count > 0)
                Patrol();
                if (PlayerInSight() || attackStateTrigger)
                {
                    attackStateTrigger = false;
                    eyeLight.color = Color.red;
                    aiState = AI.attack;
                    AudioManager.instance.EnemyAlarm(eyeAudioSource);
                }
                break;
            case AI.chase:

                break;
            case AI.attack:
                speed = engageSpeed;
                waitOnPoint = 3;
                EyeLookAtPlayer();
                Attack();
                break;
            default:
                break;
        }

        
    }

    void EyeLookAtPlayer()
    {
        Vector2 lookDirection = playerState.transform.position - eyes.position;
        if (!isFacingRight)
            lookDirection = eyes.position - playerState.transform.position;
        float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
        Vector3 forward = eyes.transform.forward;
        Quaternion lookRotation = Quaternion.AngleAxis(angle, forward);
        eyes.rotation = Quaternion.Slerp(eyes.rotation, lookRotation, 10 * Time.deltaTime);

        Debug.DrawRay(transform.position, transform.right, Color.red);
        float angleToPlayer = Vector2.Angle(transform.right, lookDirection);
        if (angleToPlayer > 90)
        {
            shouldFlipToPlayer = true;
        }
        else
            shouldFlipToPlayer = false;

    }
    void Attack()
    {
        AttackingMovement();
        if (!triggerAttacking)
        {
            triggerAttacking = true;
            InvokeRepeating("ShootAttack", .25f, Random.Range(minRandomFire, maxRandomFire));
        }
    }

    void AttackingMovement()
    {
        availableAttackPoints = GetAttackingSpots();
        
        PatrolAttackingPoints();
    }
    void PatrolAttackingPoints()
    {
        if (waiting)
            return;

        //Debug.Log("GO TO ATTACK SPOT: " + availableAttackPoints.Count + " " + availableAttackSpotIndex);

        if (availableAttackPoints.Count <= 0 || availableAttackSpotIndex >= availableAttackPoints.Count - 1)
        {
            availableAttackSpotIndex = 0;
        }

        int i = availableAttackSpotIndex;
        Vector2 targetPos = availableAttackPoints[i];
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        Vector2 direction = targetPos - pos;
        //transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        rb.AddForce(direction * speed);

        float distance = Vector2.Distance(transform.position, availableAttackPoints[i]);

        if (distance < movementPrecision)
        {
            StartCoroutine(WaitAtAttackPoint());
        }

        Flip();

        //transform.position = Vector2.MoveTowards(transform.position, targetAttackSpot, speed * Time.deltaTime);
    }
    IEnumerator WaitAtAttackPoint()
    {
        waiting = true;
        yield return new WaitForSeconds(waitOnPoint);
       // Debug.Log("CHANGE ATTACK SPOT: " + availableAttackPoints.Count + " " + availableAttackSpotIndex);
        
        availableAttackSpotIndex++;

        waiting = false;
    }

    List<Vector2> GetAttackingSpots()
    {
        List<Vector2> availableSpots = new List<Vector2>();

        for (int i = 0; i < attackPoints.Count; i++)
        {
            bool isAvailable = AvailableSpotForAttacking(attackPoints[i].position);
            if (isAvailable)
            {
                if (!availableSpots.Contains(attackPoints[i].position))
                {
                    availableSpots.Add(attackPoints[i].position);
                }
            }
        }
        if(availableSpots.Count == 0)
        {
            //Debug.Log("NO AVAILABLE SPOTS");
            foreach(Transform t in attackPoints)
            {
                availableSpots.Add(t.position);
            }
        }

        return availableSpots;
    }
    bool AvailableSpotForAttacking(Vector2 pos)
    {
        Vector2 target = new Vector2(playerState.transform.position.x, playerState.transform.position.y);
        Vector2 direction = target - pos;
        Debug.DrawRay(pos, direction, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(pos, direction, 25, ~lookLayer);
        //Debug.Log("raycasted: " + hit.collider.gameObject.name + " --- " + hit.collider.GetComponent<PlayerStateManager>());

        if (!hit)
            return false;
        if (hit.collider.GetComponent<PlayerStateManager>())
        {
            return true;
        }
        return false;
    }

    void ShootAttack()
    {
        // if (!RaycastPlayer())
        //  return;

        //GameObject inst = 

        if (createdBullets >= maxBulletSpawn)
            return;

        Vector3 spawnPosition = eyes.transform.position;
        GameObject newBullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity, null);
        newBullet.GetComponent<EnemyBullet>().enemy = this;
        createdBullets++;

        //inst.transform.position = eyes.transform.position;
        //Rigidbody2D rbb = inst.GetComponent<Rigidbody2D>();
        //Vector3 direction = playerState.transform.position - inst.transform.position;

        //rbb.AddForce(direction * bulletSpeed);
    }
    
    bool PlayerInSight()
    {
        distanceFromPlayer = Vector3.Distance(eyes.position, playerState.transform.position);

        Vector2 playerDirection = playerState.transform.position - eyes.position;
        Vector2 lookDirection = eyes.transform.right;
        if (!isFacingRight)
            lookDirection = -eyes.transform.right;
        Debug.DrawRay(eyes.transform.position, lookDirection * 5, Color.green);
        Debug.DrawRay(eyes.transform.position, playerDirection, Color.cyan);

        angleFromPlayer = Vector2.Angle(lookDirection.normalized, playerDirection.normalized);

        bool inSight = false;
        if (distanceFromPlayer < sightRange && angleFromPlayer < sightAngle)
        {
            inSight = RaycastPlayer();
        }

        return inSight;
    }

    bool RaycastPlayer()
    {
        RaycastHit2D hit;
        Vector2 origin = transform.position;
        Vector2 playerDirection = playerState.transform.position - transform.position;

        Debug.DrawRay(origin, playerDirection, Color.red);

        hit = Physics2D.Raycast(origin, playerDirection, sightRange, ~lookLayer);

        if (!hit)
        {
            // Debug.Log(hit.collider.gameObject.name);
            return false;
        }

        return hit.collider.GetComponent<PlayerStateManager>();
    }

    void Patrol()
    {
        if (waiting)
            return;

        int i = patrolIndex;
        Vector2 targetPos = patrolPoints[i].position;
        Vector2 pos = new Vector2(transform.position.x, transform.position.y);
        Vector2 direction = targetPos - pos;
        //transform.position = Vector2.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        rb.AddForce(direction * speed);

        float distance = Vector2.Distance(transform.position, patrolPoints[i].position);

        if(distance < movementPrecision)
        {
            StartCoroutine(WaitAtPatrolPoint());
        }

        Flip();
    }

    IEnumerator WaitAtPatrolPoint()
    {
        waiting = true;
        yield return new WaitForSeconds(waitOnPoint);
        //Debug.Log("CHANGE PATROL POINT");
        if (patrolIndex < patrolPoints.Count - 1)
            patrolIndex++;
        else
            patrolIndex = 0;

        waiting = false;
    }
    

    public void DoDamage(DamageSettings newDamage)
    {
        Damage(newDamage);
    }

    void Damage(DamageSettings newDamage)
    {
        hp -= newDamage.damage;
        attackStateTrigger = true;
        AudioManager.instance.EnemyDamage(damageAudioSource);
    }

    void Flip()
    {
        if(isFacingRight && rb.velocity.x < 0f || !isFacingRight && rb.velocity.x > 0f || shouldFlipToPlayer && rb.velocity.magnitude <= 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
        
    }

    void Death()
    {
        eyeAudioSource.loop = false;
        AudioManager.instance.EnemyDeath(eyeAudioSource);
        CancelInvoke();
        dead = true;
        rb.gravityScale = 12;
        rb.constraints = RigidbodyConstraints2D.None;
        eyeLight.gameObject.SetActive(false);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!dead)
            return;

        if (collision.collider.gameObject.layer == 3)
        {
            GameObject inst = Instantiate(explosionPrefab);
            inst.transform.position = transform.position;
            AudioManager.instance.EnemyImpact(eyeAudioSource);
            Destroy(gameObject, 0.6f);
        }
    }

    Vector2 GetClosestPointOfList(List<Vector2> pointsList)
    {
        Vector2 closestPoint = pointsList[0];
        float shorterDistance = 100;
        for (int i = 0; i < pointsList.Count; i++)
        {
            float distance = Vector2.Distance(transform.position, pointsList[i]);
            if (shorterDistance > distance)
            {
                shorterDistance = distance;
                closestPoint = pointsList[i];
            }
        }

        return closestPoint;
    }

    
    

    public enum AI
    {
        unaware, chase, attack
    }

}
