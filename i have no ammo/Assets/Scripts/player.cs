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

    // Start is called before the first frame update
    void Start()
    {
        playerRigidbody = gameObject.GetComponent<Rigidbody2D>();
        ammoCount = 0;
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
                gameObject.GetComponent<SpriteRenderer>().color = Color.magenta;
            }

            if (parryCoolDown <= 0)
            {
                gameObject.GetComponent<SpriteRenderer>().color = Color.white;
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
            ammoText.text = ammoCount.ToString();
            Debug.Log("player bullet fired");
            //projectile newBullet = Instantiate(playerBullet, transform.position, Quaternion.identity).GetComponent<bullet>();
            //todo once projectile script is created: give projectile direction and speed
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
                Debug.Log("bullet parried");
                ammoCount++;
                ammoText.text = ammoCount.ToString();
            }
            else
            {
                gameObject.GetComponent<SpriteRenderer>().color = Color.blue;
                Debug.Log("bullet hit");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
