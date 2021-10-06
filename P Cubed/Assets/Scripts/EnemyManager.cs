using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private Enemy[] enemies = new Enemy[0];
    private bool waveLife;    
    //wave constructor for enemy waves WIP
    void Update()
    {
        foreach(Enemy nme in enemies)
        {
        }
        
    }
    /// <summary>
    /// Spawns a new enemy wave currently taking in a level parameter to set number of enemies in wave
    /// </summary>
    /// <param name="level"></param>
    void SpawnWave(int level)
    {
        waveLife = true;
        enemies = new Enemy[level];
        for(int i = 0; i < level; i++)
        {
            enemies[i] = new Enemy(1, 1);
        }
        foreach(Enemy nme in enemies)
        {

        }
    }


}
