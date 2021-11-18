using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class player : MonoBehaviour
{
    //parry variables
    private float parryTimer = 0;
    private const float parryTimerMax = .15f;
    private float parryCoolDown = 0;
    private const float parryCoolDownMax = .25f;
    private SpriteRenderer hitboxIndicator;

    //movement variables
    private Rigidbody2D playerRigidbody;
    private float speed = 300;

    //ammo variables
    private int ammoCount;
    public Text ammoText;

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

    //animation
    public Animator playerAnimator;

    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = gameObject.GetComponent<Rigidbody2D>();
        ammoCount = 0;
        maxHealth = 10;
        currentHealth = maxHealth;
        healthBarMaxWidth = healthBarImage.rectTransform.sizeDelta.x;
        isAlive = true;
        hitboxIndicator = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //parry timing
        if (parryTimer > 0)
        {
            parryTimer -= Time.deltaTime;
        }

        if (parryCoolDown > 0)
        {
            parryCoolDown -= Time.deltaTime;

            if (parryTimer <= 0)
            {
                hitboxIndicator.color = new Color(.8f, 0.06f, 0.43f, .5f);
            }

            if (parryCoolDown <= 0)
            {
                hitboxIndicator.color = new Color(1, 0.26f, 0.63f, .5f);
            }
        }

        //parry when q is pressed
        if (Input.GetKeyDown(KeyCode.Q) && parryCoolDown <= 0)
        {
            parryCoolDown = parryCoolDownMax;
            parryTimer = parryTimerMax;
        }

        //shoot when w is pressed
        if (Input.GetKeyDown(KeyCode.W) && ammoCount >= 5)
        {
            ammoCount -= 5;
            ammoText.text = string.Format("ammo: {0} {1}", ammoCount.ToString(), ammoCount >= 5 ? " <color=#00ff00ff>ready</color>" : "");
            projectile newBullet = Instantiate(playerBullet, transform.position, Quaternion.identity).GetComponent<projectile>();
            newBullet.speed = bulletSpeed;
            newBullet.speedCap = bulletSpeed;
            newBullet.speedFloor = bulletSpeed;
            newBullet.direction = Vector2.right;

            playerAnimator.SetTrigger("shoot");
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
        if (collision.tag == "enemyBullet")
        {
            //if parryTimer > 0 then the parry was successful
            if (parryTimer > 0)
            {
                //Debug.Log("bullet parried");

                ammoCount++;
                ammoText.text = string.Format("ammo: {0} {1}", ammoCount.ToString(), ammoCount >= 5 ? " <color=#33ff33>ready</color>" : "");

                Destroy(collision.gameObject); //commented out for testing
            }
            //else player gets hit
            else
            {
                //Debug.Log("bullet hit");

                currentHealth -= 2;
                if (currentHealth <= 0)
                {
                    isAlive = false;
                }

                UpdateHealthBar();

                Destroy(collision.gameObject); //commented out for testing
            }
        }
    }

    //update healthbar ui
    private void UpdateHealthBar()
    {
        healthBarImage.rectTransform.sizeDelta = new Vector2(currentHealth / maxHealth * healthBarMaxWidth, healthBarImage.rectTransform.sizeDelta.y);
    }

    //property for if the player is alive
    public bool IsAlive
    {
        get { return isAlive; }
    }
}
