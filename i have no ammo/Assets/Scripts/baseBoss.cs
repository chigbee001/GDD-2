using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class baseBoss : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField]
    float speed;
    [SerializeField]
    Vector2 direction = Vector2.up;

    [SerializeField]
    projectile projectilePrefab;

    [SerializeField]
    float maxHealth;
    public float currentHealth;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        BasicMovement();

        if (currentHealth <= 0) { Die(); }
    }

    void BasicMovement()
    {
        if (transform.position.y > 4) { direction = Vector2.down; }
        if (transform.position.y < -4) { direction = Vector2.up; }

        rb.velocity = direction * speed;
    }

    void Die()
    {
        Debug.Log(name + " has been killed");

        // TEMPORARY, JUST TO NOTE THAT THEY WOULD'VE DIED BUT RESETTING THEIR HEALTH SO IT DOESN'T SPAM THE MESSAGE EVERY FRAME
        currentHealth = maxHealth;
    }
}
