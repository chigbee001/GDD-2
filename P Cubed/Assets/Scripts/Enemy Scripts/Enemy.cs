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
    public int spawnCost = 0;
    public int spawnWeightedValue = 0;
    public int groupSpawnDiscount = 0;

    [SerializeField]
    private float damageFlashDuration = 0.25f;
    private float damageFlashTimer = 0;
    private Color defaultColor;

    public Player player;
    [SerializeField]
    private Projectile bulletPrefab;
    [SerializeField] [Min(0)]
    private float bulletSpeed;

    [SerializeField]
    private bool shootsDirect = false;
    [SerializeField]
    private bool shootsSpread = false;
    [SerializeField] [Min(2)]
    private int numBulletsInSpread;
    [SerializeField] [Min(0)]
    private float spreadAngle;
    [SerializeField] [Min(0)]
    private float shootCooldown;
    private float shootTimer = 0;
    [SerializeField] [Min(0)]
    private float initialShootDelay;

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
        spawnWeightedValue = Mathf.Clamp(spawnWeightedValue, 1, 50);
        health = GameManager.currentLevel * 2;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        defaultColor = GetComponent<SpriteRenderer>().color;

        shootTimer = initialShootDelay;
    }

    //currently just moves enemies along the path 
    public void Update()
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
        if(pursuitEnemy)
        {
            //pursuit enemies will move towards the player and stun player then die upon contact
            Vector2 movement = player.transform.position - transform.position;
            transform.Translate(movement.normalized * moveSpeed * Time.deltaTime, Space.World);
            
        }

        // Damage flash
        Flicker();

        // Update damage flash timer
        if (damageFlashTimer > 0)
        {
            damageFlashTimer -= Time.deltaTime;
            damageFlashTimer = Mathf.Max(damageFlashTimer, 0);
        }

        // Shoot if off cooldown
        if (shootsDirect && shootTimer <= 0)
        {
            DirectShot();
            shootTimer = shootCooldown;
        }
        if (shootsSpread && shootTimer <= 0)
        {
            SpreadShot(numBulletsInSpread, spreadAngle);
            shootTimer = shootCooldown;
        }

        // Update shooting timer
        if (shootTimer > 0)
        {
            shootTimer -= Time.deltaTime;
            shootTimer = Mathf.Max(shootTimer, 0);
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

    public void TakeDamage(float damageTaken)
    {
        damageFlashTimer = damageFlashDuration;

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

    /// <summary>
    /// Makes the enemy's color red if they've been recently damaged
    /// </summary>
    private void Flicker()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();

        if (damageFlashTimer > 0)
        {
            sprite.color = Color.red;
        }
        else
        {
            sprite.color = defaultColor;
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
            if(pursuitEnemy == true)
            {
                Die();
            }
            else
            {
                TakeDamage(1);
            }
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
        if (numBullets <= 1)
        {
            DirectShot();
            return;
        }

        float theta = spreadAngle / (numBullets - 1);
        float startingAngle = Mathf.Rad2Deg * Mathf.Atan((transform.position.y - player.transform.position.y) / (transform.position.x - player.transform.position.x)) - spreadAngle / 2;

        if (player.transform.position.x < transform.position.x)
        {
            startingAngle += 180;
        }

        for (int i = 0; i < numBullets; i++)
        {
            Projectile bullet = Instantiate(bulletPrefab);

            bullet.transform.position = transform.position;
            bullet.speed = bulletSpeed;
            bullet.direction = new Vector2(Mathf.Cos(Mathf.Deg2Rad * (startingAngle + i * theta)), Mathf.Sin(Mathf.Deg2Rad * (startingAngle + i * theta)));
        }
    }
}
