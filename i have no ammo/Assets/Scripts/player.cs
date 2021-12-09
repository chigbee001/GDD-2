using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//enum for player's core, changes various things about the player and adds mechanics to their shooting
public enum PlayerCore
{
    Basic,
    Shotgun,   //-5 bullets, 1 forward, 2 at +/-11.25 degrees, 2 at +/-22.5 degrees
    Missile,   //-projectile goes forward slightly, stops, then accelerates
    Remote,    //-follows player y pos as it moves, slow
    Grenade,   //-projectile moves slightly in dir of player vel, explodes after a bit into aoe hitbox
    Energize,  //-every time you parry increase speed up to a cap, when you shoot reset speed
    Devour,    //chargeCost = 2, for every ammo you gain when ammoCount > 5 damage the boss and yourself
    Quantum,   //-attack increases with ammoCount, shoot consumes all ammo, chargeCost = 1 (damage is x + (x^2 / 10) where x = ammoCount, at 10 parries damage is double ammoCount with this equation)
    Ballistic, //-chargeCost = 10, when you shoot if charge is > 50% shoot 2 bullets, if charge is > 75% shoot 3 bullets
    Pillow,    //-(easy option) auto parry, when auto parry occurs parry cooldown is 4x longer
    Heart,     //-(easy option) when you shoot heal 1 health
    Cloud,     //-(easy option) block a hit, refreshes when you gain ammo
    Plague,    //-(hard option) lose health over time, parry to gain health, can go over max health. parry doesnt give charge, gain charge by taking damage. (the idea is that this lets you shoot far more often but you also have more to manage)
    War,       //-(hard option) you take double damage, your parry window is .2, parry also deflects bullets straight ahead, deflected bullets deal 50% of your bullet damage
    Pain       //(gio option) you have 1 health, your parry window is .1 second, good luck, dumbass. shooting instantly damages the boss and deals a lot of damage
}

public class player : MonoBehaviour
{
    //parry variables
    private parry parry;

    //movement variables
    private Rigidbody2D playerRigidbody;
    private float speed = 300;

    //ammo variables
    private int ammoCount;
    public Text ammoText;
    public Image ammoChargeBarImage;
    private float ammoChargeBarMaxWidth;
    private float ammoChargePercent;
    private int chargeCost;

    //bullet variables
    private int attackDamage = 1;
    private float bulletSpeed = 7;
    private float bulletSpeedCap = 100;
    private float bulletSpeedFloor = -100;
    public GameObject playerBullet;

    //health variables
    public Image healthBarImage;
    public Image healthBarBackgroundImage;
    private float healthBarMaxWidth;
    private float maxHealth;
    private float currentHealth;
    private bool isAlive;
    private float invincibilityTime = 0;
    private const float invincibilityTimeMax = 1;

    //animation
    public Animator playerAnimator;

    //shoot type variables
    private PlayerCore shootType;

    //grenade explosion object
    public GameObject grenadeExplosion;

    //energize speed up and reset
    private const float energizeSpeedUp = 50;
    private float energizeBaseSpeed;
    private const float energizeMaxSpeed = 600;

    //heart heal
    private float heartHeal = 1;

    //cloud hit block
    private bool cloudBlockHit = false;

    //plague damage over time
    private float plagueDamageTime = 0;
    private const float plagueDamageTimeMax = 2;
    private float plagueDamage = .5f;
    private float plagueHeal = 1;

    //war damage multiplier
    private float warDamageMultiplier = 1;

    //cloud block indicator color
    private Color cloudBlockIndication = new Color(1f, 1f, .25f);

    //gamemanager
    private GameManager gamemanager;

    //shot sprites 0 = basic, 1 = missile, 2 = remote, 3 = grenade, 4 = grenade explosion
    public List<Sprite> shotSprites = new List<Sprite>();

    private SpriteRenderer playerSprite;

    // Start is called before the first frame update
    void Start()
    {
        gamemanager = FindObjectOfType<GameManager>();

        playerRigidbody = gameObject.GetComponent<Rigidbody2D>();
        playerSprite = gameObject.GetComponent<SpriteRenderer>();

        ammoChargePercent = 0;
        ammoCount = 0;
        ammoChargeBarMaxWidth = ammoChargeBarImage.rectTransform.sizeDelta.x;
        UpdateBarSize(ammoChargeBarImage, ammoChargeBarMaxWidth, ammoChargePercent);

        SetMaxHealth(10);
        invincibilityTime = 0;
        isAlive = true;

        parry = transform.GetChild(0).GetComponent<parry>();
        
        SetCore(shootType);
    }

    // Update is called once per frame
    void Update()
    {
        if (gamemanager.paused)
        {
            return;
        }

        //shoot when w is pressed
        if (Input.GetKeyDown(KeyCode.W) && ammoCount >= 1)
        {
            Shoot();
        }

        //count down invincibility time
        if (invincibilityTime > 0)
        {
            invincibilityTime -= Time.deltaTime;

            //flicker (code stolen from dan)
            if (invincibilityTime % 0.16f > 0.08f)
            {
                playerSprite.color = new Color(1, 1, 1, 1);
            }
            else
            {
                playerSprite.color = new Color(1, 1, 1, .5f);
            }

            if (invincibilityTime <= 0)
            {
                playerSprite.color = new Color(1, 1, 1, 1);
            }
        }

        //deal damage from plague core
        if(plagueDamageTime > 0)
        {
            plagueDamageTime -= Time.fixedDeltaTime;

            if(plagueDamageTime <= 0)
            {
                TakeDamage(plagueDamage);
                plagueDamageTime = plagueDamageTimeMax;

                IncreaseChargePercent();
            }
        }
    }

    private void FixedUpdate()
    {
        //player movement
        if (Input.GetKey(KeyCode.UpArrow))
        {
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, speed * Time.fixedDeltaTime);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, -1 * speed * Time.fixedDeltaTime);
        }
        else
        {
            playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 0);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            playerRigidbody.velocity = new Vector2(-1 * speed * Time.fixedDeltaTime, playerRigidbody.velocity.y);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            playerRigidbody.velocity = new Vector2(speed * Time.fixedDeltaTime, playerRigidbody.velocity.y);
        }
        else
        {
            playerRigidbody.velocity = new Vector2(0, playerRigidbody.velocity.y);
        }
        if (!Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow))
        {
            playerRigidbody.velocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //check for collision with enemy bullets
        if (collision != null && collision.tag == "enemyBullet" && invincibilityTime <= 0)
        {
            if (cloudBlockHit)
            {
                cloudBlockHit = false;
                playerSprite.color = Color.white;
                Destroy(collision.gameObject);
                invincibilityTime = invincibilityTimeMax;
            }
            else
            {
                currentHealth -= 2 * warDamageMultiplier;
                if (currentHealth <= 0)
                {
                    isAlive = false;
                }

                UpdateBarSize(healthBarImage, healthBarMaxWidth, currentHealth / maxHealth);

                Destroy(collision.gameObject);

                invincibilityTime = invincibilityTimeMax;

                if(shootType == PlayerCore.Plague)
                {
                    IncreaseChargePercent();
                }
            }
        }
    }

    //update ui bar
    private void UpdateBarSize(Image image, float imageMaxWidth, float percent)
    {
        image.rectTransform.sizeDelta = new Vector2(percent * imageMaxWidth, image.rectTransform.sizeDelta.y);
    }

    //do everything that has to happen after a successful parry occurs
    public void Parried()
    {
        if(shootType == PlayerCore.Plague)
        {
            currentHealth += plagueHeal;
            UpdateBarSize(healthBarImage, healthBarMaxWidth, currentHealth / maxHealth);
        }
        else
        {
            IncreaseChargePercent();
        }
    }

    //shoot
    private void Shoot()
    {
        if (shootType == PlayerCore.Quantum)
        {
            attackDamage = ammoCount + Mathf.RoundToInt(Mathf.Pow(ammoCount, 2) / 10);
            ammoCount -= ammoCount;
        }
        else
        {
            ammoCount--;
        }

        ammoText.text = ammoCount.ToString();
        projectile newBullet;
            
        switch(shootType)
        {
            case PlayerCore.Basic:
                newBullet = Instantiate(playerBullet, transform.position, Quaternion.identity).GetComponent<projectile>();
                newBullet.speed = bulletSpeed;
                newBullet.speedCap = bulletSpeedCap;
                newBullet.speedFloor = bulletSpeedFloor;
                newBullet.direction = Vector2.right;
                break;

            case PlayerCore.Shotgun:
                //pi / 8 = 22.5deg, pi / 16 = 11.25deg
                for (float i = -1 * Mathf.PI / 8; i <= Mathf.PI / 8; i += Mathf.PI / 16)
                {
                    newBullet = Instantiate(playerBullet, transform.position, Quaternion.identity).GetComponent<projectile>();
                    newBullet.direction = new Vector3(Mathf.Cos(i), Mathf.Sin(i));
                    newBullet.speed = bulletSpeed;
                    newBullet.speedCap = bulletSpeedCap;
                    newBullet.speedFloor = bulletSpeedFloor;
                }
                break;

            case PlayerCore.Missile:
                newBullet = Instantiate(playerBullet, transform.position + Vector3.right * .5f, Quaternion.identity).GetComponent<projectile>();
                newBullet.direction = Vector3.right;
                newBullet.speed = bulletSpeed;
                newBullet.speedCap = bulletSpeedCap;
                newBullet.speedFloor = bulletSpeedFloor;
                newBullet.acceleration = -5;
                newBullet.behavior = MissileMovement;
                //newBullet.GetComponent<SpriteRenderer>().sprite = shotSprites[1];
                break;

            case PlayerCore.Remote:
                newBullet = Instantiate(playerBullet, transform.position, Quaternion.identity).GetComponent<projectile>();
                newBullet.direction = Vector3.right;
                newBullet.speed = bulletSpeed;
                newBullet.speedCap = bulletSpeedCap;
                newBullet.speedFloor = bulletSpeedFloor;
                newBullet.behavior = RemoteMovement;
                newBullet.GetComponent<SpriteRenderer>().sprite = shotSprites[2];
                break;

            case PlayerCore.Grenade:
                newBullet = Instantiate(playerBullet, transform.position, Quaternion.identity).GetComponent<projectile>();
                newBullet.direction = playerRigidbody.velocity;
                newBullet.speed = bulletSpeed;
                newBullet.speedCap = bulletSpeedCap;
                newBullet.speedFloor = bulletSpeedFloor;
                newBullet.acceleration = -2;
                newBullet.rotationSpeed = 5;
                newBullet.rotationSpeedCap = 5;
                newBullet.rotationSpeedFloor = 0;
                newBullet.behavior = GrenadeBehavior;
                newBullet.GetComponent<SpriteRenderer>().sprite = shotSprites[3];
                break;

            case PlayerCore.Energize:
                newBullet = Instantiate(playerBullet, transform.position, Quaternion.identity).GetComponent<projectile>();
                newBullet.direction = Vector3.right;
                newBullet.speed = bulletSpeed;
                newBullet.speedCap = bulletSpeedCap;
                newBullet.speedFloor = bulletSpeedFloor;
                speed = energizeBaseSpeed;
                break;

            case PlayerCore.Devour:
                newBullet = Instantiate(playerBullet, transform.position, Quaternion.identity).GetComponent<projectile>();
                newBullet.direction = Vector3.right;
                newBullet.speed = bulletSpeed;
                newBullet.speedCap = bulletSpeedCap;
                newBullet.speedFloor = bulletSpeedFloor;
                break;

            case PlayerCore.Quantum:
                newBullet = Instantiate(playerBullet, transform.position, Quaternion.identity).GetComponent<projectile>();
                newBullet.direction = Vector3.right;
                newBullet.speed = bulletSpeed;
                newBullet.speedCap = bulletSpeedCap;
                newBullet.speedFloor = bulletSpeedFloor;
                break;

            case PlayerCore.Ballistic:
                int numBullets = 1;
                float startY = 0;

                if (ammoChargePercent >= .75f)
                {
                    numBullets = 3;
                    startY = -1;
                }
                else if(ammoChargePercent >= .5f)
                {
                    numBullets = 2;
                    startY = -.5f;
                }

                for(int i = 0; i < numBullets; i++)
                {
                    newBullet = Instantiate(playerBullet, transform.position + new Vector3(0, startY + i, 0), Quaternion.identity).GetComponent<projectile>();
                    newBullet.direction = Vector3.right;
                    newBullet.speed = bulletSpeed;
                    newBullet.speedCap = bulletSpeedCap;
                    newBullet.speedFloor = bulletSpeedFloor;
                }
                break;

            case PlayerCore.Pillow:
                newBullet = Instantiate(playerBullet, transform.position, Quaternion.identity).GetComponent<projectile>();
                newBullet.direction = Vector3.right;
                newBullet.speed = bulletSpeed;
                newBullet.speedCap = bulletSpeedCap;
                newBullet.speedFloor = bulletSpeedFloor;
                break;

            case PlayerCore.Heart:
                newBullet = Instantiate(playerBullet, transform.position, Quaternion.identity).GetComponent<projectile>();
                newBullet.direction = Vector3.right;
                newBullet.speed = bulletSpeed;
                newBullet.speedCap = bulletSpeedCap;
                newBullet.speedFloor = bulletSpeedFloor;
                currentHealth += heartHeal;
                UpdateBarSize(healthBarImage, healthBarMaxWidth, currentHealth / maxHealth);
                break;

            case PlayerCore.Cloud:
                newBullet = Instantiate(playerBullet, transform.position, Quaternion.identity).GetComponent<projectile>();
                newBullet.direction = Vector3.right;
                newBullet.speed = bulletSpeed;
                newBullet.speedCap = bulletSpeedCap;
                newBullet.speedFloor = bulletSpeedFloor;
                cloudBlockHit = true;
                break;

            case PlayerCore.Plague:
                newBullet = Instantiate(playerBullet, transform.position, Quaternion.identity).GetComponent<projectile>();
                newBullet.direction = Vector3.right;
                newBullet.speed = bulletSpeed;
                newBullet.speedCap = bulletSpeedCap;
                newBullet.speedFloor = bulletSpeedFloor;
                break;

            case PlayerCore.War:
                newBullet = Instantiate(playerBullet, transform.position, Quaternion.identity).GetComponent<projectile>();
                newBullet.direction = Vector3.right;
                newBullet.speed = bulletSpeed;
                newBullet.speedCap = bulletSpeedCap;
                newBullet.speedFloor = bulletSpeedFloor;
                break;

            case PlayerCore.Pain:
                //deal damage directly to boss
                break;

            //when everything is done remove all of the shoot types that are just this and replace this comment with the ones that are dealt with here
            default:
                newBullet = Instantiate(playerBullet, transform.position, Quaternion.identity).GetComponent<projectile>();
                newBullet.speed = bulletSpeed;
                newBullet.speedCap = bulletSpeedCap;
                newBullet.speedFloor = bulletSpeedFloor;
                newBullet.direction = Vector2.right;
                break;
        }

        playerAnimator.ResetTrigger("parry");
        playerAnimator.SetTrigger("shoot");
    }

    //handle player taking damage
    private void TakeDamage(float damage)
    {
        currentHealth -= damage * warDamageMultiplier;
        if (currentHealth <= 0)
        {
            isAlive = false;
        }

        UpdateBarSize(healthBarImage, healthBarMaxWidth, currentHealth / maxHealth);
    }

    //increase charge percent by 1 / chargecost
    private void IncreaseChargePercent()
    {
        ammoChargePercent += 1f / chargeCost;

        if (shootType == PlayerCore.Energize && speed < energizeMaxSpeed)
        {
            speed += energizeSpeedUp;
        }
        else if (shootType == PlayerCore.Devour && ammoCount > 5)
        {
            TakeDamage(1);
            //deal damage directly to boss
        }

        if (ammoChargePercent >= 1)
        {
            ammoCount++;
            ammoText.text = ammoCount.ToString();
            ammoChargePercent = 0;

            if (shootType == PlayerCore.Cloud)
            {
                cloudBlockHit = true;
                playerSprite.color = cloudBlockIndication;
            }
        }

        UpdateBarSize(ammoChargeBarImage, ammoChargeBarMaxWidth, ammoChargePercent);
    }
    
    ///property for if the player is alive
    public bool IsAlive
    {
        get { return isAlive; }
    }

    //set max health and change health bar image based on max health
    private void SetMaxHealth(float max)
    {
        maxHealth = max;
        currentHealth = maxHealth;
        Vector2 barSize = new Vector2(max * 20, healthBarImage.rectTransform.sizeDelta.y);
        healthBarImage.rectTransform.sizeDelta = barSize;
        healthBarBackgroundImage.rectTransform.sizeDelta = barSize;
        healthBarMaxWidth = healthBarImage.rectTransform.sizeDelta.x;
    }

    ///set the player's shoot type
    public void SetCore(PlayerCore core)
    {
        shootType = core;
        parry.AutoParryOn = false;
        cloudBlockHit = false;
        parry.ActiveTime = .3f;
        parry.CooldownTime = .5f;
        warDamageMultiplier = 1;
        parry.DeflectBulletOn = false;
        plagueDamageTime = 0;

        switch(core)
        {
            case PlayerCore.Basic:
                chargeCost = 5;
                bulletSpeed = 7;
                attackDamage = 5;
                bulletSpeedCap = bulletSpeed;
                bulletSpeedFloor = bulletSpeed;
                SetMaxHealth(10);
                break;

            case PlayerCore.Shotgun:
                chargeCost = 6;
                bulletSpeed = 7;
                attackDamage = 4;
                bulletSpeedCap = bulletSpeed;
                bulletSpeedFloor = bulletSpeed;
                SetMaxHealth(6);
                break;

            case PlayerCore.Missile:
                chargeCost = 6;
                bulletSpeed = 1.5f;
                attackDamage = 8;
                bulletSpeedCap = 20;
                bulletSpeedFloor = 0;
                SetMaxHealth(8);
                break;

            case PlayerCore.Remote:
                chargeCost = 5;
                bulletSpeed = 5;
                attackDamage = 4;
                bulletSpeedCap = bulletSpeed;
                bulletSpeedFloor = bulletSpeed;
                SetMaxHealth(8);
                break;

            case PlayerCore.Grenade:
                chargeCost = 3;
                bulletSpeed = 4;
                attackDamage = 4;
                bulletSpeedCap = bulletSpeed;
                bulletSpeedFloor = 0;
                SetMaxHealth(12);
                break;

            case PlayerCore.Energize:
                chargeCost = 6;
                bulletSpeed = 6;
                attackDamage = 5;
                bulletSpeedCap = bulletSpeed;
                bulletSpeedFloor = bulletSpeed;
                energizeBaseSpeed = speed;
                SetMaxHealth(10);
                break;

            case PlayerCore.Devour:
                chargeCost = 2;
                bulletSpeed = 7;
                attackDamage = 5;
                bulletSpeedCap = bulletSpeed;
                bulletSpeedFloor = bulletSpeed;
                SetMaxHealth(14);
                break;

            case PlayerCore.Quantum:
                chargeCost = 1;
                bulletSpeed = 5;
                attackDamage = 1;
                bulletSpeedCap = bulletSpeed;
                bulletSpeedFloor = bulletSpeed;
                SetMaxHealth(6);
                break;

            case PlayerCore.Ballistic:
                chargeCost = 10;
                bulletSpeed = 5;
                attackDamage = 3;
                bulletSpeedCap = bulletSpeed;
                bulletSpeedFloor = bulletSpeed;
                break;

            case PlayerCore.Pillow:
                chargeCost = 9;
                bulletSpeed = 5;
                attackDamage = 4;
                bulletSpeedCap = bulletSpeed;
                bulletSpeedFloor = bulletSpeed;
                SetMaxHealth(8);
                parry.AutoParryOn = true;
                break;

            case PlayerCore.Heart:
                chargeCost = 8;
                bulletSpeed = 5;
                attackDamage = 4;
                bulletSpeedCap = bulletSpeed;
                bulletSpeedFloor = bulletSpeed;
                SetMaxHealth(8);
                break;

            case PlayerCore.Cloud:
                chargeCost = 7;
                bulletSpeed = 5;
                attackDamage = 4;
                bulletSpeedCap = bulletSpeed;
                bulletSpeedFloor = bulletSpeed;
                SetMaxHealth(8);
                cloudBlockHit = true;
                playerSprite.color = cloudBlockIndication;
                break;

            case PlayerCore.Plague:
                chargeCost = 8;
                bulletSpeed = 10;
                attackDamage = 2;
                bulletSpeedCap = bulletSpeed;
                bulletSpeedFloor = bulletSpeed;
                SetMaxHealth(15);
                plagueDamageTime = plagueDamageTimeMax;
                break;

            case PlayerCore.War:
                chargeCost = 8;
                bulletSpeed = 5;
                attackDamage = 2;
                bulletSpeedCap = bulletSpeed;
                bulletSpeedFloor = bulletSpeed;
                SetMaxHealth(10);
                warDamageMultiplier = 2;
                parry.DeflectBulletOn = true;
                parry.ActiveTime = .2f;
                parry.CooldownTime = .3f;
                break;

            case PlayerCore.Pain:
                chargeCost = 5;
                bulletSpeed = 0;
                attackDamage = 20;
                bulletSpeedCap = bulletSpeed;
                bulletSpeedFloor = bulletSpeed;
                SetMaxHealth(1);
                parry.ActiveTime = .1f;
                parry.CooldownTime = .25f;
                break;

            default:
                shootType = PlayerCore.Basic;
                chargeCost = 5;
                bulletSpeed = 7;
                attackDamage = 5;
                bulletSpeedCap = bulletSpeed;
                bulletSpeedFloor = bulletSpeed;
                break;
        }
    }

    //movement for missile shoot type projectile
    private void MissileMovement(projectile proj)
    {
        if (proj.speed <= 0)
        {
            proj.speed = .05f;
        }
        else
        {
            proj.acceleration += 20 * Time.fixedDeltaTime;
        }
    }

    //movement for remote shoot type projectile
    private void RemoteMovement(projectile proj)
    {
        proj.transform.position += new Vector3(0, (proj.transform.position.y * -1) + transform.position.y, 0);
    }

    //movement and explosion for grenade shoot type projectile
    private void GrenadeBehavior(projectile proj)
    {
        if (proj.rotationSpeed > 0)
        {
            proj.rotationSpeed -= Time.fixedDeltaTime;
        }
        else
        {
            projectile explosion = Instantiate(grenadeExplosion, proj.transform.position, Quaternion.identity).GetComponent<projectile>();
            explosion.rotationSpeed = .25f;
            explosion.rotationSpeedFloor = 0;
            explosion.rotationSpeedCap = explosion.rotationSpeed;
            explosion.behavior = ExplosionBehavior;
            explosion.transform.localScale = new Vector3(2, 2, 1);
            explosion.GetComponent<SpriteRenderer>().sprite = shotSprites[4];
            Destroy(proj.gameObject);
        }
    }

    //explosion behavior, destroys itself after some time stored in rotationSpeed
    private void ExplosionBehavior(projectile proj) {
        proj.rotationSpeed -= Time.fixedDeltaTime;

        if (proj.rotationSpeed <= 0)
        {
            Destroy(proj.gameObject);
        }
    }

    /// <summary>
    /// get property for player's current shoot type
    /// </summary>
    public PlayerCore ShootType
    {
        get { return shootType; }
    }

    //so animation trigger can be set in parry when parry occurs
    public Animator PlayerAnimator
    {
        get { return playerAnimator; }
        set { playerAnimator = value; }
    }
}
