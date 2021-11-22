using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parry : MonoBehaviour
{
    private float parryTimer = 0;
    private const float parryTimerMax = .3f;
    private float parryCoolDown = 0;
    private const float parryCoolDownMax = .5f;
    private SpriteRenderer hitboxIndicator;
    public player playerScript;

    // Start is called before the first frame update
    void Start()
    {
        hitboxIndicator = gameObject.GetComponent<SpriteRenderer>();
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

            hitboxIndicator.color = new Color(.4f, 0.06f, 0.23f, .5f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if parryTimer > 0 then the parry was successful
        if (collision != null && collision.tag == "enemyBullet" && parryTimer > 0)
        {
            playerScript.Parried();
            Destroy(collision.gameObject);
        }
    }
}
