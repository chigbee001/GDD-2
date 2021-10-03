using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;

    [SerializeField] [Min (0)]
    private float walkSpeed = 1;

    private Vector2 direction = Vector2.zero;
    private Vector2 velocity = Vector2.zero;

    [SerializeField] [Min (0)]
    private float stunDuration = 1;
    private float stunTimer = 0;

    [SerializeField] [Min (0)]
    private float invulnerabilityDuration = 1.5f;
    private float invulnerabilityTimer = 0;

    [SerializeField] [Min(0)]
    private float shootCooldown = 0.2f;
    private float shootTimer = 0;

    public bool Stunned
    {
        get
        {
            return stunTimer > 0;
        }
    }

    public bool Invulnerable
    {
        get
        {
            return invulnerabilityTimer > 0;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Reset direction vector
        direction = Vector2.zero;

        // Get inputs for each direction and add their direction vector to the player's
        if (Input.GetKey(KeyCode.W))
        {
            direction += Vector2.up;
        }
        if (Input.GetKey(KeyCode.S))
        {
            direction += Vector2.down;
        }
        if (Input.GetKey(KeyCode.D))
        {
            direction += Vector2.right;
        }
        if (Input.GetKey(KeyCode.A))
        {
            direction += Vector2.left;
        }

        // Normalize the direction the player will be moving
        direction.Normalize();

        // Don't move if the player is stunned
        if (Stunned)
        {
            velocity = Vector2.zero;
        }

        // Otherwise, move based on the inputted direction
        else
        {
            velocity = direction * walkSpeed;
        }

        // Apply velocity changes
        rb.velocity = velocity;

        // Shoot bullets when the player left clicks
        if (Input.GetMouseButtonDown(0) && shootTimer <= 0)
        {
            // Shoots the bullet towards the cursor
            ShootBullet();

            // Starts the cooldown timer for shooting
            shootTimer = shootCooldown;
        }

        // Tick down timers
        if (stunTimer > 0)
        {
            stunTimer -= Time.deltaTime;
            stunTimer = Mathf.Max(0, stunTimer);
        }
        if (invulnerabilityTimer > 0)
        {
            invulnerabilityTimer -= Time.deltaTime;
            invulnerabilityTimer = Mathf.Max(0, invulnerabilityTimer);
        }
        if (shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
            shootTimer = Mathf.Max(0, shootTimer);
        }
    }

    /// <summary>
    /// Stuns/hits the player if possible
    /// </summary>
    /// <returns>True if the player becomes stunned, false if not</returns>
    public bool TakeDamage()
    {
        // If the player can't be hit, don't do anything and return false
        if (Invulnerable) { return false; }

        // Start the stun and invulnerability timers appropriately
        stunTimer = stunDuration;
        invulnerabilityTimer = invulnerabilityDuration;

        // Return true since the player has been stunned
        return true;
    }

    /// <summary>
    /// INCOMPLETE! Instantiates a bullet at the player's position moving towards the cursor
    /// </summary>
    private void ShootBullet()
    {
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // TODO: INSTANTIATE A BULLET HERE
        Vector2 bulletPosition = transform.position;
        Vector2 bulletVelocity = Vector2.zero; // Placeholder for the actual bullet object
        float bulletSpeed = 10;

        // Calculate the direction the bullet should move based on the player's position and the cursor's position
        Vector2 bulletDirection = mouseWorldPos - (Vector2)transform.position;
        bulletDirection.Normalize();

        // Apply that direction and speed to the bullet
        bulletVelocity = bulletDirection * bulletSpeed;

        // Just to test that it's working
        Debug.Log("Bullet shot! Direction = " + bulletDirection + ", Velocity = " + bulletVelocity + ", Position = " + bulletPosition);
    }
}
