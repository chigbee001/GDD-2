using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackPattern : MonoBehaviour
{
    public player player;
    public projectileSpawner projectileSpawner;
    public projectile[] projectiles;
    public float initialDelay;
    public int timesTofireLoop;
    private int timesFired;
    public float timeToCooldown;
    public float shotGroupingDelay;
    public Vector2[] directionsOfFire;
    public bool targeted; // If it's targeted, the "direction" of the shot will instead be an offset from the direction of the player
    public bool shotgunFire;
    public bool rifleFire;
    public float rifleDelay;
    private bool continueFire;

    private void Start()
    {
        continueFire = true;
    }

    public IEnumerator Fire()
    {
        Debug.Log("Fire reached");
        timesFired = 0;

        if (initialDelay != 0)
        {
            yield return new WaitForSeconds(initialDelay);
        }

        //fires the projectile spread if shotgun then all at once if rifle then individually with rifle delay
        while (timesFired < timesTofireLoop)
        {
            if (shotgunFire && continueFire)
            {
                continueFire = false;
                ShotgunFire();
            }
            else if (rifleFire && continueFire)
            {
                continueFire = false;
                StartCoroutine(RifleFire());
            }
            yield return new WaitForSeconds(shotGroupingDelay);
        }
        //cycles through each direction firing and continues for timeToFire then pauses for timeToCooldown before reactivating spawner for new pattern
        StartCoroutine(EnterCooldown());
        
                    
    }

    private IEnumerator RifleFire()
    {
        for(int i = 0; i < projectiles.Length; i++)
        {
            if (targeted)
            {
                Vector2 toPlayer = player.transform.position - transform.position;

                float angle = Mathf.Atan2(directionsOfFire[i].normalized.y, directionsOfFire[i].normalized.x);

                projectiles[i].direction = Quaternion.AngleAxis(angle, Vector3.forward) * toPlayer;
            }
            else
            {
                projectiles[i].direction = directionsOfFire[i];
            }

            Instantiate(projectiles[i], this.transform.position, this.transform.rotation);
            yield return new WaitForSeconds(rifleDelay);
        }
        
        continueFire = true;
        timesFired++;
    }

    private void ShotgunFire()
    {
        for (int i = 0; i < projectiles.Length; i++)
        {
            if (targeted)
            {
                Vector2 toPlayer = player.transform.position - transform.position;

                float angle = Mathf.Atan2(directionsOfFire[i].normalized.y, directionsOfFire[i].normalized.x);

                projectiles[i].direction = Quaternion.AngleAxis(angle, Vector3.forward) * toPlayer;
            }
            else
            {
                projectiles[i].direction = directionsOfFire[i];
            }

            Instantiate(projectiles[i], this.transform.position, this.transform.rotation);
        }
        continueFire = true;
        timesFired++;
    }

    private IEnumerator EnterCooldown()
    {
        yield return new WaitForSeconds(timeToCooldown);
        projectileSpawner.inUse = false;
    }
}
