using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//enum for player's core, changes various things about the player and adds mechanics to their shooting
public enum PlayerCore
{
    Basic,
    Shotgun,   //5 bullets, 1 forward, 2 at +/-11.25 degrees, 2 at +/-22.5 degrees
    Missile,   //projectile goes forward slightly, stops, then accelerates
    Remote,    //follows player y pos as it moves, slow
    Grenade,   //projectile moves slightly in dir of player vel, explodes after a bit into aoe hitbox
    Energize,  //every time you parry increase speed up to a cap, when you shoot reset speed
    Devour,    //chargeCost = 2, for every ammo you gain when ammoCount > 5 damage the boss and yourself
    Quantum,   //attack increases with ammoCount, shoot consumes all ammo, chargeCost = 1 (damage is x + (x^2 / 10) where x = ammoCount, at 10 parries damage is double ammoCount with this equation)
    Ballistic, //chargeCost = 10, when you shoot if charge is > 50% shoot 2 bullets, if charge is > 75% shoot 3 bullets
    Pillow,    //(easy option) auto parry, when auto parry occurs parry cooldown is 4x longer
    Heart,     //(easy option) when you shoot heal 1 health
    Cloud,     //(easy option) block a hit, refreshes when you gain ammo
    Plague,    //(hard option) lose health over time, parry to gain health, can go over max health. parry doesnt give charge, gain charge by taking damage. (the idea is that this lets you shoot far more often but you also have more to manage)
    War,       //(hard option) you take double damage, parry also deflects bullets straight ahead, deflected bullets deal 50% of your bullet damage
    Pain       //(gio option) you have 1 health, your parry window is .1 second, good luck, dumbass. shooting instantly damages the boss and deals (.5 * bossCurrentHealth + flatDamage or .2 * bossMaxHealth or high flat amount : decide which one based on how much health bosses will have and balance it with charge amount)
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
    private const float invincibilityTimeMax = .25f;

    //animation
    public Animator playerAnimator;

    //shoot type variables
    private PlayerCore shootType;

    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = gameObject.GetComponent<Rigidbody2D>();

        ammoChargePercent = 0;
        ammoCount = 0;
        ammoChargeBarMaxWidth = ammoChargeBarImage.rectTransform.sizeDelta.x;
        UpdateBarSize(ammoChargeBarImage, ammoChargeBarMaxWidth, ammoChargePercent);

        SetMaxHealth(10);
        invincibilityTime = 0;
        isAlive = true;

        parry = transform.GetChild(0).GetComponent<parry>();

        SetCore(PlayerCore.Basic);
    }

    // Update is called once per frame
    void Update()
    {
        //shoot when w is pressed
        if (Input.GetKeyDown(KeyCode.W) && ammoCount >= 1)
        {
            ammoCount--;
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
                    break;

                case PlayerCore.Remote:
                    newBullet = Instantiate(playerBullet, transform.position, Quaternion.identity).GetComponent<projectile>();
                    newBullet.direction = Vector3.right;
                    newBullet.speed = bulletSpeed;
                    newBullet.speedCap = bulletSpeedCap;
                    newBullet.speedFloor = bulletSpeedFloor;
                    newBullet.behavior = RemoteMovement;
                    break;
            }

            playerAnimator.SetTrigger("shoot");
        }

        //count down invincibility time
        if (invincibilityTime > 0)
        {
            invincibilityTime -= Time.deltaTime;
        }

        //test shoot types
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetCore(PlayerCore.Basic);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetCore(PlayerCore.Shotgun);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetCore(PlayerCore.Missile);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetCore(PlayerCore.Remote);
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
            currentHealth -= 2;
            if (currentHealth <= 0)
            {
                isAlive = false;
            }

            UpdateBarSize(healthBarImage, healthBarMaxWidth, currentHealth / maxHealth);

            Destroy(collision.gameObject);

            invincibilityTime = invincibilityTimeMax;
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
        ammoChargePercent += 1f / chargeCost;
        if (ammoChargePercent >= 1)
        {
            ammoCount++;
            ammoText.text = ammoCount.ToString();
            ammoChargePercent = 0;
        }

        UpdateBarSize(ammoChargeBarImage, ammoChargeBarMaxWidth, ammoChargePercent);
    }

    //property for if the player is alive
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

    //set the player's shoot type
    public void SetCore(PlayerCore core)
    {
        shootType = core;

        switch(core)
        {
            case PlayerCore.Basic:
                chargeCost = 5;
                bulletSpeed = 7;
                attackDamage = 1;
                bulletSpeedCap = bulletSpeed;
                bulletSpeedFloor = bulletSpeed;
                break;

            case PlayerCore.Shotgun:
                chargeCost = 3;
                bulletSpeed = 7;
                attackDamage = 1;
                bulletSpeedCap = bulletSpeed;
                bulletSpeedFloor = bulletSpeed;
                break;

            case PlayerCore.Missile:
                chargeCost = 6;
                bulletSpeed = 1.5f;
                attackDamage = 2;
                bulletSpeedCap = 20;
                bulletSpeedFloor = 0;
                break;

            case PlayerCore.Remote:
                chargeCost = 5;
                bulletSpeed = 5;
                attackDamage = 1;
                bulletSpeedCap = bulletSpeed;
                bulletSpeedFloor = bulletSpeed;
                break;

            case PlayerCore.Pain:
                //chargeCost = 5;
                //bulletSpeed = 0;
                //attackDamage = 20;
                //bulletSpeedCap = bulletSpeed;
                //bulletSpeedFloor = bulletSpeed;
                SetMaxHealth(1);
                parry.ActiveTime = .1f;
                parry.CooldownTime = .25f;
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
}
