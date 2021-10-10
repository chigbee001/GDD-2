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

        // Applies animation effects based on whether the player is stunned or invulnerable
        Flicker();

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
    /// Changes the player's sprite color to visualize the fact that they're stunned or invulnerable.
    /// </summary>
    private void Flicker()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();

        if (Stunned)
        {
            sprite.color = Color.red;
        }
        else if (Invulnerable)
        {
            if (invulnerabilityTimer % 0.16f > 0.08f)
            {
                sprite.color = Color.white;
            }
            else
            {
                sprite.color = Color.grey;
            }
        }
        else
        {
            sprite.color = Color.white;
        }
    }
}
