using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{        
    public Transform spawnPoint;    
    

    [SerializeField]
    private Player player;

    //currently WIP this class will be used to control spawners to allow for waves to come from seperate spawn points with similar or different pathings
    void Start()
    {  
    }

    /// <summary>
    /// Coroutine to handle the spawning of each wave
    /// </summary>
    /// <returns></returns>
    public IEnumerator WaveSpawn(Wave waveToSpawn, int wavePattern)
    {      
       
        for(int i = 0; i < waveToSpawn.enemyRanks.Length; i++)
        {
            if (waveToSpawn.enemyRanks[i].enemyNumbers > 0)
            {                
                for (int j = 0; j < waveToSpawn.enemyRanks[i].enemyNumbers; j++)
                {
                    SpawnEnemy(waveToSpawn.enemyRanks[i].enemyType, wavePattern);
                    yield return new WaitForSeconds(1 * waveToSpawn.spawnRate);
                }
            }
            else
            { 
                SpawnEnemy(waveToSpawn.enemyRanks[i].enemyType, wavePattern);
                yield return new WaitForSeconds(1 * waveToSpawn.spawnRate);
            }
        }     
        
    }

    void SpawnEnemy(GameObject enemy, int wavePattern)
    {
        
        Enemy thisEnemy = enemy.GetComponent<Enemy>();
        thisEnemy.pathNumber = wavePattern;        
        thisEnemy = Instantiate(thisEnemy, spawnPoint);
        thisEnemy.transform.parent = this.transform.GetChild(1);
        thisEnemy.player = player;
        EnemyManager.EnemiesAlive++;

    }
}
