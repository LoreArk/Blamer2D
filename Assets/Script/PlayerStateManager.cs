using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerStateManager : MonoBehaviour, I_Shootable
{

    public static PlayerStateManager Instance;

    InputManager input;
    [HideInInspector] public PlayerMovement movement;
    public bool aimInput;
    bool aiming;
    public bool shoot;
    public bool interact;

    public Transform arm;
    public Transform armSprite;
    public float aimSpeed = 10f;

    [HideInInspector] public bool isFacingRight = true;

    public Animator anim;

    [HideInInspector] public BoxCollider2D bodyCollider;
    Vector2 colCrouchOffset;
    Vector2 colCrouchSize;
    Vector2 colStandOffset;
    Vector2 colStandSize;

    [Header("Shooting Components")]
    [SerializeField] public float gunLoad = 100;
    [SerializeField] public float maxGunLoad = 100;
    [SerializeField] private float gunConsumption = 20;
    [SerializeField] private float chargedConsumption = 30;
    [SerializeField] public float bulletForce = 8;
    public GameObject bulletPrefab;
    public Transform aimBulletSpawn;
    public Transform idleBulletSpawn;
    public Transform aimTarget;
    bool canShoot;
    GameObject newBullet;
    Quaternion lookRotation;
    Vector2 lookDirection;
    float holdingFireTimer = 0;
    [SerializeField] private float ratio = .3f;
    [SerializeField] private float chargedShotTime = 2f;
    bool chargedShot;
    [SerializeField] public Transform laserStart;
    [HideInInspector] public Laser laser;
    [HideInInspector] public Vector2 pushDirection;
    [HideInInspector] public float targetBulletForce;

    [Header("Melee Components")]
    [SerializeField] public GameObject meleeCollider;
    public bool meleeAttack;
    bool meleeFlag;

    [Header("DamageSystem")]
    [SerializeField] public Material damagedMaterial;
    [SerializeField] private Material defaultMaterial;

    [HideInInspector] public DamageSystemComponent damageSystem;

    [Header("Sprites")]
    [SerializeField] GameObject idleArmSprite;
    [SerializeField] GameObject aimArmSprite1;
    [SerializeField] GameObject aimArmSprite2;
    [SerializeField] GameObject deafultSprite;
    [SerializeField] GameObject chargedShotSprite;
    [SerializeField] GameObject jumpSprite;
    [SerializeField] GameObject landSprite;
    bool defaultSpriteActive;
    PlayerSprite chargedPlayerSprite;

    [Header("Audio sources")]
    public AudioSource feetAudioSource;
    public AudioSource gunAudioSource;
    public AudioSource gunAudioSource2;
    public AudioSource landingAudioSource;
    public AudioSource damageAudioSource;

    [Header("Cameras")]
    public CinemachineVirtualCamera playerCamera;
    public CinemachineVirtualCamera aimCamera;

    Vector2 previousAimDelta;

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
    }

    private void FixedUpdate()
    {

    }

    void SwitchToAimCamera()
    {
        aimCamera.Priority = 1;
        playerCamera.Priority = 0;
    }
    void SwitchToPlayerCamera()
    {
        aimCamera.Priority = 0;
        playerCamera.Priority = 1;
    }

    void ArmRotation()
    {
        lookDirection = input.aim.ReadValue<Vector2>();

        AimSprite(aiming);
        
        if (aimInput)
        {
            aiming = true;
            if (aimCamera.Priority == 0)
                SwitchToAimCamera();

            if (lookDirection.x == 0 || lookDirection.y == 0)
                return;

            if (input.inputAsset.currentControlScheme != "Gamepad")
            {
                lookDirection = lookDirection - new Vector2(Screen.width / 2, Screen.height / 2);
                if (!isFacingRight)
                    lookDirection = new Vector2(lookDirection.x * -1, lookDirection.y * -1);
            }
            else
            {
                if (!isFacingRight)
                    lookDirection = new Vector2(lookDirection.x * -1, lookDirection.y * -1);
            }

            Debug.DrawRay(transform.position, lookDirection, Color.red);

            float angle = Mathf.Atan2(lookDirection.y, lookDirection.x) * Mathf.Rad2Deg;
            angle = Mathf.Clamp(angle, -90, 90);
            Vector3 forward = arm.transform.forward;
            lookRotation = Quaternion.AngleAxis(angle, forward);
            arm.rotation = lookRotation;
            chargedPlayerSprite.arm.rotation = lookRotation;
        }

        if(aiming)
        if (input.rightClick.WasReleasedThisFrame()  || movement.running || meleeAttack)
        {
            aiming = false;
            SwitchToPlayerCamera();
            arm.rotation = Quaternion.identity;
            chargedPlayerSprite.arm.rotation = Quaternion.identity;
        }
    }

    void AimSprite(bool aim)
    {
        if (damageSystem.isDead)
        {
            aimArmSprite1.SetActive(false);
            aimArmSprite2.SetActive(false);
            idleArmSprite.SetActive(true);
            return;
        }

        if(aim == true)
        {
            aimArmSprite1.SetActive(true);
            aimArmSprite2.SetActive(true);
            idleArmSprite.SetActive(false);
        }
        else
        {
            aimArmSprite1.SetActive(false);
            aimArmSprite2.SetActive(false);
            idleArmSprite.SetActive(true);
        }
        if (chargedShotSprite.activeInHierarchy)
        {
            aimArmSprite1.SetActive(false);
            aimArmSprite2.SetActive(false);
            idleArmSprite.SetActive(false);
        }
    }

    void HandleMeleeAttack()
    {
        if (meleeFlag == true)
            return;
        if (interact)
        {
            anim.Play("MeleeAttack");
            StartCoroutine(MeleeAttackFlag());
        }
    }

    IEnumerator MeleeAttackFlag()
    {
        meleeFlag = true;
        yield return new WaitForSeconds(.8f);
        meleeFlag = false;
    }

    void HandleShoot()
    {
        if (!canShoot)
            return;

        if(gunLoad >= gunConsumption)

        if (shoot)
        {
            canShoot = false;

            GameManager.instance.usedEnergy += gunConsumption;
            gunLoad -= gunConsumption;
            UIManger.instance.UpdateGunLoad(gunLoad);

            newBullet = Instantiate(bulletPrefab);
            if(aiming)
            newBullet.transform.position = aimBulletSpawn.position;
            else
                newBullet.transform.position = idleBulletSpawn.position;

            Bullet b = newBullet.GetComponent<Bullet>();
            b.aimTarget = aimTarget;

            StartCoroutine(WaitForReleaseFire(b));
        }
    }

    IEnumerator WaitForReleaseFire(Bullet bullet)
    {
        yield return new WaitUntil(ReleaseFire);

        if (damageSystem.isDead)
        {
            chargedShot = false;
            newBullet.GetComponent<Bullet>().SpawnHitParticleAndDestroy();

        }
        else
        {
            float chargePercent = holdingFireTimer / chargedShotTime;
            chargePercent *= 1;

            newBullet.GetComponent<Bullet>().CalculateDamage(chargePercent);

            if (chargePercent == 1)
            {

                UIManger.instance.UpdateGunLoad(gunLoad);
                BulletPush();
            }

            int chargeLevel = 0;
            if (chargePercent > .5f) chargeLevel = 1;
            if (chargePercent == 1) chargeLevel = 2;
            AudioManager.instance.PlayGunshot(gunAudioSource, chargeLevel);

            holdingFireTimer = 0;
            newBullet.transform.parent = null;
            bullet.LaunchBullet();
            StartCoroutine(RatioTimer());
        }
        
    }

    IEnumerator RatioTimer()
    {
        yield return new WaitForSeconds(ratio);

        canShoot = true;
    }

    bool ReleaseFire()
    {
        if (!shoot)
        {
            gunAudioSource2.Stop();
            gunAudioSource2.volume = 0;
            chargedShot = false;
            return true;
        }
        else
        {
            if (damageSystem.isDead)
                return true;


            gunAudioSource2.volume += Time.deltaTime * .5f;
            if (!gunAudioSource2.isPlaying)
            {
                gunAudioSource2.Play();
            }

            Vector2 spawnPoint = idleBulletSpawn.position;
            if (aiming)
                spawnPoint = aimBulletSpawn.position;
            if (chargedShot && chargedPlayerSprite.isActiveAndEnabled)
                spawnPoint = aimBulletSpawn.position;
            

            newBullet.transform.localPosition = Vector3.Lerp(newBullet.transform.position, spawnPoint, 200 * Time.deltaTime);

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
        targetBulletForce = bulletForce;
        if (movement.crouching)
            targetBulletForce = 0.5f;

        chargedShotSprite.SetActive(false);
        ToggleDefaultSprite(true);
        pushDirection =  transform.position - aimTarget.position;

        movement.bulletPush = true;
        Debug.DrawRay(transform.position, pushDirection, Color.red, 3);
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
        aimInput = input.rightClick.ReadValue<float>() > 0 && !movement.running;
        shoot = input.leftClick.ReadValue<float>() > 0;
        interact = input.interact.ReadValue<float>() > 0;
    }

    void HandleAnim()
    {
        if (damageSystem.isDead)
        {
            ToggleDefaultSprite(true);
            chargedShotSprite.SetActive(false);
            return;
        }

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
        anim.SetBool("aim", aimInput);
        anim.SetBool("run", movement.running);

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
        PlayerSprite sprites = deafultSprite.GetComponentInChildren<PlayerSprite>();
        foreach(SpriteRenderer sprite in sprites.sprites)
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
        if (damageSystem.isInvincible || damageSystem.isDead)
            return;

        damageSystem.DoDamage(dmg);
        StartCoroutine(damageSystem.Invincibility());

        StartCoroutine(SetDamageMaterial());

        if(damageSystem.isDead)
            AudioManager.instance.PlayerDeathDamage(feetAudioSource);
        
        AudioManager.instance.PlayerDamaged(damageAudioSource);

        GameManager.instance.damageReceived ++;
    }

    private IEnumerator SetDamageMaterial()
    {
        Debug.Log("DAMAGE MATERIAL");
        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        if (!damageSystem.isDead)
        {
            foreach (SpriteRenderer sprite in sprites)
            {
                sprite.material = damagedMaterial;
                //sprite.material.color = Color.red;
            }
        }

        yield return new WaitForSeconds(.3f);

        Debug.Log("DEFAULT MATERIAL");
        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.material = defaultMaterial;
            sprite.material.color = Color.white;
        }
    }

    public void Death()
    {
       // Debug.Log("GameOver");
        input.DisableGameInput();
        anim.Play("Death");

        SpriteRenderer[] sprites = GetComponentsInChildren<SpriteRenderer>();
        

        foreach (SpriteRenderer sprite in sprites)
        {
            sprite.material = defaultMaterial;
            sprite.material.color = Color.white;
        }
    }

    public void AddGunLoad(float amount)
    {
        gunLoad = gunLoad + amount;
        UIManger.instance.UpdateGunLoad(gunLoad);
    }

    public void Heal(int amount)
    {
        damageSystem.Heal(amount);
    }

    public void ToggleAttackCollider(bool enabled)
    {
        meleeCollider.SetActive(enabled);
    }

    public void SpawnJumpSprite()
    {
        
        GameObject instance = Instantiate(jumpSprite, movement.groundCheck.position, Quaternion.Euler(0,0,0), null);
        instance.transform.localScale = new Vector3(.5f, .5f, .5f);

    }

    public void SpawnLandSprite()
    {
        GameObject instance = Instantiate(landSprite, movement.groundCheck.position, Quaternion.Euler(0, 0, 0), null);
        instance.transform.localScale = new Vector3(.5f, .5f, .5f);

    }
}
