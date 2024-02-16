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
    public bool interact;

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
    [SerializeField] private float bulletForce = 8;
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public Transform aimTarget;
    bool canShoot;
    GameObject newBullet;
    Quaternion lookRotation;
    Vector2 lookDirection;
    float holdingFireTimer = 0;
    [SerializeField] private float chargedShotTime = 2f;
    bool chargedShot;
    [SerializeField] public Transform laserStart;
    [HideInInspector] public Laser laser;

    [Header("Melee Components")]
    [SerializeField] public GameObject meleeCollider;

    [Header("DamageSystem")]
    [HideInInspector] public DamageSystemComponent damageSystem;
    [SerializeField] private Material damagedMaterial;
    [SerializeField] private Material defaultMaterial;


    [Header("Sprites")]
    [SerializeField] GameObject deafultSprite;
    [SerializeField] GameObject chargedShotSprite;
    bool defaultSpriteActive;
    PlayerSprite chargedPlayerSprite;

    [Header("Audio sources")]
    public AudioSource feetAudioSource;
    public AudioSource gunAudioSource;
    public AudioSource landingAudioSource;

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
        chargedPlayerSprite = chargedShotSprite.GetComponent<PlayerSprite>();
    }

    void Start()
    {
        colCrouchOffset = new Vector2(.34f, -0.03f);
        colCrouchSize = new Vector2(.88f, 1.88f);
        colStandSize = bodyCollider.size;
        colStandOffset = bodyCollider.offset;
        meleeCollider.SetActive(false);
    }

    void Update()
    {
        HandleMeleeAttack();
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

        lookDirection = input.aim.ReadValue<Vector2>();

        Debug.Log(lookDirection);
        if (aim)
        {
            //lookDirection = Camera.main.ScreenToWorldPoint(Input.mousePosition) - arm.position;
            //Debug.Log(input.inputAsset.currentControlScheme);

            if (input.inputAsset.currentControlScheme != "Gamepad")
            {
                lookDirection = lookDirection - new Vector2(Screen.width / 2, Screen.height / 2);
                if (!isFacingRight)
                    lookDirection = new Vector2(lookDirection.x * -1, lookDirection.y * -1);
                //lookDirection = new Vector2(Screen.width / 2, Screen.height / 2) - lookDirection;
                Debug.Log("Mouse");
            }
            else
            {
                if (!isFacingRight)
                    lookDirection = new Vector2(lookDirection.x * -1, lookDirection.y * -1);
                Debug.Log("Gamepad");
            }

            Debug.DrawRay(transform.position, lookDirection, Color.red);

            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
            angle = Mathf.Clamp(angle, -90, 90);
            Vector3 forward = arm.transform.forward;
            lookRotation = Quaternion.AngleAxis(angle, forward);
            //arm.rotation = Quaternion.Slerp(arm.rotation, lookRotation, 100 * Time.deltaTime);
            arm.rotation = lookRotation;
            chargedPlayerSprite.arm.rotation = lookRotation;
        }

        if (input.rightClick.WasReleasedThisFrame())
        {
            arm.rotation = Quaternion.identity;
            chargedPlayerSprite.arm.rotation = Quaternion.identity;
        }
    }

    void HandleMeleeAttack()
    {
        if (interact)
        {
            anim.Play("MeleeAttack");
            Debug.Log("MeleeAttack");
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
            //newBullet.transform.parent = bulletSpawn.parent;
            newBullet.transform.position = bulletSpawn.position;
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
        //Debug.Log(chargePercent);
        if(chargePercent == 1)
        {
            BulletPush();
        }

        int chargeLevel = 0;
        if (chargePercent > .5f) chargeLevel = 1;
        if (chargePercent == 1) chargeLevel = 2;
        AudioManager.instance.PlayGunshot(gunAudioSource, chargeLevel);

        holdingFireTimer = 0;
        newBullet.transform.parent = null;
        bullet.LaunchBullet();
        canShoot = true;
    }

    bool ReleaseFire()
    {
        if (!shoot)
        {
            chargedShot = false;
            return true;
        }
        else
        {
            newBullet.transform.localPosition = Vector3.Lerp(newBullet.transform.position, bulletSpawn.position, 200 * Time.deltaTime);

            newBullet.transform.localScale = Vector3.Lerp(new Vector3(.25f, .25f, .25f), new Vector3(.8f,.8f,.8f), holdingFireTimer / chargedShotTime);
            holdingFireTimer += Time.deltaTime;
            if (holdingFireTimer >= chargedShotTime)
            {
                holdingFireTimer = chargedShotTime;
                chargedShot = true;
            }

            return false;
        }
    }

    private void BulletPush()
    {
        float force = bulletForce;
        if (movement.crouching)
            force = 2;
        // Debug.Log("PUSH");
        chargedShotSprite.SetActive(false);
        ToggleDefaultSprite(true);
        Vector3 pushDirection =  transform.position - aimTarget.position;
        //pushDirection.z = 0;
        //pushDirection *= -1;
        movement.bulletPush = true;
        Debug.DrawRay(transform.position, pushDirection, Color.red, 3);
        movement.rb.AddForce(pushDirection * force, ForceMode2D.Impulse);
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
        interact = input.interact.ReadValue<float>() > 0;
    }

    void HandleAnim()
    {
        float walk = movement.rb.velocity.x;
        if (movement.horizontal == 0)
            walk = 0;
        anim.SetFloat("horizontal", walk);

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
        anim.SetBool("aim", aim);

        if (chargedShot && movement.rb.velocity.magnitude <= 0.1f && movement.grounded && !movement.crouching)
        {
            if(defaultSpriteActive)
            ToggleDefaultSprite(false);

            chargedShotSprite.SetActive(true);
        }
        else
        {
            if(!defaultSpriteActive)
            ToggleDefaultSprite(true);

            chargedShotSprite.SetActive(false);
        }
    }

    public void ToggleDefaultSprite(bool enabled)
    {
        defaultSpriteActive = enabled;
        SpriteRenderer[] sprites = deafultSprite.GetComponentsInChildren<SpriteRenderer>();
        foreach(SpriteRenderer sprite in sprites)
        {
            sprite.enabled = enabled;
        }
        if (enabled)
        {
            deafultSprite.GetComponent<PlayerSprite>().SpriteSetup();
        }
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

    public void Heal(int amount)
    {
        damageSystem.Heal(amount);
    }

    public void ToggleAttackCollider(bool enabled)
    {
        meleeCollider.SetActive(enabled);
    }
}
