using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float walkSpeed = 3;

    private Vector2 playerPosition;
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
        playerPosition = transform.position;
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
        if (!Stunned)
        {
            velocity = direction * walkSpeed * Time.deltaTime;

            playerPosition += velocity;

            transform.position = playerPosition;
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
    }
}
