using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parry : MonoBehaviour
{
    private float parryTimer = 0;
    private float parryTimerMax = .3f;
    private float parryCoolDown = 0;
    private float parryCoolDownMax = .5f;
    private SpriteRenderer hitboxIndicator;
    public player playerScript;
    private bool autoParryOn = false;
    private bool deflectBullet = false;

    private GameManager gamemanager;

    // Start is called before the first frame update
    void Start()
    {
        gamemanager = FindObjectOfType<GameManager>();

        hitboxIndicator = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gamemanager.paused)
        {
            return;
        }

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

            hitboxIndicator.color = new Color(.4f, 0.06f, 0.23f, .5f);

            playerScript.PlayerAnimator.ResetTrigger("shoot");
            playerScript.playerAnimator.SetTrigger("parry");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //check if its something that can be parried
        if (collision != null && collision.tag == "enemyBullet")
        {
            bool parried = false;

            //if player didnt parry and can auto parry, auto parry
            if (parryTimer <= 0 && autoParryOn && parryCoolDown <= 0)
            {
                parryCoolDown = parryCoolDownMax * 4;
                parryTimer = parryTimerMax;

                hitboxIndicator.color = new Color(.4f, 0.06f, 0.23f, .5f);

                parried = true;
            }
            //if parryTimer > 0 then the parry was successful
            else if (parryTimer > 0)
            {
                parried = true;
            }

            if(parried)
            {
                playerScript.Parried();

                if(deflectBullet)
                {
                    projectile bullet = collision.GetComponent<projectile>();
                    bullet.tag = "playerBullet";
                    bullet.direction = Vector3.right;
                    bullet.speed = 8;
                    bullet.speedCap = bullet.speed;
                    bullet.speedFloor = bullet.speed;
                    bullet.acceleration = 0;
                    bullet.rotationAcceleration = 0;
                    bullet. behavior = null;
                }
                else
                {
                    Destroy(collision.gameObject);
                }
            }
        }
    }

    //property for active time of parry
    public float ActiveTime
    {
        get { return parryTimerMax; }
        set { parryTimerMax = value; }
    }

    //property for cooldown of parry, cooldown time includes active time
    public float CooldownTime
    {
        get { return parryCoolDownMax; }
        set { parryCoolDownMax = value; }
    }

    //property for autoparry
    public bool AutoParryOn
    {
        get { return autoParryOn; }
        set { autoParryOn = value; }
    }

    //property for deflectbullet
    public bool DeflectBulletOn
    {
        get { return deflectBullet; }
        set { deflectBullet = value; }
    }
}
