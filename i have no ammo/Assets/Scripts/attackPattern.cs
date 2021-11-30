using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class attackPattern : MonoBehaviour
{
    public projectile[] projectiles;
    public int timeTofire;
    public int timeToCooldown;
    public Vector2[] directionsOfFire;
    public bool shotgunFire;
    public bool rifleFire;
    public float rifleDelay;

    public void Fire()
    {
        //fires the projectile spread if shotgun then all at once if rifle then individually with rifle delay
        //cycles through each direction firing and continues for timeToFire then pauses for timeToCooldown before reactivating spawner for new pattern        
    }
}
