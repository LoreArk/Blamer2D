using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateManager : MonoBehaviour, I_Shootable
{

    public static PlayerStateManager Instance;

    InputManager input;
    [HideInInspector] public PlayerMovement movement;
    public bool aim;
    public bool shoot;

    public Transform arm;
    public Transform armSprite;
    public float aimSpeed = 10f;

    private bool isFacingRight = true;

    public Animator anim;

    [HideInInspector] public BoxCollider2D bodyCollider;
    Vector2 colCrouchOffset;
    Vector2 colCrouchSize;
    Vector2 colStandOffset;
    Vector2 colStandSize;

    [Header("Shooting Components")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public Transform aimTarget;
    bool canShoot;
    GameObject newBullet;
    Quaternion lookRotation;
    Vector3 lookDirection;
    float holdingFireTimer = 0;
    [SerializeField] private float chargedShotTime = 2f;

    [Header("DamageSystem")]
    private DamageSystemComponent damageSystem;
    [SerializeField] private Material damagedMaterial;
    [SerializeField] private Material defaultMaterial;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        input = GetComponent<InputManager>();
        movement = GetComponent<PlayerMovement>();
        canShoot = true;
        bodyCollider = GetComponent<BoxCollider2D>();
        damageSystem = GetComponent<DamageSystemComponent>();
    }

    void Start()
    {
        colCrouchOffset = new Vector2(.34f, -0.03f);
        colCrouchSize = new Vector2(.88f, 1.88f);
        colStandSize = bodyCollider.size;
        colStandOffset = bodyCollider.offset;
    }

    void Update()
    {


        HandleShoot();

        FlipSprites();

        GetInputs();

        HandleAnim();
    }

    public void AdjustCollider(bool crouched)
    {
        if (crouched)
        {
            bodyCollider.offset = colCrouchOffset;
            bodyCollider.size = colCrouchSize;
        }
        else
        {
            bodyCollider.offset = colStandOffset;
            bodyCollider.size = colStandSize;
        }
    }

    private void LateUpdate()
    {

        ArmRotation();
        //AimIKHandler();
    }

    private void FixedUpdate()
    {

    }

    void ArmRotation()
    {

        if (aim)
        {
            lookDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - arm.position;
            if (!isFacingRight)
                lookDirection = arm.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
            Vector3 forward = arm.transform.forward;
            lookRotation = Quaternion.AngleAxis(angle, forward);
            arm.rotation = Quaternion.Slerp(arm.rotation, lookRotation, 10 * Time.deltaTime);
        }

        if (input.rightClick.WasReleasedThisFrame())
        {
            arm.rotation = Quaternion.identity;
        }
    }


    void HandleShoot()
    {

        if (!canShoot)
            return;

        if (shoot)
        {
            canShoot = false;

            newBullet = Instantiate(bulletPrefab);
            newBullet.transform.parent = bulletSpawn.parent;
            newBullet.transform.localPosition = bulletSpawn.localPosition;
            Bullet b = newBullet.GetComponent<Bullet>();
            b.aimTarget = aimTarget;
            StartCoroutine(WaitForReleaseFire(b));
        }
    }

    IEnumerator WaitForReleaseFire(Bullet bullet)
    {
        /*while (shoot)
        {
            Debug.Log("holding fire");
        }*/
        yield return new WaitUntil(ReleaseFire);

        float chargePercent = holdingFireTimer / chargedShotTime ;
        chargePercent *= 1;
        //Debug.Log("Charge percentage " + chargePercent);
        newBullet.GetComponent<Bullet>().CalculateDamage(chargePercent);
        Debug.Log(chargePercent);
        if(chargePercent == 1)
        {
            BulletPush();
        }

        holdingFireTimer = 0;
        newBullet.transform.parent = null;
        bullet.LaunchBullet();
        canShoot = true;
    }

    bool ReleaseFire()
    {
        if (!shoot)
        {

            return true;
        }
        else
        {
            newBullet.transform.localPosition = Vector3.Lerp(newBullet.transform.localPosition, bulletSpawn.localPosition, 200 * Time.deltaTime);

            newBullet.transform.localScale = Vector3.Lerp(new Vector3(.25f, .25f, .25f), new Vector3(.8f,.8f,.8f), holdingFireTimer / chargedShotTime);
            holdingFireTimer += Time.deltaTime;
            if (holdingFireTimer >= chargedShotTime)
                holdingFireTimer = chargedShotTime;

            return false;
        }
    }

    private void BulletPush()
    {
        Debug.Log("PUSH");
        Vector3 pushDirection =  transform.position - aimTarget.position;
        //pushDirection.z = 0;
        //pushDirection *= -1;
        movement.bulletPush = true;
        Debug.DrawRay(transform.position, pushDirection, Color.red, 3);
        movement.rb.AddForce(pushDirection * 8, ForceMode2D.Impulse);
    }

    private void FlipSprites()
    {
        if (isFacingRight && movement.horizontal < 0f || !isFacingRight && movement.horizontal > 0f)
        {
            FlipBodySprite();
        }
    }

    void FlipBodySprite()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;
    }

    void GetInputs()
    {
        aim = input.rightClick.ReadValue<float>() > 0;
        shoot = input.leftClick.ReadValue<float>() > 0;
    }

    void HandleAnim()
    {
        anim.SetFloat("horizontal", movement.rb.velocity.x);

        if (movement.crouchPress && movement.crouching)
        {
            anim.Play("crouch");
        }
        if (movement.performJump && movement.grounded)
        {
            anim.Play("jump");
        }
        anim.SetBool("holdCrouch", movement.crouching);
        anim.SetBool("jump", movement.performJump);
        anim.SetBool("falling", movement.rb.velocity.y < 0);
        anim.SetBool("grounded", movement.grounded);

    }

    public void DoDamage(DamageSettings dmg)
    {
        damageSystem.DoDamage(dmg);
        StartCoroutine(damageSystem.Invincibility());

        StartCoroutine(SetDamageMaterial());
    }

    private IEnumerator SetDamageMaterial()
    {
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.material = damagedMaterial;
            sprite.material.color = Color.red;
        }
        yield return new WaitForSeconds(.3f);

        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.material = defaultMaterial;
            sprite.material.color = Color.white;
        }
    }

    public void Death()
    {
        Debug.Log("GameOver");
    }



}
