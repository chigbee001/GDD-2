using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody2D rigidBody;
    public float moveSpeed;
    public float health;
    public int pathNumber;
    private int pathIndex = 0;
    private Transform nextStep;
    public bool tankEnemy;
    public bool shooterEnemy;
    public bool pursuitEnemy;

    public Player player;
    [SerializeField]
    private Projectile bulletPrefab;
    [SerializeField]
    private float bulletSpeed;

    //enemy constructor for testing purposes
    public Enemy(float moveSpeed, float health)
    {
        this.moveSpeed = moveSpeed;
        this.health = health;
    }

    //initial start for enemies to allow for testing
    void Start()
    {
        rigidBody = this.GetComponent<Rigidbody2D>();
        Pathing.SetPath(pathNumber);
        nextStep = Pathing.pathPoints[0];
    }

    //currently just moves enemies along the path 
    private void Update()
    {
        if (!pursuitEnemy)
        {
            Vector2 movement = nextStep.position - transform.position;
            transform.Translate(movement.normalized * moveSpeed * Time.deltaTime, Space.World);
            if (Vector2.Distance(transform.position, nextStep.position) <= .2f)
            {
                GetNextStep();
            }
        }
        else
        {
            //pursuit enemies will move towards the player and stun player then die upon contact
        }
    }

    //when a path point is reached next point is obtained if endpoint is reached enemies attack and die
    private void GetNextStep()
    {
        if (pathIndex >= Pathing.pathPoints.Length - 1)
        {
            Attack();
            return;
        }
        pathIndex++;
        nextStep = Pathing.pathPoints[pathIndex];
    }

    public void TakeDamage(int damageTaken)
    {
        if (tankEnemy)
        {
            health -= .5f * damageTaken;
        }
        else
        {
            health -= damageTaken;
        }
        if(health <= 0)
        {
            Die();
        }
    }

    //enemy destroyed generic float of 1 returned for once damage is hooked up
    public float Attack()
    {
        Die();
        return 1;
    }

    public void Die()
    {
        EnemyManager.EnemiesAlive--;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player") 
        {
            other.GetComponent<Player>().TakeDamage();
        }
    }


    // NOTE: I'm not sure if this is the right place to put these, but what follows is a bunch of methods for various types of bullet patterns
    
    private void DirectShot()
    {
        Projectile bullet = Instantiate(bulletPrefab);

        bullet.transform.position = transform.position;
        bullet.speed = bulletSpeed;
        bullet.direction = player.transform.position - transform.position;
    }

    /// <summary>
    /// Fires a number of projectiles in a given cone angle
    /// </summary>
    /// <param name="numBullets">Number of projectiles to fire</param>
    /// <param name="spreadAngle">Angle between the widest apart bullets in degrees</param>
    private void SpreadShot(int numBullets, float spreadAngle)
    {
        float theta = spreadAngle / numBullets;
        float startingAngle = Vector2.Angle(player.transform.position, transform.position) - spreadAngle / 2;

        for (int i = 0; i < numBullets; i++)
        {
            Projectile bullet = Instantiate(bulletPrefab);

            bullet.transform.position = transform.position;
            bullet.speed = bulletSpeed;
            bullet.direction = new Vector2(Mathf.Cos(Mathf.Deg2Rad * (startingAngle + i * theta)), Mathf.Sin(Mathf.Deg2Rad * (startingAngle + i * theta)));
        }
    }
}
