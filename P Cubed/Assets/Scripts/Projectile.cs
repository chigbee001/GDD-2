using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed { get; set; }
    public Vector3 direction { get; set; }
    //public Player player;
    private Vector3 target;
    private float aliveTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        target = direction * 50;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        aliveTimer += Time.deltaTime;

        if(aliveTimer > 10)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Makes player bullets kill(edited to damage) enemies and enemy bullets deal damage to players
        switch (gameObject.tag)
        {
            case "pBullet":
                if(other.tag == "Enemy")
                {
                    //added functionality for enemies with more than 1 hp commented out destuction for now
                    other.GetComponentInParent<Enemy>().TakeDamage(1);
                    //Destroy(other.gameObject);
                    Destroy(gameObject);
                }
                break;
            case "eBullet":
                if(other.tag == "Player")
                {
                    other.GetComponentInParent<Player>().TakeDamage();
                    Destroy(gameObject);
                }
                break;
            default:
                Debug.Log("PUT THE TAG IN THE BULLET PLS");
                break;
        }
    }
}
