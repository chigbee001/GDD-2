using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed { get; set; }
    public Vector3 direction { get; set; }
    public Player player;
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
        // Makes player bullets kill enemies and enemy bullets deal damage to players
        switch (gameObject.tag)
        {
            case "pBullet":
                if(other.tag == "Enemy")
                {
                    Destroy(other.gameObject);   
                }
                Destroy(gameObject);
                break;
            case "eBullet":
                if(other.tag == "Player")
                {
                    player.TakeDamage();
                }
                Destroy(gameObject);
                break;
            default:
                Debug.Log("PUT THE TAG IN THE BULLET PLS");
                break;
        }
    }
}
