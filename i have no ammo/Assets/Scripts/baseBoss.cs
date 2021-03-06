using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class    baseBoss : MonoBehaviour
{
    Rigidbody2D rb;

    public int totalPhases;
    private int currentPhase;
    
    //public GameObject[] projectileSpawners;

    [SerializeField]
    float speed;

    [SerializeField]
    Vector2 direction = Vector2.up;
    [SerializeField]
    public phaseSpawners[] phaseSpawners;
    //[SerializeField]
    //projectile projectilePrefab;

    [SerializeField]
    float maxHealth;
    public float currentHealth;
    public int fireRate;
    private float fireCooldown;
    private bool holdFire;
    private bool phaseSpawnersActive;

    private float showTookDamage = 0;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        totalPhases = phaseSpawners.Length;
        currentPhase = 0;
        currentHealth = maxHealth;
        phaseSpawnersActive = false;
    }

    // Update is called once per frame
    void Update()
    {
        
        BasicMovement();
        //depreciated loop
        //if(fireCooldown <= 0 && !holdFire)
        //{
        //    holdFire = true;
        //    StartCoroutine(Fire());
        //    fireCooldown = fireRate;
        //}
        //else
        //{
        //    fireCooldown -= Time.deltaTime;
        //}
        if(!phaseSpawnersActive)
        {
            foreach(projectileSpawner p in phaseSpawners[currentPhase].projectileSpawners)
            {
                p.turnedOn = true;
            }
            phaseSpawnersActive = true;
        }
        if(currentHealth < maxHealth - (maxHealth / totalPhases) * (currentPhase + 1))
        {
            foreach (projectileSpawner p in phaseSpawners[currentPhase].projectileSpawners)
            {
                p.turnedOn = false;
            }
            currentPhase++;
            phaseSpawnersActive = false;
        }

        if (showTookDamage > 0)
        {
            showTookDamage -= Time.deltaTime;
            if (showTookDamage <= 0)
            {
                gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }


        
    }

    void BasicMovement()
    {
        if (transform.position.y > 4) { direction = Vector2.down; }
        if (transform.position.y < -4) { direction = Vector2.up; }

        rb.velocity = direction * speed;
    }

    void Die()
    {
        //Debug.Log(name + " has been killed");
        //gameObject.SetActive(false);
        // TEMPORARY, JUST TO NOTE THAT THEY WOULD'VE DIED BUT RESETTING THEIR HEALTH SO IT DOESN'T SPAM THE MESSAGE EVERY FRAME
        //currentHealth = maxHealth;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null && collision.tag == "playerBullet")
        {
            TakeDamage(collision.GetComponent<projectile>().damage);
            Destroy(collision.gameObject);

        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        showTookDamage = 1;
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, .8f, .8f, 1);
        if (currentHealth <= 0) { Die(); }
    }
    /// <summary>
    /// Will take in variable of attack pattern which will be created in the future, using this parameter it will determine which group of mounts to fire from, how many bullets,
    /// and what pattern for the bullets from each mount
    /// Currently just fires sine wave bullets from two mounts
    /// </summary>
    //IEnumerator Fire()
    //{
    //    //will go through each spawner group and only fire from the ones called for currently will just run through all and fire from all
    //    //foreach(GameObject[] spawnerGroup in projectileSpawners)
    //    //{
    //        //foreach(GameObject spawner in spawnerGroup)
    //        foreach(GameObject spawner in projectileSpawners)
    //        {
    //            //while loop will use number of bullets variable from attack pattern to determine how many to fire currently will fire 10
    //            for(int i = 0; i < 10; i++)
    //            {
    //            Instantiate(projectilePrefab, spawner.transform.position, spawner.transform.rotation);
    //            yield return new WaitForSeconds(.5f);
    //            }
    //        }
    //    holdFire = false;
    //    //}
    //}
}
