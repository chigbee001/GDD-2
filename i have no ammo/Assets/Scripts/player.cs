using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    //bullet variables
    private int attackDamage = 1;
    private float bulletSpeed = 7;
    public GameObject playerBullet;

    //health variables
    public Image healthBarImage;
    private float healthBarMaxWidth;
    private float maxHealth;
    private float currentHealth;
    private bool isAlive;
    private float invincibilityTime = 0;
    private const float invincibilityTimeMax = .25f;

    //animation
    public Animator playerAnimator;

    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = gameObject.GetComponent<Rigidbody2D>();

        ammoChargePercent = 0;
        ammoCount = 0;
        ammoChargeBarMaxWidth = ammoChargeBarImage.rectTransform.sizeDelta.x;
        UpdateBarSize(ammoChargeBarImage, ammoChargeBarMaxWidth, ammoChargePercent);

        maxHealth = 10;
        currentHealth = maxHealth;
        healthBarMaxWidth = healthBarImage.rectTransform.sizeDelta.x;
        invincibilityTime = 0;
        isAlive = true;

        parry = transform.GetChild(0).GetComponent<parry>();
    }

    // Update is called once per frame
    void Update()
    {
        //shoot when w is pressed
        if (Input.GetKeyDown(KeyCode.W) && ammoCount >= 1)
        {
            ammoCount--;
            ammoText.text = ammoCount.ToString();
            projectile newBullet = Instantiate(playerBullet, transform.position, Quaternion.identity).GetComponent<projectile>();
            newBullet.speed = bulletSpeed;
            newBullet.speedCap = bulletSpeed;
            newBullet.speedFloor = bulletSpeed;
            newBullet.direction = Vector2.right;

            playerAnimator.SetTrigger("shoot");
        }

        //count down invincibility time
        if (invincibilityTime > 0)
        {
            invincibilityTime -= Time.deltaTime;
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
        ammoChargePercent += .2f;
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
}
